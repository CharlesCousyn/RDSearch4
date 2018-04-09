using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ConfigurationJSON
{
    public class ConfigurationManager
    {
        public Config config;

        private static ConfigurationManager _instance;

        public static ConfigurationManager Instance
        {
            get
            {
                if(_instance == null)
                {
                    _instance = new ConfigurationManager();
                }
                return _instance;
            }
            protected set { _instance = value; }
        }

        private ConfigurationManager()
        {

        }

        public void Init(string path)
        {
            var realPath = $"{path}\\settings.json";
            if (path.EndsWith("settings.json"))
            {
                realPath=$"{path}";
            }
            using (StreamReader r = new StreamReader(realPath))
            {
                config = JsonConvert.DeserializeObject<Config>(r.ReadToEnd());
            }
        }
    }
}
