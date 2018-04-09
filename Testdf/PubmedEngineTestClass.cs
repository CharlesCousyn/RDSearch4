using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using MongoRepository.entities;
using MongoRepository;
using System.Net;
using System.Xml.Serialization;
using System.IO;
using WebCrawler;
using WebCrawler2;

namespace TestCrawler
{
    [TestClass]
    public class PubmedEngineTestClass
    {

        PubmedEngine pubmedEngine = new PubmedEngine();
        PublicationRepository publicationRepository = new PublicationRepository();
        Disease disease;

       [TestInitialize]
        public void InitCrawlerFetch2TestMethod()
        {
            //Empty the DB
            //publicationRepository.removeAll();
        }

        [TestMethod]
        public void CrawlerFetch2TestMethod()
        {
            /*
            disease = new Disease();
            disease.Name = "TEST";
            disease.OrphaNumber = "1000000";



            //pubmedEngine.PubmedCrawlerFetch2(0, 500, disease, new List<long> { 5848491, 5846849 }).Wait();
            //Make the request
            var retstart = 0;
            var maxBatch = 10;
            var idList = new List<long> { 5848491, 5846849 };

            var url = "https://eutils.ncbi.nlm.nih.gov/entrez/eutils/efetch.fcgi";

            //Constructing parameters ids
            string parameters = $"db=pmc&retstart={retstart}&retmax={maxBatch}&rettype=xml&retmode=xml&id=" + idList[0].ToString();
            //string parameters = $"db=pubmed&retstart={retstart}&retmax={maxBatch}{confAPI}&id=" + idList.ElementAt(0).ToString();
            for (int i = 1; i < idList.Count; i++)
            {
                parameters += "," + idList[i];
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

            XmlSerializer serializer = new XmlSerializer(typeof(Pmcarticleset));
            Pmcarticleset results;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var readerString = new StreamReader(stream, Encoding.UTF8))
            //using (var reader = XmlReader.Create(stream, new XmlReaderSettings { DtdProcessing = DtdProcessing.Ignore }))
            {

                string resString = readerString.ReadToEnd();
                try
                {
                    results = serializer.Deserialize(new StringReader(resString)) as Pmcarticleset;
                    List<Publication> lst_publications = pubmedEngine.ConvertFromPubmedArticleSetToPublications2(results, disease);

                    //Console.WriteLine(lst_publications[0].abstractText);
                    Console.WriteLine("TEEEEEEEEEESSST: "+lst_publications[0].fullText);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Error on disease: " + disease.Name + ", orphaNumber: " + disease.OrphaNumber);
                    Console.WriteLine(resString);
                }
            }*/
        }

        [TestCleanup]
        public void CleanUpCrawlerFetch2TestMethod()
        {
            //Empty the DB
            //publicationRepository.removeAll();
        }
    }
}
