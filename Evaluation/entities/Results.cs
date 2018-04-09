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
        public string Type { get; set; }
        public int RealPositives { get; set; }
        public int FalsePositives { get; set; }
        public int FalseNegatives { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F_Score { get; set; }

        public General(DateTime TimeStampP, string TypeP, int RealPositivesP, int FalsePositivesP, int FalseNegativesP, 
            double PrecisionP, double RecallP, double F_ScoreP)
        {
            TimeStamp = TimeStampP;
            Type = TypeP;
            RealPositives = RealPositivesP;
            FalsePositives = FalsePositivesP;
            FalseNegatives = FalseNegativesP;
            Precision = PrecisionP;
            Recall = RecallP;
            F_Score = F_ScoreP;
        }
    }

    public class PerDisease
    {
        public string OrphaNumber { get; set; }
        public int NumberOfPublications { get; set; }
        public string Type { get; set; }
        public int RealPositives { get; set; }
        public int FalsePositives { get; set; }
        public int FalseNegatives { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
        public double F_Score { get; set; }

        public PerDisease(string OrphaNumberP, int NumberOfPublicationsP, string TypeP, int RealPositivesP, 
            int FalsePositivesP, int FalseNegativesP, double PrecisionP, double RecallP, double F_ScoreP)
        {
            OrphaNumber = OrphaNumberP;
            NumberOfPublications = NumberOfPublicationsP;
            Type = TypeP;
            RealPositives = RealPositivesP;
            FalsePositives = FalsePositivesP;
            FalseNegatives = FalseNegativesP;
            Precision = PrecisionP;
            Recall = RecallP;
            F_Score = F_ScoreP;
        }
    }


}
