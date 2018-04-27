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
            //Environment.SetEnvironmentVariable("RD_AGGREGATOR_SETTINGS", @"C:\Users\Psycho\Source\Repos\RDSearch4\settings.json");
            Environment.SetEnvironmentVariable("RD_AGGREGATOR_SETTINGS", @"C:\Users\CharlesCOUSYN\source\repos\Aggregator\settings.json");
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
                lst_diseases = db.selectAll().Take(10).ToList();
                //lst_diseases = db.selectAll();
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

            
            //Retrieving related entities by disease AND TextMine
            TextMiningEngine textMiningEngine = new TextMiningEngine();
            RecupSymptomsAndTextMine(lst_diseases, textMiningEngine);
            //RecupLinkedDiseasesAndTextMine(lst_diseases, textMiningEngine);
            //RecupDrugsAndTextMine(lst_diseases, textMiningEngine);
            

            //Retrieving PredictionData and RealData from DB (DiseasesData with type Symptom)
            DiseasesData PredictionData = null;
            DiseasesData RealData = null;
            using (var dbPred = new MongoRepository.PredictionDataRepository())
            using (var dbReal = new MongoRepository.RealDataRepository())
            {
                PredictionData = dbPred.selectByType(type.Symptom);
                RealData = dbReal.selectByType(type.Symptom);
            }

            /*
            //Evaluation...
            if (PredictionData != null && RealData != null)
            {
                Console.WriteLine("Evaluation....");
                Evaluator.WriteListOfResultsJSONFile(
                    Evaluator.EvaluateMultipleFormulas(PredictionData,RealData));

                Console.WriteLine("Evaluation finished!");
            }*/


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
                MinMaxNormalization(PredictionData, 0.0, 1.0, TFType.RawCount, TFType.MinMaxNorm);
                Compute_TF_IDF_Terms_ToAllDiseaseData(PredictionData);
                OrderDiseaseDatas(PredictionData);
                //KeepTheBest(PredictionData);

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
                    //Cut in 20 parts
                    int numberOfDocument = 20;
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

                    //foreach(var pubs in publicationsPerDisease)
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

        static void Compute_TF_IDF_Terms_ToAllDiseaseData(
            DiseasesData PredictionData //Var to UPDATE
            )
        {
            int totalNumberOfDisease = PredictionData.DiseaseDataList.Count;

            //Get list of NbDisease_i (Number of disease where symptom i appears)
            Dictionary<RelatedEntity, int> phenotypesAlreadySeenWithOccurences = new Dictionary<RelatedEntity, int>();

            //Get list of SumOfMinMaxNorm_i (Sum of rawcount of symptom i in all diseases)
            Dictionary<RelatedEntity, double> phenotypesAlreadySeenWithSumOfMinMaxNorm_i = new Dictionary<RelatedEntity, double>();

            int countDisease = 0;
            foreach(var diseasedata in PredictionData.DiseaseDataList)
            {
                foreach (var phenotype in diseasedata.RelatedEntities.RelatedEntitiesList)
                {
                    ////////////////
                    //Compute TFs///
                    ////////////////

                    //RawCount already done by LingPipe...
                    double rawCount = phenotype.TermFrequencies.Where(TF => TF.TFType == TFType.RawCount).FirstOrDefault().Value;

                    //TF Binary
                    if (rawCount != 0.0)
                    {
                        phenotype.TermFrequencies.Where(TF => TF.TFType == TFType.Binary).FirstOrDefault().Value = 1.0;
                    }
                    else
                    {
                        phenotype.TermFrequencies.Where(TF => TF.TFType == TFType.Binary).FirstOrDefault().Value = 0.0;
                    }

                    //TF LogNorm
                    phenotype.TermFrequencies.Where(TF => TF.TFType == TFType.LogNorm).FirstOrDefault().Value = Math.Log10(1 + rawCount);

                    ////////////////
                    //Compute IDFs//
                    ////////////////

                    //Find the phenotype in alreadyseen phenotypes
                    List<KeyValuePair<RelatedEntity, int>> existantPhenotype = phenotypesAlreadySeenWithOccurences
                        .Where(p => p.Key.Name.Equals(phenotype.Name))
                        .ToList();
                    List<KeyValuePair<RelatedEntity, double>> existantPhenotypeSum = phenotypesAlreadySeenWithSumOfMinMaxNorm_i
                        .Where(p => p.Key.Name.Equals(phenotype.Name))
                        .ToList();

                    //If not existant
                    if (existantPhenotype.Count == 0)
                    {
                        //Console.WriteLine("Count");
                        //Count number of times phenotype i appears
                        int NbDisease_i = 
                            PredictionData
                            .DiseaseDataList
                            .Count(diseaseData => diseaseData
                                .RelatedEntities
                                .RelatedEntitiesList
                                .Any(p => p.Name.Equals(phenotype.Name))
                            );
                        //Sum all the MinMaxNorm of phenotype i in all diseases
                        double SumOfMinMaxNorm_i = 
                            PredictionData
                            .DiseaseDataList
                            .Sum(d =>
                            {
                                var relatedEntity = d.RelatedEntities.RelatedEntitiesList
                                .Where(p => p.Name.Equals(phenotype.Name))
                                .FirstOrDefault();
                                if(relatedEntity == null)
                                {
                                    return 0.0;
                                }
                                else
                                {
                                    return relatedEntity
                                    .TermFrequencies
                                    .Where(TF => TF.TFType == TFType.MinMaxNorm)
                                    .FirstOrDefault()
                                    .Value;
                                }
                            }
                            );

                        //Compute IDFs
                        UpdateIDFs(phenotype, totalNumberOfDisease, NbDisease_i, SumOfMinMaxNorm_i);

                        //Add to already seen list
                        phenotypesAlreadySeenWithOccurences.Add(phenotype, NbDisease_i);

                        //Add to already seen list
                        phenotypesAlreadySeenWithSumOfMinMaxNorm_i.Add(phenotype, SumOfMinMaxNorm_i);
                    }
                    //If already counted
                    else
                    {
                        //Apply weight update
                        //Console.WriteLine("No Count");
                        //Console.WriteLine($"From {phenotype.Weight}\t to {ToTF_IDF(phenotype.Weight, 1.0, totalNumberOfDisease, existantPhenotype[0].Value)}");

                        //Compute IDFs
                        UpdateIDFs(phenotype, totalNumberOfDisease, existantPhenotype[0].Value, existantPhenotypeSum[0].Value);
                    }
                }
                
                countDisease++;
            }
        }

        static void UpdateIDFs(RelatedEntity phenotype, int totalNumberOfDisease, int NbDisease_i, double SumOfMinMaxNorm_i)
        {
            //IDF UNARY
            phenotype.IDFs.Where(IDF => IDF.IDFType == IDFType.Unary).FirstOrDefault().Value = 1.0;

            //NbDisease_i
            phenotype.IDFs.Where(IDF => IDF.IDFType == IDFType.NbDisease_i).FirstOrDefault().Value =
                (double)NbDisease_i;

            //Inverse_NbDisease_i
            phenotype.IDFs.Where(IDF => IDF.IDFType == IDFType.Inverse_NbDisease_i).FirstOrDefault().Value =
                (double)totalNumberOfDisease / (double)NbDisease_i;

            //IDF_Classic_NbDisease_i
            phenotype.IDFs.Where(IDF => IDF.IDFType == IDFType.IDF_Classic_NbDisease_i).FirstOrDefault().Value =
                Math.Log10((double)totalNumberOfDisease / (double)NbDisease_i);

            //IDF_Smooth_NbDisease_i
            phenotype.IDFs.Where(IDF => IDF.IDFType == IDFType.IDF_Smooth_NbDisease_i).FirstOrDefault().Value =
                Math.Log10(1.0 + ((double)totalNumberOfDisease / (double)NbDisease_i));

            //Prob_IDF_NbDisease_i
            phenotype.IDFs.Where(IDF => IDF.IDFType == IDFType.Prob_IDF_NbDisease_i).FirstOrDefault().Value =
                Math.Log10((((double)totalNumberOfDisease - (double)NbDisease_i)) / (double)NbDisease_i);

            //SumOfRawCount_i
            phenotype.IDFs.Where(IDF => IDF.IDFType == IDFType.SumOfMinMaxNorm_i).FirstOrDefault().Value =
                (double)SumOfMinMaxNorm_i;

            //Inverse_SumOfRawCount_i
            phenotype.IDFs.Where(IDF => IDF.IDFType == IDFType.Inverse_SumOfMinMaxNorm_i).FirstOrDefault().Value =
                1.0 / (double)SumOfMinMaxNorm_i;

            //IDF_Classic_SumOfRawCount_i
            phenotype.IDFs.Where(IDF => IDF.IDFType == IDFType.IDF_Classic_SumOfMinMaxNorm_i).FirstOrDefault().Value =
                Math.Log10(1.0 / (double)SumOfMinMaxNorm_i);

            //IDF_Smooth_SumOfRawCount_i
            phenotype.IDFs.Where(IDF => IDF.IDFType == IDFType.IDF_Smooth_SumOfMinMaxNorm_i).FirstOrDefault().Value =
                Math.Log10(1.0 + (1.0 / (double)SumOfMinMaxNorm_i));

            //Prob_IDF_SumOfRawCount_i
            //phenotype.IDFs.Where(IDF => IDF.IDFType == IDFType.Prob_IDF_SumOfRawCount_i).FirstOrDefault().Value =
                //Math.Log10(((1.0 - (double)SumOfRawCount_i)) / (double)SumOfRawCount_i);
        }

        //MinMax normalization on one TFsource to one TFDest
        static void MinMaxNormalization(DiseasesData PredictionData, double NewMin, double NewMax, TFType TFTypeSource, TFType TFTypeDest)
        {
            foreach (var diseasedata in PredictionData.DiseaseDataList)
            {
                //var relatedEntities = diseasedata.RelatedEntities.RelatedEntitiesList;
                if (diseasedata.RelatedEntities.RelatedEntitiesList.Count != 0)
                {
                    //Find Min and Max for Normalization
                    double max = diseasedata.RelatedEntities.RelatedEntitiesList.Max(x => x.TermFrequencies.Where(tf => tf.TFType == TFTypeSource).FirstOrDefault().Value);
                    double min = diseasedata.RelatedEntities.RelatedEntitiesList.Min(x => x.TermFrequencies.Where(tf => tf.TFType == TFTypeSource).FirstOrDefault().Value);

                    if (max == min)
                    {
                        for (int i = 0; i < diseasedata.RelatedEntities.RelatedEntitiesList.Count; i++)
                        {
                            diseasedata.RelatedEntities.RelatedEntitiesList[i].Weight = NewMax;
                        }
                    }
                    else
                    {
                        //Symptom Weight Normalization from NewMin to NewMax
                        for (int i = 0; i < diseasedata.RelatedEntities.RelatedEntitiesList.Count; i++)
                        {
                            double value = diseasedata.RelatedEntities.RelatedEntitiesList[i].TermFrequencies.Where(tf => tf.TFType == TFTypeSource).FirstOrDefault().Value;

                            //UpdateValue
                            diseasedata
                                .RelatedEntities
                                .RelatedEntitiesList[i]
                                .TermFrequencies.Where(tf => tf.TFType == TFTypeDest)
                                .FirstOrDefault()
                                .Value =
                                NewMin + (NewMax - NewMin) * (value - min)  / (max - min);
                        }
                    }
                }
            }
            
        }

        static void OrderDiseaseDatas(DiseasesData PredictionData)
        {
            foreach (var diseasedata in PredictionData.DiseaseDataList)
            {
                //var relatedEntities = diseasedata.RelatedEntities.RelatedEntitiesList;
                if (diseasedata.RelatedEntities.RelatedEntitiesList.Count != 0)
                {
                    diseasedata.RelatedEntities.RelatedEntitiesList =
                        diseasedata.RelatedEntities.RelatedEntitiesList
                        .OrderByDescending(x => x.TermFrequencies.Where(tf => tf.TFType == TFType.RawCount).FirstOrDefault().Value)
                        .ToList();
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
                        .OrderByDescending(x => x.TermFrequencies.Where(tf => tf.TFType == TFType.RawCount).FirstOrDefault().Value)
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

            double idf = Math.Log10((double)nbDiseasesTot / (double)nbDiseases_i);

            return tf * idf;
        }
    }
}
