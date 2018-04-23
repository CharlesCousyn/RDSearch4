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
    public class TermFrequency
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public TFType TFType { get; set; }
        public double Value { get; set; }

        public TermFrequency(TFType TFTypeP, double ValueP)
        {
            TFType = TFTypeP;
            Value = ValueP;
        }
    }

    public enum TFType
    {
        Binary,
        RawCount,
        //TF_Classic,
        LogNorm
    }
}
