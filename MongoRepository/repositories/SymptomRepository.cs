using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver.Linq;
using MongoDB.Driver;
using MongoRepository.entities;

namespace MongoRepository
{
    public class SymptomRepository:IDisposable
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        protected IMongoCollection<Symptom> _collection;

        public SymptomRepository()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("RDSearch");
            _collection = _database.GetCollection<Symptom>("Symptom");
        }

        public Symptom getByID(string id)
        {
            return this._collection.Find(new BsonDocument { { "_id", new ObjectId(id) } }).FirstAsync().Result;
        }

        public List<Symptom> getByOrphaNumber(string orphaNumber)
        {
            return this._collection.Find(new BsonDocument { { "OrphaNumber", orphaNumber } }).ToListAsync().Result;
        }

        public List<Symptom> selectAll()
        {
            return this._collection.Find(new BsonDocument { }).ToListAsync().Result;
        }

        public Symptom updateSymptom(Symptom symptom)
        {
            var results = this._collection.ReplaceOneAsync(x => x._id.Equals(symptom._id), symptom).Result;
            return this.getByID(symptom._id.ToString());
        }

        public void deleteByOrphaNumber(string orphaNumber)
        {
            this._collection.DeleteMany(new BsonDocument { { "OrphaNumber", orphaNumber} });
        }

        public void insertList(List<Symptom> lst_symptom)
        {
            /*
            foreach(Symptom symptom in lst_symptom)
            {
                symptom._id = new ObjectId();
            }*/

            this._collection.InsertManyAsync(lst_symptom).Wait();
        }

        public Symptom insert(Symptom symptom)
        {
            this._collection.InsertOneAsync(symptom).Wait();
            return this.getByID(symptom._id.ToString());
        }

        public void Dispose() { }

    }
}
