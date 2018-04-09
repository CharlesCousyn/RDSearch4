using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoRepository.entities
{
    public class Config
    {
        public ObjectId _id { get; set; }

        public DateTime lastUpdateDateOfDiseasesList { get;set;}
    }
}
