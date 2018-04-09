using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoRepository.entities
{
    public class DiseasesData
    {
        public ObjectId _id { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public type Type { get; set; }
        public List<DiseaseData> DiseaseDataList { get; set; }

        public DiseasesData(type TypeP, List<DiseaseData> DiseaseDataListP)
        {
            Type = Type;
            DiseaseDataList = DiseaseDataListP;
        }
    }
}
