using ConfigurationJSON;
using CrawlerOrphanet.tools;
using MongoRepository.entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CrawlerOrphanet
{
    public class PhenotypeEngine
    {
        public List<Symptom> SymptomsList { get; set; }

        private HttpClient client;

        public PhenotypeEngine()
        {
            client = new HttpClient();
            SymptomsList = new List<Symptom>();
        }

        public void GetSymptomsList()
        {
            Console.WriteLine("Retriveing symptomsNamesList ...");
            using (HttpResponseMessage res = client.GetAsync($"{ConfigurationManager.Instance.config.URL_SymptomsList}").Result)
            using (HttpContent content = res.Content)
            using (var stringReader = new StringReader(content.ReadAsStringAsync().Result))
            {
                List<PotentialSymptom> potentialSymptoms = new List<PotentialSymptom>();

                string line;

                //Iterate throught all lines
                while ((line = stringReader.ReadLine()) != null)
                {
                    if (line.Length >= 6 && line.Substring(0, 6).Equals("[Term]"))
                    {
                        //Loop for one symptom
                        PotentialSymptom myPotentialSymptom = new PotentialSymptom();
                        while ((line = stringReader.ReadLine()) != null && !line.Equals("") && !line.Equals("\n"))
                        {
                            string name = "";
                            //idHPO
                            if (line.Length > 4 && line.Substring(0, 4).Equals("id: "))
                            {
                                myPotentialSymptom.IdHPO = line.Substring(4, 10);
                            }
                            else if (line.Length >= 6 && line.Substring(0, 6).Equals("name: "))
                            {
                                name = line.Substring(6).ToLower();
                                if (!name.Equals(""))
                                {
                                    Regex rgx = new Regex("/[^A-Za-z0-9\\s]/g");
                                    myPotentialSymptom.Name = rgx.Replace(name, "").ToLower();
                                }
                            }
                            //synonyms
                            else if (line.Length > 10 && line.Substring(0, 10).Equals("synonym: \""))
                            {
                                int index = 10;
                                Char monChar = line[10];
                                do
                                {
                                    monChar = line[index];
                                    if (!monChar.Equals('\"'))
                                    {
                                        name += monChar;
                                    }

                                    index++;
                                } while (!monChar.Equals('\"') && index < line.Length);//Char different from "

                                if (!name.Equals(""))
                                {
                                    Regex rgx = new Regex("/[^A-Za-z0-9\\s]/g");
                                    myPotentialSymptom.Synonyms.Add(rgx.Replace(name, "").ToLower());
                                }
                            }//superClass
                            else if (line.Length > 6 && line.Substring(0, 6).Equals("is_a: "))
                            {
                                myPotentialSymptom.SuperClassId = line.Substring(6, 10);
                            }
                        }

                        //PreTest
                        if (myPotentialSymptom.Name != null && !myPotentialSymptom.Name.Equals(""))
                        {
                            potentialSymptoms.Add(myPotentialSymptom);
                        }
                    }

                }


                //Good id
                string goodIdInParentNode = "HP:0000118";//Phenotypic abnormality

                //Filtration from bad ids
                List<Symptom> goodSymptoms = new List<Symptom>();
                foreach (PotentialSymptom myPotentialSymptom in potentialSymptoms)
                {
                    //Good id is not a valid symptom (only his children), so we avoid it
                    if (!myPotentialSymptom.IdHPO.Equals(goodIdInParentNode))
                    {
                        //Copy potential symptom to initiate iterating
                        PotentialSymptom temporaryPotentialSymptom = new PotentialSymptom();
                        temporaryPotentialSymptom.IdHPO = myPotentialSymptom.IdHPO;
                        temporaryPotentialSymptom.Name = myPotentialSymptom.Name;
                        temporaryPotentialSymptom.SuperClassId = myPotentialSymptom.SuperClassId;
                        temporaryPotentialSymptom.Synonyms = myPotentialSymptom.Synonyms;

                        //Iterate throught Parents and Check if it's a Valid symptom
                        bool hasASuperClass = temporaryPotentialSymptom.SuperClassId != null;
                        bool validSymptom = false;
                        while (hasASuperClass && !validSymptom)
                        {
                            if (temporaryPotentialSymptom.SuperClassId == null)
                            {
                                hasASuperClass = false;
                            }
                            else
                            {
                                //If valid id
                                if (temporaryPotentialSymptom.SuperClassId.Equals(goodIdInParentNode))
                                {
                                    validSymptom = true;
                                }
                                else
                                {
                                    //Contniue searching
                                    temporaryPotentialSymptom = potentialSymptoms.Where(x => x.IdHPO.Equals(temporaryPotentialSymptom.SuperClassId)).First();
                                }
                            }
                        }

                        if (validSymptom)
                        {
                            //Conversion from potential to real symptom
                            Symptom symptom = new Symptom();
                            symptom.Name = myPotentialSymptom.Name;
                            symptom.Synonyms = myPotentialSymptom.Synonyms;
                            SymptomsList.Add(symptom);
                        }
                    }
                }

                Console.WriteLine("SymptomsNamesList retrieved!!");
            }
        }
    }
}
