using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataContracts;
using System.ServiceModel;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;

namespace DAL
{
    public class DL_Mapping : IDisposable
    {
        public void Dispose()
        { }

        protected static IMongoDatabase _database;
        public List<DataContracts.Mapping.DC_CrossSystemMapping> Master_Get_Cross_Mapping(string SourceSystem, string SourceEntity, string SourceValue, string TargetSystem)
        {

            if (SourceSystem.ToLower() == "TLGX".ToLower())
            {
                try
                {
                    _database = MongoDBHandler.mDatabase();

                    List<DataContracts.Mapping.DC_CrossSystemMapping> result = new List<DataContracts.Mapping.DC_CrossSystemMapping>();

                    if (SourceEntity.ToLower() == "Country".ToLower())
                    {
                        var collection = _database.GetCollection<BsonDocument>("CountryMapping");

                        FilterDefinition<BsonDocument> filter;
                        filter = Builders<BsonDocument>.Filter.Empty;

                        filter = filter & Builders<BsonDocument>.Filter.Regex("CountryCode", new BsonRegularExpression(new Regex(SourceValue, RegexOptions.IgnoreCase)));
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCode", new BsonRegularExpression(new Regex(TargetSystem, RegexOptions.IgnoreCase)));

                        var searchResult = collection.Find(filter).ToList();

                        foreach (var docs in searchResult)
                        {
                            DataContracts.Mapping.DC_CrossSystemMapping subResult = new DataContracts.Mapping.DC_CrossSystemMapping();
                            subResult.Source_System_Name = "TLGX";
                            subResult.Source_System_Code = "TLGX";
                            subResult.Source_Entity = "Country";
                            subResult.Source_Value_Code = SourceValue;
                            subResult.Source_Value_Name = docs["CountryName"].AsString;
                            subResult.Target_System_Code = TargetSystem;
                            subResult.Target_System_Name = docs["SupplierName"].AsString;
                            subResult.Target_Value_Code = docs["SupplierCountryCode"].AsString;
                            subResult.Target_Value_Name = docs["SupplierCountryName"].AsString;

                            result.Add(subResult);
                        }

                        collection = null;
                        _database = null;

                    }
                    else if (SourceEntity.ToLower() == "City".ToLower())
                    {
                        var collection = _database.GetCollection<BsonDocument>("CityMapping");

                        FilterDefinition<BsonDocument> filter;
                        filter = Builders<BsonDocument>.Filter.Empty;

                        filter = filter & Builders<BsonDocument>.Filter.Regex("CityName", new BsonRegularExpression(new Regex(SourceValue, RegexOptions.IgnoreCase)));
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCode", new BsonRegularExpression(new Regex(TargetSystem, RegexOptions.IgnoreCase)));

                        var searchResult = collection.Find(filter).ToList();

                        foreach (var docs in searchResult)
                        {
                            DataContracts.Mapping.DC_CrossSystemMapping subResult = new DataContracts.Mapping.DC_CrossSystemMapping();
                            subResult.Source_System_Name = "TLGX";
                            subResult.Source_System_Code = "TLGX";
                            subResult.Source_Entity = SourceEntity;
                            subResult.Source_Value_Code = docs["CityCode"].AsString;
                            subResult.Source_Value_Name = SourceValue;
                            subResult.Target_System_Code = TargetSystem;
                            subResult.Target_System_Name = docs["SupplierName"].AsString;
                            subResult.Target_Value_Code = docs["SupplierCityCode"].AsString;
                            subResult.Target_Value_Name = docs["SupplierCityName"].AsString;

                            result.Add(subResult);
                        }

                        collection = null;
                        _database = null;
                    }

                    return result;

                }
                catch (FaultException<DataContracts.ErrorNotifier> ex)
                {
                    throw ex;
                }
            }
            else if (TargetSystem.ToLower() == "TLGX".ToLower())
            {
                try
                {
                    _database = MongoDBHandler.mDatabase();

                    List<DataContracts.Mapping.DC_CrossSystemMapping> result = new List<DataContracts.Mapping.DC_CrossSystemMapping>();

                    if (SourceEntity.ToLower() == "Country".ToLower())
                    {
                        var collection = _database.GetCollection<BsonDocument>("CountryMapping");

                        FilterDefinition<BsonDocument> filter;
                        filter = Builders<BsonDocument>.Filter.Empty;

                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCountryCode", new BsonRegularExpression(new Regex(SourceValue, RegexOptions.IgnoreCase)));
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCode", new BsonRegularExpression(new Regex(SourceSystem, RegexOptions.IgnoreCase)));

                        var searchResult = collection.Find(filter).ToList();

                        foreach (var docs in searchResult)
                        {
                            DataContracts.Mapping.DC_CrossSystemMapping subResult = new DataContracts.Mapping.DC_CrossSystemMapping();
                            subResult.Source_System_Name = docs["SupplierName"].AsString;
                            subResult.Source_System_Code = SourceSystem;
                            subResult.Source_Entity = "Country";
                            subResult.Source_Value_Code = SourceValue;
                            subResult.Source_Value_Name = docs["SupplierCountryName"].AsString;
                            subResult.Target_System_Code = "TLGX";
                            subResult.Target_System_Name = "TLGX";
                            subResult.Target_Value_Code = docs["CountryCode"].AsString;
                            subResult.Target_Value_Name = docs["CountryName"].AsString;

                            result.Add(subResult);
                        }

                        collection = null;
                        _database = null;

                    }
                    else if (SourceEntity.ToLower() == "City".ToLower())
                    {
                        var collection = _database.GetCollection<BsonDocument>("CityMapping");

                        FilterDefinition<BsonDocument> filter;
                        filter = Builders<BsonDocument>.Filter.Empty;

                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCityCode", new BsonRegularExpression(new Regex(SourceValue, RegexOptions.IgnoreCase)));
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCode", new BsonRegularExpression(new Regex(SourceSystem, RegexOptions.IgnoreCase)));

                        var searchResult = collection.Find(filter).ToList();

                        foreach (var docs in searchResult)
                        {
                            DataContracts.Mapping.DC_CrossSystemMapping subResult = new DataContracts.Mapping.DC_CrossSystemMapping();
                            subResult.Source_System_Name = docs["SupplierName"].AsString;
                            subResult.Source_System_Code = SourceSystem;
                            subResult.Source_Entity = SourceEntity;
                            subResult.Source_Value_Code = SourceValue;
                            subResult.Source_Value_Name = docs["SupplierCityName"].AsString;
                            subResult.Target_System_Code = "TLGX";
                            subResult.Target_System_Name = "TLGX";
                            subResult.Target_Value_Code = docs["CityCode"].AsString;
                            subResult.Target_Value_Name = docs["CityName"].AsString;

                            result.Add(subResult);
                        }

                        collection = null;
                        _database = null;

                    }

                    return result;

                }
                catch (FaultException<DataContracts.ErrorNotifier> ex)
                {
                    throw ex;
                }
            }
            else
            {
                try
                {
                    _database = MongoDBHandler.mDatabase();

                    List<DataContracts.Mapping.DC_CrossSystemMapping> result = new List<DataContracts.Mapping.DC_CrossSystemMapping>();

                    if (SourceEntity.ToLower() == "Country".ToLower())
                    {
                        var collection = _database.GetCollection<BsonDocument>("CountryMapping");

                        FilterDefinition<BsonDocument> filter;
                        filter = Builders<BsonDocument>.Filter.Empty;

                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCountryCode", new BsonRegularExpression(new Regex(SourceValue, RegexOptions.IgnoreCase)));
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCode", new BsonRegularExpression(new Regex(SourceSystem, RegexOptions.IgnoreCase)));

                        var searchResult = collection.Find(filter).ToList();

                        if (searchResult != null)
                        {
                            if (searchResult.Count > 0)
                            {

                                string _TlgxCode = searchResult[0]["CountryCode"].AsString;
                                string _TlgxName = searchResult[0]["CountryName"].AsString;

                                filter = Builders<BsonDocument>.Filter.Empty;
                                filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCode", new BsonRegularExpression(new Regex(TargetSystem, RegexOptions.IgnoreCase)));
                                filter = filter & Builders<BsonDocument>.Filter.Eq("CountryCode", _TlgxCode);
                                filter = filter & Builders<BsonDocument>.Filter.Eq("CountryName", _TlgxName);

                                var searchNewResult = collection.Find(filter).ToList();

                                foreach (var docs in searchNewResult)
                                {
                                    DataContracts.Mapping.DC_CrossSystemMapping subResult = new DataContracts.Mapping.DC_CrossSystemMapping();
                                    subResult.Source_System_Name = searchResult[0]["SupplierName"].AsString;
                                    subResult.Source_System_Code = SourceSystem;
                                    subResult.Source_Entity = SourceEntity;
                                    subResult.Source_Value_Code = SourceValue;
                                    subResult.Source_Value_Name = searchResult[0]["SupplierCountryName"].AsString;
                                    subResult.Target_System_Code = TargetSystem;
                                    subResult.Target_System_Name = docs["SupplierName"].AsString;
                                    subResult.Target_Value_Code = docs["SupplierCountryCode"].AsString;
                                    subResult.Target_Value_Name = docs["SupplierCountryName"].AsString;

                                    result.Add(subResult);
                                }

                            }
                            collection = null;
                            _database = null;

                        }

                    }
                    else if (SourceEntity.ToLower() == "City".ToLower())
                    {
                        var collection = _database.GetCollection<BsonDocument>("CityMapping");

                        FilterDefinition<BsonDocument> filter;
                        filter = Builders<BsonDocument>.Filter.Empty;

                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCityCode", new BsonRegularExpression(new Regex(SourceValue, RegexOptions.IgnoreCase)));
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCode", new BsonRegularExpression(new Regex(SourceSystem, RegexOptions.IgnoreCase)));

                        var searchResult = collection.Find(filter).ToList();

                        if (searchResult != null)
                        {
                            if (searchResult.Count > 0)
                            {


                                string _TlgxCityCode = searchResult[0]["CityCode"].AsString;
                                string _TlgxCityName = searchResult[0]["CityName"].AsString;
                                string _TlgxCountryCode = searchResult[0]["CountryCode"].AsString;
                                string _TlgxCountryName = searchResult[0]["CountryName"].AsString;

                                filter = Builders<BsonDocument>.Filter.Empty;
                                filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCode", new BsonRegularExpression(new Regex(TargetSystem, RegexOptions.IgnoreCase)));
                                filter = filter & Builders<BsonDocument>.Filter.Eq("CountryCode", _TlgxCountryCode);
                                filter = filter & Builders<BsonDocument>.Filter.Eq("CountryName", _TlgxCountryName);
                                filter = filter & Builders<BsonDocument>.Filter.Eq("CityCode", _TlgxCityCode);
                                filter = filter & Builders<BsonDocument>.Filter.Eq("CityName", _TlgxCityName);

                                var searchNewResult = collection.Find(filter).ToList();

                                foreach (var docs in searchNewResult)
                                {
                                    DataContracts.Mapping.DC_CrossSystemMapping subResult = new DataContracts.Mapping.DC_CrossSystemMapping();
                                    subResult.Source_System_Name = searchResult[0]["SupplierName"].AsString;
                                    subResult.Source_System_Code = SourceSystem;
                                    subResult.Source_Entity = SourceEntity;
                                    subResult.Source_Value_Code = SourceValue;
                                    subResult.Source_Value_Name = searchResult[0]["SupplierCityName"].AsString;
                                    subResult.Target_System_Code = TargetSystem;
                                    subResult.Target_System_Name = docs["SupplierName"].AsString;
                                    subResult.Target_Value_Code = docs["SupplierCityCode"].AsString;
                                    subResult.Target_Value_Name = docs["SupplierCityName"].AsString;

                                    result.Add(subResult);
                                }
                            }

                            collection = null;
                            _database = null;

                        }

                    }

                    return result;

                }
                catch (FaultException<DataContracts.ErrorNotifier> ex)
                {
                    throw ex;
                }
            }

        }

        public List<DataContracts.Mapping.CityMappingRS> Get_City_Mapping(DataContracts.Mapping.CityMappingRQ RQ)
        {
            if (RQ.SourceCode.ToLower() == "TLGX".ToLower())
            {
                try
                {
                    _database = MongoDBHandler.mDatabase();

                    List<DataContracts.Mapping.CityMappingRS> result = new List<DataContracts.Mapping.CityMappingRS>();


                    var collection = _database.GetCollection<BsonDocument>("CityMapping");

                    FilterDefinition<BsonDocument> filter;
                    filter = Builders<BsonDocument>.Filter.Empty;

                    if (!string.IsNullOrWhiteSpace(RQ.TargetCode))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCode", new BsonRegularExpression(new Regex(RQ.TargetCode, RegexOptions.IgnoreCase)));

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCityCode))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("CityCode", new BsonRegularExpression(new Regex(RQ.SourceCityCode, RegexOptions.IgnoreCase)));

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCityName))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("CityName", new BsonRegularExpression(new Regex(RQ.SourceCityName, RegexOptions.IgnoreCase)));

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCountryCode))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("CountryCode", new BsonRegularExpression(new Regex(RQ.SourceCountryCode, RegexOptions.IgnoreCase)));

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCountryName))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("CountryName", new BsonRegularExpression(new Regex(RQ.SourceCountryName, RegexOptions.IgnoreCase)));

                    var searchResult = collection.Find(filter).ToList();

                    foreach (var docs in searchResult)
                    {
                        DataContracts.Mapping.CityMappingRS subResult = new DataContracts.Mapping.CityMappingRS();
                        subResult.SourceCode = RQ.SourceCode;
                        subResult.SourceName = RQ.SourceCode;
                        subResult.SourceCityCode = RQ.SourceCityCode;
                        subResult.SourceCityName = RQ.SourceCityName;
                        subResult.SourceCountryCode = RQ.SourceCountryCode;
                        subResult.SourceCountryName = RQ.SourceCountryName;
                        subResult.TargetCode = RQ.TargetCode;
                        subResult.TargetName = docs["SupplierName"].AsString;
                        subResult.TargetCityCode = docs["SupplierCityCode"].AsString;
                        subResult.TargetCityName = docs["SupplierCityName"].AsString;
                        subResult.TargetCountryCode = docs["SupplierCountryCode"].AsString;
                        subResult.TargetCountryName = docs["SupplierCountryName"].AsString;


                        result.Add(subResult);
                    }

                    collection = null;
                    _database = null;

                    return result;

                }
                catch (FaultException<DataContracts.ErrorNotifier> ex)
                {
                    throw ex;
                }
            }
            else if (RQ.TargetCode.ToLower() == "TLGX".ToLower())
            {
                try
                {
                    _database = MongoDBHandler.mDatabase();

                    List<DataContracts.Mapping.CityMappingRS> result = new List<DataContracts.Mapping.CityMappingRS>();


                    var collection = _database.GetCollection<BsonDocument>("CityMapping");

                    FilterDefinition<BsonDocument> filter;
                    filter = Builders<BsonDocument>.Filter.Empty;

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCode))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCode", new BsonRegularExpression(new Regex(RQ.SourceCode, RegexOptions.IgnoreCase)));

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCityCode))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCityCode", new BsonRegularExpression(new Regex(RQ.SourceCityCode, RegexOptions.IgnoreCase)));

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCityName))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCityName", new BsonRegularExpression(new Regex(RQ.SourceCityName, RegexOptions.IgnoreCase)));

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCountryCode))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCountryCode", new BsonRegularExpression(new Regex(RQ.SourceCountryCode, RegexOptions.IgnoreCase)));

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCountryName))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCountryName", new BsonRegularExpression(new Regex(RQ.SourceCountryName, RegexOptions.IgnoreCase)));

                    var searchResult = collection.Find(filter).ToList();

                    foreach (var docs in searchResult)
                    {
                        DataContracts.Mapping.CityMappingRS subResult = new DataContracts.Mapping.CityMappingRS();

                        subResult.SourceCode = RQ.SourceCode;
                        subResult.SourceName = docs["SupplierName"].AsString;
                        subResult.SourceCityCode = RQ.SourceCityCode;
                        subResult.SourceCityName = RQ.SourceCityName;
                        subResult.SourceCountryCode = RQ.SourceCountryCode;
                        subResult.SourceCountryName = RQ.SourceCountryName;
                        subResult.TargetCode = RQ.TargetCode;
                        subResult.TargetName = RQ.TargetCode;
                        subResult.TargetCityCode = docs["CityCode"].AsString;
                        subResult.TargetCityName = docs["CityName"].AsString;
                        subResult.TargetCountryCode = docs["CountryCode"].AsString;
                        subResult.TargetCountryName = docs["CountryName"].AsString;


                        result.Add(subResult);
                    }

                    collection = null;
                    _database = null;

                    return result;

                }
                catch (FaultException<DataContracts.ErrorNotifier> ex)
                {
                    throw ex;
                }
            }
            else
            {
                try
                {
                    _database = MongoDBHandler.mDatabase();

                    List<DataContracts.Mapping.CityMappingRS> result = new List<DataContracts.Mapping.CityMappingRS>();


                    var collection = _database.GetCollection<BsonDocument>("CityMapping");

                    FilterDefinition<BsonDocument> filter;
                    filter = Builders<BsonDocument>.Filter.Empty;

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCode))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCode", new BsonRegularExpression(new Regex(RQ.SourceCode, RegexOptions.IgnoreCase)));

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCityCode))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCityCode", new BsonRegularExpression(new Regex(RQ.SourceCityCode, RegexOptions.IgnoreCase)));

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCityName))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCityName", new BsonRegularExpression(new Regex(RQ.SourceCityName, RegexOptions.IgnoreCase)));

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCountryCode))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCountryCode", new BsonRegularExpression(new Regex(RQ.SourceCountryCode, RegexOptions.IgnoreCase)));

                    if (!string.IsNullOrWhiteSpace(RQ.SourceCountryName))
                        filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCountryName", new BsonRegularExpression(new Regex(RQ.SourceCountryName, RegexOptions.IgnoreCase)));

                    var searchResult = collection.Find(filter).ToList();

                    if (searchResult != null)
                    {
                        if (searchResult.Count > 0)
                        {
                            string _TlgxCityCode = searchResult[0]["CityCode"].AsString;
                            string _TlgxCityName = searchResult[0]["CityName"].AsString;
                            string _TlgxCountryCode = searchResult[0]["CountryCode"].AsString;
                            string _TlgxCountryName = searchResult[0]["CountryName"].AsString;

                            filter = Builders<BsonDocument>.Filter.Empty;
                            filter = filter & Builders<BsonDocument>.Filter.Regex("SupplierCode", new BsonRegularExpression(new Regex(RQ.TargetCode, RegexOptions.IgnoreCase)));
                            filter = filter & Builders<BsonDocument>.Filter.Eq("CountryCode", _TlgxCountryCode);
                            filter = filter & Builders<BsonDocument>.Filter.Eq("CountryName", _TlgxCountryName);
                            filter = filter & Builders<BsonDocument>.Filter.Eq("CityCode", _TlgxCityCode);
                            filter = filter & Builders<BsonDocument>.Filter.Eq("CityName", _TlgxCityName);

                            var searchNewResult = collection.Find(filter).ToList();

                            foreach (var docs in searchNewResult)
                            {
                                DataContracts.Mapping.CityMappingRS subResult = new DataContracts.Mapping.CityMappingRS();

                                subResult.SourceCode = RQ.SourceCode;
                                subResult.SourceName = searchResult[0]["SupplierName"].AsString;
                                subResult.SourceCityCode = RQ.SourceCityCode;
                                subResult.SourceCityName = RQ.SourceCityName;
                                subResult.SourceCountryCode = RQ.SourceCountryCode;
                                subResult.SourceCountryName = RQ.SourceCountryName;
                                subResult.TargetCode = RQ.TargetCode;
                                subResult.TargetName = docs["SupplierName"].AsString;
                                subResult.TargetCityCode = docs["SupplierCityCode"].AsString;
                                subResult.TargetCityName = docs["SupplierCityName"].AsString;
                                subResult.TargetCountryCode = docs["SupplierCountryCode"].AsString;
                                subResult.TargetCountryName = docs["SupplierCountryName"].AsString;

                                result.Add(subResult);
                            }
                        }


                        collection = null;
                        _database = null;

                    }

                    return result;

                }
                catch (FaultException<DataContracts.ErrorNotifier> ex)
                {
                    throw ex;
                }
            }
        }
    }
}
