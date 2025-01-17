﻿using System;
using System.Linq;
using MongoDB.Bson;
using MongoRepository.entities;
using MongoDB.Driver;
using System.Collections.Generic;

namespace MongoRepository
{
    public class PredictionDataRepository : IDisposable
    {

        protected static IMongoClient _client;
        protected static IMongoDatabase _database;
        protected IMongoCollection<DiseasesData> _collection;

        public PredictionDataRepository()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("RDSearch");
            _collection = _database.GetCollection<DiseasesData>("PredictionDataRepository");
        }

        public void removeAll()
        {
            this._collection.DeleteMany(new BsonDocument { });
        }

        public List<DiseasesData> selectAll()
        {
            return this._collection.Find(new BsonDocument { }).Sort(new BsonDocument("OrphaNumber", 1)).ToListAsync().Result;
        }

        public DiseasesData selectByType(type monType)
        {
            return new DiseasesData(
                monType,
                this._collection
                    .Find(new BsonDocument { { "Type", monType.ToString() } })
                    .ToList()
                    .SelectMany(x => x.DiseaseDataList)
                    .ToList()
                );
        }


        public void insert(DiseasesData diseasesData)
        {
            this._collection.InsertOneAsync(diseasesData).Wait();
        }

        public void Dispose() { }

    }
}
