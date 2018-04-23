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
        [JsonConverter(typeof(StringEnumConverter))]
        public TFType TFType;
        [JsonConverter(typeof(StringEnumConverter))]
        public IDFType IDFType;
        public BestInfos bestInfos { get; set; }
        public List<PerThreshold> perThreshold { get; set; }

        public MetaResults(TFType TFTypeP, IDFType IDFTypeP, BestInfos bestInfosP, List<PerThreshold> perThresholdP)
        {
            TFType = TFTypeP;
            IDFType = IDFTypeP;
            bestInfos = bestInfosP;
            perThreshold = perThresholdP;
        }
        public MetaResults(TFType TFTypeP, IDFType IDFTypeP)
        {
            TFType = TFTypeP;
            IDFType = IDFTypeP;
            perThreshold = new List<PerThreshold>();
        }
    }

    public class BestInfos
    {
        public DateTime TimeStamp { get; set; }
        public string Type { get; set; }
        public double Best_Threshold { get; set; }
        public double Best_Precision { get; set; }
        public double Best_Recall { get; set; }
        public double Best_F_Score { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Criterion Criterion { get; set; }

        public BestInfos(DateTime TimeStampP,
            string TypeP, double Best_ThresholdP,
            double Best_PrecisionP, double Best_RecallP, double Best_F_ScoreP,
            Criterion CriterionP)
        {
            TimeStamp = TimeStampP;
            Type = TypeP;
            Best_Threshold = Best_ThresholdP;
            Best_Precision = Best_PrecisionP;
            Best_Recall = Best_RecallP;
            Best_F_Score = Best_F_ScoreP;
            Criterion = CriterionP;
        }
    }

    public class PerThreshold
    {
        public double Threshold { get; set; }
        public string Type { get; set; }
        public int RealPositives { get; set; }
        public int FalsePositives { get; set; }
        public int FalseNegatives { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F_Score { get; set; }

        public PerThreshold(double ThresholdP, string TypeP, int RealPositivesP,
            int FalsePositivesP, int FalseNegativesP, double PrecisionP, double RecallP, double F_ScoreP)
        {
            Threshold = ThresholdP;
            Type = TypeP;
            RealPositives = RealPositivesP;
            FalsePositives = FalsePositivesP;
            FalseNegatives = FalseNegativesP;
            Precision = PrecisionP;
            Recall = RecallP;
            F_Score = F_ScoreP;
        }
    }

    public enum Criterion
    {
        F_Score,
        Precision,
        Recall
    }
}
