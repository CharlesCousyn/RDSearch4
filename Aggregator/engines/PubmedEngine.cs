using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using MongoRepository.entities;
using System.Threading;
using System.Text;
using CrawlerOrphanet.tools;
using WebCrawler2;
using ConfigurationJSON;
using System.Net.Http;
using MongoRepository;
using Aggregator.tools;

namespace WebCrawler
{
    public class PubmedEngine
    {
        public ConcurrentBag<Publication> Publications { get; set; }

        private Mutex m;
        private PublicationRepository publicationRepository;
        private DiseaseRepository diseaseRepository;
        private HttpClient client;
        private Dictionary<string, string> defaultParameters;
        private string urlEfetch;
        private string urlESearch;
        private XmlDocument monDocActuel;

        public PubmedEngine()
        {
            publicationRepository = new PublicationRepository();
            diseaseRepository = new DiseaseRepository();
            Publications = new ConcurrentBag<Publication>();
            monDocActuel = new XmlDocument();

            client = new HttpClient();
            urlEfetch = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi";
            urlESearch= $"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?" +
                $"db=pmc" +
                $"&sort=relevance" +
                $"&retmax={1000}" +
                $"{ConfigurationManager.Instance.config.ParametersAPI}";

            defaultParameters = new Dictionary<string, string>{
                    { "db", "pmc" },
                    { "retstart", "0" },
                    { "retmax", "500" },
                    { "rettype", "xml" },
                    { "retmode", "xml" },
                    { "api_key", $"{ConfigurationManager.Instance.config.API_Key}" },
                    { "email", $"{ConfigurationManager.Instance.config.Email}" },
                    { "tool", $"{ConfigurationManager.Instance.config.Tool}" },
                    { "id", "" }
                };
            m = new Mutex();
        }

        public eSearchResult PubmedCrawlerSearch(Disease disease)
        {
            var term = disease.Name + " AND hasabstract[text] AND Humans[Mesh]";
            var url = $"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?db=pubmed&term={term}&sort=relevance&retmax=1000&usehistory=y{ConfigurationManager.Instance.config.ParametersAPI}";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;
            request.AutomaticDecompression = DecompressionMethods.GZip;

            //Console.WriteLine("Starting request for PubMed ESearch this can take some time...");

            XmlSerializer serializer = new XmlSerializer(typeof(eSearchResult));
            eSearchResult result = new eSearchResult();

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
                {
                    //Console.WriteLine("Parsing request for PubMed ESearch this can take some time...");
                    result = serializer.Deserialize(reader) as eSearchResult;
                }
            }
            catch
            {

            }

            return result;
        }

        public eSearchResult PubmedCrawlerSearch2(Disease disease)
        {
            var term = disease.Name + " AND hasabstract[text] AND Humans[Mesh]";
            var url = $"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?db=pmc&term={term}&sort=relevance&retmax=1000&usehistory=y{ConfigurationManager.Instance.config.ParametersAPI}";
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Timeout = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;
            request.AutomaticDecompression = DecompressionMethods.GZip;

            //Console.WriteLine("Starting request for PubMed ESearch this can take some time...");

            XmlSerializer serializer = new XmlSerializer(typeof(eSearchResult));
            eSearchResult result = new eSearchResult();

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
                {
                    //Console.WriteLine("Parsing request for PubMed ESearch this can take some time...");
                    result = serializer.Deserialize(reader) as eSearchResult;
                }
            }
            catch
            {

            }

            return result;
        }

        public Task PubmedCrawlerFetch3(long retstart, int maxBatch, Disease disease, List<long> idList)
        {
            return new Task(async () =>
            {
                //Initialization
                HttpClient client = new HttpClient();

                //URL and parameters
                string url = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi";
                var parameters = new Dictionary<string, string>{
                    { "db", "pmc" },
                    { "retstart", $"{retstart}" },
                    { "retmax", $"{maxBatch}" },
                    { "rettype", "xml" },
                    { "retmode", "xml" },
                    { "api_key", $"{ConfigurationManager.Instance.config.API_Key}" },
                    { "email", $"{ConfigurationManager.Instance.config.Email}" },
                    { "tool", $"{ConfigurationManager.Instance.config.Tool}" },
                    { "id", String.Join(", ", idList.ToArray()) }
                };

                var urlContent = new FormUrlEncodedContent(parameters);

                //Make the request and deserialization
                using (HttpResponseMessage res = await client.PostAsync(url, urlContent))
                using (HttpContent content = res.Content)
                //using (var stream = await content.ReadAsStreamAsync())
                {
                    string stringRes = await content.ReadAsStringAsync();//2
                    
                    try
                    {
                        XmlDocument monDoc = new XmlDocument();
                        monDoc.LoadXml(stringRes);//2
                        //monDoc.Load(stream);//1

                        //Into db
                        PublicationRepository publicationRepository = new PublicationRepository();
                        Console.WriteLine("Lancement conversion efetch.......");
                        //Conversion
                        Console.WriteLine("Debut parsing");
                        //List<Publication> lst_publications = new List<Publication>();
                        List<Publication> lst_publications = ConvertFromPubmedArticleSetToPublications3(monDoc, disease);
                        Console.WriteLine("Fin parsing");

                        Console.WriteLine("Fin conversion efetch !!");
                        if (lst_publications.Count != 0)
                        {
                            publicationRepository.insertListLessFast(lst_publications);
                        }
                        lst_publications = null;

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error on disease: " + disease.Name + ", orphaNumber: " + disease.OrphaNumber);
                        Console.WriteLine(e);
                        //Console.WriteLine(stringRes);//2
                    }
                    stringRes = null;
                }

            });
        }

        public void EFetch4(long retstart, int maxBatch, Disease disease, List<long> idList)
        {
            this.m.WaitOne();
            //Update parameters
            defaultParameters["retstart"] = $"{ retstart}";
            defaultParameters["retmax"] = $"{ maxBatch}";
            defaultParameters["id"] = String.Join(", ", idList.ToArray());

            var urlContent = new FormUrlEncodedContent(defaultParameters);
            try
            {
                //Make the request and deserialization
                using (HttpResponseMessage res = client.PostAsync(urlEfetch, urlContent).Result)
                using (HttpContent content = res.Content)
                //using (var stream = await content.ReadAsStreamAsync())
                {
                    string stringRes = content.ReadAsStringAsync().Result;


                    monDocActuel = new XmlDocument();
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
            this.m.ReleaseMutex();
        }

        public Task EFetch5(long retstart, int maxBatch, Disease disease, List<long> idList)
        {
            return new Task(() => 
            {
                //Update parameters
                defaultParameters["retstart"] = $"{ retstart}";
                defaultParameters["retmax"] = $"{ maxBatch}";
                defaultParameters["id"] = String.Join(", ", idList.ToArray());

                var urlContent = new FormUrlEncodedContent(defaultParameters);

                //Make the request and deserialization
                using (HttpResponseMessage res = client.PostAsync(urlEfetch, urlContent).Result)
                using (HttpContent content = res.Content)
                //using (var stream = await content.ReadAsStreamAsync())
                {
                    string stringRes = content.ReadAsStringAsync().Result;
                    try
                    {
                        monDocActuel.LoadXml(stringRes);

                        //Into db
                        PublicationRepository publicationRepository = new PublicationRepository();
                        //Conversion
                        //List<Publication> lst_publications = new List<Publication>();
                        List<Publication> lst_publications = ConvertFromPubmedArticleSetToPublications3(monDocActuel, disease);

                        if (lst_publications.Count != 0)
                        {
                            publicationRepository.insertListLessFast(lst_publications);
                        }
                        lst_publications = null;

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error on disease: " + disease.Name + ", orphaNumber: " + disease.OrphaNumber);
                        Console.WriteLine(e);
                        //Console.WriteLine(stringRes);//2
                    }
                    stringRes = null;
                }
            }
            );
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

        public Task<eSearchResult> PubmedCrawlerSearch3(Disease disease)
        {
            return new Task<eSearchResult>(() =>
            {
                //var term = disease.Name + " AND hasabstract[text] AND Humans[Mesh]";
                var term = disease.Name;
                var url = $"https://eutils.ncbi.nlm.nih.gov/entrez/eutils/esearch.fcgi?db=pmc&term={term}&sort=relevance&retmax=1000&usehistory=y{ConfigurationManager.Instance.config.ParametersAPI}";
                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;
                request.AutomaticDecompression = DecompressionMethods.GZip;

                //Console.WriteLine("Starting request for PubMed ESearch this can take some time...");

                XmlSerializer serializer = new XmlSerializer(typeof(eSearchResult));
                eSearchResult result = new eSearchResult();

                try
                {
                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    using (var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
                    {
                        //Console.WriteLine("Parsing request for PubMed ESearch this can take some time...");
                        result = serializer.Deserialize(reader) as eSearchResult;
                    }
                }
                catch
                {

                }

                return result;
            });
        }

        public List<long> XmlDocumentToIds(XmlDocument monDoc)
        {
            List<long> ids = new List<long>();
            XmlNodeList idNodes = monDoc.GetElementsByTagName("Id");
            foreach(XmlNode idNode in idNodes)
            {
                long.TryParse(idNode.InnerText, out long id);
                ids.Add(id);
            }

            return ids;
        }

        public List<long> ESearch4(Disease disease)
        {
            List<long> ids = new List<long>();
            try
            {
                using (HttpResponseMessage res = client.GetAsync($"{urlESearch}&term={disease.Name}").Result)
                using (HttpContent content = res.Content)
                {
                    string stringRes = content.ReadAsStringAsync().Result;
                    try
                    {
                        XmlDocument monDoc = new XmlDocument();
                        monDoc.LoadXml(stringRes);
                        ids = XmlDocumentToIds(monDoc);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        Console.WriteLine(stringRes);
                    }
                }
            }
            catch
            {

            }
            
            return ids;



            /*
                    //var term = disease.Name + " AND hasabstract[text] AND Humans[Mesh]";
                    var request = (HttpWebRequest)WebRequest.Create($"{urlESearch}&term={disease.Name}");
            request.Timeout = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;
            request.AutomaticDecompression = DecompressionMethods.GZip;

            //Console.WriteLine("Starting request for PubMed ESearch this can take some time...");

            XmlSerializer serializer = new XmlSerializer(typeof(eSearchResult));
            eSearchResult result = new eSearchResult();

            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
                {
                    //Console.WriteLine("Parsing request for PubMed ESearch this can take some time...");
                    result = serializer.Deserialize(reader) as eSearchResult;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return result;*/
        }


        public Task PubmedCrawlerLink(long retstart, int maxBatch,  List<long> idList)
        {
            return new Task(() =>
                {
                    if(idList == null || idList.Count == 0)
                    {
                        return;
                    }
                    var parameters = $"dbfrom=pubmed&linkname=pubmed_pubmed_citedin" +
                    $"&retstart={retstart}&retmax={maxBatch}&rettype=xml&retmode=xml" +
                    $"{ConfigurationManager.Instance.config.ParametersAPI}&id={+idList.ElementAt(0)}";

                    for (int i = 1; i < idList.Count; i++)
                    {
                        parameters += "&id=" + idList.ElementAt(i);
                    }

                    var url = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/elink.fcgi";
                    //Console.WriteLine(url);

                    var request = (HttpWebRequest)WebRequest.Create(url);
                    request.AutomaticDecompression = DecompressionMethods.GZip;
                    request.Method = "POST";
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.Timeout = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;


                    var data = Encoding.ASCII.GetBytes(parameters);
                    request.ContentLength = data.Length;
                    using (var stream = request.GetRequestStream())
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    
                    //Console.WriteLine("Starting request for PubMed citations this can take some time...");

                    XmlSerializer serializer = new XmlSerializer(typeof(ELinkResult));
                    ELinkResult results;

                    using (var response = (HttpWebResponse)request.GetResponse())
                    using (var stream = response.GetResponseStream())
                    using (var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
                    {
                        //Console.WriteLine("Parsing request for PubMed this can take some time...");
                        results = serializer.Deserialize(reader) as ELinkResult;

                        foreach(LinkSet linkset in results.LinkSet)
                        {
                            if(linkset.LinkSetDb != null)
                            {
                                Publication pub = publicationRepository.getByIDPubmed(linkset.IdList.Id);
                                pub.timesCited = linkset.LinkSetDb.Link.Count;
                                publicationRepository.updatePublication(pub);
                            }
                            
                        }
                    }
                }
            );
                
        }

        public Task PubmedCrawlerSummary(long retstart, int maxBatch, Disease disease, List<long> idList)
        {
            return new Task(() =>
            {
                var url = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi";

                //Constructing parameters ids
                string parameters = $"db=pubmed&retstart={retstart}&retmax={maxBatch}&rettype=xml&retmode=xml" +
                $"{ConfigurationManager.Instance.config.ParametersAPI}&id=" + idList.ElementAt(0).ToString();

                for (int i = 1; i < idList.Count; i++)
                {
                    parameters += "," + idList.ElementAt(i);
                }

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                request.Timeout = (int)TimeSpan.FromMinutes(10).TotalMilliseconds;
                request.AutomaticDecompression = DecompressionMethods.GZip;


                var data = Encoding.ASCII.GetBytes(parameters);
                request.ContentLength = data.Length;
                using (var stream = request.GetRequestStream())
                {
                    stream.Write(data, 0, data.Length);
                }

                //Console.WriteLine("Starting request for PubMed Fetch this can take some time...");

                XmlSerializer serializer = new XmlSerializer(typeof(eSummaryResult));
                eSummaryResult results;

                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var readerString = new StreamReader(stream, Encoding.UTF8))
                //using (var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
                {

                    string resString = readerString.ReadToEnd();
                    try
                    {

                        Disease dis = diseaseRepository.getByOrphaNumber(disease.OrphaNumber);

                        //Console.WriteLine("Parsing request for PubMed Fetch this can take some time...");
                        results = serializer.Deserialize(new StringReader(resString)) as eSummaryResult;
                        foreach(var documentSummary in results.Items[0].DocumentSummary)
                        {
                            string yearString = documentSummary.PubDate.Substring(0, 4);

                            int n;
                            bool isNumeric = int.TryParse(yearString, out n);
                            if(isNumeric)
                            {
                                int indexOfYear = dis.NumberPublicationsPerYear.Select(x => x.year).ToList().IndexOf(yearString);
                                if (indexOfYear != -1)
                                {
                                    dis.NumberPublicationsPerYear[indexOfYear].numberOfPublications++;
                                }
                                else
                                {
                                    NumberPublicationsForOneYear one = new NumberPublicationsForOneYear();
                                    one.year = yearString;
                                    one.numberOfPublications = 1;
                                    dis.NumberPublicationsPerYear.Add(one);
                                }
                            }
                        }

                        diseaseRepository.updateDisease(dis);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error on disease: " + disease.Name + ", orphaNumber: " + disease.OrphaNumber);
                        Console.WriteLine(resString);
                    }
                }
            }
            );
        }

        public void Start(List<Disease> lst_diseases)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            
            /////////////////////////////////////////////////////
            List<Tuple<Task<eSearchResult>, Disease>> esearchToDo = new List<Tuple<Task<eSearchResult>, Disease>>();
            //Get EsearchTo DO
            Parallel.ForEach(lst_diseases, (disease) =>
                {
                    //Construct esearchs...
                    esearchToDo.Add(new Tuple<Task<eSearchResult>, Disease>(this.PubmedCrawlerSearch3(disease), disease));
                }
            );


            //Execute esearchTODO and construct efetchs
            Console.WriteLine("ESearch...");
            TimeLeft.Instance.operationsToDo = esearchToDo.Count;
            var efetchsToDOO = new List<Task>();

            var defaultNumber = 5;
            for (int i = 0; i < esearchToDo.Count; i += defaultNumber)
            {
                int number = defaultNumber;
                if ((i + defaultNumber) > esearchToDo.Count)
                {
                    number = esearchToDo.Count - i;
                }
                var selectedESearchs = esearchToDo.GetRange(i, number);
                Stopwatch diffTime = new Stopwatch();
                diffTime.Start();

                Parallel.ForEach(selectedESearchs, (esearch) =>
                {
                    if (esearch.Item1 != null)
                    {
                        esearch.Item1.Start();
                    }
                });

                //Wait until 10 tasks are finished
                Task.WaitAll(selectedESearchs.Select(x => x.Item1).ToArray());


                //construct efetchs
                Parallel.ForEach(selectedESearchs, (esearch) =>
                {
                    var res = esearch.Item1.Result;
                    if (res.IdList != null && res.Count != 0)
                    {
                        List<long> idListTemp = res.IdList.ToList();

                        //Batch and size
                        int batchSizeTemp = ConfigurationManager.Instance.config.BatchSizePMC;
                        int nombreBatchTemp = (res.IdList.Length / batchSizeTemp) + 1;
                        if ((nombreBatchTemp - 1) * batchSizeTemp == res.IdList.Length)
                        {
                            nombreBatchTemp--;
                        }

                        for (int j = 0; j < nombreBatchTemp; j++)
                        {
                            efetchsToDOO.Add(this.PubmedCrawlerFetch3(j * batchSizeTemp, batchSizeTemp, esearch.Item2, idListTemp));
                        }
                    }
                });


                diffTime.Stop();
                TimeLeft.Instance.IncrementOfXOperations(TimeSpan.FromMilliseconds(diffTime.ElapsedMilliseconds).Seconds, number);
                TimeLeft.Instance.CalcAndShowTimeLeft();


                if (diffTime.ElapsedMilliseconds < 1000)
                {
                    Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds - (int)diffTime.ElapsedMilliseconds + 100);
                }
            }

            TimeLeft.Instance.Reset();

            TimeLeft.Instance.operationsToDo = efetchsToDOO.Count;
            Console.WriteLine(efetchsToDOO.Count + " Efetchs to do...");
            //var compteur = 0;
            for (int i = 0; i < efetchsToDOO.Count; i += defaultNumber)
            {
                int number = defaultNumber;
                if ((i + defaultNumber) > efetchsToDOO.Count)
                {
                    number = efetchsToDOO.Count - i;
                }
                var selectedEfetchs = efetchsToDOO.GetRange(i, number);
                Stopwatch diffTime = new Stopwatch();
                diffTime.Start();

                Parallel.ForEach(selectedEfetchs, (efetch) =>
                //foreach(var efetch in selectedEfetchs)
                {
                    if (efetch != null)
                    {
                        efetch.Start();
                    }
                }
                );

                Task.WaitAll(selectedEfetchs.ToArray());

                
                selectedEfetchs = null;

                diffTime.Stop();
                TimeLeft.Instance.IncrementOfXOperations(TimeSpan.FromMilliseconds(diffTime.ElapsedMilliseconds).Seconds, number);
                TimeLeft.Instance.CalcAndShowTimeLeft();

                Thread.Sleep(1500);

                /*
                if (diffTime.ElapsedMilliseconds < 1000)
                {
                    Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds - (int)diffTime.ElapsedMilliseconds + 4000);
                }
                */
            }

            TimeLeft.Instance.Reset();
            
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        public void Start2(List<Disease> lst_diseases)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            TimeLeft.Instance.operationsToDo = lst_diseases.Count;
            Stopwatch diffTimeEsearch = new Stopwatch();
            foreach (var disease in lst_diseases)
            {
                //Timing global
                Stopwatch diffTime = new Stopwatch();
                diffTime.Start();

                //Timing esearch
                diffTimeEsearch.Reset();
                diffTimeEsearch.Start();

                //Esearch and Result of esearch
                var idList = this.ESearch4(disease);

                
                diffTimeEsearch.Stop();

                //Pause time
                int pauseTime = 100;//in millisecond
                if (diffTimeEsearch.ElapsedMilliseconds < pauseTime)
                {
                    Thread.Sleep((int)TimeSpan.FromMilliseconds(pauseTime).Milliseconds - (int)diffTimeEsearch.ElapsedMilliseconds + 10);
                }

                Thread.Sleep(pauseTime);

                //Update ids to remove existant ids in DB
                idList = RemoveExistantsIds(idList, disease);

                //Separating in batches
                int batchSize = ConfigurationManager.Instance.config.BatchSizePMC;
                int nombreBatch = (idList.Count / batchSize) + 1;
                if ((nombreBatch - 1) * batchSize == idList.Count)
                {
                    nombreBatch--;
                }

                //Executing 
                Stopwatch diffTimeEfetch= new Stopwatch();
                for (int j = 0; j < nombreBatch; j++)
                {
                    diffTimeEfetch.Reset();
                    diffTimeEfetch.Start();
                    //Launch efetch + parsing + stock in DB
                    this.EFetch4(j * batchSize, batchSize, disease, idList);
                    diffTimeEfetch.Stop();

                    //Pause time
                    if (diffTimeEfetch.ElapsedMilliseconds < pauseTime)
                    {
                        Thread.Sleep((int)TimeSpan.FromMilliseconds(pauseTime).Milliseconds - (int)diffTimeEfetch.ElapsedMilliseconds + 10);
                    }
                }

                diffTime.Stop();
                TimeLeft.Instance.IncrementOfXOperations(TimeSpan.FromMilliseconds(diffTime.ElapsedMilliseconds).Seconds, 1);
                TimeLeft.Instance.CalcAndShowTimeLeft();
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        public List<long> RemoveExistantsIds(List<long> idList, Disease disease)
        {
            List<long> existantesIds = publicationRepository.selectAllByDisease(disease.OrphaNumber).Select(x => x.idPMC).ToList();

            //Remove existant ids from idList...
            idList = idList.Except(existantesIds).ToList();

            return idList;
        }

        public void Start2AsyncParallel(List<Disease> lst_diseases)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            TimeLeft.Instance.operationsToDo = lst_diseases.Count;
            foreach (var disease in lst_diseases)
            {
                //Timing
                Stopwatch diffTime = new Stopwatch();
                diffTime.Start();

                //Esearch and Result of esearch
                var idList = this.ESearch4(disease);

                //Separating in batches
                int batchSize = ConfigurationManager.Instance.config.BatchSizePMC;
                int nombreBatch = (idList.Count / batchSize) + 1;
                if ((nombreBatch - 1) * batchSize == idList.Count)
                {
                    nombreBatch--;
                }
                //Console.WriteLine($"{nombreBatch} efetches for disease {disease.OrphaNumber}");
                //Pause time
                int pauseTime = 120;

                Thread.Sleep(pauseTime);

                //In a paralel loop
                //Executing 10 batchs(request) + pause
                int defaultNumber = 10;
                for (int i = 0; i < nombreBatch; i += defaultNumber)
                {
                    //Find the number of efetch to do (MAX = defaultNumber)
                    int number = defaultNumber;
                    if ((i + defaultNumber) > nombreBatch)
                    {
                        number = nombreBatch - i;
                    }


                    //Timing
                    Stopwatch diffTimeEfetchs = new Stopwatch();
                    diffTimeEfetchs.Start();

                    //Construct iterating object
                    List<int> numsEfetch = new List<int>();
                    for(int j=0; j<number; j++)
                    {
                        numsEfetch.Add(j);
                    }

                    //Executing batches
                    //for (int j = 0; j < number; j++)
                    //foreach(int j in numsEfetch)
                    Parallel.ForEach(numsEfetch, j =>
                        {
                            EFetch4(j * batchSize, batchSize, disease, idList);
                        }
                    );

                    diffTimeEfetchs.Stop();

                    if (diffTimeEfetchs.ElapsedMilliseconds < 1000)
                    {
                        Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds - (int)diffTimeEfetchs.ElapsedMilliseconds + 100);
                    }
                    
                }

                diffTime.Stop();
                TimeLeft.Instance.IncrementOfXOperations(TimeSpan.FromMilliseconds(diffTime.ElapsedMilliseconds).Seconds, 1);
                TimeLeft.Instance.CalcAndShowTimeLeft();
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds);
            Console.WriteLine("RunTime " + elapsedTime);
        }

        public void Start2AsyncParallel2(List<Disease> lst_diseases)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            TimeLeft.Instance.operationsToDo = lst_diseases.Count;
            foreach (var disease in lst_diseases)
            {
                //Timing
                Stopwatch diffTime = new Stopwatch();
                diffTime.Start();

                //Esearch and Result of esearch
                var idList = this.ESearch4(disease);

                //Separating in batches
                int batchSize = ConfigurationManager.Instance.config.BatchSizePMC;
                int nombreBatch = (idList.Count / batchSize) + 1;
                if ((nombreBatch - 1) * batchSize == idList.Count)
                {
                    nombreBatch--;
                }
                //Console.WriteLine($"{nombreBatch} efetches for disease {disease.OrphaNumber}");
                //Pause time
                int pauseTime = 120;

                Thread.Sleep(pauseTime);

                //In a paralel loop
                //Executing 10 batchs(request) + pause
                int defaultNumber = 10;
                for (int i = 0; i < nombreBatch; i += defaultNumber)
                {
                    //Find the number of efetch to do (MAX = defaultNumber)
                    int number = defaultNumber;
                    if ((i + defaultNumber) > nombreBatch)
                    {
                        number = nombreBatch - i;
                    }


                    //Timing
                    Stopwatch diffTimeEfetchs = new Stopwatch();
                    diffTimeEfetchs.Start();

                    //Construct eftechs objects
                    ConcurrentBag<Efetch> Efetches = new ConcurrentBag<Efetch>();
                    for (int j = 0; j < number; j++)
                    {
                        Efetches.Add(new Efetch(j * batchSize, batchSize, disease, idList, client, urlEfetch, publicationRepository));
                    }

                    //Executing batches
                    //for (int j = 0; j < number; j++)
                    //foreach(int j in numsEfetch)
                    Parallel.ForEach(Efetches, efetch =>
                    {
                        efetch.Launch();
                    }
                    );

                    diffTimeEfetchs.Stop();

                    if (diffTimeEfetchs.ElapsedMilliseconds < 1000)
                    {
                        Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds - (int)diffTimeEfetchs.ElapsedMilliseconds + 100);
                    }

                }

                diffTime.Stop();
                TimeLeft.Instance.IncrementOfXOperations(TimeSpan.FromMilliseconds(diffTime.ElapsedMilliseconds).Seconds, 1);
                TimeLeft.Instance.CalcAndShowTimeLeft();
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
            ts.Hours, ts.Minutes, ts.Seconds,
            ts.Milliseconds);
            Console.WriteLine("RunTime " + elapsedTime);
        }

    }
}
