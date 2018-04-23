using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoRepository.entities
{
    public class IDF
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public IDFType IDFType { get; set; }
        public double Value { get; set; }

        public IDF(IDFType IDFTypeP, double ValueP)
        {
            IDFType = IDFTypeP;
            Value = ValueP;
        }
    }

    public enum IDFType
    {
        Unary,
        IDF_Classic,
        IDF_Smooth,
        Prob_IDF
    }
}
