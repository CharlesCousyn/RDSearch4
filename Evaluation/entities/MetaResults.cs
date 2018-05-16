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
    public class MetaResults
    {
        public BestInfos bestInfos { get; set; }
        public List<PerCombinaison> perCombinaison { get; set; }

        public MetaResults(BestInfos bestInfosP, List<PerCombinaison> perCombinaisonP)
        {
            bestInfos = bestInfosP;
            perCombinaison = perCombinaisonP;
        }
        public MetaResults()
        {
            perCombinaison = new List<PerCombinaison>();
        }
    }

    public class BestInfos
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

        public BestInfos(DateTime TimeStampP,
            int NumberOfDiseasesWithKnownPhenotypesP,
            int NumberOfDiseasesWithPublicationsInPredictionDataP,
            int NumberOfDiseasesEvaluatedForRealP,
            type TypeP, double MeanNumberOfRelatedEntitiesFoundP, double StandardDeviationNumberOfRelatedEntitiesFoundP, TFType TFTypeP, IDFType IDFTypeP, int RealPositivesP, int FalsePositivesP, int FalseNegativesP,
            double PrecisionP, double RecallP, double F_ScoreP, double MeanRankRealPositivesP,double StandardDeviationRankRealPositivesGeneralP, Criterion CriterionP)
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

    public class PerCombinaison
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

        public PerCombinaison(DateTime TimeStampP,
            int NumberOfDiseasesWithKnownPhenotypesP,
            int NumberOfDiseasesWithPublicationsInPredictionDataP,
            int NumberOfDiseasesEvaluatedForRealP,
            type TypeP, double MeanNumberOfRelatedEntitiesFoundP, double StandardDeviationNumberOfRelatedEntitiesFoundP, TFType TFTypeP, IDFType IDFTypeP, int RealPositivesP, int FalsePositivesP, int FalseNegativesP,
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

    public enum Criterion
    {
        F_Score,
        Precision,
        Recall,
        MeanRankRealPositives
    }
}
