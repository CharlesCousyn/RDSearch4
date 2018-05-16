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

    //NbDisease_i: Number of disease where phenotype i appears
    //SumOfMinMaxNorm_i: Sum of RawCounts Normalized (MinMaxNorm) of phenotype i in all diseases
    public enum IDFType
    {
        Unary,                          //Always 1.0

        NbDisease_i,                    //NbDisease_i
        Inverse_NbDisease_i,            //(totalNumberOfDisease / NbDisease_i)
        IDF_Classic_NbDisease_i,        //Log10(totalNumberOfDisease / NbDisease_i);
        IDF_Smooth_NbDisease_i,         //Log10(1.0 + (totalNumberOfDisease / NbDisease_i));
        Prob_IDF_NbDisease_i,           //Log10((totalNumberOfDisease-NbDisease_i) / NbDisease_i);
        
        SumOfMinMaxNorm_i,              //SumOfMinMaxNorm_i
        Inverse_SumOfMinMaxNorm_i,      //(1 / SumOfMinMaxNorm_i)
        IDF_Classic_SumOfMinMaxNorm_i,  //Log10(1/SumOfMinMaxNorm_i);
        IDF_Smooth_SumOfMinMaxNorm_i,   //Log10(1.0 + (1 / SumOfMinMaxNorm_i));
        Prob_IDF_SumOfMinMaxNorm_i,     //Log10((1 - SumOfMinMaxNorm_i) / SumOfMinMaxNorm_i);
    }
}
