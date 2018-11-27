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
using System.ComponentModel;

namespace DAL
{
    public enum PushStatus
    {
        INSERT,
        SCHEDULED,
        RUNNNING,
        COMPLETED,
        ERROR
    }

    public class DL_LoadData : IDisposable
    {
        public void Dispose()
        {
        }

        protected static IMongoDatabase _database;

        #region Masters

        public void LoadCountryMaster(Guid LogId)
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

                    var Log = context.DistributionLayerRefresh_Log.Find(LogId);
                    if (Log != null)
                    {
                        Log.Status = "Completed";
                        context.SaveChanges();
                    }

                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }

        }

        public void LoadCityMaster(Guid LogId)
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
                                   where city.Status != "DELETED"
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

                    var Log = context.DistributionLayerRefresh_Log.Find(LogId);
                    if (Log != null)
                    {
                        Log.Status = "Completed";
                        context.SaveChanges();
                    }
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadSupplierMaster(Guid LogId)
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

                    var Log = context.DistributionLayerRefresh_Log.Find(LogId);
                    if (Log != null)
                    {
                        Log.Status = "Completed";
                        context.SaveChanges();
                    }
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

        public void LoadStates(Guid LogId)
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

                    var Log = context.DistributionLayerRefresh_Log.Find(LogId);
                    if (Log != null)
                    {
                        Log.Status = "Completed";
                        context.SaveChanges();
                    }
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadPorts(Guid LogId)
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {


                    List<DataContracts.Masters.DC_Port> dataList = new List<DataContracts.Masters.DC_Port>();

                    dataList = (from p in context.m_PortMaster
                                join c in context.m_CountryMaster on p.Country_Id equals c.Country_Id into cj
                                from clj in cj.DefaultIfEmpty()
                                join cty in context.m_CityMaster on p.City_Id equals cty.City_Id into ctyj
                                from ctylj in ctyj.DefaultIfEmpty()
                                join st in context.m_States on ctylj.State_Id equals st.State_Id into stj
                                from stlj in stj.DefaultIfEmpty()
                                select new DataContracts.Masters.DC_Port
                                {
                                    oag_country_code = p.oag_ctry ?? string.Empty,
                                    oag_country_name = p.oag_ctryname ?? string.Empty,
                                    oag_country_subcode = p.oag_subctry ?? string.Empty,
                                    oag_inactive_indicator = p.oag_inactive ?? string.Empty,
                                    oag_latitiude = p.oag_lat ?? string.Empty,
                                    oag_location_code = p.OAG_loc ?? string.Empty,
                                    oag_location_name = p.oag_name ?? string.Empty,
                                    oag_location_subtype = (
                                                                p.OAG_subtype == "A" ? "Airport" :
                                                                p.OAG_subtype == "B" ? "Bus Station" :
                                                                p.OAG_subtype == "H" ? "Harbour" :
                                                                p.OAG_subtype == "O" ? "Off-line Point" :
                                                                p.OAG_subtype == "R" ? "Rail Station" :
                                                                p.OAG_subtype == "U" ? "Metro/Underground" :
                                                                p.OAG_subtype == "V" ? "Miscellaneous" : "Multi Airport City"
                                                            ),
                                    oag_location_subtype_code = p.OAG_subtype ?? string.Empty,
                                    oag_location_type = (
                                                            p.OAG_type == "L" ? "Location with one port" :
                                                            p.OAG_type == "A" ? "Airport belonging to multi airport city" :
                                                            p.OAG_type == "M" ? "Multi airport city" : string.Empty
                                                        ),
                                    oag_location_type_code = p.OAG_type ?? string.Empty,
                                    oag_longitude = p.oag_lon ?? string.Empty,
                                    oag_multi_airport_citycode = p.OAG_multicity ?? string.Empty,
                                    oag_port_name = p.oag_portname ?? string.Empty,
                                    oag_state_code = p.oag_state ?? string.Empty,
                                    oag_state_subcode = p.oag_substate ?? string.Empty,
                                    oag_time_division = p.oag_timediv ?? string.Empty,

                                    tlgx_country_code = (clj.Code ?? string.Empty),
                                    tlgx_country_name = (clj.Name ?? string.Empty),
                                    tlgx_city_code = (ctylj.Code ?? string.Empty),
                                    tlgx_city_name = (ctylj.Name ?? string.Empty),
                                    tlgx_state_code = (stlj.StateCode ?? string.Empty),
                                    tlgx_state_name = (stlj.StateName ?? string.Empty)

                                }).ToList();

                    if (dataList.Count > 0)
                    {
                        _database = MongoDBHandler.mDatabase();
                        _database.DropCollection("PortMaster");
                        var collection = _database.GetCollection<DataContracts.Masters.DC_Port>("PortMaster");
                        collection.InsertManyAsync(dataList);

                        collection = null;
                        _database = null;
                    }

                    var Log = context.DistributionLayerRefresh_Log.Find(LogId);
                    if (Log != null)
                    {
                        Log.Status = "Completed";
                        context.SaveChanges();
                    }

                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadMasterAccommodation(Guid LogId, Guid Accommodation_Id)
        {
            if ((Accommodation_Id == Guid.Empty && LogId == Guid.Empty) || (Accommodation_Id != Guid.Empty && LogId != Guid.Empty))
            {
                return;
            }

            int BatchSize = 1000;
            int TotalCount = 0;
            int Counter = 0;
            int NoOfBatch = 0;

            if (Accommodation_Id != Guid.Empty && LogId == Guid.Empty)
            {
                BatchSize = 1;
                TotalCount = 1;
                NoOfBatch = 1;
            }

            try
            {
                List<DataContracts.Masters.DC_Accomodation> _AccoList = new List<DataContracts.Masters.DC_Accomodation>();

                if (Accommodation_Id == Guid.Empty && LogId != Guid.Empty)
                {
                    UpdateDistLogInfo(LogId, PushStatus.INSERT, 0, 0, Guid.Empty.ToString(), "ACCOMMODATION", "MASTER");
                    UpdateDistLogInfo(LogId, PushStatus.RUNNNING, 0, Counter, Guid.Empty.ToString(), "ACCOMMODATION", "MASTER");

                    using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                    {
                        context.Database.CommandTimeout = 0;

                        try
                        {
                            TotalCount = context.Accommodations.AsNoTracking().Where(w => w.IsActive == true).Count();
                        }
                        catch (Exception ex) { }
                    }

                    NoOfBatch = TotalCount / BatchSize;
                    int mod = TotalCount % BatchSize;
                    if (mod > 0)
                    {
                        NoOfBatch = NoOfBatch + 1;
                    }

                    UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalCount, Counter, Guid.Empty.ToString(), "ACCOMMODATION", "MASTER");
                }

                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Masters.DC_Accomodation>("AccommodationMaster");

                for (int BatchNo = 0; BatchNo < NoOfBatch; BatchNo++)
                {
                    _AccoList = GetAccommodationMaster_InBatches(BatchSize, BatchNo, Accommodation_Id);
                    if (_AccoList.Count > 0)
                    {
                        foreach (var acco in _AccoList)
                        {
                            var filter = Builders<DataContracts.Masters.DC_Accomodation>.Filter.Eq(c => c.CommonHotelId, acco.CommonHotelId);
                            collection.ReplaceOne(filter, acco, new UpdateOptions { IsUpsert = true });
                        }

                        if (Accommodation_Id == Guid.Empty && LogId != Guid.Empty)
                        {
                            Counter = (BatchNo * BatchSize) + _AccoList.Count;
                            UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalCount, Counter, Guid.Empty.ToString(), "ACCOMMODATION", "MASTER");
                        }
                    }
                }

                //Local Function To Get Master Accommodation Data
                List<DataContracts.Masters.DC_Accomodation> GetAccommodationMaster_InBatches(int batchSize, int batchNo, Guid gAccommodation_Id)
                {
                    List<DataContracts.Masters.DC_Accomodation> _AccoListResultMain = new List<DataContracts.Masters.DC_Accomodation>();
                    try
                    {
                        int skip = batchNo * batchSize;

                        #region AccoMasterQuery
                        StringBuilder sbSelectAccoMaster = new StringBuilder();

                        sbSelectAccoMaster.Append(@"  
                                                select ACC.CompanyHotelID as CommonHotelId,ACC.HotelName ,MCM.Code CountryCode,MCM.Name CountryName,CM.Code CityCode,MST.StateCode, MST.StateName,
                                                Cm.Name CityName,ACC.StreetName ,ACC.StreetNumber,ACC.Street3 ,ACC.Street4 ,ACC.Street5 ,ACC.PostalCode ,ACC.Town,
                                                ACC.Location ,ACC.Area,ACC.TLGXAccoId ,ACC.ProductCategory ,ACC.ProductCategorySubType ,isnull(ACC.IsRoomMappingCompleted,0)  as IsRoomMappingCompleted ,
                                                ACC.HotelRating,ACC.CompanyRating,ACC.CompanyRecommended,ACC.RecommendedFor,ACC.Brand,ACC.Chain,ACC.Latitude,ACC.Longitude,ACC.FullAddress, ACC.HotelRating as HotelStarRating,
                                                ACC.Brand,ACC.Chain,Cont.Email,Cont.Fax,Cont.WebSiteURL,Cont.Telephone from Accommodation ACC with(nolock) Left Join m_CityMaster CM with(nolock)  on Cm.City_Id = ACC.City_Id and CM.Country_Id = Acc.Country_Id
                                                Left join m_CountryMaster MCM with(nolock) on MCM.Country_Id = ACC.Country_Id
                                                LEft Join m_States MST with(nolock) on MST.State_Id = CM.State_Id
                                                outer apply 
                                                (SELECT top 1 * from Accommodation_Contact ACT with(nolock)  where ACT.Accommodation_Id = ACC.Accommodation_Id)
                                                Cont ");

                        if (gAccommodation_Id == Guid.Empty)
                        {
                            sbSelectAccoMaster.AppendLine(" ORDER BY ACC.CompanyHotelID  OFFSET " + (skip).ToString() + " ROWS FETCH NEXT " + batchSize.ToString() + " ROWS ONLY;");
                        }
                        else
                        {
                            sbSelectAccoMaster.AppendLine(" WHERE ACC.Accommodation_Id = '" + gAccommodation_Id + "';");
                        }

                        #endregion

                        using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                        {
                            context.Database.CommandTimeout = 0;
                            _AccoListResultMain = context.Database.SqlQuery<DataContracts.Masters.DC_Accomodation>(sbSelectAccoMaster.ToString()).ToList();
                        }
                    }
                    catch (Exception ex) { }
                    return _AccoListResultMain;
                }

                if (Accommodation_Id == Guid.Empty && LogId != Guid.Empty)
                {
                    UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalCount, Counter, Guid.Empty.ToString(), "ACCOMMODATION", "MASTER");
                }

                collection = null;
                _database = null;
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalCount, Counter, Guid.Empty.ToString(), "ACCOMMODATION", "MASTER");
                throw ex;
            }
        }

        #endregion

        #region ZoneMaster
        public void LoadZoneMaster(Guid LogId)
        {
            int TotalZoneCount = 0;
            int MongoInsertedCount = 0;
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    context.Database.CommandTimeout = 0;
                    if (LogId != Guid.Empty)
                    {
                        UpdateDistLogInfo(LogId, PushStatus.RUNNNING);
                    }
                    List<DataContracts.Masters.DC_Zone_Master> _ZoneList = new List<DataContracts.Masters.DC_Zone_Master>();
                    // int total = 0;
                    int BatchSize = 1000;
                    string strTotalCount = @"SELECT COUNT(1) FROM m_ZoneMaster with(nolock)";
                    context.Configuration.AutoDetectChangesEnabled = false;
                    try
                    {
                        TotalZoneCount = context.Database.SqlQuery<int>(strTotalCount.ToString()).FirstOrDefault();
                    }
                    catch (Exception ex) { }
                    int NoOfBatch = TotalZoneCount / BatchSize;
                    int mod = TotalZoneCount % BatchSize;
                    if (mod > 0)
                    {
                        NoOfBatch = NoOfBatch + 1;
                    }
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("ZoneMaster");
                    var collection = _database.GetCollection<DataContracts.Masters.DC_Zone_Master>("ZoneMaster");

                    for (int BatchNo = 0; BatchNo < NoOfBatch; BatchNo++)
                    {
                        _ZoneList = GetZoneMasterdataToLoad(BatchSize, BatchNo);
                        if (_ZoneList.Count > 0)
                        {
                            collection.InsertManyAsync(_ZoneList);
                            #region To update CounterIn DistributionLog
                            if (LogId != Guid.Empty)
                            {
                                MongoInsertedCount = MongoInsertedCount + _ZoneList.Count();
                                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalZoneCount, MongoInsertedCount);
                            }
                            #endregion
                        }
                    }

                    collection.Indexes.CreateOne(Builders<DataContracts.Masters.DC_Zone_Master>.IndexKeys.Ascending(_ => _.TLGXCountryCode).Ascending(_ => _.Zone_SubType).Ascending(_ => _.Zone_Type));
                    collection = null;
                    _database = null;
                    if (LogId != Guid.Empty)
                    {
                        UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalZoneCount, MongoInsertedCount);
                    }
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalZoneCount, MongoInsertedCount);
                throw ex;
            }
        }

        private List<DataContracts.Masters.DC_Zone_Master> GetZoneMasterdataToLoad(int batchSize, int batchNo)
        {
            List<DataContracts.Masters.DC_Zone_Master> _ZoneListResultMain = new List<DataContracts.Masters.DC_Zone_Master>();
            List<DataContracts.Masters.DC_Zone_MasterRQ> _ZoneListResult = new List<DataContracts.Masters.DC_Zone_MasterRQ>();
            try
            {
                #region ==== ZoneMasterQuery
                StringBuilder sbSelectZoneMaster = new StringBuilder();
                StringBuilder sbOrderbyZoneMaster = new StringBuilder();
                sbSelectZoneMaster.Append(@" SELECT ('ZONE'+cast(ROW_NUMBER() OVER (ORDER BY Zone_Name) as varchar)) as Id,
                                            Zone_id, upper(ltrim(rtrim(zm.Zone_Name))) as Zone_Name , upper(ltrim(rtrim(zm.Zone_Type))) as Zone_Type , 
                                            upper(ltrim(rtrim(zm.Zone_SubType))) as Zone_SubType ,zm.Zone_Radius , 
                                            zm.Latitude, zm.Longitude,upper(ltrim(rtrim(co.Code))) as TLGXCountryCode
                                            FROM  m_zoneMaster zm  with(Nolock)
                                            LEFT JOIN m_CountryMaster co  with(Nolock) ON co.Country_Id= zm.Country_Id 
                                            WHERE zm.isActive=1 ");
                int skip = batchNo * batchSize;
                sbOrderbyZoneMaster.Append("  ORDER BY zm.Zone_id  OFFSET " + (skip).ToString() + " ROWS FETCH NEXT " + batchSize.ToString() + " ROWS ONLY ");

                StringBuilder sbfinalZoneMaster = new StringBuilder();
                sbfinalZoneMaster.Append(sbSelectZoneMaster + " ");
                sbfinalZoneMaster.Append(" " + sbOrderbyZoneMaster + " ");
                #endregion
                #region ==== ZoneProductMapping SQL QUERY
                StringBuilder sbSelectZoneProduct = new StringBuilder();
                StringBuilder sbOrderByZoneProduct = new StringBuilder();
                sbSelectZoneProduct.Append(@" SELECT zp.Zone_id, ac.CompanyHotelID as TLGXCompanyHotelID ,zp.Distance,
                                                  upper(ltrim(rtrim(ac.HotelName))) as TLGXHotelName,zp.Included as IsIncluded,
                                                  upper(ltrim(rtrim(zp.ProductType))) as TLGXProductType,upper(ltrim(rtrim(zp.Unit))) as Unit
                                                  FROM  ZoneProduct_Mapping zp with(NOLOCK) 
                                                  JOIN Accommodation ac with(NOLOCK)  on zp.Product_Id = ac.Accommodation_Id ");
                sbOrderByZoneProduct.Append(" ORDER BY Distance ");

                StringBuilder sbfinalZoneProduct = new StringBuilder();
                sbfinalZoneProduct.Append(sbSelectZoneProduct + " ");
                sbfinalZoneProduct.Append(" WHERE zp.zone_id  in ( ");
                #endregion
                #region ==== ZoneCityMapping SQL QUERY
                StringBuilder sbSelectZoneCity = new StringBuilder();
                sbSelectZoneCity.Append(@" SELECT zc.Zone_id, upper(ltrim(rtrim(cm.code))) as TLGXCityCode
                                               FROM  ZoneCity_Mapping zc with(NOLOCK) 
                                               JOIN m_citymaster cm with(NOLOCK)  on zc.city_id = cm.city_id ");

                StringBuilder sbfinalZoneCity = new StringBuilder();
                sbfinalZoneCity.Append(sbSelectZoneCity + " ");
                sbfinalZoneCity.Append(" WHERE zc.zone_id  in ( ");

                #endregion

                List<DataContracts.Masters.DC_Zone_ProductMappingRQ> _ZoneProdListResult = new List<DataContracts.Masters.DC_Zone_ProductMappingRQ>();

                List<DataContracts.Masters.DC_Zone_CityMappingRQ> _ZoneCityListResult = new List<DataContracts.Masters.DC_Zone_CityMappingRQ>();

                StringBuilder sbZone_id = new StringBuilder();

                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    try
                    {
                        _ZoneListResult = context.Database.SqlQuery<DataContracts.Masters.DC_Zone_MasterRQ>(sbfinalZoneMaster.ToString()).ToList();

                        //Add  Zone_id for ZpList and Zc List
                        foreach (var id in _ZoneListResult)
                        {
                            sbZone_id.Append("'" + id.Zone_id + "',");
                        }
                        //To Get Zone product by Zone id
                        sbfinalZoneProduct.Append(sbZone_id.ToString().TrimEnd(',') + ")");
                        sbfinalZoneProduct.Append(sbOrderByZoneProduct);
                        _ZoneProdListResult = context.Database.SqlQuery<DataContracts.Masters.DC_Zone_ProductMappingRQ>(sbfinalZoneProduct.ToString()).ToList();
                        // End == To Get Zone product by Zone id


                        //To Get Zone city by Zone id
                        sbfinalZoneCity.Append(sbZone_id.ToString().TrimEnd(',') + ")");
                        _ZoneCityListResult = context.Database.SqlQuery<DataContracts.Masters.DC_Zone_CityMappingRQ>(sbfinalZoneCity.ToString()).ToList();

                        foreach (var item in _ZoneListResult)
                        {
                            item.Zone_ProductMapping = new List<DataContracts.Masters.DC_Zone_ProductMappingRQ>();
                            item.Zone_ProductMapping = _ZoneProdListResult.Where(w => w.Zone_id == item.Zone_id).ToList();
                            item.Zone_CityMapping = new List<DataContracts.Masters.DC_Zone_CityMappingRQ>();
                            item.Zone_CityMapping = _ZoneCityListResult.Where(w => w.Zone_id == item.Zone_id).ToList();
                        }
                        _ZoneListResultMain = ConvertListWithoutId(_ZoneListResult);
                    }
                    catch (Exception ex) { }
                }

                return _ZoneListResultMain;
            }
            catch (Exception ex) { }
            return _ZoneListResultMain;
        }

        // Remove Zone_Id from above List 
        private List<DataContracts.Masters.DC_Zone_Master> ConvertListWithoutId(List<DataContracts.Masters.DC_Zone_MasterRQ> RQ)
        {
            List<DataContracts.Masters.DC_Zone_Master> _ResultList = new List<DataContracts.Masters.DC_Zone_Master>();
            try
            {
                _ResultList = RQ.ConvertAll(item => new DataContracts.Masters.DC_Zone_Master
                {
                    Id = item.Id,
                    Zone_Name = item.Zone_Name,
                    Zone_Type = item.Zone_Type,
                    Zone_SubType = item.Zone_SubType,
                    Latitude = item.Latitude,
                    Longitude = item.Longitude,
                    Zone_Radius = item.Zone_Radius,
                    TLGXCountryCode = item.TLGXCountryCode,
                    Zone_CityMapping = item.Zone_CityMapping.ConvertAll(xcity => new DataContracts.Masters.DC_Zone_CityMapping
                    {
                        TLGXCityCode = xcity.TLGXCityCode
                    }),
                    Zone_ProductMapping = item.Zone_ProductMapping.ConvertAll(xProd => new DataContracts.Masters.DC_Zone_ProductMapping
                    {
                        TLGXCompanyHotelID = xProd.TLGXCompanyHotelID,
                        TLGXHotelName = xProd.TLGXHotelName,
                        TLGXProductType = xProd.TLGXProductType,
                        Distance = xProd.Distance,
                        Unit = xProd.Unit,
                        IsIncluded = xProd.IsIncluded
                    })

                });
            }
            catch (Exception ex) { throw; }
            return _ResultList;
        }


        #endregion

        #region ZoneTypeMaster
        public void LoadZoneTypeMaster(Guid LogId)
        {
            int TotalZoneTypeCount = 0;
            int MongoInsertedCount = 0;
            try
            {
                if (LogId != Guid.Empty)
                {
                    UpdateDistLogInfo(LogId, PushStatus.RUNNNING);
                }
                _database = MongoDBHandler.mDatabase();
                _database.DropCollection("ZoneTypeMaster");
                var collection = _database.GetCollection<DataContracts.Masters.DC_ZoneTypeMaster>("ZoneTypeMaster");

                List<DataContracts.Masters.DC_ZoneTypeMaster> dataList = new List<DataContracts.Masters.DC_ZoneTypeMaster>();
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    context.Database.CommandTimeout = 0;
                    context.Configuration.AutoDetectChangesEnabled = false;
                    dataList = (from ma in context.m_masterattribute
                                join mav in context.m_masterattributevalue on ma.MasterAttribute_Id equals mav.MasterAttribute_Id
                                where ma.MasterFor.ToUpper() == "ZONE" && ma.Name.ToUpper() == "ZONETYPE"
                                select new DataContracts.Masters.DC_ZoneTypeMaster
                                {
                                    Zone_Type = mav.AttributeValue.Trim().ToUpper(),
                                    Zone_SubType = (from a in context.m_masterattributevalue
                                                    where a.ParentAttributeValue_Id == mav.MasterAttributeValue_Id
                                                    select a.AttributeValue.Trim().ToUpper()).ToList()

                                }).ToList();
                    if (dataList.Count > 0)
                    {
                        collection.InsertManyAsync(dataList);
                        #region To update CounterIn DistributionLog
                        if (LogId != Guid.Empty)
                        {
                            TotalZoneTypeCount = dataList.Count();
                            MongoInsertedCount = MongoInsertedCount + dataList.Count();
                            UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalZoneTypeCount, MongoInsertedCount);
                        }
                        #endregion
                    }

                    collection = null;
                    _database = null;
                }

            }
            catch (Exception ex)
            {
                UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalZoneTypeCount, MongoInsertedCount);
                throw ex;
            }
        }
        #endregion

        #region Country And City Mapping

        public void LoadCountryMapping(Guid LogId)
        {
            try
            {
                bool Is_IX_SupplierCode_SupplierCountryCode_Exists = false;
                bool Is_IX_SupplierCode_CountryCode_Exists = false;
                bool Is_IX_MapId_Exists = false;

                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_CountryMapping>("CountryMapping");


                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    var MappedData = (from cm in context.m_CountryMapping.AsNoTracking()
                                      join c in context.m_CountryMaster.AsNoTracking() on cm.Country_Id equals c.Country_Id
                                      join s in context.Suppliers.AsNoTracking() on cm.Supplier_Id equals s.Supplier_Id
                                      where cm.Status == "MAPPED"
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

                    if (MappedData != null && MappedData.Count > 0)
                    {
                        foreach (var city in MappedData)
                        {
                            var filter = Builders<DataContracts.Mapping.DC_CountryMapping>.Filter.Eq(c => c.MapId, city.MapId);
                            collection.ReplaceOne(filter, city, new UpdateOptions { IsUpsert = true });
                        }
                    }

                    var NotMappedData = (from cm in context.m_CountryMapping.AsNoTracking()
                                         where cm.Status != "MAPPED"
                                         select new DataContracts.Mapping.DC_CountryMapping
                                         {
                                             MapId = cm.MapID ?? 0
                                         }).ToList();

                    if (NotMappedData != null && NotMappedData.Count > 0)
                    {
                        foreach (var country in NotMappedData)
                        {
                            var filter = Builders<DataContracts.Mapping.DC_CountryMapping>.Filter.Eq(c => c.MapId, country.MapId);
                            collection.DeleteOne(filter);
                        }
                    }

                    var Log = context.DistributionLayerRefresh_Log.Find(LogId);
                    if (Log != null)
                    {
                        Log.Status = "Completed";
                        context.SaveChanges();
                    }

                }

                #region Index Management
                var listOfindexes = collection.Indexes.List().ToList();
                foreach (var index in listOfindexes)
                {
                    Newtonsoft.Json.Linq.JObject rss = Newtonsoft.Json.Linq.JObject.Parse(index.ToJson());
                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SupplierCountryCode"] != null)
                    {
                        Is_IX_SupplierCode_SupplierCountryCode_Exists = true;
                    }

                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["CountryCode"] != null)
                    {
                        Is_IX_SupplierCode_CountryCode_Exists = true;
                    }

                    if ((string)rss["key"]["MapId"] != null)
                    {
                        Is_IX_MapId_Exists = true;
                    }
                }

                if (!Is_IX_SupplierCode_SupplierCountryCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CountryMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CountryMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierCountryCode);
                    CreateIndexModel<DataContracts.Mapping.DC_CountryMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_CountryMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                if (!Is_IX_SupplierCode_CountryCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CountryMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CountryMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.CountryCode);
                    CreateIndexModel<DataContracts.Mapping.DC_CountryMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_CountryMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                if (!Is_IX_MapId_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CountryMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CountryMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.MapId);
                    CreateIndexModel<DataContracts.Mapping.DC_CountryMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_CountryMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                #endregion

                collection = null;
                _database = null;


            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadCityMapping(Guid LogId)
        {
            try
            {
                bool Is_IX_SupplierCode_SupplierCityCode_Exists = false;
                bool Is_IX_SupplierCode_CityCode_Exists = false;
                bool Is_IX_MapId_Exists = false;

                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_CityMapping>("CityMapping");

                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {

                    context.Configuration.AutoDetectChangesEnabled = false;

                    var CityListMapped = (from cm in context.m_CityMapping.AsNoTracking()
                                          join city in context.m_CityMaster.AsNoTracking() on cm.City_Id equals city.City_Id
                                          join country in context.m_CountryMaster.AsNoTracking() on cm.Country_Id equals country.Country_Id
                                          join supplier in context.Suppliers.AsNoTracking() on cm.Supplier_Id equals supplier.Supplier_Id
                                          where cm.Status == "MAPPED"
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

                    if (CityListMapped != null && CityListMapped.Count > 0)
                    {
                        foreach (var city in CityListMapped)
                        {
                            var filter = Builders<DataContracts.Mapping.DC_CityMapping>.Filter.Eq(c => c.MapId, city.MapId);
                            collection.ReplaceOne(filter, city, new UpdateOptions { IsUpsert = true });
                        }
                        //collection.InsertMany(CityListMapped);
                    }


                    var CityListNotMapped = (from cm in context.m_CityMapping.AsNoTracking()
                                             where cm.Status != "MAPPED"
                                             select new DataContracts.Mapping.DC_CityMapping
                                             {
                                                 MapId = cm.MapID ?? 0
                                             }).ToList();

                    if (CityListNotMapped != null && CityListNotMapped.Count > 0)
                    {
                        foreach (var city in CityListNotMapped)
                        {
                            var filter = Builders<DataContracts.Mapping.DC_CityMapping>.Filter.Eq(c => c.MapId, city.MapId);
                            collection.DeleteOne(filter);
                        }
                    }

                    var Log = context.DistributionLayerRefresh_Log.Find(LogId);
                    if (Log != null)
                    {
                        Log.Status = "Completed";
                        Log.Create_Date = DateTime.Now;
                        context.SaveChanges();
                    }
                }

                #region Index Management
                var listOfindexes = collection.Indexes.List().ToList();
                foreach (var index in listOfindexes)
                {
                    Newtonsoft.Json.Linq.JObject rss = Newtonsoft.Json.Linq.JObject.Parse(index.ToJson());
                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SupplierCityCode"] != null)
                    {
                        Is_IX_SupplierCode_SupplierCityCode_Exists = true;
                    }

                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["CityCode"] != null)
                    {
                        Is_IX_SupplierCode_CityCode_Exists = true;
                    }

                    if ((string)rss["key"]["MapId"] != null)
                    {
                        Is_IX_MapId_Exists = true;
                    }
                }

                if (!Is_IX_SupplierCode_SupplierCityCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CityMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CityMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierCityCode);
                    CreateIndexModel<DataContracts.Mapping.DC_CityMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_CityMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                if (!Is_IX_SupplierCode_CityCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CityMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CityMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.CityCode);
                    CreateIndexModel<DataContracts.Mapping.DC_CityMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_CityMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                if (!Is_IX_MapId_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CityMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CityMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.MapId);
                    CreateIndexModel<DataContracts.Mapping.DC_CityMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_CityMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                #endregion

                collection = null;
                _database = null;

            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Product Mapping Push

        public void LoadProductMapping(Guid LogId, Guid ProdMapId)
        {
            int TotalAPMCount = 0;
            int MongoInsertedCount = 0;
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_ProductMapping>("ProductMapping");

                if (ProdMapId == Guid.Empty)
                {
                    UpdateDistLogInfo(LogId, PushStatus.RUNNNING);

                    //collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode));
                    //collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemProductCode));
                    //collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemCityCode));

                    StringBuilder setNewStatus = new StringBuilder();
                    using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                    {
                        context.Configuration.AutoDetectChangesEnabled = false;
                        context.Database.CommandTimeout = 0;
                        //ALL APM Count
                        TotalAPMCount = context.Accommodation_ProductMapping.AsNoTracking().Count();
                        List<string> SupplierCodes = context.Suppliers.Where(w => (w.StatusCode ?? string.Empty) == "ACTIVE").Select(s => s.Code.ToUpper()).Distinct().ToList();
                        //List<string> SupplierCodes = context.Suppliers.Where(w => (w.StatusCode ?? string.Empty) == "ACTIVE" && w.Code == "GTA").Select(s => s.Code.ToUpper()).Distinct().ToList();
                        foreach (var SupplierCode in SupplierCodes)
                        {
                            var productMapList = (from apm in context.Accommodation_ProductMapping.AsNoTracking()

                                                  join s in context.Suppliers.AsNoTracking() on apm.Supplier_Id equals s.Supplier_Id

                                                  join cm in context.m_CityMaster.AsNoTracking() on apm.City_Id equals cm.City_Id into LJCityMaster
                                                  from citymaster in LJCityMaster.DefaultIfEmpty()

                                                  join con in context.m_CountryMaster.AsNoTracking() on citymaster.Country_Id equals con.Country_Id into LJCountryMaster
                                                  from countrymaster in LJCountryMaster.DefaultIfEmpty()

                                                  join a in context.Accommodations.AsNoTracking() on apm.Accommodation_Id equals a.Accommodation_Id into LJAcco
                                                  from acco in LJAcco.DefaultIfEmpty()

                                                  where s.Code == SupplierCode

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
                                                      TlgxMdmHotelId = (acco == null ? string.Empty : acco.TLGXAccoId.ToUpper()),

                                                      SystemCountryCode = (countrymaster != null ? countrymaster.Code.ToUpper() : string.Empty),
                                                      SystemCountryName = (countrymaster != null ? countrymaster.Name.ToUpper() : string.Empty),
                                                      SystemCityCode = (citymaster != null ? citymaster.Code.ToUpper() : string.Empty),
                                                      SystemCityName = (citymaster != null ? citymaster.Name.ToUpper() : string.Empty)

                                                  }).ToList();

                            if (productMapList.Count() > 0)
                            {
                                var res = collection.DeleteMany(x => x.SupplierCode == SupplierCode);

                                foreach (var prodMap in productMapList)
                                {
                                    collection.InsertOne(prodMap);
                                }

                                #region To update CounterIn DistributionLog
                                MongoInsertedCount = MongoInsertedCount + productMapList.Count();
                                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount);
                                #endregion
                            }
                        }

                        collection = null;
                        _database = null;

                        UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalAPMCount, MongoInsertedCount);
                    }
                }
                else
                {
                    #region If Map ID is not 0 delete and insert single Record
                    using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                    {
                        StringBuilder sbSelect = new StringBuilder();
                        sbSelect.Append(@"select  UPPER(s.Code) as SupplierCode,
                                        UPPER(apm.SupplierProductReference) as SupplierProductCode,
                                        UPPER(apm.CountryCode) as SupplierCountryCode,
                                        UPPER(apm.CountryName) asSupplierCountryName ,
                                        UPPER(apm.CityCode) as SupplierCityCode,
                                        UPPER(apm.CityName) as SupplierCityName,
                                        UPPER(apm.ProductName) as SupplierProductName,
                                        UPPER(apm.Status) as MappingStatus,
                                        apm.MapId,
                                        UPPER(a.CompanyHotelID) as SystemProductCode,
                                        UPPER(a.HotelName) as SystemProductName,
                                        UPPER(a.ProductCategorySubType) as SystemProductType,
                                        UPPER(con.Code) as SystemCountryCode,
                                        UPPER(con.Name) as SystemCountryName,
                                        UPPER(cm.Code) as SystemCityCode,
                                        UPPER(cm.Name) as SystemCityName,
                                        UPPER(ISNULL(a.TLGXAccoId,'')) as TlgxMdmHotelId
                                        from Accommodation_productMapping  apm  with(nolock)
                                        join Supplier s  with(nolock) on apm.supplier_id= s.supplier_id 
                                        left Join m_CityMaster cm with(nolock) on apm.City_Id = cm.City_Id
                                        left join m_CountryMaster con with(nolock) on apm.country_id = con.country_id
                                        left join Accommodation a with(nolock) on apm.Accommodation_Id = a.Accommodation_Id
                                        where  apm.Accommodation_ProductMapping_Id= '" + ProdMapId + "'");
                        var prod = context.Database.SqlQuery<DataContracts.Mapping.DC_ProductMapping>(sbSelect.ToString()).FirstOrDefault();
                        if (prod != null)
                        {
                            var res = collection.DeleteMany(x => x.MapId == prod.MapId);
                            collection.InsertOneAsync(prod);
                        }
                    }
                    #endregion
                }

            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount);
                throw ex;
            }
        }

        public void LoadProductMappingLite(Guid LogId, Guid ProdMapId)
        {
            int TotalAPMCount = 0;
            int MongoInsertedCount = 0;
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_ProductMappingLite>("ProductMappingLite");
                if (ProdMapId == Guid.Empty)
                {
                    UpdateDistLogInfo(LogId, PushStatus.RUNNNING);
                    //collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMappingLite>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode));
                    //collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMappingLite>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemProductCode));
                    using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                    {
                        context.Configuration.AutoDetectChangesEnabled = false;
                        context.Database.CommandTimeout = 0;
                        //TotalCount
                        TotalAPMCount = context.Accommodation_ProductMapping.AsNoTracking().Where(w => w.Status.Trim().ToUpper() == "MAPPED" || w.Status.Trim().ToUpper() == "AUTOMAPPED").Count();

                        List<string> SupplierCodes = context.Suppliers.Where(w => (w.StatusCode ?? string.Empty) == "ACTIVE").Select(s => s.Code.ToUpper()).Distinct().ToList();
                        foreach (var SupplierCode in SupplierCodes)
                        {
                            var productMapList = (from apm in context.Accommodation_ProductMapping.AsNoTracking()
                                                  join a in context.Accommodations.AsNoTracking() on apm.Accommodation_Id equals a.Accommodation_Id
                                                  join s in context.Suppliers.AsNoTracking() on apm.Supplier_Id equals s.Supplier_Id
                                                  where (apm.Status.Trim().ToUpper() == "MAPPED" || apm.Status.Trim().ToUpper() == "AUTOMAPPED") && s.Code == SupplierCode
                                                  select new DataContracts.Mapping.DC_ProductMappingLite
                                                  {
                                                      SupplierCode = s.Code.Trim().ToUpper(),
                                                      SupplierProductCode = apm.SupplierProductReference.ToUpper(),
                                                      MapId = apm.MapId ?? 0,
                                                      SystemProductCode = a.CompanyHotelID.ToString().ToUpper(),
                                                      TlgxMdmHotelId = (a.TLGXAccoId == null ? string.Empty : a.TLGXAccoId.ToUpper())
                                                  }).ToList();

                            if (productMapList.Count() > 0)
                            {
                                var res = collection.DeleteMany(x => x.SupplierCode == SupplierCode);
                                collection.InsertMany(productMapList);

                                #region To update CounterIn DistributionLog
                                MongoInsertedCount = MongoInsertedCount + productMapList.Count();
                                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount);
                                #endregion
                            }
                        }
                        collection = null;
                        _database = null;

                        UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalAPMCount, MongoInsertedCount);

                    }
                }
                else
                {
                    #region If Map ID is not 0 delete and insert single Record
                    using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                    {
                        StringBuilder sbSelect = new StringBuilder();
                        sbSelect.Append(@"select  UPPER(s.Code) as SupplierCode,
                                        UPPER(apm.SupplierProductReference) as SupplierProductCode,
                                        apm.MapId,
                                        UPPER(a.CompanyHotelID) as SystemProductCode,
                                        UPPER(ISNULL(a.TLGXAccoId,'')) as TlgxMdmHotelId,
                                        UPPER(ISNULL(apm.Status,'UNMAPPED')) as MappingStatus
                                        from Accommodation_productMapping  apm  with(nolock)
                                        join Supplier s  with(nolock) on apm.supplier_id= s.supplier_id 
                                        left join Accommodation a with(nolock) on apm.Accommodation_Id = a.Accommodation_Id 
                                        where  apm.Accommodation_ProductMapping_Id ='" + ProdMapId + "';");
                        var prod = context.Database.SqlQuery<DataContracts.Mapping.DC_ProductMappingLite_WithStatus>(sbSelect.ToString()).FirstOrDefault();
                        if (prod != null)
                        {
                            var res = collection.DeleteMany(x => x.MapId == prod.MapId);

                            if (prod.MappingStatus == "AUTOMAPPED" || prod.MappingStatus == "MAPPED")
                            {
                                collection.InsertOneAsync(new DataContracts.Mapping.DC_ProductMappingLite
                                {
                                    MapId = prod.MapId,
                                    SupplierCode = prod.SupplierCode,
                                    SupplierProductCode = prod.SupplierProductCode,
                                    SystemProductCode = prod.SystemProductCode,
                                    TlgxMdmHotelId = prod.TlgxMdmHotelId
                                });
                            }
                        }
                    }
                    #endregion
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount);
                throw ex;
            }
        }

        public void LoadHotelMapping(Guid LogId)
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    var Log = context.DistributionLayerRefresh_Log.Find(LogId);
                    if (Log != null)
                    {
                        Log.Status = "Completed";
                        context.SaveChanges();
                    }
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }

        }

        #endregion

        #region Activity Mapping

        public void LoadActivityMapping(Guid LogId)
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;

                    var collection = _database.GetCollection<DataContracts.Mapping.DC_ProductMapping>("ActivityMapping");
                    var productMapList = (from apm in context.Activity_SupplierProductMapping.AsNoTracking()
                                          join a in context.Activities.AsNoTracking() on apm.Activity_ID equals a.Activity_Id
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

                    if (productMapList.Count > 0)
                    {
                        _database = MongoDBHandler.mDatabase();
                        _database.DropCollection("ActivityMapping");
                        collection.InsertMany(productMapList);
                        collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode));
                        collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_ProductMapping>.IndexKeys.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemProductCode));
                        collection = null;
                        _database = null;
                    }

                    var Log = context.DistributionLayerRefresh_Log.Find(LogId);
                    if (Log != null)
                    {
                        Log.Status = "Completed";
                        context.SaveChanges();
                    }
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

        public void UpdateActivityMedia()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");

                    context.Configuration.AutoDetectChangesEnabled = false;

                    var ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                        join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                        join am in context.Activity_Media.AsNoTracking() on a.Activity_Flavour_Id equals am.Activity_Flavour_Id
                                        where a.CityCode != null
                                        //&& (spm.SupplierCode == "viator" || spm.SupplierCode == "Hotelbeds")
                                        && (spm.IsActive ?? false) == true
                                        select new
                                        {
                                            a.CommonProductNameSubType_Id,
                                            am.Media_Caption,
                                            am.Description,
                                            am.Media_URL,
                                            am.Media_Height,
                                            am.Category,
                                            am.SubCategory,
                                            am.Media_Position,
                                            am.Media_Width
                                        }).ToList();

                    int iTotalCount = ActivityList.Count();
                    int iCounter = 0;

                    var distinctActivity = ActivityList.Select(s => s.CommonProductNameSubType_Id).Distinct().ToList();

                    foreach (var Activity in distinctActivity)
                    {
                        try
                        {
                            var ActivityMedia = (ActivityList.Where(w => w.CommonProductNameSubType_Id == Activity).Select(s => new DataContracts.Activity.Media
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

                            var searchResultCount = collection.Find(f => f.SystemActivityCode == Convert.ToInt32(Activity)).Count();
                            if (searchResultCount > 0)
                            {
                                int i = 1;
                            }

                            var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity));

                            var UpdateData = Builders<DataContracts.Activity.ActivityDefinition>.Update.Set(x => x.ActivityMedia, ActivityMedia);

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

                            var SupplierCityDepartureCodes = context.Activity_SupplierCityDepartureCode.Where(w => w.CityCode == Activity.SupplierCityCode).Select(s => new DataContracts.Activity.SupplierCityDepartureCode
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

        public void UpdateActivityInterestType()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    context.Database.CommandTimeout = 0;
                    context.Configuration.AutoDetectChangesEnabled = false;
                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");
                    //var ActivityList = (from a in context.Activity_Flavour select new { Activity_Flavour_Id = a.Activity_Flavour_Id, CommonProductNameSubType_Id = a.CommonProductNameSubType_Id }).ToList();
                    var ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                        join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                        where a.CityCode != null && (spm.IsActive ?? false) == true
                                        && spm.SupplierName == "viator"
                                        select new { Activity_Flavour_Id = a.Activity_Flavour_Id, CommonProductNameSubType_Id = a.CommonProductNameSubType_Id }).ToList();
                    int iTotalCount = ActivityList.Count();
                    int iCounter = 0;
                    foreach (var Activity in ActivityList)
                    {
                        try
                        {
                            var InterestTypeArray = (context.Activity_CategoriesType.Where(w => w.Activity_Flavour_Id == Activity.Activity_Flavour_Id)).Select(s => s.SystemInterestType).Distinct().ToArray();
                            var InterestType = string.Join(",", InterestTypeArray);
                            var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity.CommonProductNameSubType_Id));
                            var UpdateData = Builders<DataContracts.Activity.ActivityDefinition>.Update.Set(x => x.InterestType, InterestType);
                            var updateResult = collection.FindOneAndUpdate(filter, UpdateData);

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

        /// <summary>
        /// To update DaysOfWeek for activity by Supplier
        /// </summary>
        public void UpdateActivityDOW()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");
                    //var ActivityList = (from a in context.Activity_Flavour select new { Activity_Flavour_Id = a.Activity_Flavour_Id, CommonProductNameSubType_Id = a.CommonProductNameSubType_Id }).ToList();
                    var ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                        join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                        where a.CityCode != null && (spm.IsActive ?? false) == true
                                        && spm.SupplierName == "ckis"
                                        select new { Activity_Flavour_Id = a.Activity_Flavour_Id, CommonProductNameSubType_Id = a.CommonProductNameSubType_Id }).ToList();
                    int iTotalCount = ActivityList.Count();
                    int iCounter = 0;
                    foreach (var Activity in ActivityList)
                    {
                        try
                        {

                            var ActivityDOW = (from a in context.Activity_DaysOfWeek.AsNoTracking()
                                               where a.Activity_Flavor_ID == Activity.Activity_Flavour_Id
                                               select a).ToList();

                            var ActivityOD = (from a in context.Activity_DaysOfOperation.AsNoTracking()
                                              where a.Activity_Flavor_ID == Activity.Activity_Flavour_Id
                                              && (a.IsOperatingDays ?? true) == true
                                              select a).ToList();

                            var ActivityDP = (from a in context.Activity_DeparturePoints.AsNoTracking()
                                              where a.Activity_Flavor_ID == Activity.Activity_Flavour_Id
                                              select a).ToList();

                            var DaysOfTheWeek = (from DOW in ActivityDOW
                                                 join OD in ActivityOD on DOW.Activity_DaysOfOperation_Id equals OD.Activity_DaysOfOperation_Id into ODlj
                                                 from ODljS in ODlj.DefaultIfEmpty()
                                                 join DP in ActivityDP on DOW.Activity_DaysOfWeek_ID equals DP.Activity_DaysOfWeek_ID into DPlj
                                                 from DPljS in DPlj.DefaultIfEmpty()
                                                 select new DataContracts.Activity.DaysOfWeek
                                                 {
                                                     OperatingFromDate = ODljS == null ? string.Empty : ODljS.FromDate.ToString(),
                                                     OperatingToDate = ODljS == null ? string.Empty : ODljS.ToDate.ToString(),

                                                     SupplierDuration = DOW.SupplierDuration ?? string.Empty,
                                                     SupplierEndTime = DOW.SupplierEndTime ?? string.Empty,
                                                     SupplierFrequency = DOW.SupplierFrequency ?? string.Empty,
                                                     SupplierSession = DOW.SupplierSession ?? string.Empty,
                                                     SupplierStartTime = DOW.SupplierStartTime ?? string.Empty,

                                                     Session = DOW.Session ?? string.Empty,
                                                     StartTime = DOW.StartTime ?? string.Empty,
                                                     EndTime = DOW.EndTime ?? string.Empty,
                                                     Duration = DOW.Duration ?? string.Empty,
                                                     DurationType = DOW.DurationType ?? string.Empty,


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

                            var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity.CommonProductNameSubType_Id));
                            var UpdateData = Builders<DataContracts.Activity.ActivityDefinition>.Update.Set(x => x.DaysOfTheWeek, DaysOfTheWeek);
                            var updateResult = collection.FindOneAndUpdate(filter, UpdateData);

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

        public void UpdateActivityPrices()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");
                    var ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                        join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                        where a.CityCode != null && (spm.IsActive ?? false) == true
                                        && spm.SupplierName == "viator"
                                        select new { Activity_Flavour_Id = a.Activity_Flavour_Id, CommonProductNameSubType_Id = a.CommonProductNameSubType_Id }).ToList();
                    int iTotalCount = ActivityList.Count();
                    int iCounter = 0;
                    foreach (var Activity in ActivityList)
                    {
                        try
                        {

                            var ActivityPrices = (from a in context.Activity_Prices.AsNoTracking()
                                                  where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                                  select a).ToList();

                            var Prices = ActivityPrices.OrderBy(o => o.Price).Select(s => new DataContracts.Activity.Prices
                            {
                                OptionCode = s.Price_OptionCode,
                                PriceFor = s.Price_For,
                                Price = Convert.ToDouble(s.Price),
                                PriceType = s.Price_Type,
                                PriceBasis = s.PriceBasis,
                                PriceId = s.PriceCode,
                                SupplierCurrency = s.PriceCurrency,
                                FromPax = s.FromPax,
                                ToPax = s.ToPax,
                                Market = s.Market,
                                PersonType = s.PersonType,
                                ValidFrom = s.Price_ValidFrom == null ? string.Empty : s.Price_ValidFrom.ToString(),
                                ValidTo = s.Price_ValidTo == null ? string.Empty : s.Price_ValidTo.ToString()
                            }).ToList();

                            var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity.CommonProductNameSubType_Id));
                            var UpdateData = Builders<DataContracts.Activity.ActivityDefinition>.Update.Set(x => x.Prices, Prices);
                            var updateResult = collection.FindOneAndUpdate(filter, UpdateData);

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

        public void UpdateActivityDescription()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");
                    var ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                        join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                        where a.CityCode != null && (spm.IsActive ?? false) == true
                                        && spm.SupplierName == "xoxoday"
                                        select new { Activity_Flavour_Id = a.Activity_Flavour_Id, CommonProductNameSubType_Id = a.CommonProductNameSubType_Id }).ToList();
                    int iTotalCount = ActivityList.Count();
                    int iCounter = 0;
                    foreach (var Activity in ActivityList)
                    {
                        try
                        {

                            var ActivityDesc = (from a in context.Activity_Descriptions.AsNoTracking()
                                                where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                                select a).ToList();

                            var Description = (ActivityDesc.Where(w => w.DescriptionType == "Short").Select(s => s.Description).FirstOrDefault());

                            var Overview = (ActivityDesc.Where(w => w.DescriptionType == "Long").Select(s => s.Description).FirstOrDefault());

                            var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity.CommonProductNameSubType_Id));


                            var UpdateData = Builders<DataContracts.Activity.ActivityDefinition>.Update.Set(x => x.Description, Description);
                            var updateResult = collection.FindOneAndUpdate(filter, UpdateData);

                            var UpdateDataOverview = Builders<DataContracts.Activity.ActivityDefinition>.Update.Set(x => x.Overview, Overview);
                            var updateResultOverview = collection.FindOneAndUpdate(filter, UpdateDataOverview);

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

        public void UpdateActivitySpecial()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");
                    var ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                        join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                        where a.CityCode != null && (spm.IsActive ?? false) == true
                                        && spm.SupplierName == "viator"
                                        select new { Activity_Flavour_Id = a.Activity_Flavour_Id, CommonProductNameSubType_Id = a.CommonProductNameSubType_Id }).ToList();
                    int iTotalCount = ActivityList.Count();
                    int iCounter = 0;
                    foreach (var Activity in ActivityList)
                    {
                        try
                        {
                            //var Specials = (from a in context.Activity_ClassificationAttributes.AsNoTracking()
                            //                where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id && a.AttributeType == "Product"
                            //                && a.AttributeSubType == "Specials"
                            //                select a.AttributeValue).ToList();

                            var Highlights = (from a in context.Activity_ClassificationAttributes.AsNoTracking()
                                              where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id && a.AttributeType == "Product"
                                              && a.AttributeSubType == "Highlights"
                                              select a.AttributeValue).ToArray();

                            var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity.CommonProductNameSubType_Id));

                            //var UpdateData = Builders<DataContracts.Activity.ActivityDefinition>.Update.Set(x => x.Specials, Specials);
                            var UpdateData = Builders<DataContracts.Activity.ActivityDefinition>.Update.Set(x => x.Highlights, Highlights);

                            var updateResult = collection.FindOneAndUpdate(filter, UpdateData);

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
                    context.Configuration.AutoDetectChangesEnabled = false;

                    _database = MongoDBHandler.mDatabase();

                    //_database.DropCollection("ActivityDefinitions");
                    var fromDate = DateTime.Now.Add(TimeSpan.FromDays(-1));
                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");

                    List<Activity_Flavour> ActivityList;

                    if (Activity_Flavour_Id == Guid.Empty)
                    {
                        ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                        join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                        where a.CityCode != null && (spm.IsActive ?? false) == true
                                        && spm.SupplierName == "bemyguest"
                                        select a).ToList();
                    }
                    else
                    {
                        ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                        where a.Activity_Flavour_Id == Activity_Flavour_Id && a.CityCode != null
                                        select a).ToList();
                    }
                    if (ActivityList.Count > 0)
                    {
                        LoadActivityData(ActivityList);
                    }

                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        #region Activity Push Method
        /// <summary>
        /// push data to mongo
        /// </summary>
        /// <param name="ActivityList"></param>
        public bool LoadActivityData(List<Activity_Flavour> ActivityList, string log_id = "")
        {
            using (TLGX_DEVEntities context = new TLGX_DEVEntities())
            {
                context.Configuration.AutoDetectChangesEnabled = false;

                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");
                int iCounter = 0;
                int totalcount = ActivityList.Count();
                foreach (var Activity in ActivityList)
                {
                    try
                    {
                        //Update Status
                        if (!string.IsNullOrWhiteSpace(log_id))
                        {
                            if (iCounter % 100 == 0)
                                UpdateDistLogInfo(Guid.Parse(log_id), PushStatus.RUNNNING, totalcount, iCounter, string.Empty, string.Empty, string.Empty);
                        }
                        iCounter++;
                        var ActivityClassAttr = (from a in context.Activity_ClassificationAttributes.AsNoTracking()
                                                 where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                                 select a).ToList();

                        var ActivityDesc = (from a in context.Activity_Descriptions.AsNoTracking()
                                            where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                            select a).ToList();

                        var ActivityInc = (from a in context.Activity_Inclusions.AsNoTracking()
                                           where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                           select a).ToList();

                        var ActivityIncDetails = (from a in context.Activity_InclusionDetails.AsNoTracking()
                                                  where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                                  select a).ToList();

                        var ActivityPolicy = (from a in context.Activity_Policy.AsNoTracking()
                                              where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                              select a).ToList();

                        var ActivityMedia = (from a in context.Activity_Media.AsNoTracking()
                                             where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                             select a).ToList();

                        var ActivityReviews = (from a in context.Activity_ReviewsAndScores.AsNoTracking()
                                               where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                               select a).ToList();

                        var ActivitySPM = (from a in context.Activity_SupplierProductMapping.AsNoTracking()
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
                                               Currency = a.Currency,
                                               TourType = a.SupplierTourType,
                                               AreaAddress = a.Location
                                           }).FirstOrDefault();

                        var ActivityDeals = (from a in context.Activity_Deals.AsNoTracking()
                                             where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                             select a).ToList();

                        var ActivityPrices = (from a in context.Activity_Prices.AsNoTracking()
                                              where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                              select a).ToList();

                        //var ActivitySPMCA = (from a in context.Activity_SupplierProductMapping_CA
                        //                     where a.Activity_SupplierProductMapping_CA_Id == Activity.Activity_Flavour_Id
                        //                     select a).ToList();

                        var ActivityFO = (from a in context.Activity_FlavourOptions.AsNoTracking()
                                          where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                          select a).ToList();

                        var ActivityFOAttribute = (from a in context.Activity_ClassificationAttributes.AsNoTracking()
                                                   where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id
                                                   && a.AttributeType == "ProductOption"
                                                   select a).ToList();

                        var ActivityDOW = (from a in context.Activity_DaysOfWeek.AsNoTracking()
                                           where a.Activity_Flavor_ID == Activity.Activity_Flavour_Id
                                           select a).ToList();

                        var ActivityOD = (from a in context.Activity_DaysOfOperation.AsNoTracking()
                                          where a.Activity_Flavor_ID == Activity.Activity_Flavour_Id
                                          && (a.IsOperatingDays ?? true) == true
                                          select a).ToList();

                        var ActivityCT = (from a in context.Activity_CategoriesType.AsNoTracking()
                                          where a.Activity_Flavour_Id == Activity.Activity_Flavour_Id && a.SystemProductNameSubType_ID != null
                                          && (a.IsActive ?? false) == true
                                          select a).ToList();

                        var ActivityDP = (from a in context.Activity_DeparturePoints.AsNoTracking()
                                          where a.Activity_Flavor_ID == Activity.Activity_Flavour_Id
                                          select a).ToList();

                        //create new mongo object record
                        var newActivity = new DataContracts.Activity.ActivityDefinition();

                        //newActivity.Activity_Flavour_Id = Activity.Activity_Flavour_Id.ToString();

                        newActivity.SystemActivityCode = Convert.ToInt32(Activity.CommonProductNameSubType_Id);

                        newActivity.SupplierCompanyCode = ActivitySPM.SupplierCode;

                        if (ActivitySPM.SupplierCode == "gta")
                        {
                            newActivity.SupplierCityDepartureCodes = context.Activity_SupplierCityDepartureCode.Where(w => w.CityCode == ActivitySPM.SupplierCityCode).Select(s => new DataContracts.Activity.SupplierCityDepartureCode
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

                        newActivity.InterestType = string.Join(",", ActivityCT.Select(s => s.SystemInterestType).Distinct());

                        newActivity.Category = string.Join(",", ActivityCT.Select(s => s.SystemProductCategorySubType).Distinct());

                        newActivity.Type = string.Join(",", ActivityCT.Select(s => s.SystemProductType).Distinct());

                        newActivity.SubType = string.Join(",", ActivityCT.Select(s => s.SystemProductNameSubType).Distinct());

                        newActivity.ProductSubTypeId = ActivityCT.Select(s => s.SystemProductNameSubType_ID.ToString().ToUpper()).ToList();

                        newActivity.Name = Activity.ProductName;

                        newActivity.Description = (ActivityDesc.Where(w => w.DescriptionType == "Long").Select(s => s.Description).FirstOrDefault());

                        newActivity.DeparturePoint = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "DeparturePoint").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.ReturnDetails = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "ReturnDetails").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.PhysicalIntensity = (ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "PhysicalIntensity").Select(s => s.AttributeValue).FirstOrDefault());

                        newActivity.Overview = (ActivityDesc.Where(w => w.DescriptionType == "Short").Select(s => s.Description).FirstOrDefault());

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
                                PricingCurrency = ActivitySPM.Currency,
                                AreaAddress = ActivitySPM.AreaAddress,
                                TourType = ActivitySPM.TourType
                            };
                        }


                        newActivity.Deals = ActivityDeals.Select(s => new DataContracts.Activity.Deals { Currency = s.Deal_Currency, DealId = s.DealCode, DealPrice = s.Deal_Price, DealText = s.DealText, OfferTermsAndConditions = s.Deal_TnC }).ToList();

                        newActivity.Prices = ActivityPrices.OrderBy(o => o.Price).Select(s => new DataContracts.Activity.Prices
                        {
                            OptionCode = s.Price_OptionCode,
                            PriceFor = s.Price_For,
                            Price = Convert.ToDouble(s.Price),
                            PriceType = s.Price_Type,
                            PriceBasis = s.PriceBasis,
                            PriceId = s.PriceCode,
                            SupplierCurrency = s.PriceCurrency,
                            FromPax = s.FromPax,
                            ToPax = s.ToPax,
                            Market = s.Market,
                            PersonType = s.PersonType,
                            ValidFrom = s.Price_ValidFrom == null ? string.Empty : s.Price_ValidFrom.ToString(),
                            ValidTo = s.Price_ValidTo == null ? string.Empty : s.Price_ValidTo.ToString()
                        }).ToList();

                        newActivity.ProductOptions = (from afo in ActivityFO
                                                      select new DataContracts.Activity.ProductOptions
                                                      {
                                                          SystemActivityOptionCode = afo.TLGXActivityOptionCode,
                                                          OptionCode = afo.Activity_OptionCode,
                                                          ActivityType = afo.Activity_Type,
                                                          DealText = afo.Activity_DealText,
                                                          Options = afo.Activity_OptionName,
                                                          Language = afo.Activity_Language,
                                                          LanguageCode = afo.Activity_LanguageCode,
                                                          Activity_FlavourOptions_Id = afo.Activity_FlavourOptions_Id
                                                      }).ToList();

                        foreach (var item in newActivity.ProductOptions)
                        {
                            var itemCA = (from a in ActivityFOAttribute where a.Activity_FlavourOptions_Id == item.Activity_FlavourOptions_Id select a).ToList();
                            if (itemCA != null && itemCA.Count > 0)
                            {
                                item.ClassificationAttrributes = new List<DataContracts.Activity.ClassificationAttrributes>();
                                foreach (var itemCAI in itemCA)
                                {
                                    item.ClassificationAttrributes.Add(new DataContracts.Activity.ClassificationAttrributes
                                    {
                                        Group = itemCAI.AttributeSubType,
                                        Type = itemCAI.AttributeType,
                                        Value = itemCAI.AttributeValue

                                    });
                                }
                            }
                        }

                        newActivity.ClassificationAttrributes = new List<DataContracts.Activity.ClassificationAttrributes>();
                        newActivity.ClassificationAttrributes.AddRange(ActivityClassAttr.Where(w => w.AttributeType == "Internal").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList());
                        newActivity.ClassificationAttrributes.AddRange(ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "ThingsToCarry").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList());
                        newActivity.ClassificationAttrributes.AddRange(ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "RecommendedDuration").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList());
                        newActivity.ClassificationAttrributes.AddRange(ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "Market").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList());
                        newActivity.ClassificationAttrributes.AddRange(ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "AdditionalInfo").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList());
                        newActivity.ClassificationAttrributes.AddRange(ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "Notes").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList());
                        newActivity.ClassificationAttrributes.AddRange(ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "Physicalntensity").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList());
                        newActivity.ClassificationAttrributes.AddRange(ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "Advisory").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList());
                        newActivity.ClassificationAttrributes.AddRange(ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "PackagePeriod").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList());
                        newActivity.ClassificationAttrributes.AddRange(ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "BestFor").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList());
                        newActivity.ClassificationAttrributes.AddRange(ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "Itinerary").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList());
                        newActivity.ClassificationAttrributes.AddRange(ActivityClassAttr.Where(w => w.AttributeType == "Product" && w.AttributeSubType == "TagWith").Select(s => new DataContracts.Activity.ClassificationAttrributes { Group = s.AttributeSubType, Type = s.AttributeType, Value = s.AttributeValue }).ToList());

                        newActivity.SystemMapping = new DataContracts.Activity.SystemMapping { SystemID = string.Empty, SystemName = string.Empty };//ActivitySPM.Select(s => new DataContracts.Activity.SystemMapping { SystemID = string.Empty, SystemName = string.Empty }).FirstOrDefault();

                        newActivity.DaysOfTheWeek = (from DOW in ActivityDOW
                                                     join OD in ActivityOD on DOW.Activity_DaysOfOperation_Id equals OD.Activity_DaysOfOperation_Id into ODlj
                                                     from ODljS in ODlj.DefaultIfEmpty()
                                                     join DP in ActivityDP on DOW.Activity_DaysOfWeek_ID equals DP.Activity_DaysOfWeek_ID into DPlj
                                                     from DPljS in DPlj.DefaultIfEmpty()
                                                     select new DataContracts.Activity.DaysOfWeek
                                                     {
                                                         OperatingFromDate = ODljS == null ? string.Empty : ODljS.FromDate.ToString(),
                                                         OperatingToDate = ODljS == null ? string.Empty : ODljS.ToDate.ToString(),

                                                         SupplierDuration = DOW.SupplierDuration ?? string.Empty,
                                                         SupplierEndTime = DOW.SupplierEndTime ?? string.Empty,
                                                         SupplierFrequency = DOW.SupplierFrequency ?? string.Empty,
                                                         SupplierSession = DOW.SupplierSession ?? string.Empty,
                                                         SupplierStartTime = DOW.SupplierStartTime ?? string.Empty,

                                                         Session = DOW.Session ?? string.Empty,
                                                         StartTime = DOW.StartTime ?? string.Empty,
                                                         EndTime = DOW.EndTime ?? string.Empty,
                                                         Duration = DOW.Duration ?? string.Empty,
                                                         DurationType = DOW.DurationType ?? string.Empty,


                                                         Sunday = DOW.Sun ?? false,
                                                         Monday = DOW.Mon ?? false,
                                                         Tuesday = DOW.Tues ?? false,
                                                         Wednesday = DOW.Wed ?? false,
                                                         Thursday = DOW.Thur ?? false,
                                                         Friday = DOW.Fri ?? false,
                                                         Saturday = DOW.Sat ?? false,

                                                         DepartureCode = DPljS == null ? string.Empty : DPljS.DepartureCode,
                                                         DeparturePoint = DPljS == null ? string.Empty : DPljS.DeparturePoint,
                                                         DepartureDescription = DPljS == null ? string.Empty : DPljS.Description

                                                     }).ToList();

                        //if (Activity_Flavour_Id == Guid.Empty)
                        //{
                        //    collection.InsertOneAsync(newActivity);
                        //}
                        //else
                        //{
                        var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity.CommonProductNameSubType_Id));
                        collection.ReplaceOneAsync(filter, newActivity, new UpdateOptions { IsUpsert = true });
                        // }

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
                        ActivityFOAttribute = null;
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
                return true;

            }
        }
        #endregion
        /// <summary>
        /// get data by Supplier to push into mongo
        /// </summary>
        /// <param name="suppliername"></param>
        public void LoadActivityDefinitionBySupplier(string log_id, string suppliername)
        {
            using (TLGX_DEVEntities context = new TLGX_DEVEntities())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                List<Activity_Flavour> ActivityList;

                if (suppliername != string.Empty)
                {
                    ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                    join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                    where a.CityCode != null && (spm.IsActive ?? false) == true
                                    && spm.SupplierName == suppliername
                                    select a).ToList();
                    int totalCount = ActivityList.Count;
                    if (totalCount > 0)
                    {
                        UpdateDistLogInfo(Guid.Parse(log_id), PushStatus.RUNNNING, totalCount, 0, string.Empty, string.Empty, string.Empty);
                        LoadActivityData(ActivityList, log_id);
                        UpdateDistLogInfo(Guid.Parse(log_id), PushStatus.COMPLETED, totalCount, totalCount, string.Empty, string.Empty, string.Empty);
                    }
                }
                //Remove Inactive or deleted Data
                try
                {
                    RemoveActivityDefinitionBySupplier(suppliername);
                }
                catch (Exception)
                {
                }
            }
        }

        private void RemoveActivityDefinitionBySupplier(string suppliername, string Activity_Flavour_Id = null)
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    List<Activity_Flavour> ActivityList;
                    if (Activity_Flavour_Id != null)
                    {
                        Guid _guidActivity_flavour_id = Guid.Parse(Activity_Flavour_Id);
                        ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                        join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                        where spm.SupplierName == suppliername && a.Activity_Flavour_Id == _guidActivity_flavour_id
                                        select a).ToList();
                    }
                    else
                    {
                        ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                        join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                        where a.CityCode != null && (spm.IsActive ?? false) == false
                                        && spm.SupplierName == suppliername
                                        select a).ToList();
                    }
                    _database = MongoDBHandler.mDatabase();
                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");
                    foreach (var Activity in ActivityList)
                    {
                        try
                        {
                            var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity.CommonProductNameSubType_Id));
                            if (filter != null)
                                collection.DeleteOne(filter);
                        }
                        catch (Exception ex)
                        {

                        }



                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void UpdateLogStatus(string log_id, string strStatus, string stredituser, int count = 0)
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    var LogUpdate = context.DistributionLayerRefresh_Log.Find(Guid.Parse(log_id));
                    if (LogUpdate != null)
                    {
                        LogUpdate.Status = strStatus;
                        LogUpdate.Edit_Date = DateTime.Now;
                        LogUpdate.Edit_User = stredituser;
                        if (count > 0)
                            LogUpdate.MongoPushCount = count;
                        context.SaveChanges();
                    }

                    LogUpdate = null;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Supplier Accommodation Static Data

        public void LoadAccoStaticData(Guid log_id, Guid SupplierId)
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    context.Database.CommandTimeout = 0;
                    context.Configuration.AutoDetectChangesEnabled = true;
                    //set the distribution log to running status
                    var Log = context.DistributionLayerRefresh_Log.Find(log_id);
                    if (Log != null)
                    {
                        Log.Status = "Running";
                        Log.Edit_Date = DateTime.Now;
                        Log.Edit_User = "MongoPush";
                        context.SaveChanges();
                    }
                    Log = null;

                    context.Configuration.AutoDetectChangesEnabled = false;
                    //Get Master Attributes and Supplier Attribute Mapping
                    string sqlMasterFor = "'HotelInfo','FacilityInfo','RoomInfo','RoomAmenities','Media','BedType','RoomInfo','RoomMedia','RoomAmenities'";

                    _database = MongoDBHandler.mDatabase();
                    var collection = _database.GetCollection<DataContracts.StaticData.Accomodation>("AccoStaticData");

                    string sqlSupplierProducts = "";
                    sqlSupplierProducts = "Select  SupplierEntity_Id,Parent_Id,Supplier_Id,SupplierName,SupplierProductCode,Entity,Create_Date,Create_User from SupplierEntity with (NoLock) where Entity ='HotelInfo'";
                    sqlSupplierProducts = sqlSupplierProducts + " and Supplier_Id = '" + SupplierId.ToString() + "';";
                    var SupplierProducts = context.Database.SqlQuery<DC_SupplierEntity>(sqlSupplierProducts.ToString()).ToList();
                    int iTotalCount = SupplierProducts.Count();
                    int iCounter = 0;

                    foreach (var product in SupplierProducts)
                    {
                        try
                        {
                            ////check if the product already exists
                            //var searchResultCount = collection.Find(f => f.AccomodationInfo.CompanyId == product.SupplierName.ToUpper() && f.AccomodationInfo.CompanyProductId == product.SupplierProductCode.ToUpper()).Count();
                            //if (searchResultCount > 0)
                            //{
                            //    if (iCounter % 500 == 0)
                            //    {
                            //        UpdateCount(iTotalCount, iCounter, log_id);
                            //    }
                            //    iCounter++;
                            //    continue;
                            //}

                            var newProduct = new DataContracts.StaticData.Accomodation();


                            string sql = "";
                            sql = "Select a.SupplierProperty, IIF(a.SystemValue<> null,a.SystemValue,a.SupplierValue ) as Value, a.AttributeMap_Id, SystemAttribute = Upper(MAM.Name)";
                            sql = sql + "from SupplierEntityValues a with (NoLock)  join m_MasterAttributeMapping MA with (NoLock)  on a.AttributeMap_Id = MA.MasterAttributeMapping_Id ";
                            sql = sql + "join m_masterattribute MAM with (NoLock)  on MA.SystemMasterAttribute_Id = MAM.MasterAttribute_Id join Supplier S with (NoLock)  on MA.Supplier_Id = S.Supplier_Id where a.SupplierEntity_Id = '" + product.SupplierEntity_Id + "'" + "and  MAM.MasterFor in  (" + sqlMasterFor + ")";

                            var HotelInfoDetails = context.Database.SqlQuery<DC_SupplierProductValues>(sql.ToString()).ToList();

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

                                    //New field Added
                                    CityCode = HotelInfoDetails.Where(w => w.SystemAttribute == "CITYCODE").Select(s => s.Value).FirstOrDefault(),
                                    CountryCode = HotelInfoDetails.Where(w => w.SystemAttribute == "COUNTRYCODE").Select(s => s.Value).FirstOrDefault(),
                                    StateCode = HotelInfoDetails.Where(w => w.SystemAttribute == "STATECODE").Select(s => s.Value).FirstOrDefault(),
                                    DistrictId = HotelInfoDetails.Where(w => w.SystemAttribute == "DISTRICTID").Select(s => s.Value).FirstOrDefault(),
                                    DistrictName = HotelInfoDetails.Where(w => w.SystemAttribute == "DISTRICTNAME").Select(s => s.Value).FirstOrDefault(),
                                    Region = HotelInfoDetails.Where(w => w.SystemAttribute == "REGION").Select(s => s.Value).FirstOrDefault(),

                                    //New field Added

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
                                Affiliations = HotelInfoDetails.Where(w => w.SystemAttribute == "AFFILIATIONS").Select(s => s.Value).FirstOrDefault(),
                                Brand = HotelInfoDetails.Where(w => w.SystemAttribute == "BRAND").Select(s => s.Value).FirstOrDefault(),
                                Chain = HotelInfoDetails.Where(w => w.SystemAttribute == "CHAIN").Select(s => s.Value).FirstOrDefault(),
                                CheckInTime = HotelInfoDetails.Where(w => w.SystemAttribute == "CHECKINTIME").Select(s => s.Value).FirstOrDefault(),
                                CheckOutTime = HotelInfoDetails.Where(w => w.SystemAttribute == "CHECKOUTTIME").Select(s => s.Value).FirstOrDefault(),
                                CommonProductId = string.Empty,
                                CompanyId = product.SupplierName.ToUpper(),
                                CompanyName = product.SupplierName.ToUpper(),
                                CompanyProductId = product.SupplierProductCode.ToUpper(),
                                CompanyRating = HotelInfoDetails.Where(w => w.SystemAttribute == "RATING").Select(s => s.Value).FirstOrDefault(),
                                Stars = HotelInfoDetails.Where(w => w.SystemAttribute == "STARS").Select(s => s.Value).FirstOrDefault(),

                                //New field Added

                                CheckinCloseTime = HotelInfoDetails.Where(w => w.SystemAttribute == "CHECKINCLOSETIME").Select(s => s.Value).FirstOrDefault(),
                                CheckoutCloseTime = HotelInfoDetails.Where(w => w.SystemAttribute == "CHECKOUTCLOSETIME").Select(s => s.Value).FirstOrDefault(),




                                currency = HotelInfoDetails.Where(w => w.SystemAttribute == "CURRENCY").Select(s => s.Value).FirstOrDefault(),
                                DefaultLanguage = HotelInfoDetails.Where(w => w.SystemAttribute == "DEFAULTLANGUAGE").Select(s => s.Value).FirstOrDefault(),

                                //New field Added



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
                                        Website = HotelInfoDetails.Where(w => w.SystemAttribute == "WEBSITE").Select(s => s.Value).FirstOrDefault(),
                                          MobileAppUrl = HotelInfoDetails.Where(w => w.SystemAttribute == "MOBILEAPPURL").Select(s => s.Value).FirstOrDefault()
                                    }
                                },
                                DisplayName = HotelInfoDetails.Where(w => w.SystemAttribute == "HOTELNAME").Select(s => s.Value).FirstOrDefault(),
                                FamilyDetails = null,
                                FinanceControlId = null,
                                General = new DataContracts.StaticData.General
                                {
                                    Extras = new List<DataContracts.StaticData.Extras>
                                    {
                                        new DataContracts.StaticData.Extras {  Label = "Short", Description = HotelInfoDetails.Where(w => w.SystemAttribute == "SHORTDESCRIPTION").Select(s => s.Value).FirstOrDefault() } ,
                                        new DataContracts.StaticData.Extras {  Label = "Long", Description = HotelInfoDetails.Where(w => w.SystemAttribute == "LONGDESCRIPTION").Select(s => s.Value).FirstOrDefault() },
                                          //New field Added
                                        new DataContracts.StaticData.Extras {  Label = "Exterior", Description = HotelInfoDetails.Where(w => w.SystemAttribute == "EXTERIORDESCRIPTION").Select(s => s.Value).FirstOrDefault() },
                                        new DataContracts.StaticData.Extras {  Label = "Lobby", Description = HotelInfoDetails.Where(w => w.SystemAttribute == "LOBBYDESCRIPTION").Select(s => s.Value).FirstOrDefault() },
                                        new DataContracts.StaticData.Extras {  Label = "Position", Description = HotelInfoDetails.Where(w => w.SystemAttribute == "POSITIONDESCRIPTION").Select(s => s.Value).FirstOrDefault() },
                                        new DataContracts.StaticData.Extras {  Label = "Restaurant", Description = HotelInfoDetails.Where(w => w.SystemAttribute == "RESTAURANTDESCRIPTION").Select(s => s.Value).FirstOrDefault() },
                                        new DataContracts.StaticData.Extras {  Label = "Room", Description = HotelInfoDetails.Where(w => w.SystemAttribute == "ROOMDESCRIPTION").Select(s => s.Value).FirstOrDefault() }
                                          //New field Added

                                    },
                                    YearBuilt = HotelInfoDetails.Where(w => w.SystemAttribute == "YEARBUILT").Select(s => s.Value).FirstOrDefault(),
                                    YearRenovated = HotelInfoDetails.Where(w => w.SystemAttribute == "RENOVATIONYEAR").Select(s => s.Value).FirstOrDefault(),

                                },
                                IsMysteryProduct = false,
                                IsTwentyFourHourCheckout = false,
                                Name = HotelInfoDetails.Where(w => w.SystemAttribute == "HOTELNAME").Select(s => s.Value).FirstOrDefault(),

                                //New field Added
                                ProductCategorySubTypeId = HotelInfoDetails.Where(w => w.SystemAttribute == "PRODUCTCATEGORYSUBTYPEID").Select(s => s.Value).FirstOrDefault(),
                                ExactRating = HotelInfoDetails.Where(w => w.SystemAttribute == "EXACTRATING").Select(s => s.Value).FirstOrDefault(),
                                HotelMessage = HotelInfoDetails.Where(w => w.SystemAttribute == "HOTELMESSAGE").Select(s => s.Value).FirstOrDefault(),
                                MaxPersonReservation = HotelInfoDetails.Where(w => w.SystemAttribute == "MAXPERSONRESERVATION").Select(s => s.Value).FirstOrDefault(),
                                MaxRoomReservation = HotelInfoDetails.Where(w => w.SystemAttribute == "MAXROOMRESERVATION").Select(s => s.Value).FirstOrDefault(),
                                noOfReviews = HotelInfoDetails.Where(w => w.SystemAttribute == "NOOFREVIEWS").Select(s => s.Value).FirstOrDefault(),
                                HotelRanking = HotelInfoDetails.Where(w => w.SystemAttribute == "HOTELRANKING").Select(s => s.Value).FirstOrDefault(),
                                ReviewScore = HotelInfoDetails.Where(w => w.SystemAttribute == "REVIEWSCORE").Select(s => s.Value).FirstOrDefault(),
                                SpokenLanguages = HotelInfoDetails.Where(w => w.SystemAttribute == "SPOKENLANGUAGES").Select(s => s.Value).FirstOrDefault(),


                                Facilities = HotelInfoDetails.Where(w => w.SystemAttribute == "FACILITIES").Select(s => s.Value).FirstOrDefault(),
                                //New field Added

                                ProductCatSubType = HotelInfoDetails.Where(w => w.SystemAttribute == "PRODUCTCATEGORYSUBTYPE").Select(s => s.Value).FirstOrDefault(),
                                Rating = HotelInfoDetails.Where(w => w.SystemAttribute == "RATING").Select(s => s.Value).FirstOrDefault(),
                                RatingDatedOn = null,
                                RecommendedFor = HotelInfoDetails.Where(w => w.SystemAttribute == "RECOMMENDEDFOR").Select(s => s.Value).ToList(),
                                ResortType = HotelInfoDetails.Where(w => w.SystemAttribute == "PRODUCTCATEGORYSUBTYPE").Select(s => s.Value).FirstOrDefault()
                            };

                            //NoOfFloors = HotelInfoDetails.Where(w => w.SystemAttribute == "NOOFFLOORS").Select(s => s.Value).FirstOrDefault(),
                            //    NoOfRooms = HotelInfoDetails.Where(w => w.SystemAttribute == "NOOFROOMS").Select(s => s.Value).FirstOrDefault(),

                            //New field Added
                            //IsRatingEstimatedAutomatically = ;


                            var ratingEstimated = HotelInfoDetails.Where(w => w.SystemAttribute == "ISRATINGESTIMATEDAUTOMATICALLY").Select(s => s.Value).FirstOrDefault();
                            if (ratingEstimated != null)
                            {
                                bool IsratingEstimatedValue;
                                var IsratingEstimated = CustomConvert(ratingEstimated, typeof(bool));
                                if (Boolean.TryParse(Convert.ToString(IsratingEstimated), out IsratingEstimatedValue))
                                {
                                    newProduct.AccomodationInfo.IsRatingEstimatedAutomatically = IsratingEstimatedValue;
                                }
                            }



                            var creditcardRequired = HotelInfoDetails.Where(w => w.SystemAttribute == "ISCREDITCARDREQUIRED").Select(s => s.Value).FirstOrDefault();
                            if (creditcardRequired != null)
                            {
                                bool IscreditcardRequiredValue;
                                var IsratingEstimated = CustomConvert(creditcardRequired, typeof(bool));
                                if (Boolean.TryParse(Convert.ToString(IsratingEstimated), out IscreditcardRequiredValue))
                                {
                                    newProduct.AccomodationInfo.IsCreditcardRequired = IscreditcardRequiredValue;
                                }

                            }



                            var BookWithoutCC = HotelInfoDetails.Where(w => w.SystemAttribute == "ISBOOKWITHOUTCC").Select(s => s.Value).FirstOrDefault();
                            if (BookWithoutCC != null)
                            {
                                bool IsBookWithoutCCValue;
                                var IsBookWithoutCC = CustomConvert(BookWithoutCC, typeof(bool));
                                if (Boolean.TryParse(Convert.ToString(IsBookWithoutCC), out IsBookWithoutCCValue))
                                {
                                    newProduct.AccomodationInfo.IsBookWithoutCC = IsBookWithoutCCValue;
                                }
                            }


                            var isRecommended = HotelInfoDetails.Where(w => w.SystemAttribute == "ISRECOMMENDED").Select(s => s.Value).FirstOrDefault();
                            if (isRecommended != null)
                            {
                                bool IsRecommendedvalue;
                                var IsRecommended = CustomConvert(isRecommended, typeof(bool));
                                if (Boolean.TryParse(Convert.ToString(IsRecommended), out IsRecommendedvalue))
                                {
                                    newProduct.AccomodationInfo.IsRecommended = IsRecommendedvalue;
                                }
                            }



                            newProduct.Policies = new List<DataContracts.StaticData.Policies>
                                {
                                    new DataContracts.StaticData.Policies { Type = "ChildrenPolicy" , Description = HotelInfoDetails.Where(w => w.SystemAttribute == "CHILDRENPOLICY").Select(s => s.Value).FirstOrDefault() } ,
                                    new DataContracts.StaticData.Policies { Type = "InternetPolicy" , Description =HotelInfoDetails.Where(w => w.SystemAttribute == "INTERNETPOLICY").Select(s => s.Value).FirstOrDefault() },
                                    new DataContracts.StaticData.Policies { Type = "ParkingPolicy" , Description =HotelInfoDetails.Where(w => w.SystemAttribute == "PARKINGPOLICY").Select(s => s.Value).FirstOrDefault() } ,
                                    new DataContracts.StaticData.Policies { Type = "PetsPolicy",Description = HotelInfoDetails.Where(w => w.SystemAttribute == "PETSPOLICY").Select(s => s.Value).FirstOrDefault() } ,
                                    new DataContracts.StaticData.Policies { Type = "GroupsPolicy",Description  = HotelInfoDetails.Where(w => w.SystemAttribute == "GROUPSPOLICY").Select(s => s.Value).FirstOrDefault()} ,
                                    new DataContracts.StaticData.Policies { Type = "Policies",Description  = HotelInfoDetails.Where(w => w.SystemAttribute == "POLICIES").Select(s => s.Value).FirstOrDefault()} ,
                                    new DataContracts.StaticData.Policies { Type = "RecreationPolicy",Description  = HotelInfoDetails.Where(w => w.SystemAttribute == "RECREATIONPOLICY").Select(s => s.Value).FirstOrDefault()} ,
                                    new DataContracts.StaticData.Policies { Type = "TermsAndConditions",Description  = HotelInfoDetails.Where(w => w.SystemAttribute == "TERMSANDCONDITIONS").Select(s => s.Value).FirstOrDefault()} ,
                                };

                            //New field Added

                            int NoOfFloors, NoOfRooms;

                            var strNoOfFloors = HotelInfoDetails.Where(w => w.SystemAttribute == "NOOFFLOORS").Select(s => s.Value).FirstOrDefault();
                            if (strNoOfFloors != null)
                            {
                                strNoOfFloors = Regex.Match(strNoOfFloors, @"\d+").Value;
                            }

                            var strNoOfRooms = HotelInfoDetails.Where(w => w.SystemAttribute == "NOOFROOMS").Select(s => s.Value).FirstOrDefault();
                            if (strNoOfRooms != null)
                            {
                                strNoOfRooms = Regex.Match(strNoOfRooms, @"\d+").Value;
                            }

                            if (int.TryParse(strNoOfFloors, out NoOfFloors))
                            {
                                newProduct.AccomodationInfo.NoOfFloors = NoOfFloors;
                            }

                            if (int.TryParse(strNoOfRooms, out NoOfRooms))
                            {
                                newProduct.AccomodationInfo.NoOfRooms = NoOfRooms;
                            }

                            newProduct.Overview = new DataContracts.StaticData.Overview
                            {
                                IsCompanyRecommended = false,
                                Duration = null,
                                HashTag = null,
                                Highlights = null,
                                //Interest = HotelInfoDetails.Where(w => w.SystemAttribute == "INTEREST").Select(s => s.Value).ToList(),
                                Interest = null,
                                SellingTips = null,
                                Usp = null
                            };

                            //Get & Set Facilities
                            newProduct.Facility = new List<DataContracts.StaticData.Facility>();

                            string sqlFacilityInfo = "";
                            sqlFacilityInfo = "select a.Entity, a.SupplierEntity_Id from SupplierEntity a with (NoLock)  where a.Parent_Id = '" + product.SupplierEntity_Id + "' and a.Entity ='FacilityInfo'";
                            var FacilityInfoList = context.Database.SqlQuery<DC_SupplierEntity>(sqlFacilityInfo.ToString()).ToList();

                            foreach (var Facility in FacilityInfoList)
                            {
                                string resFacility = "";
                                resFacility = "select b.SupplierProperty, IIF(b.SystemValue<> null,b.SystemValue,b.SupplierValue ) as Value,b.AttributeMap_Id,SystemAttribute = Upper(MAM.Name) from SupplierEntity a with (NoLock) ";
                                resFacility = resFacility + " join SupplierEntityValues b with (NoLock) on a.SupplierEntity_Id = b.SupplierEntity_Id" + " join m_MasterAttributeMapping MA with (NoLock) on b.AttributeMap_Id = MA.MasterAttributeMapping_Id join m_masterattribute MAM with (NoLock) on MA.SystemMasterAttribute_Id = MAM.MasterAttribute_Id join Supplier S with (NoLock) on MA.Supplier_Id = S.Supplier_Id where MAM.MasterFor in  (" + sqlMasterFor + ")";
                                resFacility = resFacility + " and a.Supplier_Id = '" + product.Supplier_Id + "' and a.SupplierEntity_Id = '" + Facility.SupplierEntity_Id + " ' ";
                                var FacilityInfoDetails = context.Database.SqlQuery<DC_SupplierProductValues>(resFacility.ToString()).ToList();

                                newProduct.Facility.Add(new DataContracts.StaticData.Facility
                                {
                                    Facility_Id = FacilityInfoDetails.Where(w => w.SystemAttribute == "FACILITY_ID").Select(s => s.Value).FirstOrDefault(),
                                    Type = FacilityInfoDetails.Where(w => w.SystemAttribute == "FACILITYTYPE").Select(s => s.Value).FirstOrDefault(),
                                    FacilityCategoryID = FacilityInfoDetails.Where(w => w.SystemAttribute == "FACILITYCATEGORYID").Select(s => s.Value).FirstOrDefault(),
                                    Category = FacilityInfoDetails.Where(w => w.SystemAttribute == "FACILITYCATEGORY").Select(s => s.Value).FirstOrDefault(),
                                    ExtraCharge = FacilityInfoDetails.Where(w => w.SystemAttribute == "EXTRACHARGE").Select(s => s.Value).FirstOrDefault(),
                                });
                            }

                            //Get & Set Media
                            newProduct.Media = new List<DataContracts.StaticData.Media>();
                            string sqlMedia = "";
                            sqlMedia = "select a.Entity, a.SupplierEntity_Id from SupplierEntity a with (NoLock) where a.Parent_Id = '" + product.SupplierEntity_Id + "' and a.Entity = 'Media'";
                            var MediaList = context.Database.SqlQuery<DC_SupplierEntity>(sqlMedia.ToString()).ToList();

                            foreach (var Media in MediaList)
                            {
                                string sqlmedia = "";
                                sqlmedia = "select b.SupplierProperty, IIF(b.SystemValue <> null, b.SystemValue, b.SupplierValue) as Value,b.AttributeMap_Id,SystemAttribute = Upper(MAM.Name) from SupplierEntity a with (NoLock) ";
                                sqlmedia = sqlmedia + " join SupplierEntityValues b with (NoLock) on a.SupplierEntity_Id = b.SupplierEntity_Id join m_MasterAttributeMapping MA with (NoLock) on b.AttributeMap_Id = MA.MasterAttributeMapping_Id ";
                                sqlmedia = sqlmedia + " join m_masterattribute MAM with (NoLock) on MA.SystemMasterAttribute_Id = MAM.MasterAttribute_Id join Supplier S with (NoLock) on MA.Supplier_Id = S.Supplier_Id where" + " MAM.MasterFor in  (" + sqlMasterFor + ")";
                                sqlmedia = sqlmedia + " and a.Supplier_Id = '" + product.Supplier_Id + "' and  a.SupplierEntity_Id ='" + Media.SupplierEntity_Id + "'";

                                var MediaDetails = context.Database.SqlQuery<DC_SupplierProductValues>(sqlmedia.ToString()).ToList();


                                newProduct.Media.Add(new DataContracts.StaticData.Media
                                {
                                    MediaId = MediaDetails.Where(w => w.SystemAttribute == "MEDIAID").Select(s => s.Value).FirstOrDefault(),
                                    Description = MediaDetails.Where(w => w.SystemAttribute == "DESCRIPTION").Select(s => s.Value).FirstOrDefault(),
                                    FileType = "IMAGE",
                                    FileName = MediaDetails.Where(w => w.SystemAttribute == "LARGEIMAGEURL").Select(s => s.Value).FirstOrDefault() ?? MediaDetails.Where(w => w.SystemAttribute == "SMALLIMAGEURL").Select(s => s.Value).FirstOrDefault(),

                                    //New field Added
                                    MediaPosition = MediaDetails.Where(w => w.SystemAttribute == "MEDIAPOSITION").Select(s => s.Value).FirstOrDefault(),
                                    MediaWidth = MediaDetails.Where(w => w.SystemAttribute == "MEDIAWIDTH").Select(s => s.Value).FirstOrDefault(),
                                    MediaHeight = MediaDetails.Where(w => w.SystemAttribute == "MEDIAHEIGHT").Select(s => s.Value).FirstOrDefault(),
                                    LargeImageURL = MediaDetails.Where(w => w.SystemAttribute == "LARGEIMAGEURL").Select(s => s.Value).FirstOrDefault(),
                                    ThumbnailUrl = MediaDetails.Where(w => w.SystemAttribute == "THUMBNAILURL").Select(s => s.Value).FirstOrDefault(),
                                    MediaFileFormat = MediaDetails.Where(w => w.SystemAttribute == "MEDIAFILEFORMAT").Select(s => s.Value).FirstOrDefault(),
                                    SmallImageURL = MediaDetails.Where(w => w.SystemAttribute == "SMALLIMAGEURL").Select(s => s.Value).FirstOrDefault(),
                                    //New field Added


                                });
                            }

                            #region Room Details fatch


                            newProduct.Rooms = new List<DataContracts.StaticData.Rooms>();

                            string sqlRoomInfo = "";
                            sqlRoomInfo = "select a.Entity, a.SupplierEntity_Id from SupplierEntity a with (NoLock)  where a.Parent_Id = '" + product.SupplierEntity_Id + "' and a.Entity ='RoomInfo'";
                            var RoomInfoList = context.Database.SqlQuery<DC_SupplierEntity>(sqlRoomInfo.ToString()).ToList();

                            foreach (var RoomInfo in RoomInfoList)
                            {

                                //------------------------------Room Details 
                                string resRoom = "";
                                resRoom = "select b.SupplierProperty, IIF(b.SystemValue<> null,b.SystemValue,b.SupplierValue ) as Value,b.AttributeMap_Id,SystemAttribute = Upper(MAM.Name) from SupplierEntity a with (NoLock) ";
                                resRoom = resRoom + " join SupplierEntityValues b with (NoLock) on a.SupplierEntity_Id = b.SupplierEntity_Id" + " join m_MasterAttributeMapping MA with (NoLock) on b.AttributeMap_Id = MA.MasterAttributeMapping_Id join m_masterattribute MAM with (NoLock) on MA.SystemMasterAttribute_Id = MAM.MasterAttribute_Id join Supplier S with (NoLock) on MA.Supplier_Id = S.Supplier_Id where MAM.MasterFor in  (" + sqlMasterFor + ")";
                                resRoom = resRoom + " and a.Supplier_Id = '" + product.Supplier_Id + "' and a.SupplierEntity_Id = '" + RoomInfo.SupplierEntity_Id + " ' ";
                                var RoomInfoDetails = context.Database.SqlQuery<DC_SupplierProductValues>(resRoom.ToString()).ToList();

                                var room = new DataContracts.StaticData.Rooms
                                {
                                    RoomTypeId = RoomInfoDetails.Where(w => w.SystemAttribute == "ROOMTYPEID").Select(s => s.Value).FirstOrDefault(),
                                    RoomTypeName = RoomInfoDetails.Where(w => w.SystemAttribute == "ROOMTYPENAME").Select(s => s.Value).FirstOrDefault(),
                                    RoomCategoryCode = RoomInfoDetails.Where(w => w.SystemAttribute == "ROOMCATEGORYCODE").Select(s => s.Value).FirstOrDefault(),
                                    RoomCategoryName = RoomInfoDetails.Where(w => w.SystemAttribute == "ROOMCATEGORYNAME").Select(s => s.Value).FirstOrDefault(),
                                    RoomCode = RoomInfoDetails.Where(w => w.SystemAttribute == "ROOMCODE").Select(s => s.Value).FirstOrDefault(),
                                    CompanyRoomCategory = RoomInfoDetails.Where(w => w.SystemAttribute == "COMPANYROOMCATEGORY").Select(s => s.Value).FirstOrDefault(),
                                    RoomDescription = RoomInfoDetails.Where(w => w.SystemAttribute == "ROOMDESCRIPTION").Select(s => s.Value).FirstOrDefault(),
                                    //BathRoomCount = RoomInfoDetails.Where(w => w.SystemAttribute == "FACILITY_ID").Select(s => s.Value).FirstOrDefault(),
                                    BathRoomType = RoomInfoDetails.Where(w => w.SystemAttribute == "BATHROOMTYPE").Select(s => s.Value).FirstOrDefault(),
                                    Size = RoomInfoDetails.Where(w => w.SystemAttribute == "SIZE").Select(s => s.Value).FirstOrDefault(),
                                    View = RoomInfoDetails.Where(w => w.SystemAttribute == "VIEW").Select(s => s.Value).FirstOrDefault(),
                                    RoomRanking = RoomInfoDetails.Where(w => w.SystemAttribute == "ROOMRANKING").Select(s => s.Value).FirstOrDefault(),
                                    AmenitiesType = RoomInfoDetails.Where(w => w.SystemAttribute == "AMENITIESTYPE").Select(s => s.Value).FirstOrDefault(),
                                    IsRoomBookable = false,


                                    //MaxPrice = RoomInfoDetails.Where(w => w.SystemAttribute == "MAXPRICE").Select(s => s.Value).FirstOrDefault(),
                                    //MinPrice = RoomInfoDetails.Where(w => w.SystemAttribute == "MINPRICE").Select(s => s.Value).FirstOrDefault(),

                                    //        Facility_Id = FacilityInfoDetails.Where(w => w.SystemAttribute == "FACILITY_ID").Select(s => s.Value).FirstOrDefault(),
                                    //        Type = FacilityInfoDetails.Where(w => w.SystemAttribute == "FACILITYTYPE").Select(s => s.Value).FirstOrDefault(),
                                    //        FacilityCategoryID = FacilityInfoDetails.Where(w => w.SystemAttribute == "FACILITYCATEGORYID").Select(s => s.Value).FirstOrDefault(),
                                    //        Category = FacilityInfoDetails.Where(w => w.SystemAttribute == "FACILITYCATEGORY").Select(s => s.Value).FirstOrDefault(),
                                    //        ExtraCharge = FacilityInfoDetails.Where(w => w.SystemAttribute == "EXTRACHARGE").Select(s => s.Value).FirstOrDefault(),
                                };


                                var isRoomBookable = RoomInfoDetails.Where(w => w.SystemAttribute == "ISROOMBOOKABLE").Select(s => s.Value).FirstOrDefault();
                                if (isRoomBookable != null)
                                {
                                    bool isRoomBookablevalue;
                                    var IsRoomBookable = CustomConvert(isRoomBookable, typeof(bool));
                                    if (Boolean.TryParse(Convert.ToString(IsRoomBookable), out isRoomBookablevalue))
                                    {
                                        room.IsRoomBookable = isRoomBookablevalue;
                                    }
                                }




                                int BathRoomCount;
                                var strBathRooms = HotelInfoDetails.Where(w => w.SystemAttribute == "BATHROOMCOUNT").Select(s => s.Value).FirstOrDefault();
                                if (strBathRooms != null)
                                {
                                    strBathRooms = Regex.Match(strBathRooms, @"\d+").Value;
                                }


                                if (int.TryParse(strBathRooms, out BathRoomCount))
                                {
                                    room.BathRoomCount = BathRoomCount;
                                }

                                //------------------------------Room Details 

                                //------------------------------Room RoomAmenities
                                //Get & Set RoomAmenities
                                room.RoomAmenities = new List<DataContracts.StaticData.RoomAmenities>();

                                string sqlRoomAmenityInfo = "";
                                sqlRoomAmenityInfo = "select a.Entity, a.SupplierEntity_Id from SupplierEntity a with (NoLock)  where a.Parent_Id = '" + RoomInfo.SupplierEntity_Id + "' and a.Entity ='RoomAmenities'";
                                var AmenityInfoList = context.Database.SqlQuery<DC_SupplierEntity>(sqlRoomAmenityInfo.ToString()).ToList();

                                foreach (var Amenity in AmenityInfoList)
                                {
                                    string resAmenity = "";
                                    resAmenity = "select b.SupplierProperty, IIF(b.SystemValue<> null,b.SystemValue,b.SupplierValue ) as Value,b.AttributeMap_Id,SystemAttribute = Upper(MAM.Name) from SupplierEntity a with (NoLock) ";
                                    resAmenity = resAmenity + " join SupplierEntityValues b with (NoLock) on a.SupplierEntity_Id = b.SupplierEntity_Id" + " join m_MasterAttributeMapping MA with (NoLock) on b.AttributeMap_Id = MA.MasterAttributeMapping_Id join m_masterattribute MAM with (NoLock) on MA.SystemMasterAttribute_Id = MAM.MasterAttribute_Id join Supplier S with (NoLock) on MA.Supplier_Id = S.Supplier_Id where MAM.MasterFor in  (" + sqlMasterFor + ")";
                                    resAmenity = resAmenity + " and a.Supplier_Id = '" + product.Supplier_Id + "' and a.SupplierEntity_Id = '" + Amenity.SupplierEntity_Id + " ' ";
                                    var AmenityInfoDetails = context.Database.SqlQuery<DC_SupplierProductValues>(resAmenity.ToString()).ToList();

                                    room.RoomAmenities.Add(new DataContracts.StaticData.RoomAmenities
                                    {
                                        AmenityId = AmenityInfoDetails.Where(w => w.SystemAttribute == "AMENITYID").Select(s => s.Value).FirstOrDefault(),
                                        RoomAmenityType = AmenityInfoDetails.Where(w => w.SystemAttribute == "ROOMAMENITYTYPE").Select(s => s.Value).FirstOrDefault(),
                                        AmenityCategoryID = AmenityInfoDetails.Where(w => w.SystemAttribute == "AMENITYCATEGORYID").Select(s => s.Value).FirstOrDefault(),
                                        AmenityCategory = AmenityInfoDetails.Where(w => w.SystemAttribute == "AMENITYCATEGORY").Select(s => s.Value).FirstOrDefault(),

                                    });
                                }
                                //------------------------------Room RoomAmenities


                                //------------------------------Room BedRooms
                                //Get & Set BedRooms
                                room.BedRooms = new List<DataContracts.StaticData.BedRooms>();

                                var resu = RoomInfoDetails.Where(w => w.SystemAttribute == "BEDTYPE").Select(s => s.Value).FirstOrDefault();

                                string sqlBedRoomsInfo = "";
                                sqlBedRoomsInfo = "select a.Entity, a.SupplierEntity_Id from SupplierEntity a with (NoLock)  where a.Parent_Id = '" + RoomInfo.SupplierEntity_Id + "' and a.Entity ='BedType'";
                                var BedRoomsInfoList = context.Database.SqlQuery<DC_SupplierEntity>(sqlBedRoomsInfo.ToString()).ToList();

                                if (BedRoomsInfoList.Count > 0)
                                {
                                    foreach (var BedRoom in BedRoomsInfoList)
                                    {
                                        string resBedRoom = "";
                                        resBedRoom = "select b.SupplierProperty, IIF(b.SystemValue<> null,b.SystemValue,b.SupplierValue ) as Value,b.AttributeMap_Id,SystemAttribute = Upper(MAM.Name) from SupplierEntity a with (NoLock) ";
                                        resBedRoom = resBedRoom + " join SupplierEntityValues b with (NoLock) on a.SupplierEntity_Id = b.SupplierEntity_Id" + " join m_MasterAttributeMapping MA with (NoLock) on b.AttributeMap_Id = MA.MasterAttributeMapping_Id join m_masterattribute MAM with (NoLock) on MA.SystemMasterAttribute_Id = MAM.MasterAttribute_Id join Supplier S with (NoLock) on MA.Supplier_Id = S.Supplier_Id where MAM.MasterFor in  (" + sqlMasterFor + ")";
                                        resBedRoom = resBedRoom + " and a.Supplier_Id = '" + product.Supplier_Id + "' and a.SupplierEntity_Id = '" + BedRoom.SupplierEntity_Id + " ' ";
                                        var BedRoomInfoDetails = context.Database.SqlQuery<DC_SupplierProductValues>(resBedRoom.ToString()).ToList();

                                        room.BedRooms.Add(new DataContracts.StaticData.BedRooms
                                        {
                                            BedTypeID = BedRoomInfoDetails.Where(w => w.SystemAttribute == "BEDTYPEID").Select(s => s.Value).FirstOrDefault(),
                                            BedType = BedRoomInfoDetails.Where(w => w.SystemAttribute == "BEDTYPE").Select(s => s.Value).FirstOrDefault(),
                                            BeddingConfiguration = BedRoomInfoDetails.Where(w => w.SystemAttribute == "BEDDINGCONFIGURATION").Select(s => s.Value).FirstOrDefault(),

                                            BedRoomCount = BedRoomInfoDetails.Where(w => w.SystemAttribute == "BEDROOMCOUNT").Select(s => s.Value).FirstOrDefault(),
                                            MaxAdultWithExtraBed = BedRoomInfoDetails.Where(w => w.SystemAttribute == "MAXADULTWITHEXTRABED").Select(s => s.Value).FirstOrDefault(),
                                            MaxChildWithExtraBed = BedRoomInfoDetails.Where(w => w.SystemAttribute == "MAXCHILDWITHEXTRABED").Select(s => s.Value).FirstOrDefault(),
                                            NoOfExreaBeds = BedRoomInfoDetails.Where(w => w.SystemAttribute == "NOOFEXREABEDS").Select(s => s.Value).FirstOrDefault(),
                                        });
                                    }
                                }
                                else
                                {
                                    room.BedRooms.Add(new DataContracts.StaticData.BedRooms
                                    {
                                        BedTypeID = RoomInfoDetails.Where(w => w.SystemAttribute == "BEDTYPEID").Select(s => s.Value).FirstOrDefault(),
                                        BedType = RoomInfoDetails.Where(w => w.SystemAttribute == "BEDTYPE").Select(s => s.Value).FirstOrDefault(),
                                        BeddingConfiguration = RoomInfoDetails.Where(w => w.SystemAttribute == "BEDDINGCONFIGURATION").Select(s => s.Value).FirstOrDefault(),
                                        BedRoomCount = RoomInfoDetails.Where(w => w.SystemAttribute == "BEDROOMCOUNT").Select(s => s.Value).FirstOrDefault(),
                                        MaxAdultWithExtraBed = RoomInfoDetails.Where(w => w.SystemAttribute == "MAXADULTWITHEXTRABED").Select(s => s.Value).FirstOrDefault(),
                                        MaxChildWithExtraBed = RoomInfoDetails.Where(w => w.SystemAttribute == "MAXCHILDWITHEXTRABED").Select(s => s.Value).FirstOrDefault(),
                                        NoOfExreaBeds = RoomInfoDetails.Where(w => w.SystemAttribute == "NOOFEXREABEDS").Select(s => s.Value).FirstOrDefault(),
                                    });
                                }

                                //------------------------------Room BedRooms

                                //------------------------------RoomMedia
                                //Get & Set RoomMedia
                                room.RoomMedia = new List<DataContracts.StaticData.Media>();
                                string sqlRoomMedia = "";
                                sqlRoomMedia = "select a.Entity, a.SupplierEntity_Id from SupplierEntity a with (NoLock) where a.Parent_Id = '" + RoomInfo.SupplierEntity_Id + "' and a.Entity = 'RoomMedia'";
                                var RoomMediaList = context.Database.SqlQuery<DC_SupplierEntity>(sqlRoomMedia.ToString()).ToList();

                                foreach (var RoomMedia in RoomMediaList)
                                {
                                    string sqlroommedia = "";
                                    sqlroommedia = "select b.SupplierProperty, IIF(b.SystemValue <> null, b.SystemValue, b.SupplierValue) as Value,b.AttributeMap_Id,SystemAttribute = Upper(MAM.Name) from SupplierEntity a with (NoLock) ";
                                    sqlroommedia = sqlroommedia + " join SupplierEntityValues b with (NoLock) on a.SupplierEntity_Id = b.SupplierEntity_Id join m_MasterAttributeMapping MA with (NoLock) on b.AttributeMap_Id = MA.MasterAttributeMapping_Id ";
                                    sqlroommedia = sqlroommedia + " join m_masterattribute MAM with (NoLock) on MA.SystemMasterAttribute_Id = MAM.MasterAttribute_Id join Supplier S with (NoLock) on MA.Supplier_Id = S.Supplier_Id where" + " MAM.MasterFor in  (" + sqlMasterFor + ")";
                                    sqlroommedia = sqlroommedia + " and a.Supplier_Id = '" + product.Supplier_Id + "' and  a.SupplierEntity_Id ='" + RoomMedia.SupplierEntity_Id + "'";

                                    var MediaDetails = context.Database.SqlQuery<DC_SupplierProductValues>(sqlroommedia.ToString()).ToList();


                                    room.RoomMedia.Add(new DataContracts.StaticData.Media
                                    {
                                        MediaId = MediaDetails.Where(w => w.SystemAttribute == "MEDIAID").Select(s => s.Value).FirstOrDefault(),
                                        Description = MediaDetails.Where(w => w.SystemAttribute == "DESCRIPTION").Select(s => s.Value).FirstOrDefault(),
                                        FileType = "IMAGE",
                                        FileName = MediaDetails.Where(w => w.SystemAttribute == "LARGEIMAGEURL").Select(s => s.Value).FirstOrDefault() ?? MediaDetails.Where(w => w.SystemAttribute == "SMALLIMAGEURL").Select(s => s.Value).FirstOrDefault(),

                                        //New field Added
                                        MediaPosition = MediaDetails.Where(w => w.SystemAttribute == "MEDIAPOSITION").Select(s => s.Value).FirstOrDefault(),
                                        MediaWidth = MediaDetails.Where(w => w.SystemAttribute == "MEDIAWIDTH").Select(s => s.Value).FirstOrDefault(),
                                        MediaHeight = MediaDetails.Where(w => w.SystemAttribute == "MEDIAHEIGHT").Select(s => s.Value).FirstOrDefault(),
                                        LargeImageURL = MediaDetails.Where(w => w.SystemAttribute == "LARGEIMAGEURL").Select(s => s.Value).FirstOrDefault(),
                                        ThumbnailUrl = MediaDetails.Where(w => w.SystemAttribute == "THUMBNAILURL").Select(s => s.Value).FirstOrDefault(),
                                        MediaFileFormat = MediaDetails.Where(w => w.SystemAttribute == "MEDIAFILEFORMAT").Select(s => s.Value).FirstOrDefault(),
                                        SmallImageURL = MediaDetails.Where(w => w.SystemAttribute == "SMALLIMAGEURL").Select(s => s.Value).FirstOrDefault(),
                                        //New field Added


                                    });
                                }
                                //------------------------------RoomMedia

                                newProduct.Rooms.Add(room);

                            }

                            #endregion



                            var filter = Builders<DataContracts.StaticData.Accomodation>.Filter.Eq(c => c.AccomodationInfo.CompanyId, product.SupplierName.ToUpper());
                            filter = filter & Builders<DataContracts.StaticData.Accomodation>.Filter.Eq(c => c.AccomodationInfo.CompanyProductId, product.SupplierProductCode.ToUpper());
                            collection.ReplaceOneAsync(filter, newProduct, new UpdateOptions { IsUpsert = true });

                            if (iCounter % 500 == 0)
                            {
                                UpdateCount(iTotalCount, iCounter, log_id);
                            }
                            iCounter++;
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }

                    UpdateCount(iTotalCount, iCounter, log_id);

                    context.Configuration.AutoDetectChangesEnabled = true;
                    //set the distribution log to running status
                    Log = context.DistributionLayerRefresh_Log.Find(log_id);
                    if (Log != null)
                    {
                        Log.Status = "Completed";
                        Log.Edit_Date = DateTime.Now;
                        Log.Edit_User = "MongoPush";
                        context.SaveChanges();
                    }

                    Log = null;

                    collection = null;
                    _database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    var Log = context.DistributionLayerRefresh_Log.Find(log_id);
                    if (Log != null)
                    {
                        Log.Status = "Error";
                        Log.Edit_Date = DateTime.Now;
                        Log.Edit_User = "MongoPush";
                        context.SaveChanges();
                    }
                    Log = null;
                }

                throw ex;
            }
        }

        public void UpdateAccoStaticDataSingleColumn()
        {
            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    _database = MongoDBHandler.mDatabase();
                    var collection = _database.GetCollection<DataContracts.StaticData.Accomodation>("AccoStaticData");

                    string sql = "";
                    sql = "Select UPPER(S.Name) SupplierName, UPPER(SE.SupplierProductCode) AS SupplierProductCode, SEV.SupplierValue from SupplierEntityValues SEV WITH (NOLOCK) ";
                    sql = sql + "INNER JOIN SupplierEntity SE  WITH (NOLOCK) ON SEV.SupplierEntity_Id = SE.SupplierEntity_Id ";
                    sql = sql + "INNER JOIN Supplier S  WITH (NOLOCK) ON SE.Supplier_Id = S.Supplier_Id ";
                    sql = sql + "INNER JOIN m_MasterAttributeMapping MAM  WITH (NOLOCK) ON SEV.AttributeMap_Id = MAM.MasterAttributeMapping_Id ";
                    sql = sql + "INNER JOIN m_masterattribute MA  WITH (NOLOCK) ON MA.MasterAttribute_Id = MAM.SystemMasterAttribute_Id ";
                    sql = sql + "WHERE MA.MasterFor = 'HotelInfo' and MA.Name = 'HotelName' AND SE.Entity = 'HotelInfo' AND S.Name = 'FITRUMS';";

                    context.Database.CommandTimeout = 0;
                    var HotelInfoDetails = context.Database.SqlQuery<Dc_SupplierEntitySingleValue>(sql.ToString()).ToList();
                    int iTotalCount = HotelInfoDetails.Count();
                    int iCounter = 0;

                    var collectionNew = _database.GetCollection<BsonDocument>("AccoStaticData");
                    var Filter = new BsonDocument("AccomodationInfo.Name", BsonNull.Value);
                    var Project = new BsonDocument("AccomodationInfo.CompanyProductId", 1);
                    var list = collectionNew.Find(Filter).Project(Project).ToList();
                    foreach (var data in list)
                    {
                        var supprodref = data["AccomodationInfo"]["CompanyProductId"].ToString();
                        var HotelName = HotelInfoDetails.Where(w => w.SupplierName == "FITRUMS" && w.SupplierProductCode == supprodref).Select(s => s.SupplierValue).FirstOrDefault();

                        var filter = Builders<DataContracts.StaticData.Accomodation>.Filter.Eq(c => c.AccomodationInfo.CompanyName, "FITRUMS");
                        filter = filter & Builders<DataContracts.StaticData.Accomodation>.Filter.Eq(c => c.AccomodationInfo.CompanyProductId, supprodref);

                        if (HotelName == null)
                        {
                            var removeResult = collection.DeleteOne(filter);
                        }
                        else
                        {
                            var UpdateData = Builders<DataContracts.StaticData.Accomodation>.Update.Set(x => x.AccomodationInfo.DisplayName, HotelName).Set(x => x.AccomodationInfo.Name, HotelName);
                            var updateResult = collection.UpdateMany(filter, UpdateData);
                        }

                        iCounter++;

                    }


                    //foreach (var HotelInfo in HotelInfoDetails)
                    //{
                    //    try
                    //    {
                    //        var filter = Builders<DataContracts.StaticData.Accomodation>.Filter.Eq(c => c.AccomodationInfo.CompanyName, HotelInfo.SupplierName);
                    //        filter = filter & Builders<DataContracts.StaticData.Accomodation>.Filter.Eq(c => c.AccomodationInfo.CompanyProductId, HotelInfo.SupplierProductCode);

                    //        var UpdateData = Builders<DataContracts.StaticData.Accomodation>.Update.Set(x => x.AccomodationInfo.DisplayName, HotelInfo.SupplierValue).Set(x => x.AccomodationInfo.Name, HotelInfo.SupplierValue);
                    //        var updateResult = collection.UpdateManyAsync(filter, UpdateData).Status;

                    //        iCounter++;

                    //    }
                    //    catch (Exception e)
                    //    {
                    //        continue;
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Load Progess Update

        public void UpdateCount(int totalcount, int MongoPushCount, Guid log_id)
        {
            using (TLGX_DEVEntities context = new TLGX_DEVEntities())
            {
                var Log = context.DistributionLayerRefresh_Log.Find(log_id);
                if (Log != null)
                {
                    Log.MongoPushCount = MongoPushCount;
                    Log.TotalCount = totalcount;
                    Log.Edit_Date = DateTime.Now;
                    Log.Edit_User = "MongoPush";
                    context.SaveChanges();
                }
            }
        }

        private void UpdateDistLogInfo(Guid LogId, PushStatus status, int totalCount = 0, int insertedCount = 0, string Supplier_Id = "", string Element = "", string Type = "")
        {
            string Status = string.Empty;
            string EditUser = "MPUSH";
            StringBuilder setNewStatus = new StringBuilder();

            if (status == PushStatus.INSERT)
            {
                Status = "Scheduled";
            }
            else if (status == PushStatus.RUNNNING)
            {
                Status = "Running";
            }
            else if (status == PushStatus.COMPLETED)
            {
                Status = "Completed";
            }
            else if (status == PushStatus.ERROR)
            {
                Status = "Error";
            }

            if (status == PushStatus.INSERT)
            {
                setNewStatus.Append("INSERT INTO DistributionLayerRefresh_Log(Id,Element,Type,Create_Date,Create_User,Status,Supplier_Id) VALUES(");
                setNewStatus.Append("'" + LogId + "','" + Element + "','" + Type + "', GETDATE() " + ", '" + "MONGOPUSH" + "', '" + Status + "', '" + Supplier_Id + "');");
            }
            else
            {
                setNewStatus.Append("UPDATE DistributionLayerRefresh_Log SET TotalCount = " + totalCount.ToString() + " , MongoPushCount = " + insertedCount.ToString() + ", Status ='" + Status + "',  Edit_Date = getDate(),  Edit_User='" + EditUser + "' WHERE Id= '" + LogId + "';");
            }


            using (TLGX_DEVEntities context = new TLGX_DEVEntities())
            {
                context.Configuration.AutoDetectChangesEnabled = false;
                context.Database.ExecuteSqlCommand(setNewStatus.ToString());
            }
            setNewStatus = null;
        }

        #endregion

        #region Room Type Mapping

        public void UpdateHotelRoomTypeMapping(Guid Logid, Guid Supplier_Id)
        {

            bool Is_IX_SupplierCode_SupplierProductCode_SupplierRoomTypeCode_Exists = false;
            bool Is_IX_MapId_Exists = false;

            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>("RoomTypeMapping");

            try
            {
                //Get Supplier ID from log id
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    List<Guid> SupplierIds = new List<Guid>();
                    if (Supplier_Id != Guid.Empty)
                    {
                        SupplierIds.Add(Supplier_Id);
                    }
                    else
                    {
                        SupplierIds = context.Accommodation_SupplierRoomTypeMapping.AsNoTracking().Where(w => w.Supplier_Id != null).Select(s => s.Supplier_Id ?? Guid.Empty).Distinct().ToList();
                    }

                    foreach (var SupplierId in SupplierIds)
                    {
                        int counter = 0;
                        int TotalCount = 0;

                        if (Logid == Guid.Empty)
                        {
                            Logid = Guid.NewGuid();
                            UpdateDistLogInfo(Logid, PushStatus.INSERT, 0, 0, SupplierId.ToString(), "RoomType", "Mapping");
                        }

                        UpdateDistLogInfo(Logid, PushStatus.RUNNNING);

                        List<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest> _objHRTM = new List<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>();

                        _objHRTM = GetDataToPushMongo_RTM(SupplierId);

                        UpdateDistLogInfo(Logid, PushStatus.RUNNNING, _objHRTM.Count(), 0, SupplierId.ToString(), "RoomType", "Mapping");

                        if (_objHRTM.Count > 0)
                        {
                            //For Upsert 
                            foreach (var item in _objHRTM)
                            {
                                var filter = Builders<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>.Filter.Eq(c => c.SystemRoomTypeMapId, item.SystemRoomTypeMapId);
                                collection.ReplaceOne(filter, item, new UpdateOptions { IsUpsert = true });

                                counter++;

                                if (counter % 100 == 0)
                                {
                                    UpdateDistLogInfo(Logid, PushStatus.RUNNNING, TotalCount, counter);
                                }
                            }

                            _objHRTM = null;
                        }
                        UpdateDistLogInfo(Logid, PushStatus.COMPLETED, TotalCount, counter);
                    }
                }

                #region Index Management
                var listOfindexes = collection.Indexes.List().ToList();
                foreach (var index in listOfindexes)
                {
                    Newtonsoft.Json.Linq.JObject rss = Newtonsoft.Json.Linq.JObject.Parse(index.ToJson());
                    if ((string)rss["key"]["supplierCode"] != null && (string)rss["key"]["SupplierProductId"] != null && (string)rss["key"]["SupplierRoomTypeCode"] != null)
                    {
                        Is_IX_SupplierCode_SupplierProductCode_SupplierRoomTypeCode_Exists = true;
                    }

                    if ((string)rss["key"]["SystemRoomTypeMapId"] != null)
                    {
                        Is_IX_MapId_Exists = true;
                    }
                }

                if (!Is_IX_SupplierCode_SupplierProductCode_SupplierRoomTypeCode_Exists)
                {
                    collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>.IndexKeys.Ascending(_ => _.supplierCode).Ascending(_ => _.SupplierProductId).Ascending(_ => _.SupplierRoomTypeCode));
                }

                if (!Is_IX_MapId_Exists)
                {
                    collection.Indexes.CreateOne(Builders<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>.IndexKeys.Ascending(_ => _.SystemRoomTypeMapId));
                }

                #endregion

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private List<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest> GetDataToPushMongo_RTM(Guid Supplier_id)
        {
            List<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest> _objHRTM = new List<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>();
            List<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest_IM> _objHRTM_IM = new List<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest_IM>();
            List<DataContracts.Mapping.DC_RoomTypeMapping_Attributes_HRTM_IM> _objAttributes_IM = new List<DataContracts.Mapping.DC_RoomTypeMapping_Attributes_HRTM_IM>();

            try
            {
                using (TLGX_DEVEntities context = new TLGX_DEVEntities())
                {
                    context.Database.CommandTimeout = 0;
                    context.Configuration.AutoDetectChangesEnabled = false;

                    #region Select Query
                    StringBuilder sbSelect = new StringBuilder();
                    sbSelect.Append(@"select
                                        SRTM.Accommodation_SupplierRoomTypeMapping_Id,
                                        A.TLGXAccoId,               
                                        ARI.TLGXAccoRoomId,
                                        S.Code AS supplierCode,      
                                        SRTM.SupplierProductId,
                                        SRTM.SupplierRoomId,
                                        SRTM.SupplierRoomTypeCode,   
                                        SRTM.SupplierRoomName, 
                                        SRTM.SupplierRoomCategory,   
                                        SRTM.SupplierRoomCategoryId, 
                                        SRTM.MaxAdults,              
                                        SRTM.MaxInfants, 
                                        SRTM.MaxGuestOccupancy,      
                                        SRTM.Quantity, 
                                        SRTM.RatePlan,               
                                        SRTM.RatePlanCode, 
                                        SRTM.RoomSize,               
                                        SRTM.BathRoomType, 
                                        SRTM.roomviewcode AS RoomView,    
                                        SRTM.FloorName, 
                                        SRTM.FloorNumber,            
                                        SRTM.Amenities, 
                                        SRTM.RoomLocationCode,       
                                        SRTM.ChildAge, 
                                        SRTM.ExtraBed,               
                                        SRTM.Bedrooms, 
                                        SRTM.Smoking,                
                                        SRTM.BedTypeCode AS BedType, 
                                        SRTM.MinGuestOccupancy,      
                                        SRTM.PromotionalVendorCode, 
                                        SRTM.BeddingConfig,          
                                        SRTMV.MapId AS SystemRoomTypeMapId,
                                        SRTM.RoomDescription,
                                        A.CompanyHotelID AS SystemProductCode,
                                        ARI.RoomId AS SystemRoomTypeCode, 
                                        ARI.RoomName AS SystemRoomTypeName,
                                        ARI.RoomCategory AS SystemRoomCategory,
                                        UPPER(SRTM.TX_RoomName) AS SystemNormalizedRoomType,
                                        UPPER(SRTM.Tx_StrippedName) AS SystemStrippedRoomType,
                                        'MAPPED' As [Status],
                                        SRTMV.MatchingScore
                                        From Accommodation_SupplierRoomTypeMapping SRTM with (nolock) 
                                        inner join Accommodation_SupplierRoomTypeMapping_Values SRTMV with (nolock) ON SRTM.Accommodation_SupplierRoomTypeMapping_Id = SRTMV.Accommodation_SupplierRoomTypeMapping_Id
                                        inner Join Accommodation_RoomInfo ARI with (nolock)  On ARI.Accommodation_RoomInfo_Id = SRTMV.Accommodation_RoomInfo_Id
                                        inner join Supplier S WITH (NOLOCK) ON SRTM.Supplier_Id = S.Supplier_Id
                                        inner join Accommodation A WITH (NOLOCK) ON A.Accommodation_Id = SRTM.Accommodation_Id 
                                        Where SRTMV.UserMappingStatus = 'MAPPED' ");

                    StringBuilder sbWhere = new StringBuilder();
                    sbWhere.Append(" AND SRTM.Supplier_Id = '" + Convert.ToString(Supplier_id) + "'");

                    StringBuilder sbfinal = new StringBuilder();

                    sbfinal.Append(sbSelect);
                    sbfinal.Append(sbWhere);

                    #endregion

                    context.Configuration.AutoDetectChangesEnabled = false;
                    try { _objHRTM_IM = context.Database.SqlQuery<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest_IM>(sbfinal.ToString()).ToList(); } catch (Exception ex) { }

                    StringBuilder sbRoomTypeAttributefinalQuery = new StringBuilder();
                    sbRoomTypeAttributefinalQuery.Append(@"SELECT RoomTypeMap_Id, SupplierRoomTypeAttribute AS [Value],SystemAttributeKeyword As [Key]
                    from Accommodation_SupplierRoomTypeAttributes WITH (NOLOCK) Where RoomTypeMap_Id IN
                    (Select SRTMV.Accommodation_SupplierRoomTypeMapping_Id From Accommodation_SupplierRoomTypeMapping SRTM with (nolock) 
                    inner join Accommodation_SupplierRoomTypeMapping_Values SRTMV with (nolock) 
                    ON SRTM.Accommodation_SupplierRoomTypeMapping_Id = SRTMV.Accommodation_SupplierRoomTypeMapping_Id
                     WHERE SRTMV.UserMappingStatus = 'MAPPED' and SRTM.Supplier_Id = '" + Supplier_id.ToString() + "')");

                    try { _objAttributes_IM = context.Database.SqlQuery<DataContracts.Mapping.DC_RoomTypeMapping_Attributes_HRTM_IM>(sbRoomTypeAttributefinalQuery.ToString()).ToList(); } catch (Exception ex) { }

                    foreach (var item in _objHRTM_IM)
                    {
                        _objHRTM.Add(new DataContracts.Mapping.DC_HotelRoomTypeMappingRequest
                        {
                            TLGXAccoId = item.TLGXAccoId?.ToUpper(),
                            Accommodation_SupplierRoomTypeMapping_Id = item.Accommodation_SupplierRoomTypeMapping_Id,
                            Amenities = item.Amenities?.ToUpper(),
                            Attibutes = _objAttributes_IM.Where(w => w.RoomTypeMap_Id == item.Accommodation_SupplierRoomTypeMapping_Id).Select(s => new DataContracts.Mapping.DC_RoomTypeMapping_Attributes_HRTM
                            {
                                Key = s.Key?.ToUpper(),
                                Value = s.Value?.ToUpper(),
                                RoomTypeMap_Id = s.RoomTypeMap_Id
                            }).ToList(),

                            BathRoomType = item.BathRoomType?.ToUpper(),
                            BeddingConfig = item.BeddingConfig?.ToUpper(),
                            Bedrooms = item.Bedrooms?.ToUpper(),
                            BedType = item.BedType?.ToUpper(),
                            ChildAge = item.ChildAge,
                            ExtraBed = item.ExtraBed?.ToUpper(),
                            FloorName = item.FloorName?.ToUpper(),
                            FloorNumber = item.FloorNumber,
                            MatchingScore = item.MatchingScore,
                            MaxAdults = item.MaxAdults,
                            MaxChild = item.MaxChild,
                            MaxGuestOccupancy = item.MaxGuestOccupancy,
                            MaxInfants = item.MaxInfants,
                            MinGuestOccupancy = item.MinGuestOccupancy,
                            PromotionalVendorCode = item.PromotionalVendorCode?.ToUpper(),
                            Quantity = item.Quantity,
                            RatePlan = item.RatePlan?.ToUpper(),
                            RatePlanCode = item.RatePlanCode?.ToUpper(),
                            RoomDescription = item.RoomDescription?.ToUpper(),
                            RoomLocationCode = item.RoomLocationCode?.ToUpper(),
                            RoomSize = item.RoomSize?.ToUpper(),
                            RoomView = item.RoomView?.ToUpper(),
                            Smoking = item.Smoking?.ToUpper(),
                            Status = item.Status?.ToUpper(),
                            supplierCode = item.supplierCode?.ToUpper(),
                            SupplierProductId = item.SupplierProductId?.ToUpper(),
                            SupplierRoomCategory = item.SupplierRoomCategory?.ToUpper(),
                            SupplierRoomCategoryId = item.SupplierRoomCategoryId?.ToUpper(),
                            SupplierRoomId = item.SupplierRoomId?.ToUpper(),
                            SupplierRoomName = item.SupplierRoomName?.ToUpper(),
                            SupplierRoomTypeCode = item.SupplierRoomTypeCode?.ToUpper(),
                            SystemNormalizedRoomType = item.SystemNormalizedRoomType?.ToUpper(),
                            SystemProductCode = item.SystemProductCode,
                            SystemRoomTypeCode = item.SystemRoomTypeCode?.ToUpper(),
                            SystemRoomTypeMapId = item.SystemRoomTypeMapId,
                            SystemRoomTypeName = item.SystemRoomTypeName?.ToUpper(),
                            SystemRoomCategory = item.SystemRoomCategory?.ToUpper(),
                            SystemStrippedRoomType = item.SystemStrippedRoomType?.ToUpper(),
                            TLGXAccoRoomId = item.TLGXAccoRoomId?.ToUpper()
                        });
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }

            return _objHRTM;
        }

        #endregion

        //Custom Converting for 1,0 to boolean value
        private object CustomConvert(object value, Type targetType)
        {
            decimal numericValue;
            if ((targetType == typeof(bool) || targetType == typeof(bool?)) &&
                value is string &&
                decimal.TryParse((string)value, out numericValue))
            {
                return numericValue != 0;
            }
            var valueType = value.GetType();
            var c1 = TypeDescriptor.GetConverter(valueType);
            if (c1.CanConvertTo(targetType)) // this returns false for string->bool
            {
                return c1.ConvertTo(value, targetType);
            }
            var c2 = TypeDescriptor.GetConverter(targetType);
            if (c2.CanConvertFrom(valueType)) // this returns true for string->bool, but will throw for "1"
            {
                return c2.ConvertFrom(value);
            }
            return Convert.ChangeType(value, targetType); // this will throw for "1"
        }

    }
}
