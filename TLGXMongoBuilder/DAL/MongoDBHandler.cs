using MongoDB.Driver;

namespace DAL
{
    public static class MongoDBHandler
    {
        static IMongoClient _client;
        static IMongoDatabase _database;
        static string MongoDBConnectionString;

        static IMongoClient mClientConnection()
        {
            MongoDBConnectionString = System.Configuration.ConfigurationManager.AppSettings["MongoDBConnectionString"];
            _client = new MongoClient(MongoDBConnectionString);
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
                var mongoUrl = new MongoUrl(MongoDBConnectionString);
                _database = _client.GetDatabase(mongoUrl.DatabaseName);
            }
            return _database;
        }
    }
}

