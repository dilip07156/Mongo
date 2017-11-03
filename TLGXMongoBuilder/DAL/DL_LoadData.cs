﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataContracts;
using System.ServiceModel;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using MongoDB.Driver.Builders;

namespace DAL
{
    public class DL_LoadData : IDisposable
    {
        public void Dispose()
        {
        }

        protected static IMongoDatabase _database;
        public void LoadCountryMaster()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("CountryMaster");

                    var collection = _database.GetCollection<BsonDocument>("CountryMaster");

                    var CountryList = from c in context.m_CountryMaster
                                      orderby c.Name
                                      select new
                                      {
                                          c.Name,
                                          c.Code
                                      };

                    List<BsonDocument> docs = new List<BsonDocument>();
                    foreach (var country in CountryList)
                    {
                        var document = new BsonDocument
                        {
                            { "CountryName", country.Name },
                            { "CountryCode", country.Code }
                        };
                        docs.Add(document);
                        document = null;
                    }
                    collection.InsertMany(docs);
                    docs = null;
                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }

        }

        public void LoadCityMaster()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("CityMaster");

                    var collection = _database.GetCollection<BsonDocument>("CityMaster");
                    var CityList = from city in context.m_CityMaster
                                   join country in context.m_CountryMaster on city.Country_Id equals country.Country_Id
                                   orderby country.Name, city.Name
                                   select new
                                   {
                                       CountryName = country.Name,
                                       CountryCode = country.Code ?? string.Empty,
                                       CityName = city.Name,
                                       CityCode = city.Code ?? string.Empty,
                                       StateName = city.StateName ?? string.Empty,
                                       StateCode = city.StateCode ?? string.Empty
                                   };

                    List<BsonDocument> docs = new List<BsonDocument>();
                    foreach (var city in CityList)
                    {
                        var document = new BsonDocument
                        {
                            { "CityName", city.CityName},
                            { "CityCode", city.CityCode },
                            { "StateName", city.StateName },
                            { "StateCode", city.StateCode },
                            { "CountryCode", city.CountryCode },
                            { "CountryName", city.CountryName },
                        };
                        docs.Add(document);
                        document = null;
                    }
                    collection.InsertMany(docs);
                    docs = null;
                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadSupplierMaster()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("Supplier");

                    var collection = _database.GetCollection<BsonDocument>("Supplier");
                    var SupplierList = from s in context.Suppliers
                                       orderby s.Name
                                       select new
                                       {
                                           SupplierName = s.Name,
                                           SupplierCode = s.Code,
                                           SupplierType = s.SupplierType ?? string.Empty,
                                           SupplierOwner = s.SupplierOwner ?? string.Empty
                                       };

                    List<BsonDocument> docs = new List<BsonDocument>();
                    foreach (var supplier in SupplierList)
                    {
                        var document = new BsonDocument
                        {
                            { "SupplierName", supplier.SupplierName },
                            { "SupplierCode", supplier.SupplierCode },
                            { "SupplierOwner", supplier.SupplierOwner },
                            { "SupplierType", supplier.SupplierType }
                        };
                        docs.Add(document);
                        document = null;
                    }
                    collection.InsertMany(docs);
                    docs = null;
                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadCountryMapping()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("CountryMapping");

                    var collection = _database.GetCollection<DataContracts.Mapping.DC_CountryMapping>("CountryMapping");
                    var SupplierList = (from cm in context.m_CountryMapping
                                        join c in context.m_CountryMaster on cm.Country_Id equals c.Country_Id
                                        join s in context.Suppliers on cm.Supplier_Id equals s.Supplier_Id
                                        select new DataContracts.Mapping.DC_CountryMapping
                                        {
                                            SupplierName = s.Name,
                                            SupplierCode = s.Code,
                                            CountryCode = c.Code,
                                            CountryName = c.Name,
                                            //CountryMapping_Id = cm.CountryMapping_Id.ToString(),
                                            //Supplier_Id = s.Supplier_Id.ToString(),
                                            //Country_Id = c.Country_Id.ToString(),
                                            SupplierCountryName = (cm.CountryName ?? string.Empty),
                                            SupplierCountryCode = (cm.CountryCode ?? string.Empty),
                                            MapId = cm.MapID ?? 0
                                        }).ToList();

                    collection.InsertMany(SupplierList);
                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadCityMapping()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("CityMapping");

                    var collection = _database.GetCollection<DataContracts.Mapping.DC_CityMapping>("CityMapping");
                    var CityList = (from cm in context.m_CityMapping
                                    join city in context.m_CityMaster on cm.City_Id equals city.City_Id
                                    join country in context.m_CountryMaster on cm.Country_Id equals country.Country_Id
                                    join supplier in context.Suppliers on cm.Supplier_Id equals supplier.Supplier_Id
                                    where (cm.Status ?? string.Empty) != "UNMAPPED"
                                    select new DataContracts.Mapping.DC_CityMapping
                                    {
                                        //CityMapping_Id = cm.CityMapping_Id.ToString(),
                                        CityName = (city.Name ?? string.Empty),
                                        CityCode = (city.Code ?? string.Empty),
                                        SupplierCityCode = (cm.CityCode ?? string.Empty),
                                        SupplierCityName = (cm.CityName ?? string.Empty),
                                        SupplierName = supplier.Name,
                                        SupplierCode = supplier.Code,
                                        CountryCode = country.Code,
                                        CountryName = country.Name,
                                        SupplierCountryName = (cm.CountryName ?? string.Empty),
                                        SupplierCountryCode = (cm.CountryCode ?? string.Empty),
                                        MapId = cm.MapID ?? 0
                                    }).ToList();

                    collection.InsertMany(CityList);
                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadProductMapping()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("ProductMapping");

                    var collection = _database.GetCollection<DataContracts.Mapping.DC_ProductMapping>("ProductMapping");
                    var productMapList = (from apm in context.Accommodation_ProductMapping
                                          join a in context.Accommodations on apm.Accommodation_Id equals a.Accommodation_Id
                                          select new DataContracts.Mapping.DC_ProductMapping
                                          {
                                              SupplierCode = apm.SupplierName.ToUpper(),
                                              SupplierProductCode = apm.SupplierProductReference.ToUpper(),
                                              SupplierCountryCode = apm.CountryCode.ToUpper(),
                                              SupplierCountryName = apm.CountryName.ToUpper(),
                                              SupplierCityCode = apm.CityCode.ToUpper(),
                                              SupplierCityName = apm.CityName.ToUpper(),
                                              SupplierProductName = apm.ProductName.ToUpper(),
                                              MappingStatus = apm.Status.ToUpper(),
                                              MapId = apm.MapId ?? 0,
                                              SystemProductCode = a.CompanyHotelID.ToString().ToUpper(),
                                              SystemProductName = a.HotelName.ToUpper(),
                                              SystemCountryName = a.country.ToUpper(),
                                              SystemCityName = a.city.ToUpper(),
                                              SystemProductType = a.ProductCategorySubType.ToUpper()
                                          }).ToList();

                    if (productMapList.Count() > 0)
                    {
                        collection.InsertMany(productMapList);
                        collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode));
                        collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemProductCode));
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

        public void LoadProductMappingLite()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("ProductMappingLite");

                    var collection = _database.GetCollection<DataContracts.Mapping.DC_ProductMappingLite>("ProductMappingLite");
                    var productMapList = (from apm in context.Accommodation_ProductMapping
                                          join a in context.Accommodations on apm.Accommodation_Id equals a.Accommodation_Id
                                          select new DataContracts.Mapping.DC_ProductMappingLite
                                          {
                                              SupplierCode = apm.SupplierName.ToUpper(),
                                              SupplierProductCode = apm.SupplierProductReference.ToUpper(),
                                              MapId = apm.MapId ?? 0,
                                              SystemProductCode = a.CompanyHotelID.ToString().ToUpper(),
                                          }).ToList();

                    if (productMapList.Count() > 0)
                    {
                        collection.InsertMany(productMapList);
                        collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMappingLite>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode));
                        collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMappingLite>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemProductCode));
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

        public void LoadActivityMapping()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("ActivityMapping");

                    var collection = _database.GetCollection<DataContracts.Mapping.DC_ProductMapping>("ActivityMapping");
                    var productMapList = (from apm in context.Activity_SupplierProductMapping
                                          join a in context.Activities on apm.Activity_ID equals a.Activity_Id
                                          select new DataContracts.Mapping.DC_ProductMapping
                                          {
                                              SupplierCode = apm.SupplierName.ToUpper(),
                                              SupplierProductCode = apm.SuplierProductCode.ToUpper(),
                                              SupplierCountryCode = apm.SupplierCountryCode.ToUpper(),
                                              SupplierCountryName = apm.SupplierCountryName.ToUpper(),
                                              SupplierCityCode = apm.SupplierCityCode.ToUpper(),
                                              SupplierCityName = apm.SupplierCityName.ToUpper(),
                                              SupplierProductName = apm.SupplierProductName.ToUpper(),
                                              MappingStatus = apm.MappingStatus.ToUpper(),
                                              MapId = apm.MapID ?? 0,
                                              SystemProductCode = a.CommonProductID.ToString().ToUpper(),
                                              SystemProductName = a.Product_Name.ToUpper(),
                                              SystemCountryName = a.Country.ToUpper(),
                                              SystemCityName = a.City.ToUpper(),
                                              SystemProductType = "Activity".ToUpper() //a.ProductCategorySubType
                                          }).ToList();

                    collection.InsertMany(productMapList);
                    collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode));
                    collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemProductCode));

                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadActivityMappingLite()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("ActivityMappingLite");

                    var collection = _database.GetCollection<DataContracts.Mapping.DC_ProductMappingLite>("ActivityMappingLite");
                    var productMapList = (from apm in context.Activity_SupplierProductMapping
                                          join a in context.Activities on apm.Activity_ID equals a.Activity_Id
                                          select new DataContracts.Mapping.DC_ProductMappingLite
                                          {
                                              SupplierCode = apm.SupplierName.ToUpper(),
                                              SupplierProductCode = apm.SuplierProductCode.ToUpper(),
                                              MapId = apm.MapID ?? 0,
                                              SystemProductCode = a.CommonProductID.ToString().ToUpper(),
                                          }).ToList();

                    collection.InsertMany(productMapList);
                    collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMappingLite>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode));
                    collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMappingLite>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemProductCode));

                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadRoomTypeMapping()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("RoomTypeMapping");

                    var collection = _database.GetCollection<DataContracts.Mapping.DC_RoomTypeMapping>("RoomTypeMapping");

                    var Accommodation = (from a in context.Accommodations select a).AsQueryable();
                    var Accommodation_RoomInfo = (from a in context.Accommodation_RoomInfo select a).AsQueryable();
                    var Accommodation_SupplierRoomTypeAttributes = (from a in context.Accommodation_SupplierRoomTypeAttributes select a).AsQueryable();

                    var Accommodation_SupplierRoomTypeMapping = (from a in context.Accommodation_SupplierRoomTypeMapping select a).AsQueryable();
                    IQueryable<DAL.Accommodation_SupplierRoomTypeMapping> Accommodation_SupplierRoomTypeMapping_Loop;

                    int TotalRecords = Accommodation_SupplierRoomTypeMapping.Count();
                    int iBatchSize = 100;
                    int iDataInsertedCounter = 0;
                    bool bAllDataInserted = false;

                    while (!bAllDataInserted)
                    {
                        Accommodation_SupplierRoomTypeMapping_Loop = Accommodation_SupplierRoomTypeMapping.Where(w => w.MapId > iDataInsertedCounter && w.MapId <= (iDataInsertedCounter + iBatchSize)).Select(s => s);

                        if (Accommodation_SupplierRoomTypeMapping_Loop.Count() > 0)
                        {
                            var roomTypeMapList = (from asrtm in Accommodation_SupplierRoomTypeMapping_Loop
                                                   join acco in Accommodation on asrtm.Accommodation_Id equals acco.Accommodation_Id
                                                   join accori in Accommodation_RoomInfo on new { AccoId = acco.Accommodation_Id, AccoRIId = asrtm.Accommodation_RoomInfo_Id ?? Guid.Empty } equals new { AccoId = accori.Accommodation_Id ?? Guid.Empty, AccoRIId = accori.Accommodation_RoomInfo_Id } into accoritemp
                                                   from accorinew in accoritemp.DefaultIfEmpty()
                                                   select new DataContracts.Mapping.DC_RoomTypeMapping
                                                   {
                                                       SupplierCode = asrtm.SupplierName.ToUpper().Trim(),
                                                       SupplierProductCode = asrtm.SupplierProductId.ToUpper().Trim(),
                                                       SupplierRoomTypeCode = asrtm.SupplierRoomId.ToUpper().Trim(),
                                                       SupplierRoomTypeName = asrtm.SupplierRoomName.ToUpper().Trim(),
                                                       Status = asrtm.MappingStatus.ToUpper().Trim(),
                                                       SystemRoomTypeMapId = asrtm.MapId.ToString(),
                                                       SystemProductCode = acco.CompanyHotelID.ToString().ToUpper().Trim(),
                                                       SystemRoomTypeCode = accorinew.RoomId.ToUpper().Trim(),
                                                       SystemRoomTypeName = accorinew.RoomCategory.ToUpper().Trim(),
                                                       SystemNormalizedRoomType = asrtm.TX_RoomName.ToUpper().Trim(),
                                                       SystemStrippedRoomType = asrtm.Tx_StrippedName.ToUpper().Trim(),
                                                       Attibutes = (Accommodation_SupplierRoomTypeAttributes.Where(w => w.RoomTypeMap_Id == asrtm.Accommodation_SupplierRoomTypeMapping_Id).Select(s => new DataContracts.Mapping.DC_RoomTypeMapping_Attributes { Type = s.SystemAttributeKeyword, Value = s.SupplierRoomTypeAttribute }).ToList())
                                                   }).ToList();

                            if (roomTypeMapList.Count() > 0)
                            {
                                collection.InsertMany(roomTypeMapList);
                            }

                            iDataInsertedCounter = iDataInsertedCounter + Accommodation_SupplierRoomTypeMapping_Loop.Count();

                        }
                        else
                        {
                            bAllDataInserted = true;
                        }
                    }

                    collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_RoomTypeMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode).Ascending(_ => _.SupplierRoomTypeCode));
                    collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_RoomTypeMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode).Ascending(_ => _.SupplierRoomTypeName));

                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadKeywords()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("Keywords");

                    var collection = _database.GetCollection<DataContracts.Masters.DC_Keywords>("Keywords");

                    var searchList = (from k in context.m_keyword
                                      where k.Status == "ACTIVE"
                                      orderby k.Sequence ?? 0
                                      select new DataContracts.Masters.DC_Keywords
                                      {
                                          Keyword = k.Keyword,
                                          Sequence = k.Sequence ?? 0,
                                          EntityFor = k.EntityFor,
                                          AttributeType = k.AttributeType,
                                          AttributeSubLevelValue = k.AttributeSubLevelValue,
                                          AttributeSubLevel = k.AttributeSubLevel,
                                          AttributeLevel = k.AttributeLevel,
                                          Attribute = k.Attribute ?? false,
                                          Aliases = (from a in context.m_keyword_alias
                                                     where a.Keyword_Id == k.Keyword_Id && a.Status == "ACTIVE"
                                                     orderby (a.Sequence ?? 0), (a.NoOfHits ?? 0) descending
                                                     select new DataContracts.Masters.DC_Keyword_Aliases
                                                     {
                                                         Value = a.Value,
                                                         Sequence = a.Sequence ?? 0,
                                                         NoOfHits = a.NoOfHits ?? 0
                                                     }).ToList()
                                      }).ToList();

                    if (searchList.Count() > 0)
                    {
                        collection.InsertMany(searchList);
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

        public void LoadActivityDefinition()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();

                    _database.DropCollection("ActivityDefinitions");

                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");
                    var ActivityList = (from a in context.Activity_Flavour
                                       where a.CityCode != null
                                       select a);

                    foreach (var Activity in ActivityList)
                    {

                        ////check if record is already exists
                        //var searchResultCount = collection.Find(f => f.Le == Acco.CompanyHotelID.ToString()).Count();
                        //if (searchResultCount > 0)
                        //{
                        //    continue;
                        //}

                        var ActivityClassAttr = (from a in context.Activity_ClassificationAttributes
                                                 where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                                 select a).ToList();

                        var ActivityDesc = (from a in context.Activity_Descriptions
                                            where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                            select a).ToList();

                        var ActivityInc = (from a in context.Activity_Inclusions
                                           where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                           select a).ToList();

                        var ActivityIncDetails = (from a in context.Activity_InclusionDetails
                                                  where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                                  select a).ToList();

                        var ActivityPolicy = (from a in context.Activity_Policy
                                              where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                              select a).ToList();

                        var ActivityMedia = (from a in context.Activity_Media
                                             where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                             select a).ToList();

                        var ActivityReviews = (from a in context.Activity_ReviewsAndScores
                                               where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                               select a).ToList();

                        var ActivitySPM = (from a in context.Activity_SupplierProductMapping
                                           where a.Activity_ID == Activity.Activity_Flavour_Id
                                           select a).ToList();

                        var ActivityDeals = (from a in context.Activity_Deals
                                             where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                             select a).ToList();

                        var ActivityPrices = (from a in context.Activity_Prices
                                              where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                              select a).ToList();

                        var ActivitySPMCA = (from a in context.Activity_SupplierProductMapping_CA
                                             where a.Activity_SupplierProductMapping_CA_Id == Activity.Activity_Flavour_Id
                                             select a).ToList();

                        var ActivityFO = (from a in context.Activity_FlavourOptions
                                          where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                          select a).ToList();

                        //create new mongo object record
                        var newActivity = new DataContracts.Activity.ActivityDefinition();

                        //newActivity.Activity_Flavour_Id = Activity.Activity_Flavour_Id.ToString();

                        newActivity.SystemActivityCode = Convert.ToInt32(Activity.CommonProductNameSubType_Id);

                        newActivity.SupplierCompanyCode = ActivitySPM.Select(s => s.SupplierCode).FirstOrDefault();

                        newActivity.SupplierProductCode = ActivitySPM.Select(s => s.SuplierProductCode).FirstOrDefault();//Activity.CompanyProductNameSubType_Id;

                        newActivity.Category = Activity.ProductCategorySubType;

                        newActivity.Type = Activity.ProductType;

                        newActivity.SubType = Activity.ProductNameSubType;

                        newActivity.Name = Activity.ProductName;

                        newActivity.Description = (ActivityDesc.Where(w => w.DescriptionType == "Short").Select(s => s.Description).FirstOrDefault());

                        newActivity.Session = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "Sessions").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.StartTime = (ActivityClassAttr.Where(w => w.AttributeType == "Duration" && w.AttributeSubType == "StartTime").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.EndTime = (ActivityClassAttr.Where(w => w.AttributeType == "Duration" && w.AttributeSubType == "EndTime").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.DeparturePoint = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "DeparturePoint").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.ReturnDetails = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "ReturnDetails").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.DaysOfTheWeek = (ActivityClassAttr.Where(w => w.AttributeType == "Duration" && w.AttributeSubType == "DaysofWeek").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.PhysicalIntensity = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "PhysicalIntensity").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.Overview = (ActivityDesc.Where(w => w.DescriptionType == "Long").Select(s => s.Description).FirstOrDefault());

                        newActivity.Recommended = (Activity.CompanyReccom ?? false).ToString();

                        newActivity.CountryName = Activity.Country;

                        newActivity.CountryCode = Activity.CountryCode;

                        newActivity.CityName = Activity.City;

                        newActivity.CityCode = Activity.CityCode;

                        newActivity.StarRating = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "Rating").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.NumberOfPassengers = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "NoofPassengers").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.NumberOfReviews = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "NoofReviews").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.NumberOfLikes = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "NoofLikes").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.NumberOfViews = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "NoofViews").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.ActivityInterests = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "Interests").Select(s => s.AttributeValue).ToArray());

                        newActivity.Inclusions = (ActivityInc.Where(w => (w.IsInclusion ?? false) == true).Select(s => new DataContracts.Activity.Inclusions { Name = s.InclusionName, Description = s.InclusionDescription }).ToList());

                        newActivity.Exclusions = (ActivityInc.Where(w => (w.IsInclusion ?? false) == false).Select(s => new DataContracts.Activity.Exclusions { Name = s.InclusionName, Description = s.InclusionDescription }).ToList());

                        newActivity.Highlights = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "Highlights").Select(s => s.AttributeValue).ToArray());

                        newActivity.BookingPolicies = (ActivityClassAttr.Where(w => w.AttributeType == "Policies" && w.AttributeSubType != "TermsAndConditions").Select(s => new DataContracts.Activity.ImportantInfoAndBookingPolicies { InfoType = s.AttributeSubType, InfoText = s.AttributeValue }).ToList());

                        newActivity.TermsAndConditions = (ActivityClassAttr.Where(w => w.AttributeType == "Policies" && w.AttributeSubType == "TermsAndConditions").Select(s => s.AttributeValue).ToArray());

                        newActivity.ActivityMedia = (ActivityMedia.Select(s => new DataContracts.Activity.Media
                        {
                            Caption = s.Media_Caption,
                            Description = s.Description,
                            FullUrl = s.Media_URL,
                            Height = s.Media_Height ?? 0,
                            MediaType = s.Category,
                            MediaSubType = s.SubCategory,
                            SortOrder = (s.Media_Position ?? 0).ToString(),
                            ThumbUrl = s.Media_URL,
                            Width = s.Media_Width ?? 0
                        }).ToList());

                        newActivity.Duration = new DataContracts.Activity.ActivityDuration
                        {
                            Hours = (ActivityClassAttr.Where(w => w.AttributeType == "Duration" && w.AttributeSubType == "Hours").Select(s => s.AttributeValue).FirstOrDefault()),
                            Minutes = (ActivityClassAttr.Where(w => w.AttributeType == "Duration" && w.AttributeSubType == "Minutes").Select(s => s.AttributeValue).FirstOrDefault()),
                            Text = (ActivityClassAttr.Where(w => w.AttributeType == "Duration" && w.AttributeSubType == "Text").Select(s => s.AttributeValue).FirstOrDefault())
                        };

                        newActivity.ReviewScores = (ActivityReviews.Where(w => w.IsCustomerReview == false).Select(s => new DataContracts.Activity.ReviewScores { Score = s.Review_Score ?? 0, Source = s.Review_Source, Type = s.Review_Type }).ToList());

                        newActivity.CustomerReviews = (ActivityReviews.Where(w => w.IsCustomerReview == true).Select(s => new DataContracts.Activity.CustomerReviews { Score = s.Review_Score ?? 0, Source = s.Review_Source, Type = s.Review_Type, Author = s.Review_Author, Comment = s.Review_Description, Date = s.Review_PostedDate.ToString(), Title = s.Review_Title }).ToList());

                        newActivity.ActivityLocation = new DataContracts.Activity.ActivityLocation
                        {
                            Address = Activity.Street + ", " + Activity.Street2 + ", " + Activity.Street3 + ", " + Activity.Street4 + ", " + Activity.Street5,
                            Area = Activity.Area,
                            Latitude = Activity.Latitude,
                            Location = Activity.Location,
                            Longitude = Activity.Longitude
                        };

                        newActivity.TourGuideLanguages = (from a in ActivityInc
                                                          join ad in ActivityIncDetails on a.Activity_Inclusions_Id equals ad.Activity_Inclusions_Id
                                                          where a.IsInclusion ?? false == true
                                                          select new DataContracts.Activity.TourGuideLanguages
                                                          {
                                                              Language = ad.GuideLanguage,
                                                              LanguageID = ad.GuideLanguageCode
                                                          }).ToList();

                        newActivity.SupplierDetails = ActivitySPM.Select(s => new DataContracts.Activity.SupplierDetails
                        {
                            SupplierName = s.SupplierName,
                            SupplierID = s.SupplierCode,
                            TourActivityID = s.SuplierProductCode,
                            CountryCode = s.SupplierCountryCode,
                            CountryName = s.SupplierCountryName,
                            CityCode = s.SupplierCityCode,
                            CityName = s.SupplierCityName,
                            PricingCurrency = s.Currency
                        }).FirstOrDefault();

                        newActivity.Deals = ActivityDeals.Select(s => new DataContracts.Activity.Deals { Currency = s.Deal_Currency, DealId = s.DealCode, DealPrice = s.Deal_Price, DealText = s.DealText, OfferTermsAndConditions = s.Deal_TnC }).ToList();

                        newActivity.Prices = ActivityPrices.Where(w => w.Price_For == "Product").Select(s => new DataContracts.Activity.Prices { Price = s.Price, PriceType = s.Price_Type, PriceBasis = s.PriceBasis, PriceId = s.PriceCode, SupplierCurrency = s.PriceCurrency }).ToList();

                        newActivity.SimliarProducts = (from afo in ActivityFO
                                                       join ap in ActivityPrices on afo.Activity_FlavourOptions_Id equals (ap.Activity_FlavourOptions_Id ?? Guid.Empty)
                                                       where ap.Price_For == "Options" && ap.Price_Type == "MerchantNetPrice"
                                                       select new DataContracts.Activity.SimliarProducts
                                                       {
                                                           SystemActivityOptionCode = afo.TLGXActivityOptionCode,
                                                           OptionCode = afo.Activity_OptionCode,
                                                           ActivityType = afo.Activity_Type,
                                                           DealText = afo.Activity_DealText,
                                                           Options = afo.Activity_OptionName,
                                                           TotalNetPrice = ap.Price.ToString()
                                                       }).ToList();

                        newActivity.ClassificationAttrributes = ActivityClassAttr.Where(w => w.AttributeType == "Internal").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList();

                        newActivity.SystemMapping = ActivitySPM.Select(s => new DataContracts.Activity.SystemMapping { SystemID = string.Empty, SystemName = string.Empty }).FirstOrDefault();

                        collection.InsertOneAsync(newActivity);

                        newActivity = null;
                        ActivityClassAttr = null;
                        ActivityDesc = null;
                        ActivityInc = null;
                        ActivityIncDetails = null;
                        ActivityPolicy = null;
                        ActivityMedia = null;
                        ActivityReviews = null;
                        ActivitySPM = null;
                        ActivityDeals = null;
                        ActivityPrices = null;
                        ActivitySPMCA = null;
                        ActivityFO = null;
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
    }
}
