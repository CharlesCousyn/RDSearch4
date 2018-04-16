using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ConfigurationJSON;
using MongoRepository.entities;
using Evaluation.entities;

namespace Evaluation
{
    public class Evaluator
    {
        public static Results Evaluate(DiseasesData PredictionData, DiseasesData RealData, double threshold = 0.0)
        {
            //Object to write in JSON
            Results results = new Results();

            int RP = 0;//RealPositive general
            int FP = 0;//FalsePositive general
            int FN = 0;//FalseNegative general

            int NumberOfDiseasesWithKnownPhenotypes = RealData.DiseaseDataList.Count;
            int NumberOfDiseasesWithPublicationsInPredictionData = PredictionData.DiseaseDataList.Count(x => x.Disease.NumberOfPublications != 0);
            int NumberOfDiseasesEvaluatedForReal = 0;

            //For each existent rare disease
            foreach (string orphaNumber in PredictionData.DiseaseDataList.Select(x => x?.Disease?.OrphaNumber))
            {
                //Find THE diseaseData of ONE disease (real and predicted data)
                DiseaseData RealDiseaseData = RealData.DiseaseDataList.Where(x => x?.Disease?.OrphaNumber == orphaNumber).FirstOrDefault();
                DiseaseData PredictionDiseaseData = PredictionData.DiseaseDataList.Where(
                    x => x?.Disease?.OrphaNumber == orphaNumber && 
                    x.Disease.NumberOfPublications != 0).FirstOrDefault();

                //If we don't find the disease in both dataset, we shoud pass to another disease
                if(RealDiseaseData != null && PredictionDiseaseData != null)
                {
                    NumberOfDiseasesEvaluatedForReal++;//Increase number of diseases evaluated

                    int RP_Disease = 0;//RealPositive of one disease
                    int FP_Disease = 0;//FalsePositive of one disease
                    int FN_Disease = 0;//FalseNegative of one disease

                    //Compute RP and FP
                    List<string> RelatedEntitiesNamesReal =
                        RealDiseaseData
                        .RelatedEntities.RelatedEntitiesList
                        .Select(x => x.Name)
                        .ToList();

                    for (int j = 0; j < PredictionDiseaseData.RelatedEntities.RelatedEntitiesList.Count; j++)
                    {
                        if (threshold == 0.0 || PredictionDiseaseData.RelatedEntities.RelatedEntitiesList[j].Weight >= threshold)
                        {
                            //Is my predicted related entity is present in the real data?
                            if (RelatedEntitiesNamesReal.IndexOf(PredictionDiseaseData.RelatedEntities.RelatedEntitiesList[j].Name) != -1)
                            {
                                RP++;
                                RP_Disease++;
                            }
                            else
                            {
                                FP++;
                                FP_Disease++;
                            }
                        }
                    }

                    //Compute FN
                    List<string> RelatedEntitiesNamesPred =
                        PredictionDiseaseData
                        .RelatedEntities.RelatedEntitiesList
                        .Where(entity => threshold == 0.0 || entity.Weight >= threshold)
                        .Select(x => x.Name)
                        .ToList();
                    for (int j = 0; j < RealDiseaseData.RelatedEntities.RelatedEntitiesList.Count; j++)
                    {
                        //Is my real related entity is present in the predicted data?
                        if (RelatedEntitiesNamesPred.IndexOf(RealDiseaseData.RelatedEntities.RelatedEntitiesList[j].Name) == -1)
                        {
                            FN++;
                            FN_Disease++;
                        }
                    }

                    //Compute Precision/recall and F_score
                    double PrecisionDisease=0.0;
                    double RecallDisease=0.0;
                    double F_ScoreDisease=0.0;
                    if (RP_Disease + FP_Disease != 0)
                    {
                        PrecisionDisease = Math.Round((double)RP_Disease / (double)(RP_Disease + FP_Disease), 4);
                    }
                    if(RP_Disease + FN_Disease != 0)
                    {
                        RecallDisease = Math.Round((double)RP_Disease / (double)(RP_Disease + FN_Disease), 4);
                    }
                    if(PrecisionDisease + RecallDisease != 0.0)
                    {
                        F_ScoreDisease = Math.Round(2 * PrecisionDisease * RecallDisease / (PrecisionDisease + RecallDisease), 4);
                    }

                    //Construct results object
                    PerDisease OnePerDisease = new PerDisease(orphaNumber,
                        PredictionDiseaseData.Disease.NumberOfPublications, 
                        PredictionData.Type.ToString(),
                        RP_Disease, 
                        FP_Disease, 
                        FN_Disease,
                        PrecisionDisease,//Precision
                        RecallDisease, //Recall
                        F_ScoreDisease
                        );

                    results.perDisease.Add(OnePerDisease);
                }

            }

            //Compute Precision/recall and F_score general
            double Precision = 0.0;
            double Recall = 0.0;
            double F_Score = 0.0;
            if (RP + FP != 0)
            {
                Precision = Math.Round((double)RP / (double)(RP + FP), 4);
            }
            if (RP + FN != 0)
            {
                Recall = Math.Round((double)RP / (double)(RP + FN), 4);
            }
            if (Precision + Recall != 0.0)
            {
                F_Score = Math.Round(2 * Precision * Recall / (Precision + Recall), 4);
            }

            //Construct results object
            results.general = new General(
                DateTime.Now,
                threshold,
                NumberOfDiseasesWithKnownPhenotypes,
                NumberOfDiseasesWithPublicationsInPredictionData,
                NumberOfDiseasesEvaluatedForReal,
                PredictionData.Type.ToString(), 
                RP, 
                FP, 
                FN,  
                Precision,  
                Recall, 
                F_Score);

            return results;
        }

        public static void WriteResultsJSONFile(Results results, string wantedFileName = "")
        {
            string output = JsonConvert.SerializeObject(results, Formatting.Indented);

            //Choose the name
            string fileName = "";
            if (wantedFileName != "")
            {
                fileName = wantedFileName;
            }
            else
            {

                fileName = "results_" + results.general.TimeStamp.ToString("yyyy-MM-dd_HH-mm-ss") + ".json";
            }

            File.WriteAllText(ConfigurationManager.Instance.config.ResultsFolder + fileName, output);
        }

        public static MetaResults MetaEvaluate(DiseasesData PredictionData, DiseasesData RealData, double minWeight, double maxWeight, double step, Criterion criterion)
        {
            //Create MetaResult
            MetaResults metaResults = new MetaResults();

            //Compute all results and put them in metaResults
            List<Results> listResults = new List<Results>();
            for (double i = minWeight; i <= maxWeight; i+=step)
            {
                Results currentRes = Evaluate(PredictionData, RealData, i);
                listResults.Add(currentRes);
                metaResults.perThreshold.Add(
                    new PerThreshold(
                        i,
                        currentRes.general.Type,
                        currentRes.general.RealPositives,
                        currentRes.general.FalsePositives,
                        currentRes.general.FalseNegatives,
                        currentRes.general.Precision,
                        currentRes.general.Recall,
                        currentRes.general.F_Score
                        ));
            }

            //Find best results
            Results Best_Result;
            switch (criterion)
            {
                case Criterion.F_Score:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.F_Score > savedRes.general.F_Score ? currentRes : savedRes);
                    break;
                case Criterion.Precision:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.Precision > savedRes.general.Precision ? currentRes : savedRes);
                    break;
                case Criterion.Recall:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.Recall > savedRes.general.Recall ? currentRes : savedRes);
                    break;
                default:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.F_Score > savedRes.general.F_Score ? currentRes : savedRes);
                    break;
            }

            //Complete metaResults
            metaResults.bestInfos = new BestInfos(
                    Best_Result.general.TimeStamp,
                    Best_Result.general.Type,
                    Best_Result.general.Threshold,
                    Best_Result.general.Precision,
                    Best_Result.general.Recall,
                    Best_Result.general.F_Score,
                    criterion
                );

            return metaResults;
        }

        public static void WriteMetaResultsJSONFile(MetaResults metaResults, string wantedFileName = "")
        {
            string output = JsonConvert.SerializeObject(metaResults, Formatting.Indented);

            //Choose the name
            string fileName = "";
            if (wantedFileName != "")
            {
                fileName = wantedFileName;
            }
            else
            {
                fileName = "metaResults_" + metaResults.bestInfos.TimeStamp.ToString("yyyy-MM-dd_HH-mm-ss") + ".json";
            }

            File.WriteAllText(ConfigurationManager.Instance.config.ResultsFolder + fileName, output);
        }
    }
}
