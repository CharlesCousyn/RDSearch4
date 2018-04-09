using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MongoRepository.entities
{
    public class Disease
    {
        public ObjectId _id { get; set; }

        public string Name { get; set; }

        public string OrphaNumber { get; set; }

        public List<string> Synonyms { get; set; }

        public List<TextualInformation> TextualInformationList { get; set; }

        #region EXPERT VALUES

        public string Prevalence { get; set; }

        public string Inheritance { get; set; }

        public List<string> AgesOnSet { get; set; }

        public string OMIMID { get; set; }

        public string OMIMUrl { get; set; }

        public string ICDTen { get; set; }

        public string UMLS { get; set; }

        public string MeSH { get; set; }

        public string GARD { get; set; }

        public string MedDRA { get; set; }

        public string Epidemiology { get; set; }

        public string ClinicalDescription { get; set; }

        public string Etiology { get; set; }

        public string GeneticCounseling { get; set; }
        #endregion

        public List<Symptom> symptoms { get; set; }
        public int NumberOfPublications { get; set; }

        public Disease(string OrphaNumberP, string NameP)
        {
            symptoms = new List<Symptom>();
            Synonyms = new List<string>();
            TextualInformationList = new List<TextualInformation>();
            NumberPublicationsPerYear = new List<NumberPublicationsForOneYear>();
            OrphaNumber = OrphaNumberP;
            Name = NameP;
            Synonyms = new List<string>();
        }

        public Disease(string OrphaNumberP, string NameP, int NumberOfPublicationsP)
        {
            symptoms = new List<Symptom>();
            Synonyms = new List<string>();
            TextualInformationList = new List<TextualInformation>();
            NumberPublicationsPerYear = new List<NumberPublicationsForOneYear>();
            OrphaNumber = OrphaNumberP;
            Name = NameP;
            Synonyms = new List<string>();
            NumberOfPublications = NumberOfPublicationsP;
        }

        public Disease(string OrphaNumberP, string NameP, List<string> SynonymsP)
        {
            symptoms = new List<Symptom>();
            Synonyms = new List<string>();
            TextualInformationList = new List<TextualInformation>();
            NumberPublicationsPerYear = new List<NumberPublicationsForOneYear>();
            OrphaNumber = OrphaNumberP;
            Name = NameP;
            Synonyms = SynonymsP;
        }

        public List<NumberPublicationsForOneYear> NumberPublicationsPerYear { get; set; }

        public Disease()
        {
            symptoms = new List<Symptom>();
            Synonyms = new List<string>();
            TextualInformationList = new List<TextualInformation>();
            NumberPublicationsPerYear = new List<NumberPublicationsForOneYear>();
        }

    }
}
