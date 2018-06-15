using MongoRepository.entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evaluation
{
    public class Results
    {
        public General general { get; set; }
        public List<PerDisease> perDisease { get; set; }

        public Results(General generalP, List<PerDisease> perDiseaseP)
        {
            general = generalP;
            perDisease = perDiseaseP;
        }
        public Results()
        {
            perDisease = new List<PerDisease>();
        }
    }

    public class General
    {
        public DateTime TimeStamp { get; set; }
        public int NumberOfDiseasesWithKnownPhenotypes { get; set; }
        public int NumberOfDiseasesWithPublicationsInPredictionData { get; set; }
        public int NumberOfDiseasesEvaluatedForReal { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public type Type { get; set; }
        public double MeanNumberOfRelatedEntitiesFound { get; set; }
        public double StandardDeviationNumberOfRelatedEntitiesFound { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public TFType TFType { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public IDFType IDFType { get; set; }
        public double WeightThreshold { get; set; }
        public int RealPositives { get; set; }
        public int FalsePositives { get; set; }
        public int FalseNegatives { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F_Score { get; set; }
        public double MeanRankRealPositives { get; set; }
        public double StandardDeviationRankRealPositivesGeneral { get; set; }

        public General(DateTime TimeStampP,
            int NumberOfDiseasesWithKnownPhenotypesP, 
            int NumberOfDiseasesWithPublicationsInPredictionDataP,
            int NumberOfDiseasesEvaluatedForRealP,
            type TypeP, double MeanNumberOfRelatedEntitiesFoundP, double StandardDeviationNumberOfRelatedEntitiesFoundP, TFType TFTypeP, IDFType IDFTypeP, double WeightThresholdP,
            int RealPositivesP, int FalsePositivesP, int FalseNegativesP, 
            double PrecisionP, double RecallP, double F_ScoreP, double MeanRankRealPositivesP, double StandardDeviationRankRealPositivesGeneralP)
        {
            TimeStamp = TimeStampP;
            NumberOfDiseasesWithKnownPhenotypes = NumberOfDiseasesWithKnownPhenotypesP;
            NumberOfDiseasesWithPublicationsInPredictionData = NumberOfDiseasesWithPublicationsInPredictionDataP;
            NumberOfDiseasesEvaluatedForReal = NumberOfDiseasesEvaluatedForRealP;
            Type = TypeP;
            MeanNumberOfRelatedEntitiesFound = MeanNumberOfRelatedEntitiesFoundP;
            StandardDeviationNumberOfRelatedEntitiesFound = StandardDeviationNumberOfRelatedEntitiesFoundP;
            TFType = TFTypeP;
            IDFType = IDFTypeP;
            WeightThreshold = WeightThresholdP;
            RealPositives = RealPositivesP;
            FalsePositives = FalsePositivesP;
            FalseNegatives = FalseNegativesP;
            Precision = PrecisionP;
            Recall = RecallP;
            F_Score = F_ScoreP;
            MeanRankRealPositives = MeanRankRealPositivesP;
            StandardDeviationRankRealPositivesGeneral = StandardDeviationRankRealPositivesGeneralP;
        }
    }

    public class PerDisease
    {
        public string OrphaNumber { get; set; }
        public int NumberOfPublications { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public type Type { get; set; }
        public int NumberOfRelatedEntitiesFound { get; set; }
        public int RealPositives { get; set; }
        public int FalsePositives { get; set; }
        public int FalseNegatives { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F_Score { get; set; }
        public double MeanRankRealPositives { get; set; }
        

        public PerDisease(string OrphaNumberP, int NumberOfPublicationsP, type TypeP, int NumberOfRelatedEntitiesFoundP, int RealPositivesP, 
            int FalsePositivesP, int FalseNegativesP, double PrecisionP, double RecallP, double F_ScoreP, double MeanRankRealPositivesP)
        {
            OrphaNumber = OrphaNumberP;
            NumberOfPublications = NumberOfPublicationsP;
            Type = TypeP;
            NumberOfRelatedEntitiesFound = NumberOfRelatedEntitiesFoundP;
            RealPositives = RealPositivesP;
            FalsePositives = FalsePositivesP;
            FalseNegatives = FalseNegativesP;
            Precision = PrecisionP;
            Recall = RecallP;
            F_Score = F_ScoreP;
            MeanRankRealPositives = MeanRankRealPositivesP;
        }
    }


}
