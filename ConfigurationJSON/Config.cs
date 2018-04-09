using System;
using System.Collections.Generic;
using System.Text;

namespace ConfigurationJSON
{
    public class Config
    {
        public string ResultsFolder { get; set; }
        public string URL_Rare_Diseases { get; set; }
        public string URL_SymptomsList { get; set; }
        public string URL_RealSymptomsByDisease { get; set; }
        public string ParametersAPI { get; set; }
        public string API_Key { get; set; }
        public string Tool { get; set; }
        public string Email { get; set; }
        public int BatchSizeDiseases { get; set; }
        public int BatchSizePMC { get; set; }
        public int BatchSizeTextMining { get; set; }
        public int MaxNumberSymptoms { get; set; }
    }
}
