using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace DAL
{
    public static class MongoDBHandler

    {
        static IMongoClient _client;
        static IMongoDatabase _database;

        static IMongoClient mClientConnection()
        {
            string MongoDBApplicationName = System.Configuration.ConfigurationManager.AppSettings["MongoDBApplicationName"];
            string MongoDBServerHost = System.Configuration.ConfigurationManager.AppSettings["MongoDBServerHost"];
            string MongoDBServerPort = System.Configuration.ConfigurationManager.AppSettings["MongoDBServerPort"];
            string MongoDBServerUser = System.Configuration.ConfigurationManager.AppSettings["MongoDBServerUser"];
            string MongoDBServerPassword = System.Configuration.ConfigurationManager.AppSettings["MongoDBServerPassword"];
            string MongoDBServerAuthenticationDatabase = System.Configuration.ConfigurationManager.AppSettings["MongoDBServerAuthenticationDatabase"];

            MongoClientSettings mcs = new MongoClientSettings();
            mcs.ApplicationName = MongoDBApplicationName;
            mcs.ConnectionMode = ConnectionMode.Automatic;
            mcs.ConnectTimeout = new TimeSpan(0, 0, 5);
            mcs.Server = new MongoServerAddress(MongoDBServerHost, Convert.ToInt32(MongoDBServerPort));

            mcs.MaxConnectionPoolSize = 1000;
            mcs.WaitQueueSize = 5000;
            mcs.WaitQueueTimeout = new TimeSpan(0, 0, 10);
            mcs.MaxConnectionLifeTime = new TimeSpan(0, 2, 0);
            mcs.MaxConnectionIdleTime = new TimeSpan(0, 1, 0);

            if (MongoDBServerUser != null && MongoDBServerPassword != null && MongoDBServerAuthenticationDatabase != null)
            {
                mcs.Credential = MongoCredential.CreateCredential(MongoDBServerAuthenticationDatabase, MongoDBServerUser, MongoDBServerPassword);
            }

            _client = new MongoClient(mcs);
            return _client;
        }

        public static IMongoDatabase mDatabase()
        {
            if (_client == null)
            {
                _client = mClientConnection();
            }

            if (_database == null)
            {
                _database = _client.GetDatabase(System.Configuration.ConfigurationManager.AppSettings["Mongo_DB_Name"]);//,new MongoDatabaseSettings { ReadConcern = ReadConcern.Local, WriteConcern = WriteConcern.Unacknowledged, ReadPreference = ReadPreference.Primary });          
            }

            return _database;
        }

        //public async Task<bool> MongoDBConnection()
        //{
        //    MongoClientSettings mcs = new MongoClientSettings();
        //    mcs.ApplicationName = "TGLX_MAPPER";
        //    mcs.ConnectionMode = ConnectionMode.Automatic;
        //    mcs.ConnectTimeout = new TimeSpan(0, 0, 10);
        //    mcs.Server = new MongoServerAddress("10.21.21.20", 27017);
        //    _client = new MongoClient(mcs);
        //    MongoDatabaseSettings mDBS = new MongoDatabaseSettings();
        //    MongoServerSettings mSVRs = new MongoServerSettings();
        //    mSVRs.Server = new MongoServerAddress("10.21.21.20", 27017);
        //    MongoServer mSVR = new MongoServer(mSVRs);

        //    MongoDatabase DB = new MongoDatabase(mSVR, "TGLX_MAPPER", mDBS);

        //    _database = _client.GetDatabase("TLGX_MAPPER");

        //    var document = new BsonDocument
        //    {
        //        { "address" , new BsonDocument
        //            {
        //                { "street", "2 Avenue" },
        //                { "zipcode", "10075" },
        //                { "building", "1480" },
        //                { "coord", new BsonArray { 73.9557413, 40.7720266 } }
        //            }
        //        },
        //        { "borough", "Manhattan" },
        //        { "cuisine", "Italian" },
        //        { "grades", new BsonArray
        //            {
        //                new BsonDocument
        //                {
        //                    { "date", new DateTime(2014, 10, 1, 0, 0, 0, DateTimeKind.Utc) },
        //                    { "grade", "A" },
        //                    { "score", 11 }
        //                },
        //                new BsonDocument
        //                {
        //                    { "date", new DateTime(2014, 1, 6, 0, 0, 0, DateTimeKind.Utc) },
        //                    { "grade", "B" },
        //                    { "score", 17 }
        //                }
        //            }
        //        },
        //        { "name", "Vella" },
        //        { "restaurant_id", "41704620" }
        //    };

        //    var collection = _database.GetCollection<BsonDocument>("restaurants");
        //    await collection.InsertOneAsync(document);

        //    collection = _database.GetCollection<BsonDocument>("restaurants");

        //    var filter = new BsonDocument();
        //    //var count = 0;
        //    using (var cursor = await collection.FindAsync(filter))
        //    {
        //        while (await cursor.MoveNextAsync())
        //        {
        //            var batch = cursor.Current;
        //            //BsonDocument document = new BsonDocument();
        //            //foreach (document in batch)
        //            //{
        //            //    // process document
        //            //    count++;
        //            //}
        //        }
        //    }


        //    return true;
        //}

        //public class MongoConnectionHelper<T> where T : class
        //{
        //    public MongoCollection<T> collection { get; private set; }

        //    public MongoConnectionHelper()
        //    {
        //        string connectionString = System.Configuration.ConfigurationManager.AppSettings["MongoDBServerHost"];
        //        MongoServerSettings mss = new MongoServerSettings();
        //        mss.ApplicationName = "TLGX_MAPPER";
        //        mss.ConnectionMode = ConnectionMode.Automatic;
        //        mss.ConnectTimeout = new TimeSpan(10);
        //        //mss.Credentials = new MongoCredential { };

        //        var server = new MongoServer(new MongoServerSettings { (connectionString);
        //        if (server.State == MongoServerState.Disconnected)
        //        {
        //            server.Connect();
        //        }
        //        //var conn = server.GetDatabase("Acord");
        //        //collection = conn.GetCollection<T>("Mappings");
        //    }
        //}
    }
}

