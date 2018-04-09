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
        public static List<List<Publication>> publicationsPerDisease;
        static void Main(string[] args)
        {
            //Environnement variables
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

            //Update number of publications per disease
            Console.WriteLine("Update number of publications per disease.....");
            using (var dbDisease = new MongoRepository.DiseaseRepository())
            using (var dbPublication = new MongoRepository.PublicationRepository())
            {
                //Update all diseases
                foreach(var disease in lst_diseases)
                {
                    long numberPublications = dbPublication.countForOneDisease(disease.OrphaNumber);
                    disease.NumberOfPublications = (int) numberPublications;
                    dbDisease.updateDisease(disease);
                }
            }
            Console.WriteLine("Update number of publications per disease finished");

            
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


            //Evaluation...
            if(PredictionData != null && RealData != null)
            {
                Evaluator.Evaluate(PredictionData, RealData);
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
                    publicationsPerDisease = new List<List<Publication>>();
                    using (var publicationRepository = new MongoRepository.PublicationRepository())
                    {
                        //Retrieving publications of selected diseases
                        //Parallel.ForEach(lst_diseases, (disease) =>
                        foreach (Disease disease in selectedDiseases)
                        {
                            List<Publication> pubs = publicationRepository.getByOrphaNumberOfLinkedDisease(disease.OrphaNumber);
                            if (pubs.Count != 0)
                            {
                                publicationsPerDisease.Add(pubs);
                            }
                        }
                        //Console.WriteLine("Publications recup finished!");
                        //);

                        //Extraction Symptomes
                        //Console.WriteLine("Extraction Symptoms...");

                        //foreach(List<Publication> pubs in publicationsPerDisease)
                        Parallel.ForEach(publicationsPerDisease, (pubs) =>
                        {
                            if (pubs.Count != 0)
                            {
                                //Extract symptoms
                                DiseaseData dataOneDisease = textMiningEngine.GetPredictionDataFromPublicationsOfOneDisease(
                                    pubs,
                                    selectedDiseases.Where(disease => disease.OrphaNumber == pubs[0].orphaNumberOfLinkedDisease).FirstOrDefault());

                                PredictionData.DiseaseDataList.Add(dataOneDisease);
                            }
                        }
                        );

                        
                    }

                    diffTime.Stop();
                    TimeLeft.Instance.IncrementOfXOperations(TimeSpan.FromMilliseconds(diffTime.ElapsedMilliseconds).Seconds, 1);
                    TimeLeft.Instance.CalcAndShowTimeLeft(i + 1, nombreBatch);
                }

                //Insert in DB
                if (PredictionData.DiseaseDataList.Count != 0)
                {
                    try
                    {
                        //Cut in 10 parts
                        int numberOfDocument = 10;
                        int numberDiseases = PredictionData.DiseaseDataList.Count / numberOfDocument;
                        int rest = PredictionData.DiseaseDataList.Count % numberOfDocument;

                        for (int i = 0; i< numberOfDocument; i++)
                        {
                            if (rest !=0 && i == numberOfDocument - 1)
                            {
                                predictionDataRepository.insert(
                                new DiseasesData(
                                    type.Symptom,
                                    PredictionData.DiseaseDataList
                                    .Skip(i* numberDiseases)
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
                                    PredictionData.DiseaseDataList
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
        }
    }
}
