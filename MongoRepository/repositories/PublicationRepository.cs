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
    public class PublicationRepository : IDisposable
    {
        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        protected IMongoCollection<Publication> _collection;

        public PublicationRepository()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("RDSearch");
            _collection = _database.GetCollection<Publication>("Publication");
        }

        public long countForOneDisease(string orphaNumber)
        {
            return this._collection.CountAsync(new BsonDocument { { "orphaNumberOfLinkedDisease", orphaNumber } }).Result;
        }

        public void removeAll()
        {
            this._collection.DeleteMany(new BsonDocument { });
        }

        public void removeFromOrphaNumber(string orphaNumber)
        {
            this._collection.DeleteMany(new BsonDocument { { "orphaNumberOfLinkedDisease", orphaNumber } });
        }

        public Publication getByID(string id)
        {
            return this._collection.Find(new BsonDocument { { "_id", new ObjectId(id) } }).FirstAsync().Result;
        }

        public Publication getByIDPubmed(string idPubmed)
        {
            return this._collection.Find(new BsonDocument { { "idPubmed", idPubmed } }).FirstAsync().Result;
        }

        public List<Publication> getByOrphaNumberOfLinkedDisease(string orphaNumberOfLinkedDisease)
        {
            return this._collection.Find(new BsonDocument { { "orphaNumberOfLinkedDisease", orphaNumberOfLinkedDisease } }).ToListAsync().Result;
        }

        public List<Publication> selectAll()
        {
            return this._collection.Find(new BsonDocument { }).ToListAsync().Result;
        }

        public List<Publication> selectAllByDisease(string orphaNumberOfLinkedDisease)
        {
            return this._collection.Find(new BsonDocument { { "orphaNumberOfLinkedDisease", orphaNumberOfLinkedDisease } }).ToListAsync().Result;
        }

        public void updatePublication(Publication publication)
        {
            this._collection.ReplaceOneAsync(x => x._id.Equals(publication._id), publication);
        }

        public List<Publication> insertList(List<Publication> lst_publication)
        {
            this._collection.InsertManyAsync(lst_publication).Wait();
            return this.selectAll();
        }

        public void insertListFast(List<Publication> lst_publication)
        {
            this._collection.InsertManyAsync(lst_publication);
        }

        public void insertListLessFast(List<Publication> lst_publication)
        {
            this._collection.InsertManyAsync(lst_publication).Wait();
        }

        public Publication insertList(Publication publication)
        {
            this._collection.InsertOneAsync(publication).Wait();
            return this.getByID(publication._id.ToString());
        }

        public void Dispose()
        {
        }

    }
}
