using ConfigurationJSON;
using Evaluation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoRepository.entities;
using Moq;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EvaluationTest
{
    [TestClass]
    public class EvaluateTestClass
    {

        DiseasesData FakePredictionData;
        DiseasesData FakeRealData;

        [TestInitialize]
        public void InitEvaluateTestMethod()
        {
            //FakePredictionData initialisation
            FakePredictionData = new DiseasesData(
                type.Symptom, 
                new List<DiseaseData>()
                    {
                        new DiseaseData(
                            new Disease("101039", "Female restricted epilepsy with intellectual disability", 42), 
                            new RelatedEntities(
                                type.Symptom, 
                                new List<RelatedEntity>()
                                    {
                                    new RelatedEntity(type.Symptom, "hyperactivity", 2.4),
                                    new RelatedEntity(type.Symptom, "epileptic encephalopathy", 2.0)
                                })),
                        new DiseaseData(
                            new Disease("100080", "Test Disease", 12),
                            new RelatedEntities(
                                type.Symptom,
                                new List<RelatedEntity>()
                                    {
                                    new RelatedEntity(type.Symptom, "atherosclerosis", 50.0),
                                    new RelatedEntity(type.Symptom, "death", 16.0),
                                    new RelatedEntity(type.Symptom, "brain neoplasm", 70.0)
                                }))
                    });

            //FakeRealData initialisation
            FakeRealData = new DiseasesData(
                type.Symptom,
                new List<DiseaseData>()
                    {
                        new DiseaseData(
                            new Disease("101039", "Female restricted epilepsy with intellectual disability", 42),
                            new RelatedEntities(
                                type.Symptom,
                                new List<RelatedEntity>()
                                    {
                                    //1 predicted symptom deleted
                                    new RelatedEntity(type.Symptom, "epileptic encephalopathy", 2.0),

                                    //2 real symptom added
                                    new RelatedEntity(type.Symptom, "SymptomTest1", 40.0),
                                    new RelatedEntity(type.Symptom, "SymptomTest2", 70.0)
                                })),
                        new DiseaseData(
                            new Disease("100080", "Test Disease", 12),
                            new RelatedEntities(
                                type.Symptom,
                                new List<RelatedEntity>()
                                    {
                                    //2 predicted symptom deleted
                                    new RelatedEntity(type.Symptom, "death", 16.0),

                                    //1 real symptom added
                                    new RelatedEntity(type.Symptom, "SymptomTest3", 45.6)
                                }))
                    });
        }

        [TestMethod]
        public void EvaluateTestMethod()
        {
            //Mock mechanism...
            IDAL fakeDAL = Mock.Of<IDAL>();
            Mock.Get(fakeDAL).Setup(dal => dal.GetPredictionData()).Returns(FakePredictionData) ;
            Mock.Get(fakeDAL).Setup(dal => dal.GetRealData()).Returns(FakeRealData);

            //Obtain fake/test data
            DiseasesData PredictionData = fakeDAL.GetPredictionData();
            DiseasesData RealData = fakeDAL.GetRealData();

            //Check that it's the same type comparison
            Assert.AreEqual(PredictionData.Type, RealData.Type);

            //Do the evaluation
            var testFileName = "UnitTestResults.json";
            System.Console.WriteLine(testFileName);
            Evaluator.Evaluate(PredictionData, RealData, testFileName);

            //Test the linkedFile....
            System.Console.WriteLine("Config.User.ResultsFolder: " + ConfigurationManager.Instance.config.ResultsFolder + "results.json");

            //using (StreamReader r = new StreamReader(Config.User.ResultsFolder + "results.json"))
            using (StreamReader r = new StreamReader(ConfigurationManager.Instance.config.ResultsFolder + testFileName))
            {
                Results results = JsonConvert.DeserializeObject<Results>(r.ReadToEnd());


                Assert.AreEqual(2, results.general.RealPositives);
                Assert.AreEqual(3, results.general.FalsePositives);
                Assert.AreEqual(3, results.general.FalseNegatives);

                Assert.AreEqual(System.Math.Round(2.0 / 5.0, 4), results.general.Precision);
                Assert.AreEqual(System.Math.Round(2.0 / 5.0, 4), results.general.Recall);
                Assert.AreEqual(System.Math.Round(0.4000, 4), results.general.F_Score);

                //Disease 1 "101039"
                Assert.AreEqual(1, results.perDisease[0].RealPositives);
                Assert.AreEqual(1, results.perDisease[0].FalsePositives);
                Assert.AreEqual(2, results.perDisease[0].FalseNegatives);

                Assert.AreEqual(System.Math.Round(1.0 / 2.0, 4), results.perDisease[0].Precision);
                Assert.AreEqual(System.Math.Round(1.0 / 3.0, 4), results.perDisease[0].Recall);
                Assert.AreEqual(System.Math.Round(0.4000, 4), results.perDisease[0].F_Score);

                //Disease 2 "100080"
                Assert.AreEqual(1, results.perDisease[1].RealPositives);
                Assert.AreEqual(2, results.perDisease[1].FalsePositives);
                Assert.AreEqual(1, results.perDisease[1].FalseNegatives);

                Assert.AreEqual(System.Math.Round(1.0 / 3.0, 4), results.perDisease[1].Precision);
                Assert.AreEqual(System.Math.Round(1.0 / 2.0, 4), results.perDisease[1].Recall);
                Assert.AreEqual(System.Math.Round(0.4000, 4), results.perDisease[1].F_Score);
            }
        }

        [TestCleanup]
        public void CleanUpEvaluateTestMethod()
        {
            // nettoyer les variables, ...
        }
    }
}
