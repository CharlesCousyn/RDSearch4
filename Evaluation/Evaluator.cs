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
        public static Results Evaluate(DiseasesData PredictionData, DiseasesData RealData,
            Tuple<TFType, IDFType> WeightCombinaison,
            double threshold = -1.0)
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

                    Dictionary<RelatedEntity, double> RealWeightOfPhenotypes = new Dictionary<RelatedEntity, double>();
                    List<RelatedEntity> RealPhenotypes = new List<RelatedEntity>();

                    double MR_Disease = 0.0;//MeanRank RealPhenotype of one disease
                    int RP_Disease = 0;//RealPositive of one disease
                    int FP_Disease = 0;//FalsePositive of one disease
                    int FN_Disease = 0;//FalseNegative of one disease

                    //Compute RP and FP
                    List<string> RelatedEntitiesNamesReal =
                        RealDiseaseData
                        .RelatedEntities.RelatedEntitiesList
                        .Select(x => x.Name)
                        .ToList();

                    int NumberOfRelatedEntitiesFound = PredictionDiseaseData.RelatedEntities.RelatedEntitiesList.Count;

                    for (int j = 0; j < NumberOfRelatedEntitiesFound; j++)
                    {
                        double realWeight = PredictionDiseaseData.RelatedEntities.RelatedEntitiesList[j]
                            .CalcFinalWeight(WeightCombinaison.Item1, WeightCombinaison.Item2);

                        RealWeightOfPhenotypes.Add(PredictionDiseaseData.RelatedEntities.RelatedEntitiesList[j], realWeight);

                        if (threshold == -1.0 || realWeight >= threshold)
                        {
                            //Is my predicted related entity is present in the real data?
                            if (RelatedEntitiesNamesReal.IndexOf(PredictionDiseaseData.RelatedEntities.RelatedEntitiesList[j].Name) != -1)
                            {
                                RP++;
                                RP_Disease++;
                                RealPhenotypes.Add(PredictionDiseaseData.RelatedEntities.RelatedEntitiesList[j]);
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
                    double PrecisionDisease = 0.0;
                    double RecallDisease = 0.0;
                    double F_ScoreDisease = 0.0;
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

                    ////////////////////
                    //Compute MeanRank//
                    ////////////////////
                    
                    //Compute Ranks
                    Dictionary<RelatedEntity, double> RanksPhenotypes = new Dictionary<RelatedEntity, double>();
                    RanksPhenotypes = RealWeightOfPhenotypes.OrderByDescending(p => p.Value).Select((p, i) => new KeyValuePair<RelatedEntity, double>(p.Key, i + 1.0)).ToDictionary(p=>p.Key, p=>p.Value);

                    //Keep Only real Phenotypes
                    RanksPhenotypes = 
                        RanksPhenotypes
                        .Where(elem => RealPhenotypes.Select(x => x.Name).ToList().IndexOf(elem.Key.Name) != -1)
                        .ToDictionary(p => p.Key, p => p.Value);

                    //MeanRank of Real Phenotypes in one disease
                    if(RanksPhenotypes.Count != 0)
                    {
                        MR_Disease = RanksPhenotypes.Average(p => p.Value);
                    }
                      

                    //Construct results object
                    PerDisease OnePerDisease = new PerDisease(orphaNumber,
                        PredictionDiseaseData.Disease.NumberOfPublications, 
                        PredictionData.Type,
                        NumberOfRelatedEntitiesFound,
                        RP_Disease, 
                        FP_Disease, 
                        FN_Disease,
                        PrecisionDisease,//Precision
                        RecallDisease, //Recall
                        F_ScoreDisease,
                        MR_Disease
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

            //Compute MeanRank general
            double MeanRankRealPositiveGeneral = 0.0;//MeanRank RealPhenotype general

            //Compute standard deviation
            double StandardDeviationRankRealPositivesGeneral = 0.0;

            //Filter PerDisease where MeanRankRealPositives = 0.0
            List<PerDisease> perdiseasesFiltered = results.perDisease.Where(pd => pd.MeanRankRealPositives != 0.0).ToList();

            if (perdiseasesFiltered.Count != 0)
            {
                MeanRankRealPositiveGeneral = perdiseasesFiltered.Average(pd => pd.MeanRankRealPositives);

                StandardDeviationRankRealPositivesGeneral =
                Math.Sqrt
                (
                    perdiseasesFiltered.Average
                    (
                        pd => Math.Pow(pd.MeanRankRealPositives - MeanRankRealPositiveGeneral, 2)
                    )
                );
            }
            


            //Compute MeanNumberOfRelatedEntitiesFound
            double MeanNumberOfRelatedEntitiesFound = results.perDisease.Average(pd => pd.NumberOfRelatedEntitiesFound);

            //Compute standard deviation
            double StandardDeviationNumberOfRelatedEntitiesFound = 
                Math.Sqrt
                (
                    results.perDisease.Average
                    (
                        pd => Math.Pow(pd.NumberOfRelatedEntitiesFound - MeanNumberOfRelatedEntitiesFound, 2)
                    )
                );

            //Construct results object
            results.general = new General(
                DateTime.Now,
                NumberOfDiseasesWithKnownPhenotypes,
                NumberOfDiseasesWithPublicationsInPredictionData,
                NumberOfDiseasesEvaluatedForReal,
                PredictionData.Type,
                MeanNumberOfRelatedEntitiesFound,
                StandardDeviationNumberOfRelatedEntitiesFound,
                WeightCombinaison.Item1,
                WeightCombinaison.Item2,
                threshold,
                RP, 
                FP, 
                FN,  
                Precision,  
                Recall, 
                F_Score,
                MeanRankRealPositiveGeneral,
                StandardDeviationRankRealPositivesGeneral);

            return results;
        }

        public static List<Tuple<TFType, IDFType>> GenerateDisctinctsTupleForWeightComputation()
        {
            List<Tuple<TFType, IDFType>> res = new List<Tuple<TFType, IDFType>>();

            foreach(TFType valEnumTFType in Enum.GetValues(typeof(TFType)))
            {
                foreach (IDFType valEnumIDFType in Enum.GetValues(typeof(IDFType)))
                {
                    res.Add(new Tuple<TFType, IDFType>(valEnumTFType, valEnumIDFType));
                }
            }

            return res;
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
        /*
        public static MetaResults MetaEvaluate(DiseasesData PredictionData, DiseasesData RealData, Tuple<TFType, IDFType> WeightCombinaison, double minWeight, double maxWeight, double step, Criterion criterion)
        {
            //Create MetaResult
            MetaResults metaResults = new MetaResults(WeightCombinaison.Item1, WeightCombinaison.Item2);

            //Compute all results and put them in metaResults
            List<Results> listResults = new List<Results>();
            for (double i = minWeight; i <= maxWeight; i+=step)
            {
                Results currentRes = Evaluate(PredictionData, RealData, WeightCombinaison, i);
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
        }*/

        public static MetaResults MetaEvaluate(DiseasesData PredictionData, DiseasesData RealData, Criterion criterion, params Tuple<TFType, IDFType>[] WeightCombinaisons)
        {
            //Create MetaResult
            MetaResults metaResults = new MetaResults();

            //Compute all results and put them in metaResults
            List<Results> listResults = new List<Results>();

            //If not precised, we generate
            if(WeightCombinaisons.Length == 0)
            {
                WeightCombinaisons = GenerateDisctinctsTupleForWeightComputation().ToArray();
            }

            foreach (var tuple in WeightCombinaisons)
            {
                Results currentRes = Evaluate(PredictionData, RealData, tuple);
                listResults.Add(currentRes);
                metaResults.perCombinaison.Add(
                    new PerCombinaison(
                        currentRes.general.TimeStamp,
                        currentRes.general.NumberOfDiseasesWithKnownPhenotypes,
                        currentRes.general.NumberOfDiseasesWithPublicationsInPredictionData,
                        currentRes.general.NumberOfDiseasesEvaluatedForReal,
                        currentRes.general.Type,
                        currentRes.general.MeanNumberOfRelatedEntitiesFound,
                        currentRes.general.StandardDeviationNumberOfRelatedEntitiesFound,
                        currentRes.general.TFType,
                        currentRes.general.IDFType,
                        currentRes.general.RealPositives,
                        currentRes.general.FalsePositives,
                        currentRes.general.FalseNegatives,
                        currentRes.general.Precision,
                        currentRes.general.Recall,
                        currentRes.general.F_Score,
                        currentRes.general.MeanRankRealPositives,
                        currentRes.general.StandardDeviationRankRealPositivesGeneral,
                        criterion
                        ));
            }

            //Find best results and sort by perCombinaison
            Results Best_Result;
            switch (criterion)
            {
                case Criterion.MeanRankRealPositives:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.MeanRankRealPositives < savedRes.general.MeanRankRealPositives ? currentRes : savedRes);
                    metaResults.perCombinaison = metaResults.perCombinaison.OrderBy(pc => pc.MeanRankRealPositives).ToList();
                    break;
                case Criterion.F_Score:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.F_Score > savedRes.general.F_Score ? currentRes : savedRes);
                    metaResults.perCombinaison = metaResults.perCombinaison.OrderByDescending(pc => pc.F_Score).ToList();
                    break;
                case Criterion.Precision:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.Precision > savedRes.general.Precision ? currentRes : savedRes);
                    metaResults.perCombinaison = metaResults.perCombinaison.OrderByDescending(pc => pc.Precision).ToList();
                    break;
                case Criterion.Recall:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.Recall > savedRes.general.Recall ? currentRes : savedRes);
                    metaResults.perCombinaison = metaResults.perCombinaison.OrderByDescending(pc => pc.Recall).ToList();
                    break;
                default:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.MeanRankRealPositives < savedRes.general.MeanRankRealPositives ? currentRes : savedRes);
                    metaResults.perCombinaison = metaResults.perCombinaison.OrderBy(pc => pc.MeanRankRealPositives).ToList();
                    break;
            }

            //Complete metaResults
            metaResults.bestInfos = new BestInfos(
                    Best_Result.general.TimeStamp,
                        Best_Result.general.NumberOfDiseasesWithKnownPhenotypes,
                        Best_Result.general.NumberOfDiseasesWithPublicationsInPredictionData,
                        Best_Result.general.NumberOfDiseasesEvaluatedForReal,
                        Best_Result.general.Type,
                        Best_Result.general.MeanNumberOfRelatedEntitiesFound,
                        Best_Result.general.StandardDeviationNumberOfRelatedEntitiesFound,
                        Best_Result.general.TFType,
                        Best_Result.general.IDFType,
                        Best_Result.general.RealPositives,
                        Best_Result.general.FalsePositives,
                        Best_Result.general.FalseNegatives,
                        Best_Result.general.Precision,
                        Best_Result.general.Recall,
                        Best_Result.general.F_Score,
                        Best_Result.general.MeanRankRealPositives,
                        Best_Result.general.StandardDeviationRankRealPositivesGeneral,
                        criterion
                );

            return metaResults;
        }

        public static void WriteMetaResultsJSONFile(
        MetaResults metaResults,
        string wantedFileName = "")
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
                fileName = $"metaResults_{metaResults.bestInfos.TimeStamp.ToString("yyyy-MM-dd_HH-mm-ss")}.json";
            }
            File.WriteAllText(ConfigurationManager.Instance.config.ResultsFolder + fileName, output);
        }

        public static void WriteMetaResultsWeightJSONFile(
        MetaResultsWeight metaResultsWeight,
        string wantedFileName = "")
        {
            string output = JsonConvert.SerializeObject(metaResultsWeight, Formatting.Indented);

            //Choose the name
            string fileName = "";
            if (wantedFileName != "")
            {
                fileName = wantedFileName;
            }
            else
            {
                fileName = $"metaResultsWeight_{metaResultsWeight.bestThreshold.TimeStamp.ToString("yyyy-MM-dd_HH-mm-ss")}.json";
            }
            File.WriteAllText(ConfigurationManager.Instance.config.ResultsFolder + fileName, output);
        }

        public static void WriteListOfMetaResultsJSONFile( List<MetaResults> listMetaResults)
        {
            foreach (var metaResults in listMetaResults)
            {
                WriteMetaResultsJSONFile(metaResults);
            }
        }

        public static void WriteListOfResultsJSONFile(List<Results> listResults)
        {
            Console.WriteLine($"listResults.Count: {listResults.Count}");
            foreach (var Results in listResults)
            {
                WriteResultsJSONFile(Results);
            }
        }

        public static List<Results> EvaluateMultipleFormulas(
            DiseasesData PredictionData, 
            DiseasesData RealData,
            params Tuple<TFType, IDFType>[] Combinaisons)
        {
            List<Results> listResults = new List<Results>();
            if(Combinaisons.Length == 0)
            {
                List<Tuple<TFType, IDFType>> ListOfWeightCombinaisons = GenerateDisctinctsTupleForWeightComputation();
                foreach (var element in ListOfWeightCombinaisons)
                {
                    listResults.Add(Evaluate(PredictionData, RealData, element));
                }
            }
            else
            {
                foreach (var element in Combinaisons)
                {
                    listResults.Add(Evaluate(PredictionData, RealData, element));
                }
            }
            return listResults;
        }

        public static MetaResultsWeight MetaWeightEvaluate(
            DiseasesData PredictionData, 
            DiseasesData RealData,
            Tuple<TFType, IDFType> tuple,
            double pas,
            Criterion criterion)
        {
            //Create MetaResult
            MetaResultsWeight metaResultsWeight = new MetaResultsWeight();

            //Compute all results and put them in metaResults
            List<Results> listResults = new List<Results>();

            for(double i = 0.00; i < 0.17; i += pas)
            {
                Results currentRes = Evaluate(PredictionData, RealData, tuple, i);
                listResults.Add(currentRes);
                metaResultsWeight.perThreshold.Add(
                    new PerThreshold(
                        currentRes.general.TimeStamp,
                        currentRes.general.NumberOfDiseasesWithKnownPhenotypes,
                        currentRes.general.NumberOfDiseasesWithPublicationsInPredictionData,
                        currentRes.general.NumberOfDiseasesEvaluatedForReal,
                        currentRes.general.Type,
                        currentRes.general.MeanNumberOfRelatedEntitiesFound,
                        currentRes.general.StandardDeviationNumberOfRelatedEntitiesFound,
                        currentRes.general.TFType,
                        currentRes.general.IDFType,
                        currentRes.general.WeightThreshold,
                        currentRes.general.RealPositives,
                        currentRes.general.FalsePositives,
                        currentRes.general.FalseNegatives,
                        currentRes.general.Precision,
                        currentRes.general.Recall,
                        currentRes.general.F_Score,
                        currentRes.general.MeanRankRealPositives,
                        currentRes.general.StandardDeviationRankRealPositivesGeneral,
                        criterion
                        ));
            }

            //Find best results and sort by perCombinaison
            Results Best_Result;
            switch (criterion)
            {
                case Criterion.MeanRankRealPositives:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.MeanRankRealPositives < savedRes.general.MeanRankRealPositives ? currentRes : savedRes);
                    metaResultsWeight.perThreshold = metaResultsWeight.perThreshold.OrderBy(pc => pc.MeanRankRealPositives).ToList();
                    break;
                case Criterion.F_Score:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.F_Score > savedRes.general.F_Score ? currentRes : savedRes);
                    metaResultsWeight.perThreshold = metaResultsWeight.perThreshold.OrderByDescending(pc => pc.F_Score).ToList();
                    break;
                case Criterion.Precision:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.Precision > savedRes.general.Precision ? currentRes : savedRes);
                    metaResultsWeight.perThreshold = metaResultsWeight.perThreshold.OrderByDescending(pc => pc.Precision).ToList();
                    break;
                case Criterion.Recall:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.Recall > savedRes.general.Recall ? currentRes : savedRes);
                    metaResultsWeight.perThreshold = metaResultsWeight.perThreshold.OrderByDescending(pc => pc.Recall).ToList();
                    break;
                default:
                    Best_Result = listResults.Aggregate((savedRes, currentRes) => currentRes.general.MeanRankRealPositives < savedRes.general.MeanRankRealPositives ? currentRes : savedRes);
                    metaResultsWeight.perThreshold = metaResultsWeight.perThreshold.OrderBy(pc => pc.MeanRankRealPositives).ToList();
                    break;
            }

            //Complete metaResults
            metaResultsWeight.bestThreshold = new BestThreshold(
                    Best_Result.general.TimeStamp,
                        Best_Result.general.NumberOfDiseasesWithKnownPhenotypes,
                        Best_Result.general.NumberOfDiseasesWithPublicationsInPredictionData,
                        Best_Result.general.NumberOfDiseasesEvaluatedForReal,
                        Best_Result.general.Type,
                        Best_Result.general.MeanNumberOfRelatedEntitiesFound,
                        Best_Result.general.StandardDeviationNumberOfRelatedEntitiesFound,
                        Best_Result.general.TFType,
                        Best_Result.general.IDFType,
                        Best_Result.general.WeightThreshold,
                        Best_Result.general.RealPositives,
                        Best_Result.general.FalsePositives,
                        Best_Result.general.FalseNegatives,
                        Best_Result.general.Precision,
                        Best_Result.general.Recall,
                        Best_Result.general.F_Score,
                        Best_Result.general.MeanRankRealPositives,
                        Best_Result.general.StandardDeviationRankRealPositivesGeneral,
                        criterion
                );

            return metaResultsWeight;
        }
    }
}
