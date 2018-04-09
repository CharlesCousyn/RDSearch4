using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoRepository.entities;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using System.Reflection;

namespace MongoRepository
{
    public class DiseaseRepository:IDisposable
    {

        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        protected IMongoCollection<Disease> _collection;

        public DiseaseRepository()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("RDSearch");
            _collection = _database.GetCollection<Disease>("Disease");
        }

        public void removeAll()
        {
            this._collection.DeleteMany(new BsonDocument{ });
        }

        public Disease getByID(string id)
        {
            return this._collection.Find(new BsonDocument { { "_id", new ObjectId(id)} }).FirstAsync().Result;
        }

        public Disease getByOrphaNumber(string orphaNumber)
        {
            return this._collection.Find(new BsonDocument { { "orphaNumber", orphaNumber } }).FirstAsync().Result;
        }

        public List<Disease> selectAll()
        {
            return this._collection.Find(new BsonDocument { }).Sort(new BsonDocument("OrphaNumber", 1)).ToListAsync().Result;
        }
        
        public static IAggregateFluent<Disease> Sample(IAggregateFluent<Disease> agg, int count)
        {
            var new_agg = agg.Skip(10);
            var stage = new_agg.Stages[new_agg.Stages.Count - 1];
            var newDoc = new BsonDocument {
                { "$sample", new BsonDocument {
                        {"size", count}
                    } }
            };
            stage.GetType().GetField("_document"
             , BindingFlags.Instance | BindingFlags.NonPublic)
                 .SetValue(stage, newDoc);
            return new_agg;
        }

        public List<Disease> selectRandom(int number)
        {
            return Sample(this._collection.Aggregate(), number).ToListAsync().Result;
        }

        public Disease updateDisease(Disease disease)
        {
            var results = this._collection.ReplaceOneAsync(x => x._id.Equals(disease._id), disease).Result;
            return this.getByID(disease._id.ToString());
        }

        public List<Disease> insertList(List<Disease> lst_diseases)
        {
            this._collection.InsertManyAsync(lst_diseases).Wait();
            return this.selectAll();
        }

        public Disease insert(Disease disease)
        {
            this._collection.InsertOneAsync(disease).Wait();
            return this.getByID(disease._id.ToString());
        }

        public void Dispose() { }

    }
}
