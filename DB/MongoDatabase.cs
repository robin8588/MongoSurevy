using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DB
{
    public static class MongoDatabase
    {
        public static IMongoClient _client;
        public static IMongoDatabase _database;

        static MongoDatabase()
        {
            _client = new MongoClient();
            _database = _client.GetDatabase("Survey");
        }

        public static IMongoCollection<BsonDocument> Patients
        {
            get { return _database.GetCollection<BsonDocument>("Patients"); }
        }

        public static IMongoCollection<BsonDocument> Tables
        {
            get { return _database.GetCollection<BsonDocument>("Tables"); }
        }

        public static IMongoCollection<BsonDocument> Rows
        {
            get { return _database.GetCollection<BsonDocument>("Rows"); }
        }
    }
}
