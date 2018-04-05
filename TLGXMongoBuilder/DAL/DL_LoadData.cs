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

                    var CountryList = (from c in context.m_CountryMaster
                                       orderby c.Name
                                       select new
                                       {
                                           Name = c.Name.Trim().ToUpper(),
                                           Code = c.Code.Trim().ToUpper()
                                       }).ToList();

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
                                       CountryName = country.Name.Trim().ToUpper(),
                                       CountryCode = (country.Code ?? string.Empty).Trim().ToUpper(),
                                       CityName = city.Name.Trim().ToUpper(),
                                       CityCode = (city.Code ?? string.Empty).Trim().ToUpper(),
                                       StateName = (city.StateName ?? string.Empty).Trim().ToUpper(),
                                       StateCode = (city.StateCode ?? string.Empty).Trim().ToUpper()
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
                                       where (s.StatusCode ?? string.Empty) == "ACTIVE"
                                       orderby s.Name
                                       select new
                                       {
                                           SupplierName = s.Name.Trim().ToUpper(),
                                           SupplierCode = s.Code.Trim().ToUpper(),
                                           SupplierType = (s.SupplierType ?? string.Empty).Trim().ToUpper(),
                                           SupplierOwner = (s.SupplierOwner ?? string.Empty).Trim().ToUpper()
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
                                            SupplierName = s.Name.Trim().ToUpper(),
                                            SupplierCode = s.Code.Trim().ToUpper(),
                                            CountryCode = c.Code.Trim().ToUpper(),
                                            CountryName = c.Name.Trim().ToUpper(),
                                            //CountryMapping_Id = cm.CountryMapping_Id.ToString(),
                                            //Supplier_Id = s.Supplier_Id.ToString(),
                                            //Country_Id = c.Country_Id.ToString(),
                                            SupplierCountryName = (cm.CountryName ?? string.Empty).Trim().ToUpper(),
                                            SupplierCountryCode = (cm.CountryCode ?? string.Empty).Trim().ToUpper(),
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
                                        CityName = (city.Name ?? string.Empty).ToUpper(),
                                        CityCode = (city.Code ?? string.Empty).ToUpper(),
                                        SupplierCityCode = (cm.CityCode ?? string.Empty).ToUpper(),
                                        SupplierCityName = (cm.CityName ?? string.Empty).ToUpper(),
                                        SupplierName = supplier.Name.ToUpper(),
                                        SupplierCode = supplier.Code.ToUpper(),
                                        CountryCode = country.Code.ToUpper(),
                                        CountryName = country.Name.ToUpper(),
                                        SupplierCountryName = (cm.CountryName ?? string.Empty).ToUpper(),
                                        SupplierCountryCode = (cm.CountryCode ?? string.Empty).ToUpper(),
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
                List<DataContracts.Mapping.DC_ProductMapping> productMapList = new List<DataContracts.Mapping.DC_ProductMapping>();
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;

                    productMapList = (from apm in context.Accommodation_ProductMapping

                                      join s in context.Suppliers on apm.Supplier_Id equals s.Supplier_Id

                                      join cm in context.m_CityMaster on apm.City_Id equals cm.City_Id into LJCityMaster
                                      from citymaster in LJCityMaster.DefaultIfEmpty()

                                      join con in context.m_CountryMaster on citymaster.Country_Id equals con.Country_Id into LJCountryMaster
                                      from countrymaster in LJCountryMaster.DefaultIfEmpty()

                                      join a in context.Accommodations on apm.Accommodation_Id equals a.Accommodation_Id into LJAcco
                                      from acco in LJAcco.DefaultIfEmpty()

                                      select new DataContracts.Mapping.DC_ProductMapping
                                      {
                                          SupplierCode = s.Code.Trim().ToUpper(),
                                          SupplierProductCode = apm.SupplierProductReference.ToUpper(),
                                          SupplierCountryCode = apm.CountryCode.ToUpper(),
                                          SupplierCountryName = apm.CountryName.ToUpper(),
                                          SupplierCityCode = apm.CityCode.ToUpper(),
                                          SupplierCityName = apm.CityName.ToUpper(),
                                          SupplierProductName = apm.ProductName.ToUpper(),
                                          MappingStatus = apm.Status.ToUpper(),
                                          MapId = apm.MapId ?? 0,

                                          SystemProductCode = (acco == null ? string.Empty : acco.CompanyHotelID.ToString().ToUpper()),
                                          SystemProductName = (acco == null ? string.Empty : acco.HotelName.ToUpper()),
                                          SystemProductType = (acco == null ? string.Empty : acco.ProductCategorySubType.ToUpper()),

                                          SystemCountryCode = (countrymaster != null ? countrymaster.Code.ToUpper() : string.Empty),
                                          SystemCountryName = (countrymaster != null ? countrymaster.Name.ToUpper() : string.Empty),
                                          SystemCityCode = (citymaster != null ? citymaster.Code.ToUpper() : string.Empty),
                                          SystemCityName = (citymaster != null ? citymaster.Name.ToUpper() : string.Empty)

                                      }).ToList();
                    _database = null;
                }

                if (productMapList.Count() > 0)
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("ProductMapping");

                    var collection = _database.GetCollection<DataContracts.Mapping.DC_ProductMapping>("ProductMapping");
                    foreach (var productmap in productMapList)
                    {
                        collection.InsertOneAsync(productmap);
                    }

                    collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode));
                    collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemProductCode));
                    collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemCityCode));

                    collection = null;
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
                                          join s in context.Suppliers on apm.Supplier_Id equals s.Supplier_Id
                                          where apm.Status == "MAPPED"
                                          select new DataContracts.Mapping.DC_ProductMappingLite
                                          {
                                              SupplierCode = s.Code.Trim().ToUpper(),
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

        public void UpdateActivityCategoryTypes()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");

                    var ActivityList = (from a in context.Activity_Flavour
                                        join spm in context.Activity_SupplierProductMapping on a.Activity_Flavour_Id equals spm.Activity_ID
                                        where a.CityCode != null && (spm.SupplierCode == "gta") && (spm.IsActive ?? false) == true
                                        select new { a.CommonProductNameSubType_Id, a.Activity_Flavour_Id, spm.SupplierCityCode }).ToList();
                    int iTotalCount = ActivityList.Count();
                    int iCounter = 0;

                    foreach (var Activity in ActivityList)
                    {
                        try
                        {
                            var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity.CommonProductNameSubType_Id));

                            var SupplierCityDepartureCodes = context.tbl_SupplierCityDepartureCode.Where(w => w.CityCode == Activity.SupplierCityCode).Select(s => new DataContracts.Activity.SupplierCityDepartureCode
                            {
                                CityCode = s.CityCode,
                                CityName = s.City,
                                DepartureCode = s.DepartureCode,
                                DepartureName = s.DepartureName,
                                HotelCode = s.HotelCode,
                                HotelName = s.Hotel
                            }).ToList();

                            var UpdateData = Builders<DataContracts.Activity.ActivityDefinition>.Update.Set(x => x.SupplierCityDepartureCodes, SupplierCityDepartureCodes);

                            var updateResult = collection.FindOneAndUpdateAsync(filter, UpdateData).Status;

                            iCounter++;

                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void UpdateActivitySuitableFor()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");
                    var fromDate = DateTime.Now.Add(TimeSpan.FromDays(-3));
                    var ActivityList = (from a in context.Activity_Flavour where a.Edit_Date > fromDate select new { Activity_Flavour_Id = a.Activity_Flavour_Id, CommonProductNameSubType_Id = a.CommonProductNameSubType_Id }).ToList();
                    int iTotalCount = ActivityList.Count();
                    int iCounter = 0;
                    foreach (var Activity in ActivityList)
                    {
                        try
                        {
                            var ActivitySuitableFor = (context.Activity_ClassificationAttributes.Where(w => w.Activity_Flavour_Id == Activity.Activity_Flavour_Id && w.AttributeType == "Product" && w.AttributeSubType == "SuitableFor").Select(s => s.AttributeValue).ToArray());

                            var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity.CommonProductNameSubType_Id));
                            var UpdateData = Builders<DataContracts.Activity.ActivityDefinition>.Update.Set(x => x.SuitableFor, ActivitySuitableFor);
                            var updateResult = collection.FindOneAndUpdateAsync(filter, UpdateData).Status;

                            iCounter++;

                        }
                        catch (Exception e)
                        {
                            continue;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void LoadActivityDefinition(Guid Activity_Flavour_Id)
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();

                    //_database.DropCollection("ActivityDefinitions");

                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");

                    IQueryable<Activity_Flavour> ActivityList;

                    if (Activity_Flavour_Id == Guid.Empty)
                    {
                        ActivityList = (from a in context.Activity_Flavour
                                        join spm in context.Activity_SupplierProductMapping on a.Activity_Flavour_Id equals spm.Activity_ID
                                        where a.CityCode != null && (spm.IsActive ?? false) == true
                                        select a);
                    }
                    else
                    {
                        ActivityList = (from a in context.Activity_Flavour
                                        where a.Activity_Flavour_Id == Activity_Flavour_Id && a.CityCode != null
                                        select a);
                    }

                    foreach (var Activity in ActivityList)
                    {
                        try
                        {
                            if (Activity_Flavour_Id == Guid.Empty)
                            {
                                //check if record is already exists
                                //var SupplierProductCode = context.Activity_SupplierProductMapping.Where(w => w.Activity_ID == Activity.Activity_Flavour_Id).Select(s => s.SuplierProductCode).FirstOrDefault();
                                var searchResultCount = collection.Find(f => f.SystemActivityCode == Convert.ToInt32(Activity.CommonProductNameSubType_Id)).Count();
                                if (searchResultCount > 0)
                                {
                                    continue;
                                }
                            }

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
                                               join s in context.Suppliers on a.Supplier_ID equals s.Supplier_Id
                                               where a.Activity_ID == Activity.Activity_Flavour_Id
                                               select new
                                               {
                                                   SupplierName = s.Name,
                                                   SupplierCode = s.Code.ToLower(),
                                                   SuplierProductCode = a.SuplierProductCode.ToUpper(),
                                                   SupplierCountryCode = a.SupplierCountryCode,
                                                   SupplierCountryName = a.SupplierCountryName,
                                                   SupplierCityCode = a.SupplierCityCode,
                                                   SupplierCityName = a.SupplierCityName,
                                                   Currency = a.Currency
                                               }).FirstOrDefault();

                            var ActivityDeals = (from a in context.Activity_Deals
                                                 where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                                 select a).ToList();

                            var ActivityPrices = (from a in context.Activity_Prices
                                                  where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                                  select a).ToList();

                            //var ActivitySPMCA = (from a in context.Activity_SupplierProductMapping_CA
                            //                     where a.Activity_SupplierProductMapping_CA_Id == Activity.Activity_Flavour_Id
                            //                     select a).ToList();

                            var ActivityFO = (from a in context.Activity_FlavourOptions
                                              where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                              select a).ToList();

                            var ActivityDOW = (from a in context.Activity_DaysOfWeek
                                               where a.Activity_Flavor_ID == Activity.Activity_Flavour_Id
                                               select a).ToList();

                            var ActivityOD = (from a in context.Activity_DaysOfOperation
                                              where a.Activity_Flavor_ID == Activity.Activity_Flavour_Id
                                              select a).ToList();

                            var ActivityCT = (from a in context.Activity_CategoriesType
                                              where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id && a.SystemProductNameSubType_ID != null
                                              select a).ToList();

                            var ActivityDP = (from a in context.Activity_DeparturePoints
                                              where a.Activity_Flavor_ID == Activity.Activity_Flavour_Id
                                              select a).ToList();

                            //create new mongo object record
                            var newActivity = new DataContracts.Activity.ActivityDefinition();

                            //newActivity.Activity_Flavour_Id = Activity.Activity_Flavour_Id.ToString();

                            newActivity.SystemActivityCode = Convert.ToInt32(Activity.CommonProductNameSubType_Id);

                            newActivity.SupplierCompanyCode = ActivitySPM.SupplierCode;

                            if (ActivitySPM.SupplierCode == "gta")
                            {
                                newActivity.SupplierCityDepartureCodes = context.tbl_SupplierCityDepartureCode.Where(w => w.CityCode == ActivitySPM.SupplierCityCode).Select(s => new DataContracts.Activity.SupplierCityDepartureCode
                                {
                                    CityCode = s.CityCode,
                                    CityName = s.City,
                                    DepartureCode = s.DepartureCode,
                                    DepartureName = s.DepartureName,
                                    HotelCode = s.HotelCode,
                                    HotelName = s.Hotel
                                }).ToList();
                            }

                            newActivity.SupplierProductCode = ActivitySPM.SuplierProductCode;//Activity.CompanyProductNameSubType_Id;

                            newActivity.Category = string.Join(",", ActivityCT.Select(s => s.SystemProductCategorySubType).Distinct());

                            newActivity.Type = string.Join(",", ActivityCT.Select(s => s.SystemProductType).Distinct());

                            newActivity.SubType = string.Join(",", ActivityCT.Select(s => s.SystemProductNameSubType).Distinct());

                            newActivity.ProductSubTypeId = ActivityCT.Select(s => s.SystemProductNameSubType_ID.ToString().ToUpper()).ToList();

                            newActivity.Name = Activity.ProductName;

                            newActivity.Description = (ActivityDesc.Where(w => w.DescriptionType == "Short").Select(s => s.Description).FirstOrDefault());

                            newActivity.DeparturePoint = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "DeparturePoint").Select(s => s.AttributeValue).FirstOrDefault());

                            newActivity.ReturnDetails = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "ReturnDetails").Select(s => s.AttributeValue).FirstOrDefault());

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

                            newActivity.SuitableFor = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "SuitableFor").Select(s => s.AttributeValue).ToArray());

                            newActivity.Specials = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "Specials").Select(s => s.AttributeValue).ToList());

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

                            //newActivity.Duration = new DataContracts.Activity.ActivityDuration
                            //{
                            //    Hours = (ActivityClassAttr.Where(w => w.AttributeType == "Duration" && w.AttributeSubType == "Hours").Select(s => s.AttributeValue).FirstOrDefault()),
                            //    Minutes = (ActivityClassAttr.Where(w => w.AttributeType == "Duration" && w.AttributeSubType == "Minutes").Select(s => s.AttributeValue).FirstOrDefault()),
                            //    Text = (ActivityClassAttr.Where(w => w.AttributeType == "Duration" && w.AttributeSubType == "Text").Select(s => s.AttributeValue).FirstOrDefault())
                            //};

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

                            if (ActivitySPM != null)
                            {
                                newActivity.SupplierDetails = new DataContracts.Activity.SupplierDetails
                                {
                                    SupplierName = ActivitySPM.SupplierName,
                                    SupplierID = ActivitySPM.SupplierCode,
                                    TourActivityID = ActivitySPM.SuplierProductCode,
                                    CountryCode = ActivitySPM.SupplierCountryCode,
                                    CountryName = ActivitySPM.SupplierCountryName,
                                    CityCode = ActivitySPM.SupplierCityCode,
                                    CityName = ActivitySPM.SupplierCityName,
                                    PricingCurrency = ActivitySPM.Currency
                                };
                            }


                            newActivity.Deals = ActivityDeals.Select(s => new DataContracts.Activity.Deals { Currency = s.Deal_Currency, DealId = s.DealCode, DealPrice = s.Deal_Price, DealText = s.DealText, OfferTermsAndConditions = s.Deal_TnC }).ToList();

                            newActivity.Prices = ActivityPrices.OrderBy(o => o.Price).Select(s => new DataContracts.Activity.Prices { OptionCode = s.Price_OptionCode, PriceFor = s.Price_For, Price = Convert.ToDouble(s.Price), PriceType = s.Price_Type, PriceBasis = s.PriceBasis, PriceId = s.PriceCode, SupplierCurrency = s.PriceCurrency }).ToList();

                            newActivity.ProductOptions = (from afo in ActivityFO
                                                          select new DataContracts.Activity.ProductOptions
                                                          {
                                                              SystemActivityOptionCode = afo.TLGXActivityOptionCode,
                                                              OptionCode = afo.Activity_OptionCode,
                                                              ActivityType = afo.Activity_Type,
                                                              DealText = afo.Activity_DealText,
                                                              Options = afo.Activity_OptionName,
                                                              Language = afo.Activity_Language,
                                                              LanguageCode = afo.Activity_LanguageCode
                                                          }).ToList();

                            newActivity.ClassificationAttrributes = ActivityClassAttr.Where(w => w.AttributeType == "Internal").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList();

                            newActivity.SystemMapping = new DataContracts.Activity.SystemMapping { SystemID = string.Empty, SystemName = string.Empty };//ActivitySPM.Select(s => new DataContracts.Activity.SystemMapping { SystemID = string.Empty, SystemName = string.Empty }).FirstOrDefault();

                            newActivity.DaysOfTheWeek = (from DOW in ActivityDOW
                                                         join OD in ActivityOD on DOW.Activity_DaysOfOperation_Id equals OD.Activity_DaysOfOperation_Id into ODlj
                                                         from ODljS in ODlj.DefaultIfEmpty()
                                                         join DP in ActivityDP on DOW.Activity_DaysOfWeek_ID equals DP.Activity_DaysOfWeek_ID into DPlj
                                                         from DPljS in DPlj.DefaultIfEmpty()
                                                         where (ODljS.IsOperatingDays ?? false) == true
                                                         select new DataContracts.Activity.DaysOfWeek
                                                         {
                                                             OperatingFromDate = ODljS == null ? string.Empty : ODljS.FromDate.ToString(),
                                                             OperatingToDate = ODljS == null ? string.Empty : ODljS.ToDate.ToString(),

                                                             SupplierDuration = DOW.SupplierDuration,
                                                             SupplierEndTime = DOW.SupplierEndTime,
                                                             SupplierFrequency = DOW.SupplierFrequency,
                                                             SupplierSession = DOW.SupplierSession,
                                                             SupplierStartTime = DOW.SupplierStartTime,

                                                             Session = DOW.Session,
                                                             StartTime = DOW.StartTime,
                                                             EndTime = DOW.EndTime,
                                                             Duration = DOW.Duration,

                                                             Sunday = DOW.Sun ?? false,
                                                             Monday = DOW.Mon ?? false,
                                                             Tuesday = DOW.Tues ?? false,
                                                             Wednesday = DOW.Wed ?? false,
                                                             Thursday = DOW.Thur ?? false,
                                                             Friday = DOW.Fri ?? false,
                                                             Saturday = DOW.Sat ?? false,

                                                             DepartureCode = DPljS == null ? string.Empty : DPljS.DepartureCode,
                                                             DeparturePoint = DPljS == null ? string.Empty : DPljS.DeparturePoint

                                                         }).ToList();

                            if (Activity_Flavour_Id == Guid.Empty)
                            {
                                collection.InsertOneAsync(newActivity);
                            }
                            else
                            {
                                var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity.CommonProductNameSubType_Id));
                                collection.ReplaceOneAsync(filter, newActivity, new UpdateOptions { IsUpsert = true });
                            }

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
                            //ActivitySPMCA = null;
                            ActivityFO = null;

                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
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

        public void LoadActivityMasters()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("ActivityMasters");

                    var collection = _database.GetCollection<DataContracts.Activity.ActivityMasters>("ActivityMasters");

                    List<DataContracts.Activity.ActivityMasters> dataList = new List<DataContracts.Activity.ActivityMasters>();

                    dataList = (from ma in context.m_masterattribute
                                join pma in context.m_masterattribute on ma.ParentAttribute_Id equals pma.MasterAttribute_Id into pmaj
                                from pmalj in pmaj.DefaultIfEmpty()
                                where ma.IsActive == true && ma.MasterFor == "Activity"
                                select new DataContracts.Activity.ActivityMasters
                                {
                                    Type = ma.Name.Trim(),
                                    ParentType = pmalj.Name.Trim(),
                                    Values = (from mav in context.m_masterattributevalue
                                              join pmav in context.m_masterattributevalue on mav.ParentAttributeValue_Id equals pmav.MasterAttributeValue_Id into pmavj
                                              from pmavlj in pmavj.DefaultIfEmpty()
                                              where mav.MasterAttribute_Id == ma.MasterAttribute_Id && mav.IsActive == true
                                              select new DataContracts.Activity.ActivityMasterValues
                                              {
                                                  Value = mav.AttributeValue.Trim(),
                                                  ParentValue = pmavlj.AttributeValue.Trim()
                                              }).ToList()
                                }).ToList();


                    collection.InsertManyAsync(dataList);

                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadStates()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("StateMaster");

                    var collection = _database.GetCollection<DataContracts.Masters.DC_State>("StateMaster");

                    List<DataContracts.Masters.DC_State> dataList = new List<DataContracts.Masters.DC_State>();

                    dataList = (from s in context.m_States
                                join c in context.m_CountryMaster on s.Country_Id equals c.Country_Id into cj
                                from clj in cj.DefaultIfEmpty()
                                select new DataContracts.Masters.DC_State
                                {
                                    CountryCode = (clj.Code ?? string.Empty).Trim().ToUpper(),
                                    CountryName = (clj.Name ?? string.Empty).Trim().ToUpper(),
                                    StateCode = (s.StateCode ?? string.Empty).Trim().ToUpper(),
                                    StateName = (s.StateName ?? string.Empty).Trim().ToUpper()
                                }).ToList();


                    collection.InsertManyAsync(dataList);

                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadPorts()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("PortMaster");

                    var collection = _database.GetCollection<DataContracts.Masters.DC_Port>("PortMaster");

                    List<DataContracts.Masters.DC_Port> dataList = new List<DataContracts.Masters.DC_Port>();

                    dataList = (from p in context.m_PortMaster
                                join c in context.m_CountryMaster on p.Country_Id equals c.Country_Id into cj
                                from clj in cj.DefaultIfEmpty()
                                join cty in context.m_CityMaster on p.City_Id equals cty.City_Id into ctyj
                                from ctylj in ctyj.DefaultIfEmpty()
                                select new DataContracts.Masters.DC_Port
                                {
                                    CountryCode = (clj.Code ?? p.oag_ctry).Trim().ToUpper(),
                                    CountryName = (clj.Name ?? p.oag_ctryname).Trim().ToUpper(),
                                    CityCode = (ctylj.Name ?? string.Empty).Trim().ToUpper(),
                                    CityName = (ctylj.Name ?? string.Empty).Trim().ToUpper(),
                                    Latitude = (p.oag_lat ?? string.Empty).Trim().ToUpper(),
                                    Longitude = (p.oag_lon ?? string.Empty).Trim().ToUpper(),
                                    PortCode = (p.OAG_loc ?? string.Empty).Trim().ToUpper(),
                                    PortName = (p.oag_portname ?? string.Empty).Trim().ToUpper()
                                }).ToList();


                    collection.InsertManyAsync(dataList);

                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadAccoStaticData()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    //Get Master Attributes and Supplier Attribute Mapping
                    string[] MasterFor = "HotelInfo,FacilityInfo,RoomInfo,RoomAmenities,Media".Split(',');
                    var MappingAttributes = (from MA in context.m_masterattribute
                                             join MAM in context.m_MasterAttributeMapping on MA.MasterAttribute_Id equals MAM.SystemMasterAttribute_Id
                                             join S in context.Suppliers on MAM.Supplier_Id equals S.Supplier_Id
                                             where MasterFor.Contains(MA.MasterFor)
                                             select new
                                             {
                                                 S.Supplier_Id,
                                                 SupplierCode = S.Code,
                                                 MA.MasterFor,
                                                 MasterAttribute = MA.Name.ToUpper(),
                                                 MAM.SupplierMasterAttribute,
                                                 MAM.MasterAttributeMapping_Id
                                             }).ToList();

                    _database = MongoDBHandler.mDatabase();
                    //_database.DropCollection("AccoStaticData");

                    var collection = _database.GetCollection<DataContracts.StaticData.Accomodation>("AccoStaticData");

                    List<SupplierEntity> SupplierProducts;

                    SupplierProducts = (from a in context.SupplierEntities
                                        where a.Entity == "HotelInfo"
                                        select a).ToList();

                    var SupplierProductValues = (from a in context.SupplierEntities
                                                 join b in context.SupplierEntityValues on a.SupplierEntity_Id equals b.SupplierEntity_Id
                                                 where a.Entity == "HotelInfo"
                                                 select b).ToList();

                    foreach (var product in SupplierProducts)
                    {
                        try
                        {
                            //check if the product already exists
                            var searchResultCount = collection.Find(f => f.AccomodationInfo.CompanyId == product.SupplierName.ToUpper() && f.AccomodationInfo.CompanyProductId == product.SupplierProductCode.ToUpper()).Count();
                            if (searchResultCount > 0)
                            {
                                continue;
                            }

                            var newProduct = new DataContracts.StaticData.Accomodation();

                            var HotelInfoDetails = (from a in SupplierProductValues
                                                    where a.SupplierEntity_Id == product.SupplierEntity_Id
                                                    select new
                                                    {
                                                        a.SupplierProperty,
                                                        Value = (a.SystemValue ?? a.SupplierValue),
                                                        a.AttributeMap_Id,
                                                        SystemAttribute = string.Empty
                                                    }).ToList();

                            //Get System Attribute values joined with Entity Value data
                            HotelInfoDetails = (from a in HotelInfoDetails
                                                join b in MappingAttributes on a.AttributeMap_Id equals b.MasterAttributeMapping_Id
                                                where b.Supplier_Id == product.Supplier_Id
                                                select new
                                                {
                                                    a.SupplierProperty,
                                                    a.Value,
                                                    a.AttributeMap_Id,
                                                    SystemAttribute = b.MasterAttribute
                                                }).ToList();

                            //Supplier Level Details
                            //newProduct.SupplierDetails = new DataContracts.StaticData.SupplierDetails
                            //{
                            //    SupplierCode = product.SupplierName,
                            //    SupplierProductCode = product.SupplierProductCode
                            //};

                            //Get All Child Entity Elements and their values
                            var ChildEntities = (from a in context.SupplierEntities
                                                 where a.Parent_Id == product.SupplierEntity_Id
                                                 select a).ToList();

                            var ChildEntityValues = (from a in context.SupplierEntities
                                                     join b in context.SupplierEntityValues on a.SupplierEntity_Id equals b.SupplierEntity_Id
                                                     where a.Parent_Id == product.SupplierEntity_Id
                                                     select b).ToList();


                            //Accommodation Info
                            newProduct.AccomodationInfo = new DataContracts.StaticData.AccomodationInfo
                            {
                                Address = new DataContracts.StaticData.Address
                                {
                                    HouseNumber = string.Empty,
                                    Street = HotelInfoDetails.Where(w => w.SystemAttribute == "STREET").Select(s => s.Value).FirstOrDefault(),
                                    Street2 = HotelInfoDetails.Where(w => w.SystemAttribute == "STREET2").Select(s => s.Value).FirstOrDefault(),
                                    Street3 = HotelInfoDetails.Where(w => w.SystemAttribute == "STREET3").Select(s => s.Value).FirstOrDefault(),
                                    Street4 = HotelInfoDetails.Where(w => w.SystemAttribute == "STREET4").Select(s => s.Value).FirstOrDefault(),
                                    Street5 = HotelInfoDetails.Where(w => w.SystemAttribute == "STREET5").Select(s => s.Value).FirstOrDefault(),
                                    Area = HotelInfoDetails.Where(w => w.SystemAttribute == "AREA").Select(s => s.Value).FirstOrDefault(),
                                    City = HotelInfoDetails.Where(w => w.SystemAttribute == "CITY").Select(s => s.Value).FirstOrDefault(),
                                    Country = HotelInfoDetails.Where(w => w.SystemAttribute == "COUNTRY").Select(s => s.Value).FirstOrDefault(),
                                    Geometry = new DataContracts.StaticData.Geometry
                                    {
                                        Type = "LatLng",
                                        Coordinates = HotelInfoDetails.Where(w => w.SystemAttribute == "LATITUDE" || w.SystemAttribute == "LONGITUDE").OrderBy(o => o.SystemAttribute).Select(s => Convert.ToDecimal(s.Value)).ToList()
                                    },
                                    Location = HotelInfoDetails.Where(w => w.SystemAttribute == "LOCATION").Select(s => s.Value).FirstOrDefault(),
                                    PostalCode = HotelInfoDetails.Where(w => w.SystemAttribute == "POSTALCODE").Select(s => s.Value).FirstOrDefault(),
                                    State = HotelInfoDetails.Where(w => w.SystemAttribute == "STATE").Select(s => s.Value).FirstOrDefault(),
                                    Zone = HotelInfoDetails.Where(w => w.SystemAttribute == "ZONE").Select(s => s.Value).FirstOrDefault()
                                },
                                Affiliations = string.Empty,
                                Brand = HotelInfoDetails.Where(w => w.SystemAttribute == "BRAND").Select(s => s.Value).FirstOrDefault(),
                                Chain = HotelInfoDetails.Where(w => w.SystemAttribute == "CHAIN").Select(s => s.Value).FirstOrDefault(),
                                CheckInTime = HotelInfoDetails.Where(w => w.SystemAttribute == "CHECKINTIME").Select(s => s.Value).FirstOrDefault(),
                                CheckOutTime = HotelInfoDetails.Where(w => w.SystemAttribute == "CHECKOUTTIME").Select(s => s.Value).FirstOrDefault(),
                                CommonProductId = string.Empty,
                                CompanyId = product.SupplierName.ToUpper(),
                                CompanyName = product.SupplierName.ToUpper(),
                                CompanyProductId = product.SupplierProductCode.ToUpper(),
                                CompanyRating = HotelInfoDetails.Where(w => w.SystemAttribute == "RATING").Select(s => s.Value).FirstOrDefault(),
                                ContactDetails = new List<DataContracts.StaticData.ContactDetails>
                                {
                                    new DataContracts.StaticData.ContactDetails
                                    {
                                        EmailAddress = HotelInfoDetails.Where(w => w.SystemAttribute == "EMAILADDRESS").Select(s => s.Value).FirstOrDefault(),
                                        Fax = new DataContracts.StaticData.TelephoneFormat
                                        {
                                            CityCode  = string.Empty,
                                            CountryCode = string.Empty,
                                            Number = HotelInfoDetails.Where(w => w.SystemAttribute == "FAX").Select(s => s.Value).FirstOrDefault()
                                        },
                                        Phone = new DataContracts.StaticData.TelephoneFormat
                                        {
                                            CityCode  = string.Empty,
                                            CountryCode = string.Empty,
                                            Number = HotelInfoDetails.Where(w => w.SystemAttribute == "TELEPHONE").Select(s => s.Value).FirstOrDefault()
                                        },
                                        Website = HotelInfoDetails.Where(w => w.SystemAttribute == "WEBSITE").Select(s => s.Value).FirstOrDefault()
                                    }
                                },
                                DisplayName = HotelInfoDetails.Where(w => w.SystemAttribute == "NAME").Select(s => s.Value).FirstOrDefault(),
                                FamilyDetails = null,
                                FinanceControlId = null,
                                General = new DataContracts.StaticData.General
                                {
                                    Extras = new List<DataContracts.StaticData.Extras> { new DataContracts.StaticData.Extras {  Label = "Short", Description = HotelInfoDetails.Where(w => w.SystemAttribute == "SHORTDESCRIPTION").Select(s => s.Value).FirstOrDefault() } ,
                                     new DataContracts.StaticData.Extras {  Label = "Long", Description = HotelInfoDetails.Where(w => w.SystemAttribute == "LONGDESCRIPTION").Select(s => s.Value).FirstOrDefault() } }
                                },
                                IsMysteryProduct = false,
                                IsTwentyFourHourCheckout = false,
                                Name = HotelInfoDetails.Where(w => w.SystemAttribute == "NAME").Select(s => s.Value).FirstOrDefault(),
                                NoOfFloors = HotelInfoDetails.Where(w => w.SystemAttribute == "NOOFFLOORS").Select(s => Convert.ToInt32(s.Value)).FirstOrDefault(),
                                NoOfRooms = HotelInfoDetails.Where(w => w.SystemAttribute == "NOOFROOMS").Select(s => Convert.ToInt32(s.Value)).FirstOrDefault(),
                                ProductCatSubType = HotelInfoDetails.Where(w => w.SystemAttribute == "PRODUCTCATEGORYSUBTYPE").Select(s => s.Value).FirstOrDefault(),
                                Rating = HotelInfoDetails.Where(w => w.SystemAttribute == "RATING").Select(s => s.Value).FirstOrDefault(),
                                RatingDatedOn = null,
                                RecommendedFor = null,
                                ResortType = HotelInfoDetails.Where(w => w.SystemAttribute == "PRODUCTCATEGORYSUBTYPE").Select(s => s.Value).FirstOrDefault()
                            };

                            newProduct.Overview = new DataContracts.StaticData.Overview
                            {
                                IsCompanyRecommended = false,
                                Duration = null,
                                HashTag = null,
                                Highlights = null,
                                Interest = null,
                                SellingTips = null,
                                Usp = null
                            };

                            //Get & Set Facilities
                            newProduct.Facility = new List<DataContracts.StaticData.Facility>();

                            //var FacilityInfoList = (from a in context.SupplierEntities
                            //                        where a.Parent_Id == product.SupplierEntity_Id && a.Entity == "FacilityInfo"
                            //                        select a).ToList();

                            var FacilityInfoList = (from a in ChildEntities
                                                    where a.Entity == "FacilityInfo"
                                                    select a).ToList();

                            foreach (var Facility in FacilityInfoList)
                            {
                                var FacilityInfoDetails = (from a in ChildEntityValues
                                                           where a.SupplierEntity_Id == Facility.SupplierEntity_Id
                                                           select new
                                                           {
                                                               a.SupplierProperty,
                                                               Value = (a.SystemValue ?? a.SupplierValue),
                                                               a.AttributeMap_Id,
                                                               SystemAttribute = string.Empty
                                                           }).ToList();

                                //Get System Attribute values joined with Entity Value data
                                FacilityInfoDetails = (from a in FacilityInfoDetails
                                                       join b in MappingAttributes on a.AttributeMap_Id equals b.MasterAttributeMapping_Id
                                                       where b.Supplier_Id == product.Supplier_Id
                                                       select new
                                                       {
                                                           a.SupplierProperty,
                                                           a.Value,
                                                           a.AttributeMap_Id,
                                                           SystemAttribute = b.MasterAttribute
                                                       }).ToList();


                                newProduct.Facility.Add(new DataContracts.StaticData.Facility
                                {
                                    Type = FacilityInfoDetails.Where(w => w.SystemAttribute == "FACILITYTYPE").Select(s => s.Value).FirstOrDefault()
                                });
                            }

                            //Get & Set Media
                            newProduct.Media = new List<DataContracts.StaticData.Media>();
                            //var MediaList = (from a in context.SupplierEntities
                            //                 where a.Parent_Id == product.SupplierEntity_Id && a.Entity == "Media"
                            //                 select a).ToList();

                            var MediaList = (from a in ChildEntities
                                             where a.Entity == "Media"
                                             select a).ToList();

                            foreach (var Media in MediaList)
                            {
                                var MediaDetails = (from a in ChildEntityValues
                                                    where a.SupplierEntity_Id == Media.SupplierEntity_Id
                                                    select new
                                                    {
                                                        a.SupplierProperty,
                                                        Value = (a.SystemValue ?? a.SupplierValue),
                                                        a.AttributeMap_Id,
                                                        SystemAttribute = string.Empty
                                                    }).ToList();

                                //Get System Attribute values joined with Entity Value data
                                MediaDetails = (from a in MediaDetails
                                                join b in MappingAttributes on a.AttributeMap_Id equals b.MasterAttributeMapping_Id
                                                where b.Supplier_Id == product.Supplier_Id
                                                select new
                                                {
                                                    a.SupplierProperty,
                                                    a.Value,
                                                    a.AttributeMap_Id,
                                                    SystemAttribute = b.MasterAttribute
                                                }).ToList();


                                newProduct.Media.Add(new DataContracts.StaticData.Media
                                {
                                    MediaId = MediaDetails.Where(w => w.SystemAttribute == "MEDIAID").Select(s => s.Value).FirstOrDefault(),
                                    Description = MediaDetails.Where(w => w.SystemAttribute == "DESCRIPTION").Select(s => s.Value).FirstOrDefault(),
                                    FileType = "IMAGE",
                                    FileName = MediaDetails.Where(w => w.SystemAttribute == "LARGEIMAGEURL").Select(s => s.Value).FirstOrDefault() ?? MediaDetails.Where(w => w.SystemAttribute == "SMALLIMAGEURL").Select(s => s.Value).FirstOrDefault()
                                });
                            }

                            collection.InsertOneAsync(newProduct);

                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
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
