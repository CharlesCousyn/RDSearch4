using MongoDB.Bson;
using MongoDB.Driver;
using MongoRepository.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MongoRepository
{
    public class ConfigRepository : IDisposable
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        protected IMongoCollection<Config> _collection;

        public ConfigRepository()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("RDSearch");
            _collection = _database.GetCollection<Config>("Config");
        }

        public Config selectConfig()
        {
            return this._collection.Find(new BsonDocument { }).ToListAsync().Result.FirstOrDefault();
        }

        public void deleteAll()
        {
            this._collection.DeleteManyAsync(Builders<Config>.Filter.Empty);
        }

        public Config insert(Config config)
        {
            this._collection.InsertOneAsync(config).Wait();
            return this.getByID(config._id.ToString());
        }

        public Config getByID(string id)
        {
            return this._collection.Find(new BsonDocument { { "_id", new ObjectId(id) } }).FirstAsync().Result;
        }

        public void Dispose() { }

    }
}
