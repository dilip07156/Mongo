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

                    var Accommodation_SupplierRoomTypeMapping = (from a in context.Accommodation_SupplierRoomTypeMapping select a).AsQueryable();

                    Accommodation_SupplierRoomTypeMapping = Accommodation_SupplierRoomTypeMapping.Where(w => w.MappingStatus == "MAPPED").Select(s => s);

                    var Accommodation = (from a in context.Accommodations select a).AsQueryable();
                    var Accommodation_RoomInfo = (from a in context.Accommodation_RoomInfo select a).AsQueryable();
                    var Accommodation_SupplierRoomTypeAttributes = (from a in context.Accommodation_SupplierRoomTypeAttributes select a).AsQueryable();
                    //var Keyword = (from a in context.m_keyword select a).AsQueryable();

                    var roomTypeMapList = (from asrtm in Accommodation_SupplierRoomTypeMapping
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
                                               //AlternateRoomNames = new DataContracts.Mapping.DC_RoomTypeMapping_AlternateRoomNames { Normalized = asrtm.TX_RoomName, Stripped = asrtm.Tx_StrippedName },
                                               Attibutes = (Accommodation_SupplierRoomTypeAttributes.Where(w => w.RoomTypeMap_Id == asrtm.Accommodation_SupplierRoomTypeMapping_Id).Select(s => new DataContracts.Mapping.DC_RoomTypeMapping_Attributes { Type = s.SystemAttributeKeyword, Value = s.SupplierRoomTypeAttribute }).ToList())
                                           }).ToList();

                    collection.InsertMany(roomTypeMapList);
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
    }
}
