using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MongoRepository.entities
{
    public class RelatedEntity
    {
        [JsonConverter(typeof(StringEnumConverter))]
        [BsonRepresentation(BsonType.String)]
        public type Type { get; set; }
        public string Name { get; set; }
        public List<TermFrequency> TermFrequencies { get; set; }
        public List<IDF> IDFs { get; set; }
        public double Weight { get; set; }
        public List<string> Synonyms {get; set;}

        public RelatedEntity(type TypeP, string NameP, double WeightP)
        {
            Type = TypeP;
            Name = NameP;
            Weight = WeightP;
            Synonyms = new List<string>();
            InitTFAndIdf();
        }

        public RelatedEntity(type TypeP, string NameP, double WeightP, List<string> SynonymsP)
        {
            Type = TypeP;
            Name = NameP;
            Weight = WeightP;
            Synonyms = SynonymsP;
            InitTFAndIdf();
        }

        public double CalcFinalWeight(TFType tfType, IDFType idfType)
        {
            double tf = TermFrequencies.Where(Onetf => Onetf.TFType == tfType).FirstOrDefault().Value;
            double idf = IDFs.Where(OneIDF => OneIDF.IDFType == idfType).FirstOrDefault().Value;

            return tf*idf;
        }

        private void InitTFAndIdf()
        {
            TermFrequencies = new List<TermFrequency>();
            IDFs = new List<IDF>();
            foreach (IDFType type in Enum.GetValues(typeof(IDFType)))
            {
                IDFs.Add(new IDF(type, 0.0));
            }
            foreach (TFType type in Enum.GetValues(typeof(TFType)))
            {
                TermFrequencies.Add(new TermFrequency(type, 0.0));
            }
        }
    }
}
