using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MongoRepository.entities
{
    public class Publication
    {
        public ObjectId _id { get; set; }

        public long idPubmed { get; set; }

        public long idPMC { get; set; }

        public string doi { get; set; }

        public String title { get; set; }

        public String abstractText { get; set; }

        public String fullText { get; set; }

        public DateTime datePublication { get; set; }

        public List<String> authors { get; set; }

        public int timesCited { get; set; }

        public Publication()
        {
            authors = new List<String>();
        }

        public String orphaNumberOfLinkedDisease { get; set; }
    }
}
