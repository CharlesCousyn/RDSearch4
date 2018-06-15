using MongoRepository.entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evaluation.entities
{
    public class MetaResultsWeight
    {
        public BestThreshold bestThreshold { get; set; }
        public List<PerThreshold> perThreshold { get; set; }

        public MetaResultsWeight(BestThreshold bestThresholdP, List<PerThreshold> perThresholdP)
        {
            bestThreshold = bestThresholdP;
            perThreshold = perThresholdP;
        }
        public MetaResultsWeight()
        {
            perThreshold = new List<PerThreshold>();
        }
    }

    public class BestThreshold
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
        public TFType TFType;
        [JsonConverter(typeof(StringEnumConverter))]
        public IDFType IDFType;
        public double Threshold { get; set; }
        public int RealPositives { get; set; }
        public int FalsePositives { get; set; }
        public int FalseNegatives { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F_Score { get; set; }
        public double MeanRankRealPositives { get; set; }
        public double StandardDeviationRankRealPositivesGeneral { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Criterion Criterion { get; set; }

        public BestThreshold(DateTime TimeStampP,
            int NumberOfDiseasesWithKnownPhenotypesP,
            int NumberOfDiseasesWithPublicationsInPredictionDataP,
            int NumberOfDiseasesEvaluatedForRealP,
            type TypeP, double MeanNumberOfRelatedEntitiesFoundP, double StandardDeviationNumberOfRelatedEntitiesFoundP, TFType TFTypeP, IDFType IDFTypeP, double ThresholdP, int RealPositivesP, int FalsePositivesP, int FalseNegativesP,
            double PrecisionP, double RecallP, double F_ScoreP, double MeanRankRealPositivesP, double StandardDeviationRankRealPositivesGeneralP, Criterion CriterionP)
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
            Threshold = ThresholdP;
            RealPositives = RealPositivesP;
            FalsePositives = FalsePositivesP;
            FalseNegatives = FalseNegativesP;
            Precision = PrecisionP;
            Recall = RecallP;
            F_Score = F_ScoreP;
            MeanRankRealPositives = MeanRankRealPositivesP;
            StandardDeviationRankRealPositivesGeneral = StandardDeviationRankRealPositivesGeneralP;
            Criterion = CriterionP;
        }
    }

    public class PerThreshold
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
        public TFType TFType;
        [JsonConverter(typeof(StringEnumConverter))]
        public IDFType IDFType;
        public double Threshold { get; set; }
        public int RealPositives { get; set; }
        public int FalsePositives { get; set; }
        public int FalseNegatives { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F_Score { get; set; }
        public double MeanRankRealPositives { get; set; }
        public double StandardDeviationRankRealPositivesGeneral { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        public Criterion Criterion { get; set; }

        public PerThreshold(DateTime TimeStampP,
            int NumberOfDiseasesWithKnownPhenotypesP,
            int NumberOfDiseasesWithPublicationsInPredictionDataP,
            int NumberOfDiseasesEvaluatedForRealP,
            type TypeP, double MeanNumberOfRelatedEntitiesFoundP, double StandardDeviationNumberOfRelatedEntitiesFoundP, TFType TFTypeP, IDFType IDFTypeP, double ThresholdP, int RealPositivesP, int FalsePositivesP, int FalseNegativesP,
            double PrecisionP, double RecallP, double F_ScoreP, double MeanRankRealPositivesP, double StandardDeviationRankRealPositivesGeneralP, Criterion CriterionP)
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
            Threshold = ThresholdP;
            RealPositives = RealPositivesP;
            FalsePositives = FalsePositivesP;
            FalseNegatives = FalseNegativesP;
            Precision = PrecisionP;
            Recall = RecallP;
            F_Score = F_ScoreP;
            MeanRankRealPositives = MeanRankRealPositivesP;
            StandardDeviationRankRealPositivesGeneral = StandardDeviationRankRealPositivesGeneralP;
            Criterion = CriterionP;
        }
    }
}
