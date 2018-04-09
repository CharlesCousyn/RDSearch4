using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MongoRepository.entities
{
    public class TextualInformation
    {
        public string title { get; set; }

        public string content { get; set; }

        public string sourceName { get; set; }

        public string sourceLink { get; set; }
    }
}
