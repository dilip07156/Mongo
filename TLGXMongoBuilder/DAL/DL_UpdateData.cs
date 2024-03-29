﻿using DataContracts;
using DataContracts.Mapping;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace DAL
{
    public class DL_UpdateData : IDisposable
    {
        protected static IMongoDatabase _database;

        #region Activity Mapping & Lite
        public void Insert_ActivityMapping_ByMapId(string MapId)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    _database = MongoDBHandler.mDatabase();

                    var collection = _database.GetCollection<DataContracts.Mapping.DC_ProductMapping>("ActivityMapping");
                    var activityMap = (from apm in context.Activity_SupplierProductMapping
                                       join a in context.Activities on apm.Activity_ID equals a.Activity_Id
                                       where apm.MapID == Convert.ToInt32(MapId)
                                       select new DataContracts.Mapping.DC_ProductMapping
                                       {
                                           SupplierCode = apm.SupplierName,
                                           SupplierProductCode = apm.SuplierProductCode,
                                           SupplierCountryCode = apm.SupplierCountryCode,
                                           SupplierCountryName = apm.SupplierCountryName,
                                           SupplierCityCode = apm.SupplierCityCode,
                                           SupplierCityName = apm.SupplierCityName,
                                           SupplierProductName = apm.SupplierProductName,
                                           MappingStatus = apm.MappingStatus,
                                           MapId = apm.MapID ?? 0,
                                           SystemProductCode = a.CommonProductID.ToString(),
                                           SystemProductName = a.Product_Name,
                                           SystemCountryName = a.Country,
                                           SystemCityName = a.City,
                                           SystemProductType = "Activity" //a.ProductCategorySubType
                                       }).FirstOrDefault();

                    if (activityMap != null)
                    {
                        collection.InsertOne(activityMap);
                    }
                    collection = null;


                    var collectionLite = _database.GetCollection<DataContracts.Mapping.DC_ProductMappingLite>("ActivityMappingLite");
                    var activityMapLite = (from apm in context.Activity_SupplierProductMapping
                                           join a in context.Activities on apm.Activity_ID equals a.Activity_Id
                                           where apm.MapID == Convert.ToInt32(MapId)
                                           select new DataContracts.Mapping.DC_ProductMappingLite
                                           {
                                               SupplierCode = apm.SupplierName,
                                               SupplierProductCode = apm.SuplierProductCode,
                                               MapId = apm.MapID ?? 0,
                                               SystemProductCode = a.CommonProductID.ToString(),
                                           }).FirstOrDefault();
                    if (activityMapLite != null)
                    {
                        collectionLite.InsertOne(activityMapLite);
                    }
                    collectionLite = null;

                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void Update_ActivityMapping_ByMapId(string MapId)
        {
            try
            {
                Delete_ActivityMapping_ByMapId(MapId);
                Insert_ActivityMapping_ByMapId(MapId);
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void Delete_ActivityMapping_ByMapId(string MapId)
        {
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_ProductMapping>("ActivityMapping");
                collection.DeleteOne(f => f.MapId == Convert.ToInt32(MapId));
                collection = null;

                var collectionLite = _database.GetCollection<DataContracts.Mapping.DC_ProductMappingLite>("ActivityMappingLite");
                collectionLite.DeleteOne(f => f.MapId == Convert.ToInt32(MapId));
                collectionLite = null;

            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
        }
        #endregion

        #region Country Mapping
        public void Insert_CountryMapping_ByMapId(string MapId)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    _database = MongoDBHandler.mDatabase();

                    var collection = _database.GetCollection<DataContracts.Mapping.DC_CountryMapping>("CountryMapping");
                    var countryMap = (from cm in context.m_CountryMapping
                                      join c in context.m_CountryMaster on cm.Country_Id equals c.Country_Id
                                      join s in context.Suppliers on cm.Supplier_Id equals s.Supplier_Id
                                      where cm.MapID == Convert.ToInt32(MapId) && cm.Status != "UNMAPPED"
                                      select new DataContracts.Mapping.DC_CountryMapping
                                      {
                                          SupplierName = s.Name,
                                          SupplierCode = s.Code,
                                          CountryCode = c.Code,
                                          CountryName = c.Name,
                                          SupplierCountryName = (cm.CountryName ?? string.Empty),
                                          SupplierCountryCode = (cm.CountryCode ?? string.Empty),
                                          MapId = cm.MapID ?? 0
                                      }).FirstOrDefault();

                    if (countryMap != null)
                    {
                        collection.InsertOne(countryMap);
                    }
                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void Update_CountryMapping_ByMapId(string MapId)
        {
            try
            {
                Delete_CountryMapping_ByMapId(MapId);
                Insert_CountryMapping_ByMapId(MapId);
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void Delete_CountryMapping_ByMapId(string MapId)
        {
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_CountryMapping>("CountryMapping");
                collection.DeleteOne(d => d.MapId == Convert.ToInt32(MapId));
                collection = null;
                _database = null;
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }
        #endregion

        #region City Mapping
        public void Upsert_CityMapping_ByMapId(int MapId)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    var CityMap = (from cm in context.m_CityMapping
                                   join city in context.m_CityMaster on cm.City_Id equals city.City_Id
                                   join country in context.m_CountryMaster on cm.Country_Id equals country.Country_Id
                                   join supplier in context.Suppliers on cm.Supplier_Id equals supplier.Supplier_Id
                                   where (cm.Status ?? string.Empty) == "MAPPED" && cm.MapID == MapId
                                   select new DataContracts.Mapping.DC_CityMapping
                                   {
                                       CityName = (city.Name ?? string.Empty).ToUpper(),
                                       CityCode = (city.Code ?? string.Empty).ToUpper(),
                                       SupplierCityCode = (supplier.Code.ToUpper() == "CLEARTRIP") ? (cm.CityName ?? string.Empty).ToUpper() : (cm.CityCode ?? string.Empty).ToUpper(),
                                       SupplierCityName = (cm.CityName ?? string.Empty).ToUpper(),
                                       SupplierName = supplier.Name.ToUpper(),
                                       SupplierCode = supplier.Code.ToUpper(),
                                       CountryCode = country.Code.ToUpper(),
                                       CountryName = country.Name.ToUpper(),
                                       SupplierCountryName = (cm.CountryName ?? string.Empty).ToUpper(),
                                       SupplierCountryCode = (cm.CountryCode ?? string.Empty).ToUpper(),
                                       MapId = cm.MapID ?? 0
                                   }).FirstOrDefault();

                    if (CityMap != null)
                    {
                        var CityMapDetails = new BsonDocument
                        {
                            { "CountryName", CityMap.CountryName },
                            { "CountryCode", CityMap.CountryCode },
                            { "CityName", CityMap.CityName },
                            { "CityCode", CityMap.CityCode },
                            { "SupplierName", CityMap.SupplierName },
                            { "SupplierCode", CityMap.SupplierCode },
                            { "SupplierCountryName", CityMap.SupplierCountryName },
                            { "SupplierCountryCode", CityMap.SupplierCountryCode },
                            { "SupplierCityName", CityMap.SupplierCityName },
                            { "SupplierCityCode", CityMap .SupplierCityCode},
                            { "MapId", CityMap.MapId }
                        };

                        _database = MongoDBHandler.mDatabase();
                        var collection = _database.GetCollection<BsonDocument>("CityMapping");
                        var filter = Builders<BsonDocument>.Filter.Eq("MapId", MapId);
                        var result = collection.ReplaceOne(filter, CityMapDetails, new UpdateOptions { IsUpsert = true });

                        filter = null;
                        collection = null;
                        _database = null;
                    }
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void Delete_CityMapping_ByMapId(string MapId)
        {
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_CityMapping>("CityMapping");
                collection.DeleteOne(d => d.MapId == Convert.ToInt32(MapId));
                collection = null;
                _database = null;
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Hotel Mapping & Lite
        public void Insert_HotelMapping_ByMapId(string MapId)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    _database = MongoDBHandler.mDatabase();

                    var collection = _database.GetCollection<DataContracts.Mapping.DC_ProductMapping>("ProductMapping");
                    var hotelMap = (from apm in context.Accommodation_ProductMapping
                                    join a in context.Accommodations on apm.Accommodation_Id equals a.Accommodation_Id
                                    where apm.MapId == Convert.ToInt32(MapId)
                                    select new DataContracts.Mapping.DC_ProductMapping
                                    {
                                        SupplierCode = apm.SupplierName,
                                        SupplierProductCode = apm.SupplierProductReference,
                                        SupplierCountryCode = apm.CountryCode,
                                        SupplierCountryName = apm.CountryName,
                                        SupplierCityCode = apm.CityCode,
                                        SupplierCityName = apm.CityName,
                                        SupplierProductName = apm.ProductName,
                                        MappingStatus = apm.Status,
                                        MapId = apm.MapId,
                                        SystemProductCode = a.CompanyHotelID.ToString(),
                                        SystemProductName = a.HotelName,
                                        SystemCountryName = a.country,
                                        SystemCityName = a.city,
                                        SystemProductType = a.ProductCategorySubType
                                    }).FirstOrDefault();

                    if (hotelMap != null)
                    {
                        collection.InsertOne(hotelMap);
                    }
                    collection = null;


                    var collectionLite = _database.GetCollection<DataContracts.Mapping.DC_ProductMappingLite>("ProductMappingLite");
                    var hotelMapLite = (from apm in context.Accommodation_ProductMapping
                                        join a in context.Accommodations on apm.Accommodation_Id equals a.Accommodation_Id
                                        where apm.MapId == Convert.ToInt32(MapId)
                                        select new DataContracts.Mapping.DC_ProductMappingLite
                                        {
                                            SupplierCode = apm.SupplierName,
                                            SupplierProductCode = apm.SupplierProductReference,
                                            MapId = apm.MapId,
                                            SystemProductCode = a.CompanyHotelID.ToString()
                                        }).FirstOrDefault();
                    if (hotelMapLite != null)
                    {
                        collectionLite.InsertOne(hotelMapLite);
                    }
                    collectionLite = null;

                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void Update_HotelMapping_ByMapId(string MapId)
        {
            try
            {
                Delete_HotelMapping_ByMapId(MapId);
                Insert_HotelMapping_ByMapId(MapId);
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void Delete_HotelMapping_ByMapId(string MapId)
        {
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_ProductMapping>("ProductMapping");
                collection.DeleteOne(f => f.MapId == Convert.ToInt32(MapId));
                collection = null;

                var collectionLite = _database.GetCollection<DataContracts.Mapping.DC_ProductMappingLite>("ProductMappingLite");
                collectionLite.DeleteOne(f => f.MapId == Convert.ToInt32(MapId));
                collectionLite = null;
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Country Master
        public void Insert_CountryMaster_ByCode(string Code)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    _database = MongoDBHandler.mDatabase();

                    var collection = _database.GetCollection<BsonDocument>("CountryMaster");

                    var Country = (from c in context.m_CountryMaster
                                   where c.Code == Code
                                   select new
                                   {
                                       c.Name,
                                       c.Code
                                   }).FirstOrDefault();
                    if (Country != null)
                    {
                        var document = new BsonDocument
                        {
                            { "CountryName", Country.Name },
                            { "CountryCode", Country.Code }
                        };

                        collection.InsertOne(document);
                        document = null;
                    }

                    collection = null;
                    _database = null;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update_CountryMaster_ByCode(string Code, string OldCode)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    /*
                    var CountryName = (from c in context.m_CountryMaster
                                       where c.Code == Code
                                       select c.Name).FirstOrDefault();

                    _database = MongoDBHandler.mDatabase();
                    var collection1 = _database.GetCollection<BsonDocument>("CountryMaster");
                    var filter1 = Builders<BsonDocument>.Filter.Eq("CountryCode", OldCode.ToUpper());
                    var result1 = collection1.Find(filter1).FirstOrDefault();

                    //For insert Country Name and Country Code
                    if(result1 == null)
                    {
                        Insert_CountryMaster_ByCode(Code);

                        filter1 = null;
                        collection1 = null;
                        _database = null;
                        return;
                    }

                    if (CountryName != null)
                    {
                        _database = MongoDBHandler.mDatabase();
                        var collection = _database.GetCollection<BsonDocument>("CountryMaster");
                        var filter = Builders<BsonDocument>.Filter.Eq("CountryCode", OldCode.ToUpper());
                        var update = Builders<BsonDocument>.Update.Set("CountryName", CountryName.ToUpper()).Set("CountryCode", Code.ToUpper());
                        var result = collection.UpdateOne(filter, update);
                        update = null;
                        filter = null;
                        collection = null;
                        _database = null;
                    }

                    */
                    var CountryName = (from c in context.m_CountryMaster
                                       where c.Code == Code
                                       select new DataContracts.Masters.DC_Country
                                       {
                                           CountryName = c.Name.Trim().ToUpper(),
                                           CountryCode = c.Code.Trim().ToUpper(),
                                       }).FirstOrDefault();

                    if (CountryName != null)
                    {
                        _database = MongoDBHandler.mDatabase();
                        var collection = _database.GetCollection<DataContracts.Masters.DC_Country>("CountryMaster");
                        var filter = Builders<DataContracts.Masters.DC_Country>.Filter.Eq("CountryCode", OldCode.ToUpper());
                        collection.ReplaceOne(filter, CountryName, new UpdateOptions { IsUpsert = true });
                        filter = null;
                        collection = null;
                        _database = null;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete_CountryMaster_ByCode(string Code)
        {
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<BsonDocument>("CountryMaster");
                var filter = Builders<BsonDocument>.Filter.Eq("CountryCode", Code);
                var result = collection.DeleteOne(filter);
                filter = null;
                collection = null;
                _database = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region City Master
        public void Insert_CityMaster_ByCode(string Code)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    var City = (from city in context.m_CityMaster
                                join country in context.m_CountryMaster on city.Country_Id equals country.Country_Id
                                where city.Code == Code
                                select new
                                {
                                    CountryName = country.Name,
                                    CountryCode = country.Code ?? string.Empty,
                                    CityName = city.Name,
                                    CityCode = city.Code ?? string.Empty,
                                    StateName = city.StateName ?? string.Empty,
                                    StateCode = city.StateCode ?? string.Empty
                                }).FirstOrDefault();

                    if (City != null)
                    {
                        _database = MongoDBHandler.mDatabase();
                        var collection = _database.GetCollection<BsonDocument>("CityMaster");
                        var document = new BsonDocument
                        {
                            { "CityName", City.CityName},
                            { "CityCode", City.CityCode },
                            { "StateName", City.StateName },
                            { "StateCode", City.StateCode },
                            { "CountryCode", City.CountryCode },
                            { "CountryName", City.CountryName },
                        };
                        collection.InsertOne(document);
                        document = null;
                        collection = null;
                        _database = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update_CityMaster_ByCode(string Code)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    var City = (from city in context.m_CityMaster
                                join country in context.m_CountryMaster on city.Country_Id equals country.Country_Id
                                where city.Code == Code
                                select new
                                {
                                    CountryName = country.Name,
                                    CountryCode = country.Code ?? string.Empty,
                                    CityName = city.Name,
                                    CityCode = city.Code ?? string.Empty,
                                    StateName = city.StateName ?? string.Empty,
                                    StateCode = city.StateCode ?? string.Empty
                                }).FirstOrDefault();

                    if (City != null)
                    {
                        _database = MongoDBHandler.mDatabase();
                        var collection = _database.GetCollection<BsonDocument>("CityMaster");
                        var filter = Builders<BsonDocument>.Filter.Eq("CityCode", Code);
                        var update = Builders<BsonDocument>.Update.Set("CityName", City.CityName)
                            .Set("StateName", City.StateName)
                            .Set("StateCode", City.StateCode)
                            .Set("CountryCode", City.CountryCode)
                            .Set("CountryName", City.CountryName);
                        var result = collection.UpdateOne(filter, update);
                        update = null;
                        filter = null;
                        collection = null;
                        _database = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete_CityMaster_ByCode(string Code)
        {
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<BsonDocument>("CityMaster");
                var filter = Builders<BsonDocument>.Filter.Eq("CityCode", Code);
                var result = collection.DeleteOne(filter);
                filter = null;
                collection = null;
                _database = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Upsert_CityMaster_ByCode(string Code)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    var City = (from city in context.m_CityMaster
                                join country in context.m_CountryMaster on city.Country_Id equals country.Country_Id
                                where city.Code == Code
                                select new
                                {
                                    CountryName = country.Name,
                                    CountryCode = country.Code ?? string.Empty,
                                    CityName = city.Name,
                                    CityCode = city.Code ?? string.Empty,
                                    StateName = city.StateName ?? string.Empty,
                                    StateCode = city.StateCode ?? string.Empty
                                }).FirstOrDefault();

                    if (City != null)
                    {
                        var document = new BsonDocument
                        {
                            { "CityName", City.CityName},
                            { "CityCode", City.CityCode },
                            { "StateName", City.StateName },
                            { "StateCode", City.StateCode },
                            { "CountryCode", City.CountryCode },
                            { "CountryName", City.CountryName },
                        };
                        _database = MongoDBHandler.mDatabase();
                        var collection = _database.GetCollection<BsonDocument>("CityMaster");
                        var filter = Builders<BsonDocument>.Filter.Eq("CityCode", Code);
                        var result = collection.ReplaceOne(filter, document, new UpdateOptions { IsUpsert = true });
                        filter = null;
                        collection = null;
                        _database = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Supplier Master
        public void Insert_SupplierMaster_ByCode(string Code)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {

                    var Supplier = (from s in context.Suppliers
                                    where s.Code == Code
                                    select new
                                    {
                                        SupplierName = s.Name.ToUpper(),
                                        SupplierCode = s.Code.ToUpper(),
                                        SupplierType = s.SupplierType.ToUpper() ?? string.Empty,
                                        SupplierOwner = s.SupplierOwner.ToUpper() ?? string.Empty
                                    }).FirstOrDefault();


                    if (Supplier != null)
                    {
                        _database = MongoDBHandler.mDatabase();
                        var collection = _database.GetCollection<BsonDocument>("Supplier");

                        var document = new BsonDocument
                        {
                            { "SupplierName", (Supplier.SupplierName ?? string.Empty).ToUpper() },
                            { "SupplierCode", (Supplier.SupplierCode ?? string.Empty).ToUpper() },
                            { "SupplierOwner", (Supplier.SupplierOwner ?? string.Empty).ToUpper() },
                            { "SupplierType", (Supplier.SupplierType ?? string.Empty).ToUpper() }
                        };

                        collection.InsertOne(document);
                        document = null;
                        collection = null;
                        _database = null;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Upsert_SupplierMaster_ByCode(string Code)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {

                    var Supplier = (from s in context.Suppliers
                                    where s.Code == Code
                                    select new
                                    {
                                        SupplierName = s.Name.ToUpper(),
                                        SupplierCode = s.Code.ToUpper(),
                                        SupplierType = (s.SupplierType ?? string.Empty).ToUpper(),
                                        SupplierOwner = (s.SupplierOwner ?? string.Empty).ToUpper()
                                    }).FirstOrDefault();


                    if (Supplier != null)
                    {
                        _database = MongoDBHandler.mDatabase();
                        var collection = _database.GetCollection<BsonDocument>("Supplier");
                        var filter = Builders<BsonDocument>.Filter.Eq("SupplierCode", Code);
                        var update = Builders<BsonDocument>.Update.Set("SupplierName", Supplier.SupplierName)
                            .Set("SupplierOwner", Supplier.SupplierOwner)
                            .Set("SupplierType", Supplier.SupplierType);
                        var result = collection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });
                        update = null;
                        filter = null;
                        collection = null;
                        _database = null;
                    }

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete_SupplierMaster_ByCode(string Code)
        {
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<BsonDocument>("Supplier");
                var filter = Builders<BsonDocument>.Filter.Eq("SupplierCode", Code.ToUpper());
                var result = collection.DeleteOne(filter);
                filter = null;
                collection = null;
                _database = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Hotel Master
        public void Insert_HotelMaster_ByHotelId(string HotelId)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {


                    var Acco = (from a in context.Accommodations
                                where a.CompanyHotelID == Convert.ToInt32(HotelId)
                                select a).FirstOrDefault();

                    if (Acco != null)
                    {
                        _database = MongoDBHandler.mDatabase();
                        var collection = _database.GetCollection<HotelsHotel>("Accommodations");

                        //check if record is already exists
                        var searchResultCount = collection.Find(f => f.HotelId == Acco.CompanyHotelID.ToString()).Count();
                        if (searchResultCount == 0)
                        {
                            var AccoClassAttr = (from a in context.Accommodation_ClassificationAttributes
                                                 where a.Accommodation_Id == Acco.Accommodation_Id
                                                 select a).ToList();

                            var AccoDesc = (from a in context.Accommodation_Descriptions
                                            where a.Accommodation_Id == Acco.Accommodation_Id
                                            select a).ToList();

                            var AccoNearBy = (from a in context.Accommodation_NearbyPlaces
                                              where a.Accomodation_Id == Acco.Accommodation_Id
                                              select a).ToList();

                            var AccoContacts = (from a in context.Accommodation_Contact
                                                where a.Accommodation_Id == Acco.Accommodation_Id
                                                select a).ToList();

                            var AccoFacilities = (from a in context.Accommodation_Facility
                                                  where a.Accommodation_Id == Acco.Accommodation_Id
                                                  select a).ToList();

                            var AccoRoutes = (from a in context.Accommodation_RouteInfo
                                              where a.Accommodation_Id == Acco.Accommodation_Id
                                              select a).ToList();

                            var AccoMedia = (from a in context.Accommodation_Media
                                             where a.Accommodation_Id == Acco.Accommodation_Id
                                             select a).ToList();

                            //create new mongo object record
                            var newHotel = new HotelsHotel();
                            newHotel.SupplierHotelID = string.Empty;
                            newHotel.HotelId = Acco.CompanyHotelID.ToString();
                            newHotel.name = Acco.HotelName;

                            var star = new HotelsHotelStarRating();
                            star.Level = Acco.HotelRating ?? string.Empty;
                            newHotel.StarRating = star;
                            star = null;

                            newHotel.credicards = (from a in AccoClassAttr where a.Accommodation_Id == Acco.Accommodation_Id && a.AttributeType == "Product" && a.IsActive == true && a.AttributeSubType == "Credit Cards" select a.AttributeValue).FirstOrDefault();
                            newHotel.credicards = newHotel.credicards ?? string.Empty;

                            newHotel.areatransportation = (from a in AccoClassAttr where a.Accommodation_Id == Acco.Accommodation_Id && a.AttributeType == "Product" && a.IsActive == true && a.AttributeSubType == "Area" select a.AttributeValue).FirstOrDefault();
                            newHotel.areatransportation = newHotel.areatransportation ?? string.Empty;

                            newHotel.restaurants = (from a in AccoDesc where a.Accommodation_Id == Acco.Accommodation_Id && a.DescriptionType == "Restaurant Description" && a.IsActive == true select a.Description).FirstOrDefault();
                            newHotel.restaurants = newHotel.restaurants ?? string.Empty;

                            newHotel.meetingfacility = (from a in AccoDesc where a.Accommodation_Id == Acco.Accommodation_Id && a.DescriptionType == "Meeting Facilities" && a.IsActive == true select a.Description).FirstOrDefault();
                            newHotel.meetingfacility = newHotel.meetingfacility ?? string.Empty;

                            newHotel.description = (from a in AccoDesc where a.Accommodation_Id == Acco.Accommodation_Id && a.DescriptionType == "Long" && a.IsActive == true select a.Description).FirstOrDefault();
                            newHotel.description = newHotel.description ?? string.Empty;

                            newHotel.highlight = string.Empty;
                            newHotel.overview = string.Empty;

                            newHotel.theme = (from a in AccoClassAttr where a.Accommodation_Id == Acco.Accommodation_Id && a.AttributeType == "Product" && a.IsActive == true && a.AttributeSubType == "Theme" select a.AttributeValue).FirstOrDefault();
                            newHotel.theme = newHotel.theme ?? string.Empty;

                            var landmark = (from a in AccoNearBy where a.Accomodation_Id == Acco.Accommodation_Id select a).FirstOrDefault();
                            if (landmark != null)
                            {
                                newHotel.Landmark = landmark.PlaceName ?? string.Empty;
                                newHotel.LandmarkCategory = landmark.PlaceCategory ?? string.Empty;
                                newHotel.LandmarkDescription = landmark.Description ?? string.Empty;
                            }
                            else
                            {
                                newHotel.Landmark = string.Empty;
                                newHotel.LandmarkCategory = string.Empty;
                                newHotel.LandmarkDescription = string.Empty;
                            }

                            newHotel.thumb = string.Empty;

                            newHotel.checkintime = (Acco.CheckInTime ?? string.Empty);
                            newHotel.checkouttime = (Acco.CheckOutTime ?? string.Empty);
                            newHotel.rooms = (Acco.TotalRooms ?? string.Empty);
                            newHotel.HotelChain = (Acco.Chain ?? string.Empty);
                            newHotel.BrandName = (Acco.Brand ?? string.Empty);
                            newHotel.recommends = (Acco.CompanyRecommended ?? false).ToString();
                            newHotel.latitude = (Acco.Latitude ?? string.Empty);
                            newHotel.longitude = (Acco.Longitude ?? string.Empty);

                            var AccoContact = (from c in AccoContacts
                                               orderby c.Create_Date descending
                                               where c.Accommodation_Id == Acco.Accommodation_Id
                                               select c).FirstOrDefault();

                            var address = new HotelsHotelAddress();
                            address.address = (Acco.StreetName ?? string.Empty);
                            address.city = (Acco.city ?? string.Empty);
                            address.state = (Acco.State_Name ?? string.Empty);
                            address.country = (Acco.country ?? string.Empty);
                            address.pincode = (Acco.PostalCode ?? string.Empty);
                            address.location = (from a in AccoClassAttr where a.Accommodation_Id == Acco.Accommodation_Id && a.AttributeType == "Product" && a.IsActive == true && a.AttributeSubType == "Location" select a.AttributeValue).FirstOrDefault();

                            if (AccoContact != null)
                            {
                                address.phone = (AccoContact.Telephone ?? string.Empty);
                                address.fax = (AccoContact.Fax ?? string.Empty);

                                newHotel.email = (AccoContact.Email ?? string.Empty);
                                newHotel.website = (AccoContact.WebSiteURL ?? string.Empty);
                            }
                            else
                            {
                                address.phone = string.Empty;
                                address.fax = string.Empty;

                                newHotel.email = string.Empty;
                                newHotel.website = string.Empty;
                            }
                            newHotel.Address = address;

                            AccoContact = null;
                            address = null;

                            var AccoImages = (from m in AccoMedia
                                              where m.Accommodation_Id == Acco.Accommodation_Id
                                              select new HotelsHotelImage
                                              {
                                                  path = m.Media_URL
                                              }).ToArray();

                            newHotel.image = AccoImages;
                            AccoImages = null;

                            var AccoFacility = (from f in AccoFacilities
                                                where f.Accommodation_Id == Acco.Accommodation_Id
                                                select new HotelsHotelFacility
                                                {
                                                    name = f.FacilityType
                                                }).ToArray();
                            newHotel.HotelFacility = AccoFacility;
                            AccoFacility = null;

                            var amenity = new HotelsHotelHotelAmenity();
                            amenity.Air_Conditioning = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "General" && a.FacilityType == "Air conditioning" select a).Count() == 0 ? false : true;
                            amenity.banquet = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Food and drink" && a.FacilityType == "Banquet facilities" select a).Count() == 0 ? false : true;
                            amenity.Bar = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Food and drink" && a.FacilityType == "Lounges/bars" select a).Count() == 0 ? false : true;
                            amenity.Business_Centre = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Working Away" && a.FacilityType == "Business center" select a).Count() == 0 ? false : true;
                            amenity.Coffee_Shop = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Food and drink" && a.FacilityType == "Coffee shop" select a).Count() == 0 ? false : true;
                            amenity.conference = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Working Away" && a.FacilityType == "Conference facilities" select a).Count() == 0 ? false : true;
                            amenity.fitness = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Things to do" && a.FacilityType == "Exercise gym" select a).Count() == 0 ? false : true;
                            amenity.forex = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Services" && a.FacilityType == "Currency exchange" select a).Count() == 0 ? false : true;
                            amenity.games = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Things to do" && a.FacilityType == "Game room" select a).Count() == 0 ? false : true;
                            amenity.Golf = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Things to do" && a.FacilityType == "Golf" select a).Count() == 0 ? false : true;
                            amenity.Health_Club = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Relaxation and Rejuvenation" && a.FacilityType == "Health club" select a).Count() == 0 ? false : true;
                            amenity.Internet_Access = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Internet" && a.FacilityType == "High speed internet access" select a).Count() == 0 ? false : true;
                            amenity.Parking = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "General" && a.FacilityType == "On-Site parking" select a).Count() == 0 ? false : true;
                            amenity.Pets = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "General" && a.FacilityType == "Pets allowed" select a).Count() == 0 ? false : true;
                            amenity.Restaurant = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Food and drink" && a.FacilityType == "Restaurant" select a).Count() == 0 ? false : true;
                            amenity.Room_Service = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Food and drink" && a.FacilityType == "Room service" select a).Count() == 0 ? false : true;
                            amenity.shopping = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "General" && a.FacilityType == "Shops and commercial services" select a).Count() == 0 ? false : true;
                            amenity.Swimming_Pool = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Things to do" && a.FacilityType == "Pool" select a).Count() == 0 ? false : true;
                            amenity.Tennis_Court = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Things to do" && a.FacilityType == "Tennis court" select a).Count() == 0 ? false : true;
                            amenity.travel = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "General" && a.FacilityType == "Travel Agency" select a).Count() == 0 ? false : true;
                            amenity.Wheel_Chair = (from a in AccoFacilities where a.Accommodation_Id == Acco.Accommodation_Id && a.IsActive == true && a.FacilityCategory == "Accessibility" && a.FacilityType == "Wheelchair access" select a).Count() == 0 ? false : true;
                            newHotel.HotelAmenity = amenity;
                            amenity = null;

                            var distance = new HotelsHotelHotelDistance();
                            distance.DistancefromAirport = (from a in AccoRoutes where a.Accommodation_Id == Acco.Accommodation_Id && a.FromPlace == "Airport" && a.IsActive == true select a.DistanceFromProperty + " " + a.DistanceUnit).FirstOrDefault();
                            distance.DistancefromAirport = distance.DistancefromAirport ?? string.Empty;

                            distance.DistancefromBus = (from a in AccoRoutes where a.Accommodation_Id == Acco.Accommodation_Id && a.FromPlace == "Bus Station" && a.IsActive == true select a.DistanceFromProperty + " " + a.DistanceUnit).FirstOrDefault();
                            distance.DistancefromBus = distance.DistancefromBus ?? string.Empty;

                            distance.DistancefromCityCenter = (from a in AccoRoutes where a.Accommodation_Id == Acco.Accommodation_Id && a.FromPlace == "City Centre" && a.IsActive == true select a.DistanceFromProperty + " " + a.DistanceUnit).FirstOrDefault();
                            distance.DistancefromCityCenter = distance.DistancefromCityCenter ?? string.Empty;

                            distance.DistancefromStation = (from a in AccoRoutes where a.Accommodation_Id == Acco.Accommodation_Id && a.FromPlace == "Train Station" && a.IsActive == true select a.DistanceFromProperty + " " + a.DistanceUnit).FirstOrDefault();
                            distance.DistancefromStation = distance.DistancefromStation ?? string.Empty;

                            newHotel.HotelDistance = distance;
                            distance = null;

                            collection.InsertOneAsync(newHotel);

                            newHotel = null;

                            AccoClassAttr = null;
                            AccoDesc = null;
                            AccoNearBy = null;
                            AccoContacts = null;
                            AccoFacilities = null;
                            AccoRoutes = null;
                            AccoMedia = null;
                        }

                        collection = null;
                        _database = null;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update_HotelMaster_ByHotelId(string HotelId)
        {
            try
            {
                Delete_HotelMaster_ByHotelId(HotelId);
                Insert_HotelMaster_ByHotelId(HotelId);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete_HotelMaster_ByHotelId(string HotelId)
        {
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<HotelsHotel>("Accommodations");
                collection.DeleteOne(f => f.HotelId == HotelId);
                collection = null;
                _database = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Activity Master
        public void Insert_ActivityMaster_ByActivityId(string ActivityId)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Update_ActivityMaster_ByActivityId(string ActivityId)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete_ActivityMaster_ByActivityId(string ActivityId)
        {
            try
            {
                throw new NotImplementedException();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Upsert_ActivityDefinition
        public void Upsert_ActivityDefinition(Guid Activity_Flavour_Id)
        {
            try
            {
                using (DL_LoadData obj = new DL_LoadData())
                {
                    obj.LoadActivityDefinition(Activity_Flavour_Id);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Room Type Mapping

        public void Upsert_RoomTypeMapping_ByMapId(string MapId)
        {
            int mapId = Convert.ToInt32(MapId);
            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>("RoomTypeMapping");

            using (TLGX_Entities context = new TLGX_Entities())
            {
                var RoomTypeMapping = (from srtm in context.Accommodation_SupplierRoomTypeMapping.AsNoTracking()
                                       join srtmv in context.Accommodation_SupplierRoomTypeMapping_Values.AsNoTracking() on srtm.Accommodation_SupplierRoomTypeMapping_Id equals srtmv.Accommodation_SupplierRoomTypeMapping_Id
                                       join ari in context.Accommodation_RoomInfo.AsNoTracking() on srtmv.Accommodation_RoomInfo_Id equals ari.Accommodation_RoomInfo_Id
                                       join acco in context.Accommodations on ari.Accommodation_Id equals acco.Accommodation_Id
                                       join sup in context.Suppliers on srtm.Supplier_Id equals sup.Supplier_Id
                                       where srtmv.MapId == mapId
                                       select new DataContracts.Mapping.DC_HotelRoomTypeMappingRequest
                                       {
                                           Accommodation_SupplierRoomTypeMapping_Id = srtm.Accommodation_SupplierRoomTypeMapping_Id,
                                           Amenities = srtm.Amenities,
                                           Attibutes = context.Accommodation_SupplierRoomTypeAttributes
                                           .Where(w => w.RoomTypeMap_Id == srtm.Accommodation_SupplierRoomTypeMapping_Id)
                                           .Select(s => new DataContracts.Mapping.DC_RoomTypeMapping_Attributes_HRTM
                                           {
                                               Key = s.SystemAttributeKeyword,
                                               Value = s.SupplierRoomTypeAttribute
                                           }).ToList(),
                                           BathRoomType = srtm.BathRoomType,
                                           BeddingConfig = srtm.BeddingConfig,
                                           Bedrooms = srtm.Bedrooms,
                                           BedType = srtm.BedTypeCode,
                                           ChildAge = srtm.ChildAge,
                                           ExtraBed = srtm.ExtraBed,
                                           FloorName = srtm.FloorName,
                                           FloorNumber = srtm.FloorNumber,
                                           MatchingScore = srtmv.MatchingScore,
                                           MaxAdults = srtm.MaxAdults,
                                           MaxChild = srtm.MaxChild,
                                           MaxGuestOccupancy = srtm.MaxGuestOccupancy,
                                           MaxInfants = srtm.MaxInfants,
                                           MinGuestOccupancy = srtm.MinGuestOccupancy,
                                           PromotionalVendorCode = srtm.PromotionalVendorCode,
                                           Quantity = srtm.Quantity,
                                           RatePlan = srtm.RatePlan,
                                           RatePlanCode = srtm.RatePlanCode,
                                           RoomDescription = srtm.RoomDescription,
                                           RoomLocationCode = srtm.RoomLocationCode,
                                           RoomSize = srtm.RoomSize,
                                           RoomView = srtm.RoomViewCode,
                                           Smoking = srtm.Smoking,
                                           Status = srtmv.UserMappingStatus,
                                           supplierCode = sup.Code.ToUpper(),
                                           SupplierProductId = srtm.SupplierProductId.ToUpper(),
                                           SupplierRoomCategory = srtm.SupplierRoomCategory.ToUpper(),
                                           SupplierRoomCategoryId = srtm.SupplierRoomCategoryId.ToUpper(),
                                           SupplierRoomId = srtm.SupplierRoomId.ToUpper(),
                                           SupplierRoomName = srtm.SupplierRoomName.ToUpper(),
                                           SupplierRoomTypeCode = srtm.SupplierRoomTypeCode.ToUpper(),
                                           SystemNormalizedRoomType = ari.TX_RoomName,
                                           SystemProductCode = acco.CompanyHotelID,
                                           SystemRoomCategory = ari.RoomCategory,
                                           SystemRoomTypeCode = ari.CommonRoomId,
                                           SystemRoomTypeMapId = srtmv.MapId,
                                           SystemRoomTypeName = ari.RoomName,
                                           SystemStrippedRoomType = ari.TX_RoomName_Stripped
                                       }).FirstOrDefault();

                var filter = Builders<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>.Filter.Eq(c => c.SystemRoomTypeMapId, RoomTypeMapping.SystemRoomTypeMapId);
                collection.ReplaceOne(filter, RoomTypeMapping, new UpdateOptions { IsUpsert = true });
                filter = null;
                collection = null;
                _database = null;
            }
        }        

        public void LoadCompanyAccommodationProductMappingSingle(string MapId,string SupplierProductReference,string Supplier_id)
        {

            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_ConpanyAccommodationMapping>("CompanyAccommodationProductMapping");
                List<DC_ConpanyAccommodationRoomMapping> lstMappedRooms = new List<DC_ConpanyAccommodationRoomMapping>();
                List<DataContracts.Mapping.DC_ConpanyAccommodationMapping> productMapList = new List<DataContracts.Mapping.DC_ConpanyAccommodationMapping>();
                StringBuilder sbSelectAccoRoomMapped = new StringBuilder();
                using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                    new System.Transactions.TransactionOptions()
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                        Timeout = new TimeSpan(0, 15, 0)
                    }))
                    {

                #region Generating Query
                        StringBuilder sbSelectAccoMaster = new StringBuilder();

                        sbSelectAccoMaster.Append(@"  
                                    select  
	                                      apm.Supplier_Id					     as [SupplierId] 
                                        , upper(sup.Code)	 as [SupplierCode]	
                                        , (upper(sup.Code)	 + '_'+    apm.SupplierProductReference + '_'+  av.CompanyId + '_' +   av.CompanyProductId) as _id                                                                          
	                                    , apm.SupplierProductReference		 as [SupplierProductCode]
	                                    , apm.ProductName					 as [SupplierProductName]
	                                    , av.ProductName					 as [CompanyProductName]
                                        , av.CommonProductId                 as [CommonProductId]
	                                    , av.CompanyId						 as [TLGXCompanyId]
                                        , av.CompanyProductId                as [CompanyProductId]
	                                    , av.CompanyName					 as [TLGXCompanyName]	 
	                                    , av.Country						 as [CountryName]
	                                    , av.City							 as [CityName]
	                                    , av.State							 as [StateName]
                                        , 'Accommodation'                    as [ProductCategory]
	                                    , av.ProductCatSubType				 as [ProductCategorySubType]
	                                    , av.Brand							 as [Brand]
	                                    , av.Chain							 as [Chain]
	                                    , av.Interest						 as [Interest]
	                                    , av.Accommodation_CompanyVersion_Id as [Accommodation_CompanyVersion_Id]
                                        , av.StarRating						 as [Rating]
                                        , isnull(acc.IsRoomMappingCompleted,0)  as [IsRoomMappingCompleted] 
										, isnull(apm.IsDirectContract,0)     as [IsDirectContract]   
                                    from  Accommodation_ProductMapping apm with (NOLOCk)
                                    join  Accommodation_CompanyVersion av with (NOLOCk)
		                                    on av.Accommodation_Id = apm.Accommodation_Id
                                    join  Accommodation acc with (NOLOCk)
		                                    on acc.Accommodation_Id = av.Accommodation_Id
                                    join  Supplier sup with (NOLOCk)
                                            on sup.Supplier_Id=apm.Supplier_Id
                                    where 
                                        apm.IsActive = 1  and
	                                    apm.supplier_id = '" + Supplier_id + @"' and 
                                        apm.SupplierProductReference = '" + SupplierProductReference + @"' and 
	                                    apm.STATUS in ('MAPPED', 'AUTOMAPPED') --and apm.SupplierProductReference = '136329'
                                        --and av.Accommodation_CompanyVersion_Id = '151A0F5C-B336-4939-8089-8D567099DB2E'

                                              ");

                      

                        

                        sbSelectAccoRoomMapped.Append(@"  
                                  SELECT  ASRTM.SupplierRoomId
                                 ,ASRTM.SupplierRoomTypeCode
                                 ,ASRTM.SupplierRoomName
                                 ,ASRTM.SupplierRoomCategory
                                 ,ASRTM.SupplierRoomCategoryId 
                                 ,ARIC.TlgxAccoRoomId as CompanyRoomId
                                 ,ARIC.RoomName as CompanyRoomName
                                 ,ARIC.CompanyRoomCategory 
                                 ,cast(ASRTMV.MapId  as varchar(max)) as NakshatraRoomMappingId
                                          ,ARIC.Accommodation_CompanyVersion_Id
                                          ,ASRTM.Supplier_Id 
                                          ,ASRTM.SupplierProductId
                                 ,ARIC.CommonRoomId as TLGXCommonRoomId   
                        from Accommodation_SupplierRoomTypeMapping ASRTM with(nolock) 
                               join Accommodation_SupplierRoomTypeMapping_Values ASRTMV with(nolock) 
                                             ON ASRTM.Accommodation_SupplierRoomTypeMapping_Id = ASRTMV.Accommodation_SupplierRoomTypeMapping_Id 
                               join Accommodation_RoomInfo_CompanyVersion ARIC with(nolock) 
                                 ON ARIC.Accommodation_RoomInfo_Id = ASRTMV.Accommodation_RoomInfo_Id 
                                      where  
                                             ASRTM.Supplier_Id = '" + Supplier_id + @"' and 
                                             ASRTMV.MapId = '" + MapId + @"' and 
                                             ASRTM.SupplierProductId = '" + SupplierProductReference + @"' and 
                                             (ASRTMV.SystemMappingStatus =  'AUTOMAPPED' or ASRTMV.UserMappingStatus = 'MAPPED')

                         ");

                    #endregion
                using (TLGX_Entities context = new TLGX_Entities())
                    {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.Database.CommandTimeout = 0;

                            productMapList = context.Database.SqlQuery<DataContracts.Mapping.DC_ConpanyAccommodationMapping>(sbSelectAccoMaster.ToString()).ToList();

                            lstMappedRooms = context.Database.SqlQuery<DataContracts.Mapping.DC_ConpanyAccommodationRoomMapping>(sbSelectAccoRoomMapped.ToString()).ToList();
                     }
                        scope.Complete();
                }
                    if (productMapList?.Count() > 0)
                    {
                        foreach (var product in productMapList)
                        {
                            var filter = Builders<DataContracts.Mapping.DC_ConpanyAccommodationMapping>.Filter.Eq(c => c._id, product._id);
                            if (lstMappedRooms?.Count > 0)
                            {
                                product.MappedRooms = lstMappedRooms.Where(x => x.SupplierProductId == product.SupplierProductCode && x.Accommodation_CompanyVersion_Id == product.Accommodation_CompanyVersion_Id).ToList();
                            }
                            else
                            {
                                product.MappedRooms = new List<DC_ConpanyAccommodationRoomMapping>();
                            }

                        var accommodationMappings = collection.FindAsync(a => a._id==product._id).Result.FirstOrDefault();
                        if (accommodationMappings != null)
                        {
                            var mappedrooms = accommodationMappings.MappedRooms.Where(x => x.NakshatraRoomMappingId != MapId);
                            if (mappedrooms.Any())
                            {
                                product.MappedRooms.AddRange(mappedrooms);
                            }
                        }
                        var result = collection.ReplaceOne(filter, product, new UpdateOptions { IsUpsert = true });
                        }
                    }
                
                collection = null;
                _database = null;
                lstMappedRooms = null;
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
            }
        }

        public void Delete_RoomTypeMapping_ByMapId(string MapId)
        {
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<BsonDocument>("RoomTypeMapping");
                var filter = Builders<BsonDocument>.Filter.Eq("SystemRoomTypeMapId", Convert.ToInt32(MapId));
                var result = collection.DeleteOne(filter);
                filter = null;
                collection = null;
                _database = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Delete_CompanySpecificRoomTypeMapping_ByMapId(string MapId, string SupplierProductReference, string Supplier_id)
        {
            try
            {
                _database = MongoDBHandler.mDatabase();

                string suppliercode = string.Empty;
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Database.CommandTimeout = 0;
                    if (Guid.TryParse(Supplier_id, out Guid Sup_id))
                        suppliercode = context.Suppliers.Where(x => x.Supplier_Id == Sup_id).Select(x => x.Code.ToUpper()).FirstOrDefault();
                }
                if(_database == null)
                {
                    _database = MongoDBHandler.mDatabase();
                }
                var collection = _database.GetCollection<BsonDocument>("CompanyAccommodationProductMapping");
                var filter = Builders<BsonDocument>.Filter.Eq("SupplierCode", suppliercode);
                    filter &= Builders<BsonDocument>.Filter.Eq("SupplierProductCode", SupplierProductReference);
                    filter &= Builders<BsonDocument>.Filter.Eq("MappedRooms.NakshatraRoomMappingId", MapId);

                var update = Builders<BsonDocument>.Update.PullFilter("MappedRooms",
                    Builders<BsonDocument>.Filter.Eq("NakshatraRoomMappingId", MapId));
                var result = collection.FindOneAndUpdateAsync(filter, update).Result;

                filter = null;
                collection = null;
                _database = null;
                suppliercode = null;
                result = null;
                update = null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion
    }
}
