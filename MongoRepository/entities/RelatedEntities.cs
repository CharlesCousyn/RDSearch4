using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace MongoRepository.entities
{
    public class RelatedEntities
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public type Type { get; set; }
        public List<RelatedEntity> RelatedEntitiesList { get; set; }

        public RelatedEntities(type TypeP, List<RelatedEntity> RelatedEntitiesListP)
        {
            Type = TypeP;
            RelatedEntitiesList = RelatedEntitiesListP;
        }
    }
}
