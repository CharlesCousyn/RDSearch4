using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoRepository.entities
{
    public class Symptom
    {
        public ObjectId _id { get; set; }

        public string Name { get; set; }

        public string OrphaNumber { get; set; }

        public double Weight { get; set; }

        public List<string> Synonyms { get; set; }

        public Symptom()
        {
            Synonyms = new List<string>();
        }

        public void Show()
        {
            Console.WriteLine("//////////////////////////");
            Console.WriteLine("Name=" + Name);
            Console.WriteLine("OrphaNumber=" + OrphaNumber);
            Console.WriteLine("Weight=" + Weight);
            Console.WriteLine("Synonyms=" + Synonyms.ToString());
            Console.WriteLine("//////////////////////////");
        }

    }
}
