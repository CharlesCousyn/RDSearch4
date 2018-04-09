using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoRepository.entities
{
    public class RelatedEntity
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public type Type { get; set; }
        public string Name { get; set; }
        public double Weight { get; set; }
        public List<string> Synonyms {get; set;}

        public RelatedEntity(type TypeP, string NameP, double WeightP)
        {
            Type = TypeP;
            Name = NameP;
            Weight = WeightP;
            Synonyms = new List<string>();
        }

        public RelatedEntity(type TypeP, string NameP, double WeightP, List<string> SynonymsP)
        {
            Type = TypeP;
            Name = NameP;
            Weight = WeightP;
            Synonyms = SynonymsP;
        }
    }
}
