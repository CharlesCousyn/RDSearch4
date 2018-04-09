using ConfigurationJSON;
using MongoRepository;
using MongoRepository.entities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Xml;

namespace Aggregator.tools
{
    public class Efetch
    {
        private FormUrlEncodedContent urlContent;
        private HttpClient client;
        private XmlDocument monDocActuel;
        private Disease disease;
        private string urlEfetch;
        private PublicationRepository publicationRepository;

        public Efetch(int retstart, int maxBatch, Disease diseaseP, List<long> ids, HttpClient clientP, string urlEfetchP, PublicationRepository publicationRepositoryP)
        {
            urlContent = new FormUrlEncodedContent(
                new Dictionary<string, string>{
                    { "db", "pmc" },
                    { "retstart", $"{retstart}" },
                    { "retmax", $"{maxBatch}" },
                    { "rettype", "xml" },
                    { "retmode", "xml" },
                    { "api_key", $"{ConfigurationManager.Instance.config.API_Key}" },
                    { "email", $"{ConfigurationManager.Instance.config.Email}" },
                    { "tool", $"{ConfigurationManager.Instance.config.Tool}" },
                    { "id", String.Join(", ", ids.ToArray())}
                }
                );
            disease = diseaseP;
            client = clientP;
            urlEfetch = urlEfetchP;
            publicationRepository = publicationRepositoryP;
            monDocActuel = new XmlDocument();
        }

        public void Launch()
        {
            
            try
            {
                //Make the request and deserialization
                using (HttpResponseMessage res = client.PostAsync("", urlContent).Result)
                using (HttpContent content = res.Content)
                //using (var stream = await content.ReadAsStreamAsync())
                {
                    string stringRes = content.ReadAsStringAsync().Result;
                    
                    monDocActuel.LoadXml(stringRes);

                    //Conversion
                    List<Publication> lst_publications = ConvertFromPubmedArticleSetToPublications3(monDocActuel, disease);

                    if (lst_publications.Count != 0)
                    {
                        publicationRepository.insertListLessFast(lst_publications);
                    }
                    lst_publications = null;


                    stringRes = null;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error on disease: " + disease.Name + ", orphaNumber: " + disease.OrphaNumber);
                Console.WriteLine(e);
                //Console.WriteLine(stringRes);//2
            }
        }

        public static List<Publication> ConvertFromPubmedArticleSetToPublications3(XmlDocument monDoc, Disease disease)
        {
            XmlNodeList articles = monDoc.GetElementsByTagName("article");

            //Initialisation
            List<Publication> lst_Publications = new List<Publication>();
            for (int i = 0; i < articles.Count; i++)
            {
                lst_Publications.Add(new Publication());
            }

            for (int i = 0; i < articles.Count; i++)
            {
                var articleMeta = articles[i].SelectSingleNode("./front/article-meta");

                //Ids
                lst_Publications[i].title = articleMeta?.SelectSingleNode("./title-group/article-title/text()")?.Value;
                lst_Publications[i].idPubmed = Convert.ToInt64(articleMeta?.SelectSingleNode("./article-id[@pub-id-type='pmid']/text()")?.Value);
                lst_Publications[i].idPMC = Convert.ToInt64(articleMeta?.SelectSingleNode("./article-id[@pub-id-type='pmc']/text()")?.Value);
                lst_Publications[i].doi = articleMeta?.SelectSingleNode("./article-id[@pub-id-type='doi']/text()")?.Value;


                //Authors
                var contribs = articleMeta?.SelectNodes("./contrib-group/contrib/name");
                if (contribs != null)
                {
                    foreach (XmlNode contrib in contribs)
                    {
                        lst_Publications[i].authors.Add(
                        contrib?.SelectSingleNode("./given-names/text()")?.Value
                        + " " +
                        contrib?.SelectSingleNode("./surname/text()")?.Value);
                    }
                }


                //Date
                var dateNode = articleMeta?.SelectSingleNode("./pub-date[@pub-type='ppub']");
                if (dateNode != null)
                {
                    int.TryParse(dateNode?.SelectSingleNode("./year/text()")?.Value, out int year);
                    int.TryParse(dateNode?.SelectSingleNode("./month/text()")?.Value, out int month);
                    int.TryParse(dateNode?.SelectSingleNode("./day/text()")?.Value, out int day);
                    if (year != 0 && month != 0 && day != 0)
                    {
                        lst_Publications[i].datePublication = new DateTime(year, month, day);
                    }
                }

                //Times cited
                lst_Publications[i].timesCited = 0;

                //OrphaNumber
                lst_Publications[i].orphaNumberOfLinkedDisease = disease.OrphaNumber;


                //Abstract
                lst_Publications[i].abstractText = "";
                StringBuilder sB = new StringBuilder();
                var abstracts = articleMeta.SelectNodes("./abstract");
                if (abstracts != null)
                {
                    foreach (XmlNode abstractNode in abstracts)
                    {
                        //Selecting direct Ps
                        var Ps = abstractNode?.SelectNodes("./p/text()");
                        if (Ps != null)
                        {
                            //Fusioning Ps
                            foreach (XmlNode P in Ps)
                            {
                                sB.Append(P?.Value);
                                sB.Append(" ");
                            }
                        }

                        //Selecting indirect Ps
                        var Ps2 = abstractNode?.SelectNodes("./sec/p/text()");
                        if (Ps2 != null)
                        {
                            //Fusioning Ps
                            foreach (XmlNode P in Ps2)
                            {
                                sB.Append(P?.Value);
                                sB.Append(" ");
                            }
                        }
                    }
                }
                lst_Publications[i].abstractText = sB.ToString();


                //FullText
                lst_Publications[i].fullText = "";
                sB = new StringBuilder();
                //Selecting direct Ps
                var paragraphes = articles[i]?.SelectNodes("./body/sec/p/text()");
                if (paragraphes != null)
                {
                    foreach (XmlNode paragraphe in paragraphes)
                    {
                        sB.Append(paragraphe?.Value);
                        sB.Append(" ");
                    }
                }

                //Selecting indirect Ps
                var paragraphes2 = articles[i]?.SelectNodes("./body/sec/sec/p/text()");
                if (paragraphes2 != null)
                {
                    foreach (XmlNode paragraphe in paragraphes2)
                    {
                        sB.Append(paragraphe?.Value);
                        sB.Append(" ");
                    }
                }

                lst_Publications[i].fullText = sB.ToString();
            }

            monDoc = null;

            return lst_Publications;

        }

    }
}
