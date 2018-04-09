using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CrawlerOrphanet.tools
{
    class PotentialSymptom
    {
        public string IdHPO { get; set; }

        public string SuperClassId { get; set; }

        public string Name { get; set; }

        public string OrphaNumber { get; set; }

        public double Weight { get; set; }

        public List<string> Synonyms { get; set; }

        public PotentialSymptom()
        {
            Synonyms = new List<string>();
        }
    }
}
