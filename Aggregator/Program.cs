using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoRepository.entities;
using WebCrawler;
using CrawlerOrphanet.tools;
using System.Diagnostics;
using ConfigurationJSON;
using Evaluation;
using System.IO;

namespace CrawlerOrphanet
{
    class Program
    {
        public static Dictionary<string, List<Publication>> publicationsPerDisease;
        static void Main(string[] args)
        {
            //Environnement variables
            Environment.SetEnvironmentVariable("RD_AGGREGATOR_SETTINGS", @"C:\Users\Psycho\Source\Repos\RDSearch4\settings.json");
            var path = Environment.GetEnvironmentVariable("RD_AGGREGATOR_SETTINGS");
            ConfigurationManager.Instance.Init(path);

            //TESTED AND DONE

            //Update Orphanet (diseases/real datasets)
            /*
            OrphaEngine orphaEngine = new OrphaEngine();
            orphaEngine.Start();
            */


            //Retrieving diseases from DB
            List<Disease> lst_diseases = new List<Disease>();
            using (var db = new MongoRepository.DiseaseRepository())
            {
                //lst_diseases = db.selectAll().Take(50).ToList();
                lst_diseases = db.selectAll();
            }

            //TESTED AND DONE
            /*
            //Update Publications
            PubmedEngine pubmedEngine = new PubmedEngine();
            Console.WriteLine("Starting requests at PMC this can take some time...");
            pubmedEngine.Start2(lst_diseases);
            */

            /*
            //Update number of publications per disease
            Console.WriteLine("Update number of publications per disease.....");
            using (var dbDisease = new MongoRepository.DiseaseRepository())
            using (var dbPublication = new MongoRepository.PublicationRepository())
            {
                //Update all diseases
                foreach (var disease in lst_diseases)
                {
                    long numberPublications = dbPublication.countForOneDisease(disease.OrphaNumber);
                    disease.NumberOfPublications = (int)numberPublications;
                    dbDisease.updateDisease(disease);
                }
            }
            Console.WriteLine("Update number of publications per disease finished");
            */

            /*
            //Retrieving related entities by disease AND TextMine
            TextMiningEngine textMiningEngine = new TextMiningEngine();
            RecupSymptomsAndTextMine(lst_diseases, textMiningEngine);
            //RecupLinkedDiseasesAndTextMine(lst_diseases, textMiningEngine);
            //RecupDrugsAndTextMine(lst_diseases, textMiningEngine);
            */

            //Retrieving PredictionData and RealData from DB (DiseasesData with type Symptom)
            DiseasesData PredictionData = null;
            DiseasesData RealData = null;
            using (var dbPred = new MongoRepository.PredictionDataRepository())
            using (var dbReal = new MongoRepository.RealDataRepository())
            {
                PredictionData = dbPred.selectByType(type.Symptom);
                RealData = dbReal.selectByType(type.Symptom);
            }


            //Evaluation...
            if (PredictionData != null && RealData != null)
            {
                Console.WriteLine("Evaluation....");
                //Evaluator.WriteResultsJSONFile(Evaluator.Evaluate(PredictionData, RealData, 0.0));
                Evaluator.WriteMetaResultsJSONFile(Evaluator.MetaEvaluate(PredictionData, RealData, 0.0, 100.0, 1.0, Evaluation.entities.Criterion.F_Score));
                Console.WriteLine("Evaluation finished!");
            }


            Console.WriteLine("Finished :)");
            Console.ReadLine();
        }

        static void RecupLinkedDiseasesAndTextMine(List<Disease> lst_diseases, TextMiningEngine textMiningEngine)
        {
            throw new NotImplementedException();
        }

        static void RecupDrugsAndTextMine(List<Disease> lst_diseases, TextMiningEngine textMiningEngine)
        {
            throw new NotImplementedException();
        }

        static void RecupSymptomsAndTextMine(List<Disease> lst_diseases, TextMiningEngine textMiningEngine)
        {
            using (var predictionDataRepository = new MongoRepository.PredictionDataRepository())
            {
                //Delete ALL prediction disease data...
                predictionDataRepository.removeAll();

                //Init the new PredictionData
                DiseasesData PredictionData = new DiseasesData(type.Symptom, new List<DiseaseData>());

                //BatchConfig
                int batchSize = ConfigurationManager.Instance.config.BatchSizeTextMining;
                int nombreBatch = (lst_diseases.Count / batchSize) + 1;
                if ((nombreBatch - 1) * batchSize == lst_diseases.Count)
                {
                    nombreBatch--;
                }


                //TimeLeft initialization
                TimeLeft.Instance.Reset();
                TimeLeft.Instance.operationsToDo = nombreBatch;

                //First batches to count occurences
                LaunchBatchs_Recup_Count(nombreBatch, batchSize, lst_diseases, textMiningEngine, PredictionData);

                //Treatment
                Apply_TF_IDF_ToAllDiseaseData(PredictionData);
                Normalization(PredictionData);
                KeepTheBest(PredictionData);

                //Insert in DB
                InsertPredictionInDB(PredictionData.DiseaseDataList, predictionDataRepository);

            }
        }

        static void InsertPredictionInDB(List<DiseaseData> listDiseaseData, MongoRepository.PredictionDataRepository predictionDataRepository)
        {
            if (listDiseaseData.Count != 0)
            {
                try
                {
                    //Cut in 10 parts
                    int numberOfDocument = 10;
                    int numberDiseases = listDiseaseData.Count / numberOfDocument;
                    int rest = listDiseaseData.Count % numberOfDocument;

                    for (int i = 0; i < numberOfDocument; i++)
                    {
                        if (rest != 0 && i == numberOfDocument - 1)
                        {
                            predictionDataRepository.insert(
                            new DiseasesData(
                                type.Symptom,
                                listDiseaseData
                                .Skip(i * numberDiseases)
                                .Take(rest)
                                .ToList()
                                )
                            );
                        }
                        else
                        {
                            predictionDataRepository.insert(
                            new DiseasesData(
                                type.Symptom,
                                listDiseaseData
                                .Skip(i * numberDiseases)
                                .Take(numberDiseases)
                                .ToList()
                                )
                            );
                        }

                    }
                    //predictionDataRepository.insert(PredictionData);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Console.WriteLine("Error on insertion of PredictionData");

                }
            }
        }
        static void LaunchBatchs_Recup_Count(
            int nombreBatch, //Batch config
            int batchSize, //Batch config
            List<Disease> lst_diseases, //Complete list of diseases to select diseases
            TextMiningEngine textMiningEngine, //Engine to text mine (count here)
            DiseasesData PredictionData //Var to complete
            )
        {
            for (int i = 0; i < nombreBatch; i++)
            {

                Stopwatch diffTime = new Stopwatch();
                diffTime.Start();

                //BatchSize adjustement
                int realBatchSize = batchSize;
                if ((i + 1) * realBatchSize > lst_diseases.Count)
                {
                    realBatchSize = lst_diseases.Count - i * realBatchSize;
                }
                var selectedDiseases = lst_diseases.GetRange(i * realBatchSize, realBatchSize);


                //REAL Process
                //Publication recup
                //Console.WriteLine("Publications recup...");
                publicationsPerDisease = new Dictionary<string, List<Publication>>();
                using (var publicationRepository = new MongoRepository.PublicationRepository())
                {
                    //Retrieving publications of selected diseases
                    //Parallel.ForEach(lst_diseases, (disease) =>
                    foreach (Disease disease in selectedDiseases)
                    {
                        List<Publication> pubs = publicationRepository.getByOrphaNumberOfLinkedDisease(disease.OrphaNumber);
                        if (pubs.Count != 0)
                        {
                            publicationsPerDisease.Add(disease.OrphaNumber, pubs);
                        }
                        else
                        {
                            publicationsPerDisease.Add(disease.OrphaNumber, new List<Publication>());
                        }
                    }
                    //Console.WriteLine("Publications recup finished!");
                    //);

                    //Extraction Symptomes
                    //Console.WriteLine("Extraction Symptoms...");

                    //foreach(List<Publication> pubs in publicationsPerDisease)
                    Parallel.ForEach(publicationsPerDisease, (pubs) =>
                    {
                        if (pubs.Value.Count != 0)
                        {
                            //Extract symptoms
                            DiseaseData dataOneDisease = textMiningEngine.GetPredictionDataCountFromPublicationsOfOneDisease(
                                pubs.Value,
                                selectedDiseases.Where(disease => disease.OrphaNumber == pubs.Key).FirstOrDefault());

                            PredictionData.DiseaseDataList.Add(dataOneDisease);
                        }
                        else
                        {
                            DiseaseData dataOneDisease = new DiseaseData(
                                selectedDiseases.Where(disease => disease.OrphaNumber == pubs.Key).FirstOrDefault(), 
                                new RelatedEntities(type.Symptom, new List<RelatedEntity>()));
                            
                            PredictionData.DiseaseDataList.Add(dataOneDisease);
                        }
                    }
                    );


                }

                diffTime.Stop();
                TimeLeft.Instance.IncrementOfXOperations(TimeSpan.FromMilliseconds(diffTime.ElapsedMilliseconds).Seconds, 1);
                TimeLeft.Instance.CalcAndShowTimeLeft(i + 1, nombreBatch);
            }
        }

        static void Apply_TF_IDF_ToAllDiseaseData(
            DiseasesData PredictionData //Var to UPDATE
            )
        {
            int totalNumberOfDisease = PredictionData.DiseaseDataList.Count;

            //Get list of NbDiseasei (Number of disease where symptoms i appears)
            Dictionary<RelatedEntity, int> phenotypesAlreadySeenWithOccurences = new Dictionary<RelatedEntity, int>();

            int countDisease = 0;
            foreach(var diseasedata in PredictionData.DiseaseDataList)
            {
                //Console.WriteLine(countDisease);
                foreach (var phenotype in diseasedata.RelatedEntities.RelatedEntitiesList)
                {

                    //Find the phenotype
                    List<KeyValuePair<RelatedEntity, int>> existantPhenotype = phenotypesAlreadySeenWithOccurences
                        .Where(p => p.Key.Name.Equals(phenotype.Name))
                        .ToList();
                    
                    //If not existant
                    if (existantPhenotype.Count == 0)
                    {
                        //Console.WriteLine("Count");
                        //Count number of times phenotype i appears
                        int NbDisease_i = PredictionData.DiseaseDataList.Count(
                            diseaseData => diseaseData.RelatedEntities.RelatedEntitiesList.Any(
                            p => p.Name.Equals(phenotype.Name))
                        );

                        //TO TEST OccTotJ (number of words)
                        //Console.WriteLine($"From {phenotype.Weight}\t to {ToTF_IDF(phenotype.Weight, 1.0, totalNumberOfDisease, NbDisease_i)}");
                        phenotype.Weight = ToTF_IDF(phenotype.Weight, 1.0, totalNumberOfDisease, NbDisease_i);

                        //Add to already seen list
                        phenotypesAlreadySeenWithOccurences.Add(phenotype, NbDisease_i);
                    }
                    //If already counted
                    else
                    {
                        //Apply weight update
                        //Console.WriteLine("No Count");
                        //Console.WriteLine($"From {phenotype.Weight}\t to {ToTF_IDF(phenotype.Weight, 1.0, totalNumberOfDisease, existantPhenotype[0].Value)}");
                        phenotype.Weight = ToTF_IDF(phenotype.Weight, 1.0, totalNumberOfDisease, existantPhenotype[0].Value);
                    }
                }
                countDisease++;
            }
        }

        static void Normalization(DiseasesData PredictionData)
        {
            foreach (var diseasedata in PredictionData.DiseaseDataList)
            {
                //var relatedEntities = diseasedata.RelatedEntities.RelatedEntitiesList;
                if (diseasedata.RelatedEntities.RelatedEntitiesList.Count != 0)
                {
                    //Find Min and Max for Normalization
                    double max = diseasedata.RelatedEntities.RelatedEntitiesList.Max(x => x.Weight);
                    double min = diseasedata.RelatedEntities.RelatedEntitiesList.Min(x => x.Weight);

                    double newMax = 100.0;
                    double newMin = 0.0;

                    if (max == min)
                    {
                        for (int i = 0; i < diseasedata.RelatedEntities.RelatedEntitiesList.Count; i++)
                        {
                            diseasedata.RelatedEntities.RelatedEntitiesList[i].Weight = 100.0;
                        }
                    }
                    else
                    {
                        //Symptom Weight Normalization from 0 to 100
                        for (int i = 0; i < diseasedata.RelatedEntities.RelatedEntitiesList.Count; i++)
                        {
                            diseasedata.RelatedEntities.RelatedEntitiesList[i].Weight = 
                                newMin + (newMax - newMin) * (diseasedata.RelatedEntities.RelatedEntitiesList[i].Weight - min) / (max - min);
                        }
                    }
                }
            }
            
        }

        static void KeepTheBest(DiseasesData PredictionData)
        {
            foreach (var diseasedata in PredictionData.DiseaseDataList)
            {
                //var relatedEntities = diseasedata.RelatedEntities.RelatedEntitiesList;
                if (diseasedata.RelatedEntities.RelatedEntitiesList.Count != 0)
                {
                    //Take only a the best symptoms (see config file)
                    diseasedata.RelatedEntities.RelatedEntitiesList =
                        diseasedata.RelatedEntities.RelatedEntitiesList
                        .OrderByDescending(x => x.Weight)
                        .Take(ConfigurationManager.Instance.config.MaxNumberSymptoms)
                        .ToList();
                }
            }
        }

        static double ToTF_IDF(
            double ocurrence_i_j, //Times term i appears in publications of disease j
            double ocurrenceTot_j, //Number of terms in publications of disease j
            int nbDiseasesTot, //Total number of disease
            int nbDiseases_i //Number of diseases where the term i has been detected
            )
        {
            double tf = ocurrence_i_j / ocurrenceTot_j;

            double idf = System.Math.Log10(nbDiseasesTot / nbDiseases_i);

            return tf * idf;
        }
    }
}
