using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class DL_Masters : IDisposable
    {
        public void Dispose()
        { }

        protected static IMongoDatabase _database;
        public List<DataContracts.Masters.DC_Country> Master_GetCountries()
        {
            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DataContracts.Masters.DC_Country>("CountryMaster");
            var result = collection.Find(new BsonDocument { }).SortBy(s => s.CountryName);
            return result.ToList();
        }

        public List<DataContracts.Masters.DC_Country> Master_GetCountries_ByCountryCode(string CountryCode)
        {
            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DataContracts.Masters.DC_Country>("CountryMaster");
            var result = collection.Find(c => c.CountryCode.ToLower().StartsWith(CountryCode.ToLower())).SortBy(s => s.CountryName);
            return result.ToList();
        }

        public List<DataContracts.Masters.DC_Country> Master_GetCountries_ByCountryName(string CountryName)
        {
            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DataContracts.Masters.DC_Country>("CountryMaster");
            var result = collection.Find(c => c.CountryName.ToLower().StartsWith(CountryName.ToLower())).SortBy(s => s.CountryName);
            return result.ToList();
        }

        public List<DataContracts.Masters.DC_City> Master_GetCities_ByCountryCode(string CountryCode)
        {
            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DataContracts.Masters.DC_City>("CityMaster");
            var result = collection.Find(c => c.CountryCode.ToLower().StartsWith(CountryCode.ToLower())).SortBy(s => s.CityName);
            return result.ToList();
        }

        public List<DataContracts.Masters.DC_City> Master_GetCities_ByCountryName(string CountryName)
        {
            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DataContracts.Masters.DC_City>("CityMaster");
            var result = collection.Find(c => c.CountryName.ToLower().StartsWith(CountryName.ToLower())).SortBy(s => s.CityName);
            return result.ToList();
        }

        public List<DataContracts.Masters.DC_Supplier> Master_GetSupplier_ByCode(string Code)
        {
            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DataContracts.Masters.DC_Supplier>("Supplier");
            var result = collection.Find(c => c.SupplierCode.ToLower().StartsWith(Code.ToLower())).SortBy(s => s.SupplierName);
            return result.ToList();
        }

        public List<DataContracts.Masters.DC_Supplier> Master_GetSupplier_ByName(string Name)
        {
            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DataContracts.Masters.DC_Supplier>("Supplier");
            var result = collection.Find(c => c.SupplierName.ToLower().StartsWith(Name.ToLower())).SortBy(s => s.SupplierName);
            return result.ToList();
        }

        public List<DataContracts.Masters.DC_Supplier> Master_GetAllSupplier()
        {
            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DataContracts.Masters.DC_Supplier>("Supplier");
            var result = collection.Find(f => true).SortBy(s => s.SupplierName).ToList();
            return result;
        }

    }
}
