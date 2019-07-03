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
using System.Globalization;
using System.Data;
using DataContracts.Masters;
using DataContracts.Visa;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using DataContracts.Mapping;
using DataContracts.Holiday;
using System.Threading;
using System.Collections;

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
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    _database = MongoDBHandler.mDatabase();
                    _database.DropCollection("CountryMaster");

                    var collection = _database.GetCollection<BsonDocument>("CountryMaster");

                    var CountryList = (from c in context.m_CountryMaster
                                       orderby c.Name
                                       select new
                                       {
                                           ContinentName = c.TlgxContinentName.Trim().ToUpper(),
                                           ContinentCode = c.TlgxContinentCode.Trim().ToUpper(),
                                           Name = c.Name.Trim().ToUpper(),
                                           Code = c.Code.Trim().ToUpper()
                                       }).ToList();

                    List<BsonDocument> docs = new List<BsonDocument>();
                    foreach (var country in CountryList)
                    {
                        var document = new BsonDocument
                        {
                            { "ContinentName", country.ContinentName ?? string.Empty },
                            { "ContinentCode", country.ContinentCode ?? string.Empty },
                            { "CountryName", country.Name },
                            { "CountryCode", country.Code }
                        };
                        docs.Add(document);
                        document = null;
                    }
                    collection.InsertMany(docs);
                    docs = null;
                    collection = null;
                    //_database = null;

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
                using (TLGX_Entities context = new TLGX_Entities())
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
                    //_database = null;

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
                using (TLGX_Entities context = new TLGX_Entities())
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
                    //_database = null;

                    var Log = context.DistributionLayerRefresh_Log.Find(LogId);
                    if (Log != null)
                    {
                        Log.Status = "Completed";
                        context.SaveChanges();
                    }
                }
                // Passing MCON Supplier Method below
                LoadMCONBySupplier(Guid.Empty, Guid.Empty);

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
                using (TLGX_Entities context = new TLGX_Entities())
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
                    //_database = null;
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
                using (TLGX_Entities context = new TLGX_Entities())
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
                    //_database = null;
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
                using (TLGX_Entities context = new TLGX_Entities())
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
                    //_database = null;

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
                using (TLGX_Entities context = new TLGX_Entities())
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
                        //_database = null;
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

                    using (TLGX_Entities context = new TLGX_Entities())
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
                            /* Need write CompanyVersion Function */
                            List<DC_AccomodationCompanyVersions> lstCompanyVersion = GetAccommodationCompanyVersion(acco.Accommodation_Id);
                            lstCompanyVersion.ForEach(x =>
                            {
                                RemoveDiacritics(x.CommonProductId);
                                RemoveDiacritics(x.CompanyProductId);
                                RemoveDiacritics(x.CompanyId);
                                RemoveDiacritics(x.CompanyName);
                                RemoveDiacritics(x.ProductName);
                                RemoveDiacritics(x.ProductDisplayName);
                                RemoveDiacritics(x.StarRating);
                                RemoveDiacritics(x.CompanyRating);
                                RemoveDiacritics(x.ProductCatSubType);
                                RemoveDiacritics(x.Brand);
                                RemoveDiacritics(x.Chain);
                                RemoveDiacritics(x.HouseNumber);
                                RemoveDiacritics(x.Street);
                                RemoveDiacritics(x.Street2);
                                RemoveDiacritics(x.Street3);
                                RemoveDiacritics(x.Street4);
                                RemoveDiacritics(x.Street5);
                                RemoveDiacritics(x.Zone);
                                RemoveDiacritics(x.PostalCode);
                                RemoveDiacritics(x.Country);
                                RemoveDiacritics(x.State);
                                RemoveDiacritics(x.City);
                                RemoveDiacritics(x.Area);
                                RemoveDiacritics(x.Location);
                                RemoveDiacritics(x.Latitude);
                                RemoveDiacritics(x.Longitude);
                                RemoveDiacritics(x.TLGXAccoId);
                                x.Interests = x.Interest != null ? RemoveDiacritics(x.Interest).Split(',').ToList() : new List<string>();
                            }
                            );

                            DataContracts.Masters.DC_Accomodation Accodata = new DataContracts.Masters.DC_Accomodation()
                            {
                                HotelName = RemoveDiacritics(acco.HotelName),
                                CountryCode = RemoveDiacritics(acco.CountryCode),
                                CountryName = RemoveDiacritics(acco.CountryName),
                                CityCode = RemoveDiacritics(acco.CityCode),
                                StateCode = RemoveDiacritics(acco.StateCode),
                                StateName = RemoveDiacritics(acco.StateName),
                                CityName = RemoveDiacritics(acco.CityName),
                                StreetName = RemoveDiacritics(acco.StreetName),
                                StreetNumber = RemoveDiacritics(acco.StreetNumber),
                                Street3 = RemoveDiacritics(acco.Street3),
                                Street4 = RemoveDiacritics(acco.Street4),
                                Street5 = RemoveDiacritics(acco.Street5),
                                PostalCode = RemoveDiacritics(acco.PostalCode),
                                Town = RemoveDiacritics(acco.Town),
                                Location = RemoveDiacritics(acco.Location),
                                Area = RemoveDiacritics(acco.Area),
                                TLGXAccoId = RemoveDiacritics(acco.TLGXAccoId),
                                ProductCategory = RemoveDiacritics(acco.ProductCategory),
                                ProductCategorySubType = RemoveDiacritics(acco.ProductCategorySubType),
                                IsRoomMappingCompleted = acco.IsRoomMappingCompleted,
                                CommonHotelId = acco.CommonHotelId,
                                Brand = RemoveDiacritics(acco.Brand),
                                Chain = RemoveDiacritics(acco.Chain),
                                Latitude = RemoveDiacritics(acco.Latitude),
                                Longitude = RemoveDiacritics(acco.Longitude),
                                FullAddress = RemoveDiacritics(acco.FullAddress),
                                HotelStarRating = RemoveDiacritics(acco.HotelStarRating),
                                Email = RemoveDiacritics(acco.Email),
                                Fax = RemoveDiacritics(acco.Fax),
                                WebSiteURL = RemoveDiacritics(acco.WebSiteURL),
                                Telephone = RemoveDiacritics(acco.Telephone),
                                CodeStatus = RemoveDiacritics(acco.CodeStatus),
                                Interests = acco.Interest != null ? RemoveDiacritics(acco.Interest).Split(',').ToList() : new List<string>(),
                                AccomodationCompanyVersions = lstCompanyVersion
                            };


                            var filter = Builders<DataContracts.Masters.DC_Accomodation>.Filter.Eq(c => c.CommonHotelId, acco.CommonHotelId);
                            collection.ReplaceOneAsync(filter, Accodata, new UpdateOptions { IsUpsert = true });
                        }

                        if (Accommodation_Id == Guid.Empty && LogId != Guid.Empty)
                        {
                            Counter = (BatchNo * BatchSize) + _AccoList.Count;
                            UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalCount, Counter, Guid.Empty.ToString(), "ACCOMMODATION", "MASTER");
                        }
                    }
                }

                //Local Function To Get Master Accommodation Data
                #region Get Master Accommodation Data Local Function
                List<DataContracts.Masters.DC_Accomodation> GetAccommodationMaster_InBatches(int batchSize, int batchNo, Guid gAccommodation_Id)
                {
                    List<DataContracts.Masters.DC_Accomodation> _AccoListResultMain = new List<DataContracts.Masters.DC_Accomodation>();
                    try
                    {
                        int skip = batchNo * batchSize;

                        #region AccoMasterQuery
                        StringBuilder sbSelectAccoMaster = new StringBuilder();

                        sbSelectAccoMaster.Append(@"  
                                                select  ACC.Accommodation_Id  as Accommodation_Id, ACC.CompanyHotelID as CommonHotelId,ACC.HotelName ,MCM.Code CountryCode,MCM.Name CountryName,CM.Code CityCode,MST.StateCode, MST.StateName,
                                                Cm.Name CityName,ACC.StreetName ,ACC.StreetNumber,ACC.Street3 ,ACC.Street4 ,ACC.Street5 ,ACC.PostalCode ,ACC.Town,
                                                ACC.Location ,ACC.Area,ACC.TLGXAccoId ,ACC.ProductCategory ,ACC.ProductCategorySubType ,isnull(ACC.IsRoomMappingCompleted,0)  as IsRoomMappingCompleted ,
                                                ACC.HotelRating,ACC.CompanyRating,ACC.CompanyRecommended,ACC.RecommendedFor,ACC.Brand,ACC.Chain,ACC.Latitude,ACC.Longitude,ACC.FullAddress, ACC.HotelRating as HotelStarRating,
                                                ACC.Brand,ACC.Chain,Cont.Email,Cont.Fax,Cont.WebSiteURL,Cont.Telephone ,(case when ACC.IsActive = 1 then  'Active' when ACC.IsActive = 0 then  'Inactive' else '' end) as CodeStatus , 
                                                ACC.SuburbDowntown, ACC.Interest
                                                from Accommodation ACC with(nolock) Left Join m_CityMaster CM with(nolock)  on Cm.City_Id = ACC.City_Id and CM.Country_Id = Acc.Country_Id
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

                        using (TLGX_Entities context = new TLGX_Entities())
                        {
                            context.Database.CommandTimeout = 0;
                            _AccoListResultMain = context.Database.SqlQuery<DataContracts.Masters.DC_Accomodation>(sbSelectAccoMaster.ToString()).ToList();
                        }
                    }
                    catch (Exception ex) { }
                    return _AccoListResultMain;
                }
                #endregion



                //Location Function to get Accommodation Master Version Data
                #region Get Accommodation Master Version Data

                List<DataContracts.Masters.DC_AccomodationCompanyVersions> GetAccommodationCompanyVersion(Guid gAccommodation_Id)
                {
                    List<DataContracts.Masters.DC_AccomodationCompanyVersions> _AccoListResultMain = new List<DataContracts.Masters.DC_AccomodationCompanyVersions>();
                    try
                    {


                        #region AccoMasterQuery
                        StringBuilder sbSelectAccoMaster = new StringBuilder();

                        sbSelectAccoMaster.Append(@"  
                                                Select CommonProductId,CompanyProductId,CompanyId,CompanyName,ProductName,ProductDisplayName,StarRating,CompanyRating,ProductCatSubType,
                                                Brand,Chain,HouseNumber,Street,Street2,Street3,Street4,Street5,Zone,PostalCode,Country,
                                                State,City,Area,Location,Latitude,Longitude,TLGXAccoId,Interest from Accommodation_CompanyVersion with(nolock)  ");
                        sbSelectAccoMaster.AppendLine(" WHERE Accommodation_Id = '" + gAccommodation_Id + "';");


                        #endregion

                        using (TLGX_Entities context = new TLGX_Entities())
                        {
                            context.Database.CommandTimeout = 0;
                            _AccoListResultMain = context.Database.SqlQuery<DataContracts.Masters.DC_AccomodationCompanyVersions>(sbSelectAccoMaster.ToString()).ToList();
                        }
                    }
                    catch (Exception ex) { }
                    return _AccoListResultMain;
                }
                #endregion

                if (Accommodation_Id == Guid.Empty && LogId != Guid.Empty)
                {
                    UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalCount, Counter, Guid.Empty.ToString(), "ACCOMMODATION", "MASTER");
                }

                collection = null;
                //_database = null;
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalCount, Counter, Guid.Empty.ToString(), "ACCOMMODATION", "MASTER");
                throw ex;
            }
        }
        public void LoadMasterAccommodationRoomInfo(Guid LogId, Guid Accommodation_Id)
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
                List<DataContracts.Masters.DC_AccomodationRoomInfo> _AccoRoomList = new List<DataContracts.Masters.DC_AccomodationRoomInfo>();

                if (Accommodation_Id == Guid.Empty && LogId != Guid.Empty)
                {
                    UpdateDistLogInfo(LogId, PushStatus.INSERT, 0, 0, Guid.Empty.ToString(), "ACCOMMODATIONROOMINFO", "MASTER");
                    UpdateDistLogInfo(LogId, PushStatus.RUNNNING, 0, Counter, Guid.Empty.ToString(), "ACCOMMODATIONROOMINFO", "MASTER");

                    using (TLGX_Entities context = new TLGX_Entities())
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

                    UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalCount, Counter, Guid.Empty.ToString(), "ACCOMMODATIONROOMINFO", "MASTER");
                }

                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Masters.DC_AccomodationRoomInfo>("AccommodationRoomInfoMaster");

                for (int BatchNo = 0; BatchNo < NoOfBatch; BatchNo++)
                {
                    _AccoRoomList = GetAccommodationRoomInfoMaster_InBatches(BatchSize, BatchNo, Accommodation_Id);
                    if (_AccoRoomList.Count > 0)
                    {
                        foreach (var accoRoom in _AccoRoomList)
                        {
                            /* Need write CompanyVersion Function */
                            List<DC_AccomodationRoomInfoCompanyVersion> lstRoomInfoCompanyVersion = GetAccommodationRoomInfoCompanyVersion(accoRoom.Accommodation_RoomInfo_Id);
                            lstRoomInfoCompanyVersion.ForEach(x =>
                            {
                                RemoveDiacritics(x.RoomView);
                                RemoveDiacritics(x.RoomName);
                                RemoveDiacritics(x.CompanyId);
                                RemoveDiacritics(x.CompanyName);
                                RemoveDiacritics(x.RoomCategory);
                                RemoveDiacritics(x.CompanyRoomCategory);
                                RemoveDiacritics(x.RoomDescription);
                                RemoveDiacritics(x.TLGXAccoId);
                                RemoveDiacritics(x.TLGXAccoRoomID);
                            }
                            );

                            DataContracts.Masters.DC_AccomodationRoomInfo AccoRoomdata = new DataContracts.Masters.DC_AccomodationRoomInfo()
                            {
                                CommonRoomId = accoRoom.CommonRoomId,
                                CommonHotelId = (accoRoom.CommonHotelId.HasValue ? accoRoom.CommonHotelId.Value : accoRoom.CommonHotelId),
                                RoomView = RemoveDiacritics(accoRoom.RoomView),
                                NoOfRooms = (accoRoom.NoOfRooms.HasValue ? accoRoom.NoOfRooms.Value : accoRoom.NoOfRooms),
                                RoomName = RemoveDiacritics(accoRoom.RoomName),
                                Smoking = RemoveDiacritics(accoRoom.Smoking),
                                BathRoomType = RemoveDiacritics(accoRoom.BathRoomType),
                                BedType = RemoveDiacritics(accoRoom.BedType),
                                CompanyRoomCategory = RemoveDiacritics(accoRoom.CompanyRoomCategory),
                                RoomCategory = RemoveDiacritics(accoRoom.RoomCategory),
                                AccomodationRoomInfoCompanyVersions = lstRoomInfoCompanyVersion
                            };


                            var filter = Builders<DataContracts.Masters.DC_AccomodationRoomInfo>.Filter.Eq(c => c.CommonRoomId, accoRoom.CommonRoomId);
                            collection.ReplaceOneAsync(filter, AccoRoomdata, new UpdateOptions { IsUpsert = true });
                        }

                        if (Accommodation_Id == Guid.Empty && LogId != Guid.Empty)
                        {
                            Counter = (BatchNo * BatchSize) + _AccoRoomList.Count;
                            UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalCount, Counter, Guid.Empty.ToString(), "ACCOMMODATIONROOMINFO", "MASTER");
                        }
                    }
                }

                //Local Function To Get Master Accommodation Data
                #region Get Master Accommodation RoomInfo Data Local Function
                List<DataContracts.Masters.DC_AccomodationRoomInfo> GetAccommodationRoomInfoMaster_InBatches(int batchSize, int batchNo, Guid gAccommodation_Id)
                {
                    List<DataContracts.Masters.DC_AccomodationRoomInfo> _AccoListResultMain = new List<DataContracts.Masters.DC_AccomodationRoomInfo>();
                    try
                    {
                        int skip = batchNo * batchSize;

                        #region AccoRoomInfoMasterQuery
                        StringBuilder sbSelectAccoRoomInfoMaster = new StringBuilder();

                        sbSelectAccoRoomInfoMaster.Append(@"  
                                              select ARI.Accommodation_RoomInfo_Id, ACC.CompanyHotelID as CommonHotelId,ARI .CommonRoomId,ARI.RoomView,ARI.NoOfRooms,ARI.RoomName,ARI.Smoking,ARI.BathRoomType,ARI.BedType,ARI.CompanyRoomCategory,ARI.RoomCategory,ARI.Category from Accommodation_RoomInfo ARI with(nolock) 
                                               join Accommodation ACC on ARI.Accommodation_Id = ACC.Accommodation_Id  ");

                        if (gAccommodation_Id == Guid.Empty)
                        {
                            sbSelectAccoRoomInfoMaster.AppendLine(" ORDER BY ARI.Legacy_Htl_Id  OFFSET " + (skip).ToString() + " ROWS FETCH NEXT " + batchSize.ToString() + " ROWS ONLY;");
                        }
                        else
                        {
                            sbSelectAccoRoomInfoMaster.AppendLine(" WHERE ACC.Accommodation_Id = '" + gAccommodation_Id + "';");
                        }

                        #endregion

                        using (TLGX_Entities context = new TLGX_Entities())
                        {
                            context.Database.CommandTimeout = 0;
                            _AccoListResultMain = context.Database.SqlQuery<DataContracts.Masters.DC_AccomodationRoomInfo>(sbSelectAccoRoomInfoMaster.ToString()).ToList();
                        }
                    }
                    catch (Exception ex) { }
                    return _AccoListResultMain;
                }
                #endregion



                //Location Function to get Accommodation Master Version Data
                #region Get Accommodation Master Room Info  Version Data

                List<DataContracts.Masters.DC_AccomodationRoomInfoCompanyVersion> GetAccommodationRoomInfoCompanyVersion(Guid Accommodation_RoomInfo_Id)
                {
                    List<DataContracts.Masters.DC_AccomodationRoomInfoCompanyVersion> _AccoRoomInfoVersionListResultMain = new List<DataContracts.Masters.DC_AccomodationRoomInfoCompanyVersion>();
                    try
                    {


                        #region AccoRoomInfoVersionMasterQuery
                        StringBuilder sbSelectAccoRoomVersionMaster = new StringBuilder();

                        sbSelectAccoRoomVersionMaster.Append(@"  
                                                SELECT ARICV.RoomCategory,ARICV.RoomName,ARICV.CompanyRoomCategory,ARICV.RoomDescription,ARICV.RoomView,ARICV.BedType,ARICV.Smoking,ARICV.TlgxAccoId,ARICV.TlgxAccoRoomId,ACV.CompanyId, ACV.CompanyName
                                                from Accommodation_RoomInfo_CompanyVersion ARICV  with(nolock) Join 
                                                Accommodation_CompanyVersion ACV with(nolock) on ARICV.Accommodation_CompanyVersion_Id = ACV.Accommodation_CompanyVersion_Id ");
                        sbSelectAccoRoomVersionMaster.AppendLine(" where ARICV.Accommodation_RoomInfo_Id = '" + Accommodation_RoomInfo_Id + "';");


                        #endregion

                        using (TLGX_Entities context = new TLGX_Entities())
                        {
                            context.Database.CommandTimeout = 0;
                            _AccoRoomInfoVersionListResultMain = context.Database.SqlQuery<DataContracts.Masters.DC_AccomodationRoomInfoCompanyVersion>(sbSelectAccoRoomVersionMaster.ToString()).ToList();
                        }
                    }
                    catch (Exception ex) { }
                    return _AccoRoomInfoVersionListResultMain;
                }
                #endregion

                if (Accommodation_Id == Guid.Empty && LogId != Guid.Empty)
                {
                    UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalCount, Counter, Guid.Empty.ToString(), "ACCOMMODATIONROOMINFO", "MASTER");
                }

                collection = null;
                //_database = null;
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalCount, Counter, Guid.Empty.ToString(), "ACCOMMODATIONROOMINFO", "MASTER");
                throw ex;
            }
        }



        public static String RemoveDiacritics(string s)
        {
            string data = null;
            if (s != null)
            {
                data = new string(CommonFunctions.RemoveDiacritics(s).RemoveLineEndings().Where(c => !char.IsControl(c)).ToArray());
            }

            return data;
        }
        #endregion

        #region ZoneMaster
        public void LoadZoneMaster(Guid LogId)
        {
            int TotalZoneCount = 0;
            int MongoInsertedCount = 0;
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
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
                    //_database = null;
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
                                            zm.Latitude, zm.Longitude,upper(ltrim(rtrim(co.Code))) as TLGXCountryCode,zm.zone_code,zm.Zone_House_Number,
                                            zm.Zone_Street_One,zm.Zone_Street_Two,zm.Zone_Street_Three,zm.Zone_City,zm.Zone_City_Area,zm.Zone_City_Area_Location,
                                            zm.Zone_Postal_Code,zm.Zone_Full_Adress
                                            FROM  m_zoneMaster zm  with(Nolock)
                                            LEFT JOIN m_CountryMaster co  with(Nolock) ON co.Country_Id= zm.Country_Id 
                                            WHERE zm.isActive=1 and zm.Latitude is not null and zm.Longitude is not null ");
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

                #region ==== ZoneGeography SQL QUERY
                StringBuilder sbSelectZoneGeography = new StringBuilder();
                sbSelectZoneGeography.Append(@" SELECT zc.Zone_id, upper(ltrim(rtrim(cm.code))) as TLGXCityCode,upper(ltrim(rtrim(cm.CountryCode))) as TLGXCountryCode,upper(ltrim(rtrim(cm.StateCode))) as TLGXStateCode,
                                                upper(ltrim(rtrim(zm.Zone_City_Area))) as TLGXCityAreaCode,upper(ltrim(rtrim(zm.Zone_City_Area_Location))) as TLGXCityAreaLocationCode
                                                FROM  ZoneCity_Mapping zc with(NOLOCK) 
                                                JOIN m_citymaster cm with(NOLOCK)  on zc.city_id = cm.city_id
											    JOIN m_ZoneMaster zm with(NOLOCK) on zm.Zone_id=zc.Zone_Id ");

                StringBuilder sbfinalZoneGeography = new StringBuilder();
                sbfinalZoneGeography.Append(sbSelectZoneGeography + " ");
                sbfinalZoneGeography.Append(" WHERE zc.zone_id  in ( ");

                #endregion

                #region ==== ZoneLoc SQL QUERY
                StringBuilder sbSelectZoneGeographyCoordinates = new StringBuilder();
                sbSelectZoneGeographyCoordinates.Append(@" SELECT Zone_id,Longitude ,Latitude FROM m_ZoneMaster with(NOLOCK) ");

                StringBuilder sbfinalZoneCoordinates = new StringBuilder();
                sbfinalZoneCoordinates.Append(sbSelectZoneGeographyCoordinates + " ");
                sbfinalZoneCoordinates.Append(" WHERE zone_id  in ( ");

                #endregion

                #region ==== ZoneLocation mapping SQL QUERY
                StringBuilder sbSelectZoneLocationMapping = new StringBuilder();
                sbSelectZoneLocationMapping.Append(@" SELECT ZS.Name,ZS.Code,ZS.ZoneType,ZS.ZoneSubType,HouseNumber,
                                                       ZS.StreetName,ZS.Street2,ZS.Street3,ZS.City,ZS.CityArea,ZS.CityAreaLocation,
                                                       ZS.StateCode,ZS.StateName,ZS.CountryCode,ZS.PostalCode,ZS.FullAdress,ZS.Latitude,
                                                       ZS.Longitude,ZSM.Distance,ZS.Supplier_Name,SUP.Code as Supplier_code,ZM.Zone_id from Zone_SupplierMapping ZSM with(NOLOCK) 
	                                                   INNER JOIN Zone_Supplier ZS with(NOLOCK) ON ZSM.ZoneSupplier_Id=ZS.ZoneSupplier_Id
	                                                   INNER JOIN m_ZoneMaster ZM with(NOLOCK) ON ZSM.ZoneId=ZM.Zone_id 
                                                       INNER JOIN Supplier SUP with(NOLOCK) ON SUP.Name=ZS.Supplier_Name");

                StringBuilder sbfinalZoneLocationMapping = new StringBuilder();
                sbfinalZoneLocationMapping.Append(sbSelectZoneLocationMapping + " ");
                sbfinalZoneLocationMapping.Append(" WHERE zm.zone_id  in ( ");

                #endregion


                List<DataContracts.Masters.DC_Zone_ProductMappingRQ> _ZoneProdListResult = new List<DataContracts.Masters.DC_Zone_ProductMappingRQ>();

                List<DataContracts.Masters.DC_Zone_CityMappingRQ> _ZoneCityListResult = new List<DataContracts.Masters.DC_Zone_CityMappingRQ>();

                List<DataContracts.Masters.DC_Zone_GeographyRQ> _ZoneGeographyListResult = new List<DataContracts.Masters.DC_Zone_GeographyRQ>();

                List<DC_Zone_CoordinateRQ> _Zone_GeometryCoordinatesListResult = new List<DC_Zone_CoordinateRQ>();

                List<DC_Zone_LocationMappingRQ> _Zone_LocationMappingRQListResult = new List<DC_Zone_LocationMappingRQ>();

                StringBuilder sbZone_id = new StringBuilder();

                using (TLGX_Entities context = new TLGX_Entities())
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

                        //To Get Zone Geography by Zone id
                        sbfinalZoneGeography.Append(sbZone_id.ToString().TrimEnd(',') + ")");
                        _ZoneGeographyListResult = context.Database.SqlQuery<DataContracts.Masters.DC_Zone_GeographyRQ>(sbfinalZoneGeography.ToString()).ToList();

                        //To Get Zone Geography Coordinates by Zone id
                        sbfinalZoneCoordinates.Append(sbZone_id.ToString().TrimEnd(',') + ")");
                        _Zone_GeometryCoordinatesListResult = context.Database.SqlQuery<DataContracts.Masters.DC_Zone_CoordinateRQ>(sbfinalZoneCoordinates.ToString()).ToList();

                        //To Get Zone Geography Coordinates by Zone id
                        sbfinalZoneLocationMapping.Append(sbZone_id.ToString().TrimEnd(',') + ")");
                        _Zone_LocationMappingRQListResult = context.Database.SqlQuery<DataContracts.Masters.DC_Zone_LocationMappingRQ>(sbfinalZoneLocationMapping.ToString()).ToList();



                        foreach (var item in _ZoneListResult)
                        {
                            item.Zone_ProductMapping = new List<DataContracts.Masters.DC_Zone_ProductMappingRQ>();
                            item.Zone_ProductMapping = _ZoneProdListResult.Where(w => w.Zone_id == item.Zone_id).ToList();
                            item.Zone_CityMapping = new List<DataContracts.Masters.DC_Zone_CityMappingRQ>();
                            item.Zone_CityMapping = _ZoneCityListResult.Where(w => w.Zone_id == item.Zone_id).ToList();
                            item.Zone_GeographyMapping = new List<DC_Zone_GeographyRQ>();
                            item.Zone_GeographyMapping = _ZoneGeographyListResult.Where(w => w.Zone_id == item.Zone_id).ToList();
                            List<DC_Zone_CoordinateRQ> _CoordinateRQs = _Zone_GeometryCoordinatesListResult.Where(w => w.Zone_id == item.Zone_id).ToList();
                            DC_Zone_GeometryRQ dC_Zone_GeometryRQ = new DC_Zone_GeometryRQ();
                            dC_Zone_GeometryRQ.coordinates = _CoordinateRQs.FirstOrDefault();
                            dC_Zone_GeometryRQ.type = "Point";
                            item.geometry = new List<DC_Zone_GeometryRQ>();
                            item.geometry.Add(dC_Zone_GeometryRQ);
                            item.ZoneLocationMapping = new List<DC_Zone_LocationMappingRQ>();
                            item.ZoneLocationMapping = _Zone_LocationMappingRQListResult.Where(w => w.Zone_id == item.Zone_id).ToList();
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
                    Zone_Code = item.Zone_Code,
                    Zone_House_Number = item.Zone_House_Number,
                    Zone_Street_One = item.Zone_Street_One,
                    Zone_Street_Two = item.Zone_Street_Two,
                    Zone_Street_Three = item.Zone_Street_Three,
                    Zone_City = item.Zone_City,
                    Zone_City_Area = item.Zone_City_Area,
                    Zone_City_Area_Location = item.Zone_City_Area_Location,
                    Zone_Postal_Code = item.Zone_Postal_Code,
                    Zone_Full_Adress = item.Zone_Full_Adress,
                    loc = item.geometry.ConvertAll(xGeometry => new DataContracts.Masters.DC_Zone_Geometry
                    {
                        type = xGeometry.type,
                        coordinates = new List<double> { xGeometry.coordinates.Longitude != null ? Convert.ToDouble(xGeometry.coordinates.Longitude) : 0.00, xGeometry.coordinates.Latitude != null ? Convert.ToDouble(xGeometry.coordinates.Latitude) : 0.00 }
                    }).FirstOrDefault(),

                    Zone_GeographyMapping = item.Zone_GeographyMapping.ConvertAll(xGeo => new DataContracts.Masters.DC_Zone_Geography
                    {
                        TLGXCityCode = xGeo.TLGXCityCode,
                        TLGXCountryCode = xGeo.TLGXCountryCode,
                        TLGXCityAreaCode = xGeo.TLGXCityAreaCode,
                        TLGXCityAreaLocationCode = xGeo.TLGXCityAreaLocationCode,
                        TLGXStateCode = xGeo.TLGXStateCode
                    }),
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
                    }),
                    Zone_LocationMapping = item.ZoneLocationMapping.ConvertAll(xLoc => new DataContracts.Masters.DC_Zone_LocationMapping
                    {
                        Name = xLoc.Name,
                        Code = xLoc.Code,
                        ZoneType = xLoc.ZoneType,
                        ZoneSubType = xLoc.ZoneSubType,
                        City = xLoc.City,
                        CityArea = xLoc.CityArea,
                        CityAreaLocation=xLoc.CityAreaLocation,
                        CountryCode=xLoc.CountryCode,
                        FullAdress=xLoc.FullAdress,
                        HouseNumber=xLoc.HouseNumber,
                        PostalCode=xLoc.PostalCode,
                        StateName=xLoc.StateName,
                        StateCode=xLoc.StateCode,
                        StreetName=xLoc.StreetName,
                        Street2=xLoc.Street2,
                        Street3=xLoc.Street3,
                        Latitude=xLoc.Latitude,
                        Longitude=xLoc.Longitude,
                        Distance=xLoc.Distance,
                        Supplier_code=xLoc.Supplier_code,
                        Supplier_Name=xLoc.Supplier_Name
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
                using (TLGX_Entities context = new TLGX_Entities())
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
                    //_database = null;
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


                using (TLGX_Entities context = new TLGX_Entities())
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
                //_database = null;


            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadCountryMapping(Guid LogId, Guid Supplier_ID)
        {
            try
            {
                bool Is_IX_SupplierCode_SupplierCountryCode_Exists = false;
                bool Is_IX_SupplierCode_CountryCode_Exists = false;
                bool Is_IX_MapId_Exists = false;


                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DC_CountryMapping>("CountryMapping");

                using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                    new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted, Timeout = new TimeSpan(0, 2, 0) }))
                {
                    using (TLGX_Entities context = new TLGX_Entities())
                    {
                        var MappedData = (from cm in context.m_CountryMapping.AsNoTracking()
                                          join c in context.m_CountryMaster.AsNoTracking() on cm.Country_Id equals c.Country_Id
                                          join s in context.Suppliers.AsNoTracking() on cm.Supplier_Id equals s.Supplier_Id
                                          where cm.Status == "MAPPED" && cm.Supplier_Id == Supplier_ID
                                          select new DC_CountryMapping
                                          {
                                              SupplierName = s.Name.Trim().ToUpper(),
                                              SupplierCode = s.Code.Trim().ToUpper(),
                                              CountryCode = c.Code.Trim().ToUpper(),
                                              CountryName = c.Name.Trim().ToUpper(),
                                              SupplierCountryName = (cm.CountryName ?? string.Empty).Trim().ToUpper(),
                                              SupplierCountryCode = (cm.CountryCode ?? string.Empty).Trim().ToUpper(),
                                              MapId = cm.MapID ?? 0
                                          }).ToList();

                        if (MappedData != null && MappedData.Count > 0)
                        {
                            foreach (var mapped in MappedData)
                            {
                                var filter = Builders<DataContracts.Mapping.DC_CountryMapping>.Filter.Eq(c => c.MapId, mapped.MapId) &
                                    Builders<DataContracts.Mapping.DC_CountryMapping>.Filter.Eq(c => c.SupplierCode, mapped.SupplierCode);
                                collection.ReplaceOne(filter, mapped, new UpdateOptions { IsUpsert = true });
                            }
                        }

                        var NotMappedData = (from cm in context.m_CountryMapping.AsNoTracking()
                                             where cm.Status != "MAPPED" && cm.Supplier_Id == Supplier_ID
                                             select new DataContracts.Mapping.DC_CountryMapping { MapId = cm.MapID ?? 0 }).ToList();

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

                UpdateDistLogInfo(LogId, PushStatus.COMPLETED);

                collection = null;
                //_database = null;

            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                UpdateDistLogInfo(LogId, PushStatus.ERROR, 0, 0, Supplier_ID.ToString(), "Country", "Mapping");
            }
        }

        public void LoadCityMapping(Guid LogId)
        {
            try
            {
                UpdateDistLogInfo(LogId, PushStatus.RUNNNING);

                bool Is_IX_SupplierCode_SupplierCityCode_Exists = false;
                bool Is_IX_SupplierCode_CityCode_Exists = false;
                bool Is_IX_MapId_Exists = false;
                bool Is_IX_CityCode_Exists = false;
                int TotalRecords = 0;
                int TotalProcessed = 0;

                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_CityMapping>("CityMapping");

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

                    if ((string)rss["key"]["CityCode"] != null)
                    {
                        Is_IX_CityCode_Exists = true;
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

                if (!Is_IX_CityCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CityMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CityMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.CityCode);
                    CreateIndexModel<DataContracts.Mapping.DC_CityMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_CityMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                #endregion

                List<DC_Supplier_ShortVersion> SupplierCodes = new List<DC_Supplier_ShortVersion>();

                using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                    new System.Transactions.TransactionOptions()
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                        Timeout = new TimeSpan(0, 2, 0)
                    }))
                {
                    using (TLGX_Entities context = new TLGX_Entities())
                    {
                        context.Database.CommandTimeout = 0;

                        SupplierCodes = context.Suppliers.Where(w => (w.StatusCode ?? string.Empty) == "ACTIVE").Select(s => new DC_Supplier_ShortVersion
                        { SupplierCode = s.Code.ToUpper(), Supplier_Id = s.Supplier_Id, SupplierName = s.Name.ToUpper() }).ToList();

                        TotalRecords = context.m_CityMapping.Where(w => w.Status == "MAPPED").Count();
                    }
                    scope.Complete();
                }

                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalRecords, 0, string.Empty, "City", "Mapping");

                foreach (var supplier in SupplierCodes)
                {
                    List<DataContracts.Mapping.DC_CityMapping> CityListMapped = new List<DataContracts.Mapping.DC_CityMapping>();

                    using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                    new System.Transactions.TransactionOptions()
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                        Timeout = new TimeSpan(0, 2, 0)
                    }))
                    {
                        using (TLGX_Entities context = new TLGX_Entities())
                        {
                            context.Database.CommandTimeout = 0;
                            CityListMapped = (from cm in context.m_CityMapping.AsNoTracking()
                                              join city in context.m_CityMaster.AsNoTracking() on cm.City_Id equals city.City_Id
                                              join country in context.m_CountryMaster.AsNoTracking() on cm.Country_Id equals country.Country_Id
                                              where cm.Supplier_Id == supplier.Supplier_Id && cm.Status == "MAPPED"
                                              select new DataContracts.Mapping.DC_CityMapping
                                              {
                                                  //CityMapping_Id = cm.CityMapping_Id.ToString(),
                                                  CityName = (city.Name ?? string.Empty).ToUpper(),
                                                  CityCode = (city.Code ?? string.Empty).ToUpper(),
                                                  SupplierCityCode = (supplier.SupplierCode == "CLEARTRIP") ? (cm.CityName ?? string.Empty).ToUpper() : (cm.CityCode ?? string.Empty).ToUpper(),
                                                  SupplierCityName = (cm.CityName ?? string.Empty).ToUpper(),
                                                  SupplierName = supplier.SupplierName,
                                                  SupplierCode = supplier.SupplierCode,
                                                  CountryCode = country.Code.ToUpper(),
                                                  CountryName = country.Name.ToUpper(),
                                                  SupplierCountryName = (cm.CountryName ?? string.Empty).ToUpper(),
                                                  SupplierCountryCode = (cm.CountryCode ?? string.Empty).ToUpper(),
                                                  MapId = cm.MapID ?? 0
                                              }).ToList();

                        }
                        scope.Complete();
                    }

                    var MappedIds = CityListMapped.Select(s => s.MapId).ToList();
                    var mapidsinmongo = collection.Find(x => x.SupplierCode == supplier.SupplierCode).Project(u => new { u.MapId }).ToList();
                    var MapIdsToBeDeleted = (from m in mapidsinmongo
                                             where !MappedIds.Contains(m.MapId)
                                             select m).ToList();
                    if (MapIdsToBeDeleted != null && MapIdsToBeDeleted.Count > 0)
                    {
                        foreach (var city in MapIdsToBeDeleted)
                        {
                            var filter = Builders<DataContracts.Mapping.DC_CityMapping>.Filter.Eq(c => c.MapId, city.MapId);
                            collection.DeleteMany(filter);
                        }
                    }
                    if (CityListMapped != null && CityListMapped.Count > 0)
                    {
                        foreach (var city in CityListMapped)
                        {
                            var filter = Builders<DataContracts.Mapping.DC_CityMapping>.Filter.Eq(c => c.MapId, city.MapId);
                            collection.ReplaceOne(filter, city, new UpdateOptions { IsUpsert = true });
                            TotalProcessed++;
                            if (TotalProcessed % 100 == 0)
                            {
                                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalRecords, TotalProcessed, string.Empty, "City", "Mapping");
                            }
                        }
                    }

                    //TotalProcessed += MappedIds.Count();

                    UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalRecords, TotalProcessed, string.Empty, "City", "Mapping");
                }

                collection = null;
                //_database = null;

                UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalRecords, TotalProcessed, string.Empty, "City", "Mapping");

            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadCityMapping(Guid LogId, Guid Supplier_ID)
        {
            bool Is_IX_SupplierCode_SupplierCityCode_Exists = false;
            bool Is_IX_SupplierCode_CityCode_Exists = false;
            bool Is_IX_MapId_Exists = false;
            bool Is_IX_CityCode_Exists = false;
            int TotalRecords = 0;
            int TotalProcessed = 0;

            try
            {
                UpdateDistLogInfo(LogId, PushStatus.RUNNNING);



                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_CityMapping>("CityMapping");

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

                    if ((string)rss["key"]["CityCode"] != null)
                    {
                        Is_IX_CityCode_Exists = true;
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

                if (!Is_IX_CityCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CityMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_CityMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.CityCode);
                    CreateIndexModel<DataContracts.Mapping.DC_CityMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_CityMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                #endregion

                DC_Supplier_ShortVersion supplier = new DC_Supplier_ShortVersion();

                using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                    new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted, Timeout = new TimeSpan(0, 2, 0) }))
                {
                    using (TLGX_Entities context = new TLGX_Entities())
                    {
                        context.Database.CommandTimeout = 0;

                        supplier = context.Suppliers.Where(w => w.Supplier_Id == Supplier_ID && (w.StatusCode ?? string.Empty) == "ACTIVE").Select(s => new DC_Supplier_ShortVersion
                        { SupplierCode = s.Code.ToUpper(), Supplier_Id = s.Supplier_Id, SupplierName = s.Name.ToUpper() }).FirstOrDefault();

                        TotalRecords = context.m_CityMapping.Where(w => w.Status == "MAPPED" && w.Supplier_Id == Supplier_ID).Count();
                    }
                    scope.Complete();
                }

                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalRecords, 0, string.Empty, "City", "Mapping");

                if (supplier != null)
                {
                    List<DC_CityMapping> CityListMapped = new List<DC_CityMapping>();

                    using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                    new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted, Timeout = new TimeSpan(0, 2, 0) }))
                    {
                        using (TLGX_Entities context = new TLGX_Entities())
                        {
                            context.Database.CommandTimeout = 0;

                            CityListMapped = (from cm in context.m_CityMapping.AsNoTracking()
                                              join city in context.m_CityMaster.AsNoTracking() on cm.City_Id equals city.City_Id
                                              join country in context.m_CountryMaster.AsNoTracking() on cm.Country_Id equals country.Country_Id
                                              where cm.Supplier_Id == supplier.Supplier_Id && cm.Status == "MAPPED"
                                              select new DC_CityMapping
                                              {
                                                  CityName = (city.Name ?? string.Empty).ToUpper(),
                                                  CityCode = (city.Code ?? string.Empty).ToUpper(),
                                                  SupplierCityCode = (supplier.SupplierCode == "CLEARTRIP") ? (cm.CityName ?? string.Empty).ToUpper() : (cm.CityCode ?? string.Empty).ToUpper(),
                                                  SupplierCityName = (cm.CityName ?? string.Empty).ToUpper(),
                                                  SupplierName = supplier.SupplierName,
                                                  SupplierCode = supplier.SupplierCode,
                                                  CountryCode = country.Code.ToUpper(),
                                                  CountryName = country.Name.ToUpper(),
                                                  SupplierCountryName = (cm.CountryName ?? string.Empty).ToUpper(),
                                                  SupplierCountryCode = (cm.CountryCode ?? string.Empty).ToUpper(),
                                                  MapId = cm.MapID ?? 0
                                              }).ToList();
                        }
                        scope.Complete();
                    }

                    var MappedIds = CityListMapped.Select(s => s.MapId).ToList();

                    var mapidsinmongo = collection.Find(x => x.SupplierCode == supplier.SupplierCode).Project(u => new { u.MapId }).ToList();

                    var MapIdsToBeDeleted = (from m in mapidsinmongo
                                             where !MappedIds.Contains(m.MapId)
                                             select m).ToList();

                    if (MapIdsToBeDeleted != null && MapIdsToBeDeleted.Count > 0)
                    {
                        foreach (var city in MapIdsToBeDeleted)
                        {
                            var filter = Builders<DC_CityMapping>.Filter.Eq(c => c.MapId, city.MapId);
                            collection.DeleteMany(filter);
                        }
                    }
                    if (CityListMapped != null && CityListMapped.Count > 0)
                    {
                        foreach (var city in CityListMapped)
                        {
                            var filter = Builders<DC_CityMapping>.Filter.Eq(c => c.MapId, city.MapId) & Builders<DC_CityMapping>.Filter.Eq(c => c.SupplierCode, city.SupplierCode);
                            collection.ReplaceOne(filter, city, new UpdateOptions { IsUpsert = true });
                        }
                    }

                    TotalProcessed += MappedIds.Count();

                    UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalRecords, TotalProcessed, string.Empty, "City", "Mapping");

                    collection = null;
                    //_database = null;

                    UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalRecords, TotalProcessed, string.Empty, "City", "Mapping");
                }
                else
                {
                    UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalRecords, TotalProcessed, string.Empty, "City", "Mapping");
                }
            }
            catch (FaultException<ErrorNotifier> ex)
            {
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalRecords, TotalProcessed, Supplier_ID.ToString(), "City", "Mapping");
                throw ex;
            }
        }

        #endregion

        #region UI Level MPUSH

        /// <summary>
        /// 
        /// </summary>
        /// <param name="CountryMapping_ID"></param>
        public void LoadCountryMappingByUI(Guid CountryMapping_ID)
        {
            try
            {
                bool Is_IX_SupplierCode_SupplierCountryCode_Exists = false;
                bool Is_IX_SupplierCode_CountryCode_Exists = false;
                bool Is_IX_MapId_Exists = false;

                _database = MongoDBHandler.mDatabase();

                var collection = _database.GetCollection<DataContracts.Mapping.DC_CountryMapping>("CountryMapping");

                using (TLGX_Entities context = new TLGX_Entities())
                {
                    var MappedData = (from cm in context.m_CountryMapping.AsNoTracking()
                                      join c in context.m_CountryMaster.AsNoTracking() on cm.Country_Id equals c.Country_Id
                                      join s in context.Suppliers.AsNoTracking() on cm.Supplier_Id equals s.Supplier_Id
                                      where cm.CountryMapping_Id == CountryMapping_ID
                                      select new DataContracts.Mapping.DC_CountryMapping
                                      {
                                          SupplierName = s.Name.Trim().ToUpper(),
                                          SupplierCode = s.Code.Trim().ToUpper(),
                                          CountryCode = c.Code.Trim().ToUpper(),
                                          CountryName = c.Name.Trim().ToUpper(),
                                          Status = cm.Status.Trim().ToUpper(),
                                          SupplierCountryName = (cm.CountryName ?? string.Empty).Trim().ToUpper(),
                                          SupplierCountryCode = (cm.CountryCode ?? string.Empty).Trim().ToUpper(),
                                          MapId = cm.MapID ?? 0
                                      }).FirstOrDefault();

                    if (MappedData != null)
                    {
                        if (MappedData.Status.Trim().ToUpper() == "MAPPED")
                        {
                            var filter = Builders<DataContracts.Mapping.DC_CountryMapping>.Filter.Eq(c => c.MapId, MappedData.MapId) &
                                Builders<DataContracts.Mapping.DC_CountryMapping>.Filter.Eq(c => c.SupplierCode, MappedData.SupplierCode);
                            collection.ReplaceOne(filter, MappedData, new UpdateOptions { IsUpsert = true });
                        }
                        else if (MappedData.Status.Trim().ToUpper() != "MAPPED")
                        {
                            var filter = Builders<DataContracts.Mapping.DC_CountryMapping>.Filter.Eq(c => c.MapId, MappedData.MapId) &
                                Builders<DataContracts.Mapping.DC_CountryMapping>.Filter.Eq(c => c.SupplierCode, MappedData.SupplierCode);
                            collection.DeleteOne(filter);
                        }
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
                //_database = null;
            }
            catch (FaultException<ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        #endregion

        #region Product Mapping Push

        public void LoadProductMapping(Guid LogId, Guid ProdMapId)
        {
            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DC_ProductMapping>("ProductMapping");

            int TotalAPMCount = 0;
            int MongoInsertedCount = 0;


            try
            {
                if (ProdMapId == Guid.Empty)
                {
                    #region Index Management
                    bool Is_IX_SupplierCode_SupplierProductCode_Exists = false;
                    bool Is_IX_SupplierCode_SystemProductCode_Exists = false;
                    bool Is_IX_SupplierCode_SystemCityCode_Exists = false;
                    bool Is_IX_MapId_Exists = false;

                    var listOfindexes = collection.Indexes.List().ToList();

                    foreach (var index in listOfindexes)
                    {
                        Newtonsoft.Json.Linq.JObject rss = Newtonsoft.Json.Linq.JObject.Parse(index.ToJson());
                        if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SupplierProductCode"] != null)
                        {
                            Is_IX_SupplierCode_SupplierProductCode_Exists = true;
                        }

                        if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SystemProductCode"] != null)
                        {
                            Is_IX_SupplierCode_SystemProductCode_Exists = true;
                        }

                        if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SystemCityCode"] != null)
                        {
                            Is_IX_SupplierCode_SystemCityCode_Exists = true;
                        }

                        if ((string)rss["key"]["MapId"] != null)
                        {
                            Is_IX_MapId_Exists = true;
                        }
                    }

                    if (!Is_IX_SupplierCode_SupplierProductCode_Exists)
                    {
                        IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping>();
                        var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode);
                        CreateIndexModel<DataContracts.Mapping.DC_ProductMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ProductMapping>(keys);
                        collection.Indexes.CreateOneAsync(IndexModel);
                    }

                    if (!Is_IX_SupplierCode_SystemProductCode_Exists)
                    {
                        IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping>();
                        var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemProductCode);
                        CreateIndexModel<DataContracts.Mapping.DC_ProductMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ProductMapping>(keys);
                        collection.Indexes.CreateOneAsync(IndexModel);
                    }

                    if (!Is_IX_SupplierCode_SystemCityCode_Exists)
                    {
                        IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping>();
                        var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemCityCode);
                        CreateIndexModel<DataContracts.Mapping.DC_ProductMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ProductMapping>(keys);
                        collection.Indexes.CreateOneAsync(IndexModel);
                    }

                    if (!Is_IX_MapId_Exists)
                    {
                        IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping>();
                        var keys = IndexBuilder.Ascending(_ => _.MapId);
                        CreateIndexModel<DataContracts.Mapping.DC_ProductMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ProductMapping>(keys);
                        collection.Indexes.CreateOneAsync(IndexModel);
                    }

                    #endregion

                    UpdateDistLogInfo(LogId, PushStatus.RUNNNING);

                    List<DC_Supplier_ShortVersion> SupplierCodes = new List<DC_Supplier_ShortVersion>();

                    using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                    new System.Transactions.TransactionOptions()
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                        Timeout = new TimeSpan(0, 2, 0)
                    }))
                    {
                        using (TLGX_Entities context = new TLGX_Entities())
                        {
                            context.Database.CommandTimeout = 0;

                            SupplierCodes = context.Suppliers.Where(w => (w.StatusCode ?? string.Empty) == "ACTIVE").OrderBy(o => o.Code).Select(s => new DC_Supplier_ShortVersion
                            {
                                SupplierCode = s.Code.ToUpper(),
                                Supplier_Id = s.Supplier_Id,
                                SupplierName = s.Name.ToUpper()
                            }).ToList();

                            TotalAPMCount = context.Accommodation_ProductMapping.AsNoTracking().Where(w => w.IsActive == true).Count();
                        }
                        scope.Complete();
                    }

                    foreach (var SupplierCode in SupplierCodes)
                    {
                        List<DataContracts.Mapping.DC_ProductMapping> productMapList = new List<DataContracts.Mapping.DC_ProductMapping>();
                        using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                        new System.Transactions.TransactionOptions()
                        {
                            IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                            Timeout = new TimeSpan(0, 5, 0)
                        }))
                        {
                            using (TLGX_Entities context = new TLGX_Entities())
                            {
                                context.Configuration.AutoDetectChangesEnabled = false;
                                context.Database.CommandTimeout = 0;
                                StringBuilder sbsqlselectcount = new StringBuilder();
                                sbsqlselectcount.Append("SELECT '" + SupplierCode.SupplierCode + "'  SupplierCode,APM.SupplierProductReference SupplierProductCode,APM.CountryCode SupplierCountryCode,APM.CountryName SupplierCountryName,APM.CityCode SupplierCityCode," +
                                    " APM.CityName SupplierCityName, APM.ProductName SupplierProductName, APM.Status MappingStatus, APM.MapId MapId, ISNULL(CAST(Acco.CompanyHotelID AS VARCHAR(50)), '') SystemProductCode, ISNULL(Acco.HotelName, '') SystemProductName," +
                                    " ISNULL(Acco.ProductCategorySubType, '') SystemProductType, ISNULL(Acco.TLGXAccoId, '') TlgxMdmHotelId," +
                                    " ISNULL(country.Code, '') SystemCountryCode, ISNULL(country.Name, '') SystemCountryName, ISNULL(City.Code, '') SystemCityCode, ISNULL(city.Name, '') SystemCityName" +
                                    " FROM Accommodation_ProductMapping APM with(Nolock) " +
                                    " LEFT JOIN m_CityMaster City with(Nolock) ON APM.City_Id = City.City_Id " +
                                    " LEFT JOIN m_CountryMaster Country with(Nolock) ON City.Country_Id = Country.Country_Id" +
                                    " LEFT JOIN ACCOMMODATION ACCO with(Nolock) ON APM.ACCOMMODATION_ID = ACCO.ACCOMMODATION_ID WHERE apm.Status IN('MAPPED', 'AUTOMAPPED') AND APM.isactive=1 AND APM.supplier_id='" + SupplierCode.Supplier_Id + "'");

                                productMapList = context.Database.SqlQuery<DataContracts.Mapping.DC_ProductMapping>(sbsqlselectcount.ToString()).ToList();



                            }
                            scope.Complete();
                        }

                        List<int> MappedIds = productMapList.Select(s => s.MapId).ToList();
                        List<int> mapidsinmongo = collection.Find(x => x.SupplierCode == SupplierCode.SupplierCode).Project(u => u.MapId).ToList();
                        List<int> MapIdsToBeDeleted = mapidsinmongo.Except(MappedIds).ToList();

                        if (MapIdsToBeDeleted != null && MapIdsToBeDeleted.Count > 0)
                        {
                            var filter = Builders<DataContracts.Mapping.DC_ProductMapping>.Filter.In(c => c.MapId, MapIdsToBeDeleted);
                            collection.DeleteMany(filter);
                        }

                        if (productMapList != null && productMapList.Count() > 0)
                        {

                            var writeModelDetails = new List<WriteModel<DataContracts.Mapping.DC_ProductMapping>>();
                            foreach (var item in productMapList)
                            {
                                writeModelDetails.Add(new ReplaceOneModel<DataContracts.Mapping.DC_ProductMapping>(
                                        new BsonDocument("MapId", item.MapId), item)
                                { IsUpsert = true });
                            }
                            if (writeModelDetails.Any())
                                collection.BulkWrite(writeModelDetails);
                            MongoInsertedCount = MongoInsertedCount + productMapList.Count();
                            UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount);
                        }
                    }

                    UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalAPMCount, MongoInsertedCount);
                }
                else
                {
                    #region If Map ID is not 0 delete and insert single Record
                    using (TLGX_Entities context = new TLGX_Entities())
                    {
                        var productMap = (from apm in context.Accommodation_ProductMapping.AsNoTracking()

                                          join s in context.Suppliers.AsNoTracking() on apm.Supplier_Id equals s.Supplier_Id

                                          join cm in context.m_CityMaster.AsNoTracking() on apm.City_Id equals cm.City_Id into LJCityMaster
                                          from citymaster in LJCityMaster.DefaultIfEmpty()

                                          join con in context.m_CountryMaster.AsNoTracking() on citymaster.Country_Id equals con.Country_Id into LJCountryMaster
                                          from countrymaster in LJCountryMaster.DefaultIfEmpty()

                                          join a in context.Accommodations.AsNoTracking() on apm.Accommodation_Id equals a.Accommodation_Id into LJAcco
                                          from acco in LJAcco.DefaultIfEmpty()

                                          where apm.IsActive == true && apm.Accommodation_ProductMapping_Id == ProdMapId

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
                                              MapId = apm.MapId,

                                              SystemProductCode = (acco == null ? string.Empty : acco.CompanyHotelID.ToString().ToUpper()),
                                              SystemProductName = (acco == null ? string.Empty : acco.HotelName.ToUpper()),
                                              SystemProductType = (acco == null ? string.Empty : acco.ProductCategorySubType.ToUpper()),
                                              TlgxMdmHotelId = (acco == null ? string.Empty : acco.TLGXAccoId.ToUpper()),

                                              SystemCountryCode = (countrymaster != null ? countrymaster.Code.ToUpper() : string.Empty),
                                              SystemCountryName = (countrymaster != null ? countrymaster.Name.ToUpper() : string.Empty),
                                              SystemCityCode = (citymaster != null ? citymaster.Code.ToUpper() : string.Empty),
                                              SystemCityName = (citymaster != null ? citymaster.Name.ToUpper() : string.Empty)

                                          }).FirstOrDefault();
                        if (productMap != null && (productMap.MappingStatus == "MAPPED" || productMap.MappingStatus == "AUTOMAPPED"))
                        {
                            var res = collection.DeleteMany(x => x.MapId == productMap.MapId);
                            collection.InsertOneAsync(productMap);
                        }
                        else if (productMap != null)
                        {
                            collection.DeleteOne(x => x.MapId == productMap.MapId);
                        }
                    }
                    #endregion
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount);
            }

            collection = null;
            //_database = null;
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
                    if (LogId == Guid.Empty)
                    {
                        LogId = Guid.NewGuid();
                        UpdateDistLogInfo(LogId, PushStatus.INSERT, 0, 0, string.Empty, "HOTEL", "MAPPINGLITE");
                    }

                    #region Index Management
                    bool Is_IX_SupplierCode_SupplierProductCode_Exists = false;
                    bool Is_IX_SupplierCode_SystemProductCode_Exists = false;
                    bool Is_IX_MapId_Exists = false;
                    bool Is_IX_SupplierCode_Exists = false;

                    var listOfindexes = collection.Indexes.List().ToList();

                    foreach (var index in listOfindexes)
                    {
                        Newtonsoft.Json.Linq.JObject rss = Newtonsoft.Json.Linq.JObject.Parse(index.ToJson());
                        if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SupplierProductCode"] != null)
                        {
                            Is_IX_SupplierCode_SupplierProductCode_Exists = true;
                        }

                        if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SystemProductCode"] != null)
                        {
                            Is_IX_SupplierCode_SystemProductCode_Exists = true;
                        }

                        if ((string)rss["key"]["MapId"] != null)
                        {
                            Is_IX_MapId_Exists = true;
                        }

                        if ((string)rss["key"]["SupplierCode"] != null)
                        {
                            Is_IX_SupplierCode_Exists = true;
                        }
                    }

                    if (!Is_IX_SupplierCode_SupplierProductCode_Exists)
                    {
                        IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMappingLite> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMappingLite>();
                        var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode);
                        CreateIndexModel<DataContracts.Mapping.DC_ProductMappingLite> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ProductMappingLite>(keys);
                        collection.Indexes.CreateOneAsync(IndexModel);
                    }

                    if (!Is_IX_SupplierCode_SystemProductCode_Exists)
                    {
                        IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMappingLite> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMappingLite>();
                        var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemProductCode);
                        CreateIndexModel<DataContracts.Mapping.DC_ProductMappingLite> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ProductMappingLite>(keys);
                        collection.Indexes.CreateOneAsync(IndexModel);
                    }

                    if (!Is_IX_SupplierCode_Exists)
                    {
                        IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMappingLite> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMappingLite>();
                        var keys = IndexBuilder.Ascending(_ => _.SupplierCode);
                        CreateIndexModel<DataContracts.Mapping.DC_ProductMappingLite> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ProductMappingLite>(keys);
                        collection.Indexes.CreateOneAsync(IndexModel);
                    }

                    if (!Is_IX_MapId_Exists)
                    {
                        IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMappingLite> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMappingLite>();
                        var keys = IndexBuilder.Ascending(_ => _.MapId);
                        CreateIndexModel<DataContracts.Mapping.DC_ProductMappingLite> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ProductMappingLite>(keys);
                        collection.Indexes.CreateOneAsync(IndexModel);
                    }

                    #endregion

                    UpdateDistLogInfo(LogId, PushStatus.RUNNNING, 0, 0, string.Empty, "HOTEL", "MAPPINGLITE");

                    List<DC_Supplier_ShortVersion> SupplierCodes = new List<DC_Supplier_ShortVersion>();

                    using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                    new System.Transactions.TransactionOptions()
                    {
                        IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                        Timeout = new TimeSpan(0, 2, 0)
                    }))
                    {
                        using (TLGX_Entities context = new TLGX_Entities())
                        {
                            context.Database.CommandTimeout = 0;

                            SupplierCodes = context.Suppliers.Where(w => (w.StatusCode ?? string.Empty) == "ACTIVE").OrderBy(o => o.Code).Select(s => new DC_Supplier_ShortVersion
                            {
                                SupplierCode = s.Code.ToUpper(),
                                Supplier_Id = s.Supplier_Id,
                                SupplierName = s.Name.ToUpper()
                            }).ToList();

                            TotalAPMCount = context.Accommodation_ProductMapping.AsNoTracking().Where(w => (w.Status.Trim().ToUpper() == "MAPPED" || w.Status.Trim().ToUpper() == "AUTOMAPPED") && w.IsActive == true).Count();
                        }
                        scope.Complete();
                    }

                    UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, 0, string.Empty, "HOTEL", "MAPPINGLITE");

                    foreach (var SupplierCode in SupplierCodes)
                    {
                        List<DataContracts.Mapping.DC_ProductMappingLite> productMapList = new List<DataContracts.Mapping.DC_ProductMappingLite>();

                        using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                        new System.Transactions.TransactionOptions()
                        {
                            IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                            Timeout = new TimeSpan(0, 5, 0)
                        }))
                        {
                            using (TLGX_Entities context = new TLGX_Entities())
                            {
                                context.Configuration.AutoDetectChangesEnabled = false;
                                context.Database.CommandTimeout = 0;

                                StringBuilder sbsqlselectcount = new StringBuilder();
                                sbsqlselectcount.Append("SELECT '" + SupplierCode.SupplierCode + "' AS SupplierCode,APM.SupplierProductReference AS SupplierProductCode," +
                                    " APM.MapId AS MapId, UPPER(ACC.CompanyHotelID) AS SystemProductCode, UPPER(ISNULL(ACC.TLGXAccoId, '')) AS TlgxMdmHotelId" +
                                    " FROM Accommodation_ProductMapping  APM with(Nolock) " +
                                    " INNER JOIN Accommodation ACC with(Nolock) ON APM.Accommodation_Id = Acc.Accommodation_Id" +
                                    " WHERE apm.Status IN('MAPPED', 'AUTOMAPPED') AND APM.IsActive = 1 AND apm.Supplier_Id = '" + SupplierCode.Supplier_Id + "'");

                                productMapList = context.Database.SqlQuery<DataContracts.Mapping.DC_ProductMappingLite>(sbsqlselectcount.ToString()).ToList();


                            }
                            scope.Complete();
                        }

                        List<int> MappedIds = productMapList.Select(s => s.MapId).ToList();
                        List<int> mapidsinmongo = collection.Find(x => x.SupplierCode == SupplierCode.SupplierCode).Project(u => u.MapId).ToList();

                        List<int> MapIdsToBeDeleted = mapidsinmongo.Except(MappedIds).ToList();

                        if (MapIdsToBeDeleted != null && MapIdsToBeDeleted.Count > 0)
                        {
                            var filter = Builders<DataContracts.Mapping.DC_ProductMappingLite>.Filter.In(c => c.MapId, MapIdsToBeDeleted);
                            collection.DeleteMany(filter);
                        }

                        if (productMapList != null && productMapList.Count() > 0)
                        {
                            var writeModelDetails = new List<WriteModel<DataContracts.Mapping.DC_ProductMappingLite>>();
                            foreach (var item in productMapList)
                            {
                                writeModelDetails.Add(new ReplaceOneModel<DataContracts.Mapping.DC_ProductMappingLite>(
                                        new BsonDocument("MapId", item.MapId), item)
                                { IsUpsert = true });
                            }
                            if (writeModelDetails.Any())
                                collection.BulkWrite(writeModelDetails);

                            MongoInsertedCount = MongoInsertedCount + productMapList.Count();
                            UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount, string.Empty, "HOTEL", "MAPPINGLITE");
                        }
                    }

                    UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalAPMCount, MongoInsertedCount, string.Empty, "HOTEL", "MAPPINGLITE");
                }
                else
                {
                    #region If Map ID is not 0 delete and insert single Record
                    using (TLGX_Entities context = new TLGX_Entities())
                    {
                        var prodstatus = (from apm in context.Accommodation_ProductMapping.AsNoTracking()
                                          join a in context.Accommodations.AsNoTracking() on apm.Accommodation_Id equals a.Accommodation_Id
                                          join s in context.Suppliers.AsNoTracking() on apm.Supplier_Id equals s.Supplier_Id
                                          where apm.Accommodation_ProductMapping_Id == ProdMapId
                                          && apm.IsActive == true
                                          select new { Status = apm.Status.ToUpper() }).FirstOrDefault();

                        var prod = (from apm in context.Accommodation_ProductMapping.AsNoTracking()
                                    join a in context.Accommodations.AsNoTracking() on apm.Accommodation_Id equals a.Accommodation_Id
                                    join s in context.Suppliers.AsNoTracking() on apm.Supplier_Id equals s.Supplier_Id
                                    where apm.Accommodation_ProductMapping_Id == ProdMapId
                                    && apm.IsActive == true
                                    select new DataContracts.Mapping.DC_ProductMappingLite
                                    {
                                        SupplierCode = s.Code.Trim().ToUpper(),
                                        SupplierProductCode = apm.SupplierProductReference.ToUpper(),
                                        MapId = apm.MapId,
                                        SystemProductCode = a.CompanyHotelID.ToString().ToUpper(),
                                        TlgxMdmHotelId = (a.TLGXAccoId == null ? string.Empty : a.TLGXAccoId.ToUpper())
                                    }).FirstOrDefault();
                        if (prod != null && (prodstatus.Status == "MAPPED" || prodstatus.Status == "AUTOMAPPED"))
                        {
                            var filter = Builders<DataContracts.Mapping.DC_ProductMappingLite>.Filter.Eq(c => c.MapId, prod.MapId);
                            collection.ReplaceOne(filter, prod, new UpdateOptions { IsUpsert = true });
                        }
                        else if (prod != null && prodstatus.Status == "UNMAPPED")
                        {
                            collection.DeleteOne(x => x.MapId == prod.MapId);
                        }
                    }
                    #endregion
                }
                collection = null;
                //_database = null;
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount);
            }
        }

        public void LoadCompanyAccommodationProductMappingOnSave(Guid LogId, Guid ProdMapId)
        {
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_ConpanyAccommodationMapping>("CompanyAccommodationProductMapping");
                StringBuilder CompanyWiseProductMapping = new StringBuilder();
                DataContracts.Mapping.DC_ConpanyAccommodationMapping productMapstatus = new DataContracts.Mapping.DC_ConpanyAccommodationMapping();
                DataContracts.Mapping.DC_ConpanyAccommodationMapping productMap = new DataContracts.Mapping.DC_ConpanyAccommodationMapping();
                CompanyWiseProductMapping.Append(" select apm.Supplier_Id as [SupplierId],upper(sup.code) as SupplierCode, apm.SupplierProductReference as [SupplierProductCode]" +
                                                " , ( upper(sup.code) + '_'+    apm.SupplierProductReference + '_'+  av.CompanyId + '_' +   av.CompanyProductId) as _id " +
                                                " from  Accommodation_ProductMapping apm with (NOLOCk)    join  Accommodation_CompanyVersion av with (NOLOCk)" +
                                                " on av.Accommodation_Id = apm.Accommodation_Id join  Accommodation acc with (NOLOCk)" +
                                                " on acc.Accommodation_Id = av.Accommodation_Id join supplier sup with (NOLOCk)" +
                                                " on apm.Supplier_Id=sup.Supplier_Id" +
                                                " where apm.IsActive = 1 and apm.STATUS NOT in ('MAPPED', 'AUTOMAPPED') " +
                                                " and apm.Accommodation_ProductMapping_Id = '" + ProdMapId + @"'");


                StringBuilder sbSelectAccoMaster = new StringBuilder();

                sbSelectAccoMaster.Append(@"  
                                    select  
	                                      apm.Supplier_Id					     as [SupplierId]
                                        , upper(sup.code)				 as [SupplierCode]	  
                                        , (upper(sup.code)	 + '_'+    apm.SupplierProductReference + '_'+  av.CompanyId + '_' +   av.CompanyProductId) as _id 
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
                                    join  Accommodation_CompanyVersion av 
		                                    on av.Accommodation_Id = apm.Accommodation_Id
                                    join  Accommodation acc 
		                                    on acc.Accommodation_Id = av.Accommodation_Id
                                    join supplier sup with (NOLOCk) 
                                            on apm.Supplier_Id=sup.Supplier_Id
                                    where 
                                        apm.IsActive = 1  and	                                     
	                                    apm.Accommodation_ProductMapping_Id = '" + ProdMapId + @"'");


                using (TLGX_Entities context = new TLGX_Entities())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Database.CommandTimeout = 0;
                    productMapstatus = context.Database.SqlQuery<DataContracts.Mapping.DC_ConpanyAccommodationMapping>(CompanyWiseProductMapping.ToString()).FirstOrDefault();
                    productMap = context.Database.SqlQuery<DataContracts.Mapping.DC_ConpanyAccommodationMapping>(sbSelectAccoMaster.ToString()).FirstOrDefault();
                    if (productMapstatus != null && productMap != null)
                    {
                        collection.DeleteOne(x => x._id == productMap._id);
                    }
                    else if (productMap != null)
                    {
                        var res = collection.DeleteOne(x => x._id == productMap._id);
                        collection.InsertOneAsync(productMap);
                    }
                }
                collection = null;
                //_database = null;
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                throw ex;
            }
        }

        public void LoadHotelMapping(Guid LogId)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    var Log = context.DistributionLayerRefresh_Log.Find(LogId);
                    if (Log != null)
                    {
                        Log.Status = "Completed";
                        context.SaveChanges();
                    }
                }
            }
            catch (FaultException<ErrorNotifier> ex)
            {
                throw ex;
            }

        }

        public void LoadProductMappingBySupplier(Guid LogId, Guid Supplier_ID)
        {
            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DC_ProductMapping>("ProductMapping");
            DC_Supplier_ShortVersion SupplierCodes = new DC_Supplier_ShortVersion();
            int TotalAPMCount = 0;
            int MongoInsertedCount = 0;

            try
            {
                #region Index Management

                bool Is_IX_SupplierCode_SupplierProductCode_Exists = false;
                bool Is_IX_SupplierCode_SystemProductCode_Exists = false;
                bool Is_IX_SupplierCode_SystemCityCode_Exists = false;
                bool Is_IX_MapId_Exists = false;

                var listOfindexes = collection.Indexes.List().ToList();

                foreach (var index in listOfindexes)
                {
                    Newtonsoft.Json.Linq.JObject rss = Newtonsoft.Json.Linq.JObject.Parse(index.ToJson());
                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SupplierProductCode"] != null)
                    {
                        Is_IX_SupplierCode_SupplierProductCode_Exists = true;
                    }

                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SystemProductCode"] != null)
                    {
                        Is_IX_SupplierCode_SystemProductCode_Exists = true;
                    }

                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SystemCityCode"] != null)
                    {
                        Is_IX_SupplierCode_SystemCityCode_Exists = true;
                    }

                    if ((string)rss["key"]["MapId"] != null)
                    {
                        Is_IX_MapId_Exists = true;
                    }
                }

                if (!Is_IX_SupplierCode_SupplierProductCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode);
                    CreateIndexModel<DataContracts.Mapping.DC_ProductMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ProductMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                if (!Is_IX_SupplierCode_SystemProductCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemProductCode);
                    CreateIndexModel<DataContracts.Mapping.DC_ProductMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ProductMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                if (!Is_IX_SupplierCode_SystemCityCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemCityCode);
                    CreateIndexModel<DataContracts.Mapping.DC_ProductMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ProductMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                if (!Is_IX_MapId_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ProductMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.MapId);
                    CreateIndexModel<DataContracts.Mapping.DC_ProductMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ProductMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                #endregion

                UpdateDistLogInfo(LogId, PushStatus.RUNNNING);

                #region Fetch Supplier Details 
                using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted, Timeout = new TimeSpan(0, 2, 0) }))
                {
                    using (TLGX_Entities context = new TLGX_Entities())
                    {
                        context.Database.CommandTimeout = 0;

                        SupplierCodes = context.Suppliers.Where(w => w.Supplier_Id == Supplier_ID &&
                        (w.StatusCode ?? string.Empty) == "ACTIVE").OrderBy(o => o.Code).Select(s => new DC_Supplier_ShortVersion
                        {
                            SupplierCode = s.Code.ToUpper(),
                            Supplier_Id = s.Supplier_Id,
                            SupplierName = s.Name.ToUpper()
                        }).FirstOrDefault();

                        TotalAPMCount = context.Accommodation_ProductMapping.AsNoTracking().Where(w => w.Supplier_Id == Supplier_ID && w.IsActive == true).Count();
                    }
                    scope.Complete();
                }
                #endregion

                if (SupplierCodes != null)
                {
                    #region Fetch Mapping Data

                    List<DC_ProductMapping> productMapList = new List<DC_ProductMapping>();

                    using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                    new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted, Timeout = new TimeSpan(0, 2, 0) }))
                    {
                        using (TLGX_Entities context = new TLGX_Entities())
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.Database.CommandTimeout = 0;

                            productMapList = (from apm in context.Accommodation_ProductMapping.AsNoTracking()

                                              join cm in context.m_CityMaster.AsNoTracking() on apm.City_Id equals cm.City_Id into LJCityMaster
                                              from citymaster in LJCityMaster.DefaultIfEmpty()

                                              join con in context.m_CountryMaster.AsNoTracking() on citymaster.Country_Id equals con.Country_Id into LJCountryMaster
                                              from countrymaster in LJCountryMaster.DefaultIfEmpty()

                                              join a in context.Accommodations.AsNoTracking() on apm.Accommodation_Id equals a.Accommodation_Id into LJAcco
                                              from acco in LJAcco.DefaultIfEmpty()
                                              where apm.Supplier_Id == SupplierCodes.Supplier_Id && apm.IsActive == true
                                              select new DC_ProductMapping
                                              {
                                                  SupplierCode = SupplierCodes.SupplierCode,
                                                  SupplierProductCode = apm.SupplierProductReference.ToUpper(),
                                                  SupplierCountryCode = apm.CountryCode.ToUpper(),
                                                  SupplierCountryName = apm.CountryName.ToUpper(),
                                                  SupplierCityCode = apm.CityCode.ToUpper(),
                                                  SupplierCityName = apm.CityName.ToUpper(),
                                                  SupplierProductName = apm.ProductName.ToUpper(),
                                                  MappingStatus = apm.Status.ToUpper(),
                                                  MapId = apm.MapId,

                                                  SystemProductCode = (acco == null ? string.Empty : acco.CompanyHotelID.ToString().ToUpper()),
                                                  SystemProductName = (acco == null ? string.Empty : acco.HotelName.ToUpper()),
                                                  SystemProductType = (acco == null ? string.Empty : acco.ProductCategorySubType.ToUpper()),
                                                  TlgxMdmHotelId = (acco == null ? string.Empty : acco.TLGXAccoId.ToUpper()),

                                                  SystemCountryCode = (countrymaster != null ? countrymaster.Code.ToUpper() : string.Empty),
                                                  SystemCountryName = (countrymaster != null ? countrymaster.Name.ToUpper() : string.Empty),
                                                  SystemCityCode = (citymaster != null ? citymaster.Code.ToUpper() : string.Empty),
                                                  SystemCityName = (citymaster != null ? citymaster.Name.ToUpper() : string.Empty)

                                              }).ToList();
                        }
                        scope.Complete();
                    }

                    List<int> MappedIds = productMapList.Select(s => s.MapId).ToList();
                    #endregion

                    #region Mongo Delete

                    List<int> mapidsinmongo = collection.Find(x => x.SupplierCode == SupplierCodes.SupplierCode).Project(u => u.MapId).ToList();

                    List<int> MapIdsToBeDeleted = (from m in mapidsinmongo
                                                   where !MappedIds.Contains(m)
                                                   select m).ToList();


                    if (MapIdsToBeDeleted != null && MapIdsToBeDeleted.Count > 0)
                    {
                        foreach (var MapId in MapIdsToBeDeleted)
                        {
                            var filter = Builders<DC_ProductMapping>.Filter.Eq(c => c.MapId, MapId);
                            collection.DeleteMany(filter);
                        }
                    }

                    #endregion

                    #region Mongo Add/Update

                    if (productMapList != null && productMapList.Count() > 0)
                    {
                        foreach (var product in productMapList)
                        {
                            var filter = Builders<DC_ProductMapping>.Filter.Eq(c => c.MapId, product.MapId);
                            collection.ReplaceOne(filter, product, new UpdateOptions { IsUpsert = true });
                        }

                        MongoInsertedCount = MongoInsertedCount + productMapList.Count();
                        UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount);
                    }

                    #endregion

                    UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalAPMCount, MongoInsertedCount);
                }
                else
                {
                    UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount, Supplier_ID.ToString());
                }
            }
            catch (FaultException<ErrorNotifier> ex)
            {
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount, Supplier_ID.ToString());
            }

            collection = null;
            //_database = null;
        }

        public void LoadProductMappingLiteBySupplier(Guid LogId, Guid Supplier_ID)
        {
            #region Params

            int TotalAPMCount = 0;
            int MongoInsertedCount = 0;
            bool Is_IX_SupplierCode_SupplierProductCode_Exists = false;
            bool Is_IX_SupplierCode_SystemProductCode_Exists = false;
            bool Is_IX_MapId_Exists = false;
            bool Is_IX_SupplierCode_Exists = false;

            #endregion

            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DC_ProductMappingLite>("ProductMappingLite");

                if (LogId == Guid.Empty)
                {
                    LogId = Guid.NewGuid();
                    UpdateDistLogInfo(LogId, PushStatus.INSERT, 0, 0, string.Empty, "HOTEL", "MAPPINGLITE");
                }

                #region Index Management

                var listOfindexes = collection.Indexes.List().ToList();

                foreach (var index in listOfindexes)
                {
                    JObject rss = JObject.Parse(index.ToJson());

                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SupplierProductCode"] != null) { Is_IX_SupplierCode_SupplierProductCode_Exists = true; }

                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SystemProductCode"] != null) { Is_IX_SupplierCode_SystemProductCode_Exists = true; }

                    if ((string)rss["key"]["MapId"] != null) { Is_IX_MapId_Exists = true; }

                    if ((string)rss["key"]["SupplierCode"] != null) { Is_IX_SupplierCode_Exists = true; }
                }

                if (!Is_IX_SupplierCode_SupplierProductCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DC_ProductMappingLite> IndexBuilder = new IndexKeysDefinitionBuilder<DC_ProductMappingLite>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode);
                    CreateIndexModel<DC_ProductMappingLite> IndexModel = new CreateIndexModel<DC_ProductMappingLite>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                if (!Is_IX_SupplierCode_SystemProductCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DC_ProductMappingLite> IndexBuilder = new IndexKeysDefinitionBuilder<DC_ProductMappingLite>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SystemProductCode);
                    CreateIndexModel<DC_ProductMappingLite> IndexModel = new CreateIndexModel<DC_ProductMappingLite>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                if (!Is_IX_SupplierCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DC_ProductMappingLite> IndexBuilder = new IndexKeysDefinitionBuilder<DC_ProductMappingLite>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode);
                    CreateIndexModel<DC_ProductMappingLite> IndexModel = new CreateIndexModel<DC_ProductMappingLite>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                if (!Is_IX_MapId_Exists)
                {
                    IndexKeysDefinitionBuilder<DC_ProductMappingLite> IndexBuilder = new IndexKeysDefinitionBuilder<DC_ProductMappingLite>();
                    var keys = IndexBuilder.Ascending(_ => _.MapId);
                    CreateIndexModel<DC_ProductMappingLite> IndexModel = new CreateIndexModel<DC_ProductMappingLite>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                #endregion

                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, 0, 0, string.Empty, "HOTEL", "MAPPINGLITE");

                #region Fetch Supplier Additional Details

                DC_Supplier_ShortVersion SupplierCodes = new DC_Supplier_ShortVersion();

                using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted, Timeout = new TimeSpan(0, 2, 0) }))
                {
                    using (TLGX_Entities context = new TLGX_Entities())
                    {
                        context.Database.CommandTimeout = 0;

                        SupplierCodes = context.Suppliers.Where(w => w.Supplier_Id == Supplier_ID &&
                        (w.StatusCode ?? string.Empty) == "ACTIVE").OrderBy(o => o.Code).Select(s => new DC_Supplier_ShortVersion
                        { SupplierCode = s.Code.ToUpper(), Supplier_Id = s.Supplier_Id, SupplierName = s.Name.ToUpper() }).FirstOrDefault();

                        TotalAPMCount = context.Accommodation_ProductMapping.AsNoTracking().Where(w => w.Supplier_Id == Supplier_ID && (w.Status.Trim().ToUpper() == "MAPPED" || w.Status.Trim().ToUpper() == "AUTOMAPPED") && w.IsActive == true).Count();
                    }
                    scope.Complete();
                }

                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, 0, string.Empty, "HOTEL", "MAPPINGLITE");

                #endregion

                if (SupplierCodes != null)
                {
                    #region Fetch APM Lite Details
                    List<DC_ProductMappingLite> productMapList = new List<DC_ProductMappingLite>();

                    using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                    new System.Transactions.TransactionOptions() { IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted, Timeout = new TimeSpan(0, 2, 0) }))
                    {
                        using (TLGX_Entities context = new TLGX_Entities())
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.Database.CommandTimeout = 0;

                            productMapList = (from apm in context.Accommodation_ProductMapping.AsNoTracking()
                                              join a in context.Accommodations.AsNoTracking() on apm.Accommodation_Id equals a.Accommodation_Id
                                              where (apm.Status.Trim().ToUpper() == "MAPPED" || apm.Status.Trim().ToUpper() == "AUTOMAPPED") && apm.Supplier_Id == SupplierCodes.Supplier_Id
                                              && apm.IsActive == true
                                              select new DC_ProductMappingLite
                                              {
                                                  SupplierCode = SupplierCodes.SupplierCode,
                                                  SupplierProductCode = apm.SupplierProductReference.ToUpper(),
                                                  MapId = apm.MapId,
                                                  SystemProductCode = a.CompanyHotelID.ToString().ToUpper(),
                                                  TlgxMdmHotelId = (a.TLGXAccoId == null ? string.Empty : a.TLGXAccoId.ToUpper())
                                              }).ToList();
                        }
                        scope.Complete();
                    }

                    List<int> MappedIds = productMapList.Select(s => s.MapId).ToList();
                    #endregion

                    #region Mongo Delete

                    List<int> mapidsinmongo = collection.Find(x => x.SupplierCode == SupplierCodes.SupplierCode).Project(u => u.MapId).ToList();

                    List<int> MapIdsToBeDeleted = (from m in mapidsinmongo
                                                   where !MappedIds.Contains(m)
                                                   select m).ToList();

                    if (MapIdsToBeDeleted != null && MapIdsToBeDeleted.Count > 0)
                    {
                        foreach (var MapId in MapIdsToBeDeleted) { collection.DeleteMany(x => x.MapId == MapId); }
                    }

                    #endregion

                    #region Mongo Upserts

                    if (productMapList != null && productMapList.Count() > 0)
                    {
                        foreach (var product in productMapList)
                        {
                            var filter = Builders<DC_ProductMappingLite>.Filter.Eq(c => c.MapId, product.MapId);
                            var result = collection.ReplaceOne(filter, product, new UpdateOptions { IsUpsert = true });
                        }

                        MongoInsertedCount = MongoInsertedCount + productMapList.Count();
                        UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount, string.Empty, "HOTEL", "MAPPINGLITE");
                    }

                    #endregion

                    UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalAPMCount, MongoInsertedCount, string.Empty, "HOTEL", "MAPPINGLITE");
                }
                else
                {
                    UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount);
                }

                collection = null;
                //_database = null;
            }
            catch (FaultException<ErrorNotifier> ex) { UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount); }
        }

        public void LoadCompanyAccommodationProductMapping(Guid LogId, Guid Supplier_ID)
        {
            int TotalAPMCount = 0;
            int MongoInsertedCount = 0;
            string currSupplier = string.Empty;
            //dynamic p = 1, q = 0, r = p / q; code to insert and test errors and log it to table
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_ConpanyAccommodationMapping>("CompanyAccommodationProductMapping");
                List<DC_ConpanyAccommodationRoomMapping> lstMappedRooms = new List<DC_ConpanyAccommodationRoomMapping>();

                if (LogId == Guid.Empty)
                {
                    LogId = Guid.NewGuid();
                    UpdateDistLogInfo(LogId, PushStatus.INSERT, 0, 0, string.Empty, "COMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");
                }

                #region Index Management
                bool Is_IX_SupplierCode_SupplierProductCode_Exists = false;
                bool Is_IX_SupplierCode_SystemProductCode_Exists = false;
                bool Is_IX_MapId_Exists = false;
                bool Is_IX_SupplierCode_Exists = false;

                var listOfindexes = collection.Indexes.List().ToList();

                foreach (var index in listOfindexes)
                {
                    Newtonsoft.Json.Linq.JObject rss = Newtonsoft.Json.Linq.JObject.Parse(index.ToJson());
                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SupplierProductCode"] != null)
                    {
                        Is_IX_SupplierCode_SupplierProductCode_Exists = true;
                    }

                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SystemProductCode"] != null)
                    {
                        Is_IX_SupplierCode_SystemProductCode_Exists = true;
                    }

                    if ((string)rss["key"]["MapId"] != null)
                    {
                        Is_IX_MapId_Exists = true;
                    }

                    if ((string)rss["key"]["SupplierCode"] != null)
                    {
                        Is_IX_SupplierCode_Exists = true;
                    }
                }

                if (!Is_IX_SupplierCode_SupplierProductCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ConpanyAccommodationMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ConpanyAccommodationMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierCode);
                    CreateIndexModel<DataContracts.Mapping.DC_ConpanyAccommodationMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ConpanyAccommodationMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                if (!Is_IX_SupplierCode_SystemProductCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ConpanyAccommodationMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ConpanyAccommodationMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode);
                    CreateIndexModel<DataContracts.Mapping.DC_ConpanyAccommodationMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ConpanyAccommodationMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                #endregion

                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, 0, 0, string.Empty, "COMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");

                List<DC_Supplier_ShortVersion> SupplierCodes = new List<DC_Supplier_ShortVersion>();
                List<DataContracts.Mapping.DC_CountryMapping> CountryList = new List<DataContracts.Mapping.DC_CountryMapping>();
                using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                new System.Transactions.TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                    Timeout = new TimeSpan(0, 2, 0)
                }))
                {
                    using (TLGX_Entities context = new TLGX_Entities())
                    {
                        context.Database.CommandTimeout = 0;

                        StringBuilder sbSuuplierCodes = new StringBuilder();
                        //sbSuuplierCodes.Append("SELECT upper(Code) as SupplierCode ,Supplier.Supplier_Id as Supplier_Id,UPPER(Name) as SupplierName from Supplier with(nolock) inner join supplier_productCategory with(nolock) on supplier_productCategory.Supplier_Id = Supplier.Supplier_Id where StatusCode ='ACTIVE'  and ProductCategory='Accommodation' and ProductCategorySubType='Hotel' order by SupplierName ");
                        sbSuuplierCodes.Append(" SELECT COUNT(apm.Supplier_Id) as count,apm.Supplier_Id, (select upper(Code) from supplier where supplier_id=apm.Supplier_Id) as SupplierCode, ");
                        sbSuuplierCodes.Append(" (select UPPER(Name) from supplier where supplier_id=apm.Supplier_Id) as SupplierName FROM Accommodation_ProductMapping apm with(nolock)  ");
                        sbSuuplierCodes.Append(" inner join supplier_productCategory spc with(nolock) on spc.Supplier_Id = apm.Supplier_Id where  ");
                        sbSuuplierCodes.Append(" APM.Status in ('MAPPED','AUTOMAPPED') and apm.IsActive = 1 and ProductCategory='Accommodation' and ProductCategorySubType='Hotel' group by apm.Supplier_Id order by count ");
                        SupplierCodes = context.Database.SqlQuery<DC_Supplier_ShortVersion>(sbSuuplierCodes.ToString()).ToList();
                        StringBuilder sbSelectAMPCount = new StringBuilder();
                        sbSelectAMPCount.Append(@" SELECT COUNT(1) FROM Accommodation_ProductMapping apm with(nolock)  join  Accommodation_CompanyVersion av with(nolock)
		                                    on av.Accommodation_Id = apm.Accommodation_Id where Status in ('MAPPED','AUTOMAPPED') and IsActive = 1 ");

                        if (Supplier_ID != Guid.Empty)
                        {
                            sbSelectAMPCount.Append(@" AND Supplier_Id='" + Supplier_ID + "'");
                            SupplierCodes = SupplierCodes.Where(x => x.Supplier_Id == Supplier_ID).ToList();
                        }
                        TotalAPMCount = context.Database.SqlQuery<int>(sbSelectAMPCount.ToString()).SingleOrDefault();
                    }
                    scope.Complete();
                }

                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, 0, string.Empty, "COMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");

                foreach (var SupplierCode in SupplierCodes)
                {
                    currSupplier = SupplierCode.SupplierName;
                    using (TLGX_Entities context = new TLGX_Entities())
                    {
                        context.Database.CommandTimeout = 0;
                        StringBuilder sbCountryVizCount = new StringBuilder();
                        sbCountryVizCount.Append(" select  count(1) as Count, UPPER(av.Country) as [CountryName], (select top 1 a.Country_Id from m_CountryMapping a where a.CountryName = av.Country) as Country_Id ");
                        sbCountryVizCount.Append(" from  Accommodation_ProductMapping apm with (NOLOCK) join  Accommodation_CompanyVersion av  with(nolock) on av.Accommodation_Id = apm.Accommodation_Id ");
                        sbCountryVizCount.Append(" join  Accommodation acc  with(nolock) on acc.Accommodation_Id = av.Accommodation_Id where apm.IsActive = 1  and apm.supplier_id = '" + Supplier_ID + "' ");
                        sbCountryVizCount.Append(" and apm.STATUS in ('MAPPED', 'AUTOMAPPED') group by av.Country order by count ");
                        CountryList = context.Database.SqlQuery<DataContracts.Mapping.DC_CountryMapping>(sbCountryVizCount.ToString()).ToList();
                    }
                    foreach (var Country in CountryList)
                    {
                        var CountryId = Country.Country_Id;
                        var countryName = Country.CountryName;
                        try
                        {
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
                                        , '" + SupplierCode.SupplierCode + @"'				 as [SupplierCode]	  
                                        , ('" + SupplierCode.SupplierCode + @"'	 + '_'+    apm.SupplierProductReference + '_'+  av.CompanyId + '_' +   av.CompanyProductId) as _id 
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
                                        , av.TLGXAccoId                      as [TLGXProduct_Id]
                                        , isnull(acc.IsRoomMappingCompleted,0)  as [IsRoomMappingCompleted] 
										, isnull(apm.IsDirectContract,0)     as [IsDirectContract]   
                                    from  Accommodation_ProductMapping apm with (NOLOCk)
                                    join  Accommodation_CompanyVersion av  with(nolock) 
		                                    on av.Accommodation_Id = apm.Accommodation_Id
                                    join  Accommodation acc  with(nolock) 
		                                    on acc.Accommodation_Id = av.Accommodation_Id
                                    where 
                                        apm.IsActive = 1  and
                                        apm.supplier_id = '" + SupplierCode.Supplier_Id + @"' and 
	                                    apm.STATUS in ('MAPPED', 'AUTOMAPPED') and  apm.Country_Id = '" + CountryId + "'");

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
                               join Accommodation a on a.Accommodation_Id = ASRTM.Accommodation_Id
                                      where 
                                             ASRTM.Supplier_Id = '" + SupplierCode.Supplier_Id + @"' and 
                                             (ASRTMV.SystemMappingStatus =  'AUTOMAPPED' or ASRTMV.UserMappingStatus = 'MAPPED') and a.Country_Id = '" + CountryId + "'");
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

                            List<DC_ConpanyAccommodationMapping> ConpanyAccommodationMappingList = new List<DC_ConpanyAccommodationMapping>();
                            if (productMapList?.Count() > 0)
                            {
                                foreach (var product in productMapList)
                                {
                                    try
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
                                        var result = collection.ReplaceOne(filter, product, new UpdateOptions { IsUpsert = true });
                                        ConpanyAccommodationMappingList.Add(product);
                                        MongoInsertedCount = MongoInsertedCount + 1;
                                        UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount, string.Empty, "COMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");
                                    }
                                    catch (Exception ex)
                                    {
                                        ErrorLog(ex, currSupplier);
                                    }
                                }
                                List<string> MappedIds = lstMappedRooms.Select(s => s.NakshatraRoomMappingId).ToList();
                                //List<string> SupplierCodeIds = productMapList.Select(p => p.SupplierProductCode).ToList();
                                List<DC_ConpanyAccommodationMapping> companyAccoMappingListId = collection.Find(x => x.SupplierCode == SupplierCode.SupplierCode).ToList();
                                List<DC_ConpanyAccommodationRoomMapping> mappedRoomsForSupplierCode = companyAccoMappingListId.SelectMany(x => x.MappedRooms).ToList();
                                List<DC_ConpanyAccommodationRoomMapping> MappedRoomsForSupplierCodeSQL = ConpanyAccommodationMappingList.SelectMany(x => x.MappedRooms).ToList();

                                var Mapids = mappedRoomsForSupplierCode.Distinct().Select(x => x.NakshatraRoomMappingId).ToList();
                                var MapidSQL = MappedRoomsForSupplierCodeSQL.Distinct().Select(x => x.NakshatraRoomMappingId).ToList();
                                var MapIdsToBeDeleted = Mapids.Except(MapidSQL).ToList();

                                // delete logic if not in sql data
                                var CompanyAccommodationProductMappingCollection = _database.GetCollection<BsonDocument>("CompanyAccommodationProductMapping");
                                LogSupplierStatus(currSupplier + " - " + "Deleting " + MapIdsToBeDeleted.Count() + " MapIds for " + countryName, MongoInsertedCount);
                                foreach (var id in MapIdsToBeDeleted)
                                {
                                    try
                                    {
                                        var filter = Builders<BsonDocument>.Filter.Eq("MappedRooms.NakshatraRoomMappingId", Convert.ToString(id));
                                        var update = Builders<BsonDocument>.Update.PullFilter("MappedRooms",
                                            Builders<BsonDocument>.Filter.Eq("NakshatraRoomMappingId", Convert.ToString(id)));
                                        var result = CompanyAccommodationProductMappingCollection.FindOneAndUpdate(filter, update);
                                        UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount, string.Empty, "COMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");
                                    }
                                    catch (Exception ex)
                                    {
                                        ErrorLog(ex, currSupplier);
                                    }
                                }
                            }
                            // loggin successful supplier with last count

                            LogSupplierStatus(currSupplier + " - " + countryName, MongoInsertedCount);
                            UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount, string.Empty, "COMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");
                        }
                        catch (Exception ex)
                        {
                            ErrorLog(ex, currSupplier);
                        }
                    }
                }
                UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalAPMCount, MongoInsertedCount, string.Empty, "COMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");
                collection = null;
                //_database = null;
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                ErrorLog(ex, currSupplier);
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount);
            }
            catch (Exception ex)
            {
                ErrorLog(ex, currSupplier);
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount);
            }
        }


        public void LoadCompanyAccommodationProductMappingCrossVersion(Guid LogId, string Supplier_ID)
        {
            int TotalAPMCount = 0;
            int MongoInsertedCount = 0;
            string currSupplier = string.Empty;
            //dynamic p = 1, q = 0, r = p / q; code to insert and test errors and log it to table
            try
            {
                _database = MongoDBHandler.mDatabase();
                var collection = _database.GetCollection<DataContracts.Mapping.DC_ConpanyAccommodationMapping>("CompanyAccommodationProductMapping");
                List<DC_ConpanyAccommodationRoomMapping> lstMappedRooms = new List<DC_ConpanyAccommodationRoomMapping>();

                if (LogId == Guid.Empty)
                {
                    LogId = Guid.NewGuid();
                    UpdateDistLogInfo(LogId, PushStatus.INSERT, 0, 0, string.Empty, "CROSSCOMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");
                }

                #region Index Management
                bool Is_IX_SupplierCode_SupplierProductCode_Exists = false;
                bool Is_IX_SupplierCode_SystemProductCode_Exists = false;
                bool Is_IX_MapId_Exists = false;
                bool Is_IX_SupplierCode_Exists = false;

                var listOfindexes = collection.Indexes.List().ToList();

                foreach (var index in listOfindexes)
                {
                    Newtonsoft.Json.Linq.JObject rss = Newtonsoft.Json.Linq.JObject.Parse(index.ToJson());
                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SupplierProductCode"] != null)
                    {
                        Is_IX_SupplierCode_SupplierProductCode_Exists = true;
                    }

                    if ((string)rss["key"]["SupplierCode"] != null && (string)rss["key"]["SystemProductCode"] != null)
                    {
                        Is_IX_SupplierCode_SystemProductCode_Exists = true;
                    }

                    if ((string)rss["key"]["MapId"] != null)
                    {
                        Is_IX_MapId_Exists = true;
                    }

                    if ((string)rss["key"]["SupplierCode"] != null)
                    {
                        Is_IX_SupplierCode_Exists = true;
                    }
                }

                if (!Is_IX_SupplierCode_SupplierProductCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ConpanyAccommodationMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ConpanyAccommodationMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierCode);
                    CreateIndexModel<DataContracts.Mapping.DC_ConpanyAccommodationMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ConpanyAccommodationMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                if (!Is_IX_SupplierCode_SystemProductCode_Exists)
                {
                    IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ConpanyAccommodationMapping> IndexBuilder = new IndexKeysDefinitionBuilder<DataContracts.Mapping.DC_ConpanyAccommodationMapping>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierCode).Ascending(_ => _.SupplierProductCode);
                    CreateIndexModel<DataContracts.Mapping.DC_ConpanyAccommodationMapping> IndexModel = new CreateIndexModel<DataContracts.Mapping.DC_ConpanyAccommodationMapping>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                #endregion

                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, 0, 0, string.Empty, "CROSSCOMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");

                List<DC_Supplier_ShortVersion> SupplierCodes = new List<DC_Supplier_ShortVersion>();

                using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                new System.Transactions.TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                    Timeout = new TimeSpan(0, 2, 0)
                }))
                {
                    using (TLGX_Entities context = new TLGX_Entities())
                    {
                        context.Database.CommandTimeout = 0;

                        StringBuilder sbSuuplierCodes = new StringBuilder();
                        //sbSuuplierCodes.Append("SELECT upper(Code) as SupplierCode ,Supplier.Supplier_Id as Supplier_Id,UPPER(Name) as SupplierName from Supplier with(nolock) inner join supplier_productCategory with(nolock) on supplier_productCategory.Supplier_Id = Supplier.Supplier_Id where StatusCode ='ACTIVE'  and ProductCategory='Accommodation' and ProductCategorySubType='Hotel' order by SupplierName ");
                        sbSuuplierCodes.Append(" Select distinct   cast(cast(0 as binary) as uniqueidentifier) as Supplier_Id,CompanyId as SupplierName, CompanyId as SupplierCode  from Accommodation_CompanyVersion with(nolock) order by CompanyId ");

                        SupplierCodes = context.Database.SqlQuery<DC_Supplier_ShortVersion>(sbSuuplierCodes.ToString()).ToList();
                        StringBuilder sbSelectAMPCount = new StringBuilder();
                        sbSelectAMPCount.Append(@" SELECT COUNT(1) FROM Accommodation_CompanyVersion av with(nolock) ");

                        if (Supplier_ID != Convert.ToString(Guid.Empty) && Supplier_ID != "ALL")
                        {
                            sbSelectAMPCount.Append(@" where CompanyId ='" + Supplier_ID + "'");
                            SupplierCodes = SupplierCodes.Where(x => x.SupplierCode == Supplier_ID).ToList();
                        }
                        TotalAPMCount = context.Database.SqlQuery<int>(sbSelectAMPCount.ToString()).SingleOrDefault();
                    }
                    scope.Complete();
                }

                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, 0, string.Empty, "CROSSCOMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");
                List<DataContracts.Mapping.DC_CountryMapping> CountryList = new List<DataContracts.Mapping.DC_CountryMapping>();
                           
                foreach (var SupplierCode in SupplierCodes)
                {
                    using (TLGX_Entities context = new TLGX_Entities())
                    {
                        context.Database.CommandTimeout = 0;
                        StringBuilder sbCountryVizCount = new StringBuilder();
                        sbCountryVizCount.Append(" Select distinct av.Country as [CountryName] from Accommodation_CompanyVersion apm with(nolock)  ");
                        sbCountryVizCount.Append(" join Accommodation_CompanyVersion av on apm.CommonProductId = av.CommonProductId where av.CompanyId <> apm.CompanyId ");
                        CountryList = context.Database.SqlQuery<DataContracts.Mapping.DC_CountryMapping>(sbCountryVizCount.ToString()).ToList();
                    }
                    currSupplier = SupplierCode.SupplierName;
                    foreach (var Country in CountryList)
                    {
                        var countryName = Country.CountryName;
                        try
                        {
                            List<DataContracts.Mapping.DC_ConpanyAccommodationMapping> productMapList = new List<DataContracts.Mapping.DC_ConpanyAccommodationMapping>();
                            #region Generating Query
                            StringBuilder sbSelectAccoMaster = new StringBuilder();
                            // Sending TLGXAccoId as SupplierProductCode from Accommodation_CompanyVersion for CompanySpecificHotelAndRoomTypeMapping.
                            // In return we will return them value in field tlgxProduct_Id
                            sbSelectAccoMaster.Append(@"  
                                   Select apm.CompanyId as SupplierCode
                                        , (apm.CompanyId	 + '_'+    apm.CommonProductId + '_'+  av.CompanyId + '_' +   av.CompanyProductId) as _id
                                        , apm.TLGXAccoId		 as [SupplierProductCode] 
                                        , apm.ProductName		     as [SupplierProductName]
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
                                        , av.TLGXAccoId                      as [TLGXProduct_Id]

                                    from 
                                    	Accommodation_CompanyVersion apm with(nolock)
                                    	join Accommodation_CompanyVersion av on apm.CommonProductId = av.CommonProductId
                                    where av.CompanyId <> apm.CompanyId
                                    and apm.CompanyId = '" + SupplierCode.SupplierCode + "'  and  av.Country = '" + countryName + "' --and apm.CommonProductId = '59465' "
                                    );

                            StringBuilder sbSelectAccoRoomMapped = new StringBuilder();
                            sbSelectAccoRoomMapped.Append(@"  
                                  SELECT
                                		  null as  SupplierRoomId
                                		  ,ASRTM.TlgxAccoRoomId  as SupplierRoomTypeCode
                                          ,ASRTM.RoomName  as SupplierRoomName
                                          ,ASRTM.RoomCategory as SupplierRoomCategory
                                          ,null as SupplierRoomCategoryId 		  
                                		  ,aric.TlgxAccoRoomId as CompanyRoomId
                                		  ,aric.RoomName as CompanyRoomName
                                		  ,aric.CompanyRoomCategory 
                                		  ,null as NakshatraRoomMappingId
                                		  --,ASRTM.Accommodation_CompanyVersion_Id AS ASRTMAccommodation_CompanyVersion_Id --Supplier Accommodation_CompanyVersion_Id
                                          ,ARIC.Accommodation_CompanyVersion_Id
                                		  ,cast(cast(0 as binary) as uniqueidentifier)  as Supplier_Id 
                                          ,ACV.CompanyProductId as SupplierProductId
                                          ,ARIC.CommonRoomId as TLGXCommonRoomId   
                                   FROM 
                                    Accommodation_CompanyVersion ACV with(nolock)
                                    join 
                                	Accommodation_RoomInfo_CompanyVersion ASRTM with(Nolock) 
                                	 On ASRTM.Accommodation_CompanyVersion_Id =  ACV.Accommodation_CompanyVersion_Id
                                   join Accommodation_RoomInfo_CompanyVersion aric with(nolock) On ASRTM.CommonRoomId =  aric.CommonRoomId
                                   join Accommodation_RoomInfo ARI with(nolock) on aric.Accommodation_RoomInfo_Id = ARI.Accommodation_RoomInfo_Id and ARI.IsActive = 1
                                    where 
                                    ASRTM.CommonRoomId is not null and 
                                	aric.Accommodation_CompanyVersion_Id <> ASRTM.Accommodation_CompanyVersion_Id
                                	AND ACV.CompanyId =  '" + SupplierCode.SupplierCode + @"'   and  ACV.Country = '" + countryName + "'  --and ACV.CommonProductId = '28254' ");

                            #endregion

                            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                            new System.Transactions.TransactionOptions()
                            {
                                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                                Timeout = new TimeSpan(0, 15, 0)
                            }))
                            {
                                using (TLGX_Entities context = new TLGX_Entities())
                                {
                                    context.Configuration.AutoDetectChangesEnabled = false;
                                    context.Database.CommandTimeout = 0;
                                    productMapList = context.Database.SqlQuery<DataContracts.Mapping.DC_ConpanyAccommodationMapping>(sbSelectAccoMaster.ToString()).ToList();

                                    lstMappedRooms = context.Database.SqlQuery<DataContracts.Mapping.DC_ConpanyAccommodationRoomMapping>(sbSelectAccoRoomMapped.ToString()).ToList();

                                }
                                scope.Complete();
                            }

                            List<DC_ConpanyAccommodationMapping> ConpanyAccommodationMappingList = new List<DC_ConpanyAccommodationMapping>();
                            if (productMapList?.Count() > 0)
                            {
                                foreach (var product in productMapList)
                                {
                                    try
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
                                        var result = collection.ReplaceOne(filter, product, new UpdateOptions { IsUpsert = true });
                                        ConpanyAccommodationMappingList.Add(product);
                                        MongoInsertedCount = MongoInsertedCount + 1;
                                        UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount, string.Empty, "CROSSCOMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");
                                    }
                                    catch (Exception ex)
                                    {
                                        ErrorLog(ex, currSupplier);
                                    }
                                }
                                // removed MapIds deletion code from cross version after discussion with gaurav
                                //List<string> MappedIds = lstMappedRooms.Select(s => s.NakshatraRoomMappingId).ToList();
                                //List<string> SupplierCodeIds = productMapList.Select(p => p.SupplierProductCode).ToList();
                                //List<DC_ConpanyAccommodationMapping> companyAccoMappingListId = collection.Find(x => x.SupplierCode == SupplierCode.SupplierCode).ToList();
                                //List<DC_ConpanyAccommodationRoomMapping> mappedRoomsForSupplierCode = companyAccoMappingListId.SelectMany(x => x.MappedRooms).ToList();
                                //List<DC_ConpanyAccommodationRoomMapping> MappedRoomsForSupplierCodeSQL = ConpanyAccommodationMappingList.SelectMany(x => x.MappedRooms).ToList();
                                //var Mapids = mappedRoomsForSupplierCode.Distinct().Select(x => x.NakshatraRoomMappingId).ToList();
                                //var MapidSQL = MappedRoomsForSupplierCodeSQL.Distinct().Select(x => x.NakshatraRoomMappingId).ToList();
                                //var MapIdsToBeDeleted = Mapids.Except(MapidSQL).ToList();

                                //// delete logic if not in sql data
                                //var CompanyAccommodationProductMappingCollection = _database.GetCollection<BsonDocument>("CompanyAccommodationProductMapping");
                                //LogSupplierStatus(currSupplier + " - " + "Deleting " + MapIdsToBeDeleted.Count() + " MapIds for " + Convert.ToString(countryName) + " in CROSSCOMPANYACCOMMODATIONPRODUCTMAPPING", MongoInsertedCount);
                                //foreach (var id in MapIdsToBeDeleted)
                                //{
                                //    try
                                //    {
                                //        var filter = Builders<BsonDocument>.Filter.Eq("MappedRooms.NakshatraRoomMappingId", Convert.ToString(id));
                                //        var update = Builders<BsonDocument>.Update.PullFilter("MappedRooms",
                                //            Builders<BsonDocument>.Filter.Eq("NakshatraRoomMappingId", Convert.ToString(id)));
                                //        var result = CompanyAccommodationProductMappingCollection.FindOneAndUpdate(filter, update);
                                //        UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount, string.Empty, "CROSSCOMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");
                                //    }
                                //    catch (Exception ex)
                                //    {
                                //        ErrorLog(ex, currSupplier);
                                //    }
                                //}
                            }
                            // logging successful supplier with last count
                            LogSupplierStatus(currSupplier + "-"+ countryName + "-CROSSCOMPANYACCOMMODATIONPRODUCTMAPPING", MongoInsertedCount);
                            UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount, string.Empty, "CROSSCOMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");
                        }
                        catch (Exception ex)
                        {
                            ErrorLog(ex, currSupplier);
                        }
                    }                        
                }
                UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalAPMCount, MongoInsertedCount, string.Empty, "CROSSCOMPANYACCOMMODATIONPRODUCTMAPPING", "MAPPING");
                collection = null;
                //_database = null;
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                ErrorLog(ex, currSupplier);
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount);
            }
            catch (Exception ex)
            {
                ErrorLog(ex, currSupplier);
                UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount);
            }
        }

        private void LogSupplierStatus(string SupplierName, int MongoInsertedCount)
        {
            StringBuilder strLogSupplierStatus = new StringBuilder();
            try
            {
                strLogSupplierStatus.Append("Insert Into CompanySpecificSupplierLog values('" + SupplierName + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss") + "'," + MongoInsertedCount + ")");
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Database.CommandTimeout = 0;
                    context.Database.ExecuteSqlCommand(strLogSupplierStatus.ToString());
                }
            }
            catch
            {

            }
        }

        private void ErrorLog(Exception ex, string currSupplier)
        {
            StringBuilder errorlog = new StringBuilder();
            try
            {
                errorlog.Append("Insert Into error_log values('" + Convert.ToString(ex.Message) + "','" + Convert.ToString(ex.ToString()) + "','" + Convert.ToString(ex.StackTrace) + "','" + DateTime.Now.ToString("yyyy-MM-dd HH:MM:ss") + "','" + Convert.ToString(currSupplier) + "')");
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Database.CommandTimeout = 0;
                    context.Database.ExecuteSqlCommand(errorlog.ToString());
                }
            }
            catch
            {

            }
        }

        public void LoadMCONBySupplier(Guid LogId, Guid Supplier_ID)
        {
            #region Params

            int TotalAPMCount = 0;
            int MongoInsertedCount = 0;
            bool Is_IX_SupplierName_Exists = false;
            string currSupplier = string.Empty;

            #endregion

            try
            {
                _database = MongoDBHandler.mDatabase();
                //var collection = _database.GetCollection<DC_SupplierMCONS>("SupplierMCONS");
                var collection = _database.GetCollection<DataContracts.Masters.DC_Supplier>("Supplier");


                if (LogId == Guid.Empty)
                {
                    LogId = Guid.NewGuid();
                    UpdateDistLogInfo(LogId, PushStatus.INSERT, 0, 0, string.Empty, "HOTEL", "MCONMAPPING");
                }

                #region Index Management

                var listOfindexes = collection.Indexes.List().ToList();

                foreach (var index in listOfindexes)
                {
                    JObject rss = JObject.Parse(index.ToJson());

                    if ((string)rss["key"]["SupplierName"] != null) { Is_IX_SupplierName_Exists = true; }

                }

                if (!Is_IX_SupplierName_Exists)
                {
                    IndexKeysDefinitionBuilder<DC_Supplier> IndexBuilder = new IndexKeysDefinitionBuilder<DC_Supplier>();
                    var keys = IndexBuilder.Ascending(_ => _.SupplierName).Ascending(_ => _.SupplierName);
                    CreateIndexModel<DC_Supplier> IndexModel = new CreateIndexModel<DC_Supplier>(keys);
                    collection.Indexes.CreateOneAsync(IndexModel);
                }

                #endregion

                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, 0, 0, string.Empty, "HOTEL", "MCONMAPPING");

                #region Fetch Supplier Additional Details

                List<DC_Supplier_ShortVersion> SupplierCodes = new List<DC_Supplier_ShortVersion>();

                using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew,
                new System.Transactions.TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                    Timeout = new TimeSpan(0, 2, 0)
                }))
                {
                    using (TLGX_Entities context = new TLGX_Entities())
                    {
                        context.Database.CommandTimeout = 0;

                        StringBuilder sbSuuplierCodes = new StringBuilder();
                        sbSuuplierCodes.Append(@" SELECT distinct s.Supplier_Id, upper(s.Code) as SupplierCode, upper(s.Name) as SupplierName from m_SupplierImportAttributes attr with(nolock) 
                                                join
                                                Supplier s on attr.Supplier_Id = s.Supplier_Id
                                                where attr.Entity = 'RoomType' and attr.Status = 'Active' and s.StatusCode = 'Active' ");

                        SupplierCodes = context.Database.SqlQuery<DC_Supplier_ShortVersion>(sbSuuplierCodes.ToString()).ToList();
                        StringBuilder sbSelectAMPCount = new StringBuilder();
                        sbSelectAMPCount.Append(@" SELECT COUNT(1) from m_SupplierImportAttributes attr with(nolock) 
                                                join
                                                Supplier s on attr.Supplier_Id = s.Supplier_Id
                                                where attr.Entity = 'RoomType' and attr.Status = 'Active'   ");

                        if (Supplier_ID != Guid.Empty)
                        {

                            SupplierCodes = SupplierCodes.Where(x => x.Supplier_Id == Supplier_ID).ToList();
                        }

                        TotalAPMCount = context.Database.SqlQuery<int>(sbSelectAMPCount.ToString()).SingleOrDefault();
                    }
                    scope.Complete();
                }


                UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, 0, string.Empty, "HOTEL", "MCONMAPPING");

                #endregion

                if (SupplierCodes != null)
                {
                    foreach (var SupplierCode in SupplierCodes)
                    {
                        currSupplier = SupplierCode.SupplierName;
                        try
                        {
                            List<DataContracts.Mapping.DC_SupplierMCONS> supplierMCONList = new List<DataContracts.Mapping.DC_SupplierMCONS>();
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
                                    
                                        SELECT PvTable.SupplierName,PvTable.HoldInsertOnlineRoomTypeMappingData as HoldInsertOnlineRoomTypeMappingData,PvTable.HoldReadOnlineRoomTypeMappingData as HoldReadOnlineRoomTypeMappingData from 

                                        (
                                            SELECT attrv.AttributeName,attrv.AttributeValue, '" + SupplierCode.SupplierName + @"' as SupplierName FROM m_SupplierImportAttributes attr with(nolock)
                                            join m_SupplierImportAttributeValues attrv with(nolock)
                                            On attr.SupplierImportAttribute_Id = attrv.SupplierImportAttribute_Id
                                            where attr.Entity = 'RoomType' 
                                            and attrv.AttributeValue in ('HoldReadOnlineRoomTypeMappingData','HoldInsertOnlineRoomTypeMappingData')
                                            and attr.Supplier_Id = '" + SupplierCode.Supplier_Id + @"'
                                            ) as tb
                                            Pivot 
                                            (
                                            max(tb.AttributeName)
                                            for tb.AttributeValue in ([HoldReadOnlineRoomTypeMappingData],[HoldInsertOnlineRoomTypeMappingData])
                                            ) as PvTable
                                             
                                        ");


                                #endregion

                                using (TLGX_Entities context = new TLGX_Entities())
                                {
                                    context.Configuration.AutoDetectChangesEnabled = false;
                                    context.Database.CommandTimeout = 0;
                                    supplierMCONList = context.Database.SqlQuery<DataContracts.Mapping.DC_SupplierMCONS>(sbSelectAccoMaster.ToString()).ToList();

                                }
                                scope.Complete();
                            }

                            DC_Supplier supplier = collection.Find<DC_Supplier>(x => x.SupplierName == SupplierCode.SupplierName).FirstOrDefault();


                            if (supplierMCONList?.Count() > 0 && supplier != null)
                            {
                                foreach (var SupplierMcon in supplierMCONList)
                                {
                                    try
                                    {

                                        var filter = Builders<DC_Supplier>.Filter.Eq(c => c.SupplierName, SupplierMcon.SupplierName);
                                        supplier.MCON = new DC_SupplierMCONS();
                                        supplier.MCON = SupplierMcon;
                                        var result = collection.ReplaceOne(filter, supplier, new UpdateOptions { IsUpsert = true });
                                        MongoInsertedCount = MongoInsertedCount + 1;
                                        UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount, string.Empty, "HOTEL", "MCONMAPPING");
                                    }
                                    catch (Exception ex)
                                    {
                                        ErrorLog(ex, currSupplier);
                                    }
                                }
                            }
                            // loggin successful supplier with last count
                            LogSupplierStatus(currSupplier, MongoInsertedCount);
                            UpdateDistLogInfo(LogId, PushStatus.RUNNNING, TotalAPMCount, MongoInsertedCount, string.Empty, "HOTEL", "MCONMAPPING");
                        }
                        catch (Exception ex)
                        {
                            ErrorLog(ex, currSupplier);
                        }
                    }

                    UpdateDistLogInfo(LogId, PushStatus.COMPLETED, TotalAPMCount, MongoInsertedCount, string.Empty, "HOTEL", "MCONMAPPING");
                }
                else
                {
                    UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount);
                }

                collection = null;
                //_database = null;
            }
            catch (FaultException<ErrorNotifier> ex) { UpdateDistLogInfo(LogId, PushStatus.ERROR, TotalAPMCount, MongoInsertedCount); }
        }

        #endregion


        #region Activity Mapping

        public void LoadActivityMapping(Guid LogId)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
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
                        //_database = null;
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
                using (TLGX_Entities context = new TLGX_Entities())
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
                    //_database = null;
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
                using (TLGX_Entities context = new TLGX_Entities())
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
                using (TLGX_Entities context = new TLGX_Entities())
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
                using (TLGX_Entities context = new TLGX_Entities())
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
                //using (TLGX_Entities context = new TLGX_Entities())
                //{
                //    _database = MongoDBHandler.mDatabase();
                //    context.Database.CommandTimeout = 0;
                //    context.Configuration.AutoDetectChangesEnabled = false;
                //    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");
                //    //var ActivityList = (from a in context.Activity_Flavour select new { Activity_Flavour_Id = a.Activity_Flavour_Id, CommonProductNameSubType_Id = a.CommonProductNameSubType_Id }).ToList();
                //    var ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                //                        join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                //                        where a.CityCode != null && (spm.IsActive ?? false) == true
                //                        && spm.SupplierName == "viator"
                //                        select new { Activity_Flavour_Id = a.Activity_Flavour_Id, CommonProductNameSubType_Id = a.CommonProductNameSubType_Id }).ToList();
                //    int iTotalCount = ActivityList.Count();
                //    int iCounter = 0;
                //    foreach (var Activity in ActivityList)
                //    {
                //        try
                //        {
                //            var InterestTypeArray = (context.Activity_CategoriesType.Where(w => w.Activity_Flavour_Id == Activity.Activity_Flavour_Id)).Select(s => s.SystemInterestType).Distinct().ToArray();
                //            var InterestType = string.Join(",", InterestTypeArray);
                //            var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity.CommonProductNameSubType_Id));
                //            var UpdateData = Builders<DataContracts.Activity.ActivityDefinition>.Update.Set(x => x.InterestType, InterestType);
                //            var updateResult = collection.FindOneAndUpdate(filter, UpdateData);

                //            iCounter++;

                //        }
                //        catch (Exception e)
                //        {
                //            continue;
                //        }
                //    }
                //}
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
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    _database = MongoDBHandler.mDatabase();
                    var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");
                    //var ActivityList = (from a in context.Activity_Flavour select new { Activity_Flavour_Id = a.Activity_Flavour_Id, CommonProductNameSubType_Id = a.CommonProductNameSubType_Id }).ToList();
                    var ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                        join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                        where a.CityCode != null && (spm.IsActive ?? false) == true
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
                using (TLGX_Entities context = new TLGX_Entities())
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
                using (TLGX_Entities context = new TLGX_Entities())
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
                using (TLGX_Entities context = new TLGX_Entities())
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
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Database.CommandTimeout = 0;

                    List<Activity_Flavour> ActivityList;

                    if (Activity_Flavour_Id == Guid.Empty)
                    {
                        ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                        join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                        join sup in context.Suppliers.AsNoTracking() on spm.Supplier_ID equals sup.Supplier_Id
                                        where sup.StatusCode == "ACTIVE" && a.CityCode != null && (spm.IsActive ?? false) == true
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

                        //Delete inactive records
                        if (Activity_Flavour_Id == Guid.Empty)
                        {
                            _database = MongoDBHandler.mDatabase();
                            var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");

                            var MappedIds = ActivityList.Select(s => Convert.ToInt32(s.CommonProductNameSubType_Id)).ToList();
                            var mapidsinmongo = collection.Find(x => true).Project(u => new { u.SystemActivityCode }).ToList();
                            var MapIdsToBeDeleted = (from m in mapidsinmongo
                                                     where !MappedIds.Contains(m.SystemActivityCode)
                                                     select m).ToList();
                            if (MapIdsToBeDeleted != null && MapIdsToBeDeleted.Count > 0)
                            {
                                foreach (var obj in MapIdsToBeDeleted)
                                {
                                    var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, obj.SystemActivityCode);
                                    collection.DeleteMany(filter);

                                    //Call to Generate message static method send Messages.
                                    SendToKafka.SendMessage(obj.SystemActivityCode, "ACTIVITY", "DELETE");
                                }
                            }
                        }
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
            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DataContracts.Activity.ActivityDefinition>("ActivityDefinitions");
            int iCounter = 0;
            int totalcount = ActivityList.Count();

            foreach (var Activity in ActivityList)
            {
                bool Success = false;

                DataContracts.Activity.ActivityDefinition newActivity = new DataContracts.Activity.ActivityDefinition();

                using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions()
                {
                    IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                    Timeout = new TimeSpan(0, 2, 0)
                }))
                {
                    try
                    {
                        using (TLGX_Entities context = new TLGX_Entities())
                        {
                            context.Configuration.AutoDetectChangesEnabled = false;
                            context.Database.CommandTimeout = 0;


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

                            foreach (var CT in ActivityCT)
                            {
                                if (!string.IsNullOrWhiteSpace(CT.SystemProductCategorySubType))
                                {
                                    CT.SystemProductCategorySubType = CT.SystemProductCategorySubType.Split('-')[0].Trim();
                                }

                                if (!string.IsNullOrWhiteSpace(CT.SystemProductType))
                                {
                                    if (CT.SystemProductType.ToUpper() == "ice - Outdoor Activities".ToUpper())
                                    {
                                        CT.SystemProductType = "Outdoor Activities";
                                    }
                                    else if (CT.SystemProductType.ToUpper() == "Multi-day and Extended Tours".ToUpper())
                                    {
                                        CT.SystemProductType = CT.SystemProductType;
                                    }
                                    else
                                    {
                                        CT.SystemProductType = CT.SystemProductType.Split('-')[0].Trim();
                                    }
                                }
                            }

                            var ActivityDP = (from a in context.Activity_DeparturePoints.AsNoTracking()
                                              where a.Activity_Flavor_ID == Activity.Activity_Flavour_Id
                                              select a).ToList();

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

                            newActivity.CategoryGroup = (from act in ActivityCT
                                                         select new DataContracts.Activity.ActivityCategory
                                                         {
                                                             Category = act.SystemProductCategorySubType,
                                                             InterestType = act.SystemInterestType,
                                                             SubType = act.SystemProductNameSubType,
                                                             Type = act.SystemProductType
                                                         }).ToList();

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
                                ValidFrom = s.Price_ValidFrom == null ? string.Empty : string.Format("{0:dd-MM-yyyy}", s.Price_ValidFrom),
                                ValidTo = s.Price_ValidTo == null ? string.Empty : string.Format("{0:dd-MM-yyyy}", s.Price_ValidTo),
                                PackageSupplier = s.PackageSupplier
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
                                                             OperatingFromDate = ODljS == null ? string.Empty : string.Format("{0:dd-MM-yyyy}", ODljS.FromDate),
                                                             OperatingToDate = ODljS == null ? string.Empty : string.Format("{0:dd-MM-yyyy}", ODljS.ToDate),

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

                            //Activity TLGXDisplaySubType Setting
                            newActivity.TLGXDisplaySubType = Activity.TLGXDisplaySubType;
                            // Activity Display Name
                            newActivity.DisplayName = Activity.DisplayName;


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
                        Success = true;
                    }
                    catch (Exception ex)
                    {
                        Success = false;
                    }
                    scope.Complete();
                    scope.Dispose();
                }

                if (Success)
                {
                    var filter = Builders<DataContracts.Activity.ActivityDefinition>.Filter.Eq(c => c.SystemActivityCode, Convert.ToInt32(Activity.CommonProductNameSubType_Id));
                    collection.ReplaceOne(filter, newActivity, new UpdateOptions { IsUpsert = true });

                    //Call to Generate message static method send Messages.
                    SendToKafka.SendMessage(newActivity, "ACTIVITY", "POST");

                    newActivity = null;

                    //Update Status
                    if (!string.IsNullOrWhiteSpace(log_id))
                    {
                        if (iCounter % 100 == 0)
                            UpdateDistLogInfo(Guid.Parse(log_id), PushStatus.RUNNNING, totalcount, iCounter, string.Empty, string.Empty, string.Empty);
                    }
                    iCounter++;
                }
            }

            collection = null;
            //_database = null;
            return true;
        }
        #endregion
        /// <summary>
        /// get data by Supplier to push into mongo
        /// </summary>
        /// <param name="suppliername"></param>
        public void LoadActivityDefinitionBySupplier(string log_id, string suppliername)
        {
            List<Activity_Flavour> ActivityList = new List<Activity_Flavour>();

            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions()
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                Timeout = new TimeSpan(0, 2, 0)
            }))
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Database.CommandTimeout = 0;

                    ActivityList = (from a in context.Activity_Flavour.AsNoTracking()
                                    join spm in context.Activity_SupplierProductMapping.AsNoTracking() on a.Activity_Flavour_Id equals spm.Activity_ID
                                    where a.CityCode != null && (spm.IsActive ?? false) == true
                                    && spm.SupplierName == suppliername
                                    select a).ToList();


                }
                scope.Complete();
                scope.Dispose();
            }

            int totalCount = ActivityList.Count;
            if (totalCount > 0)
            {
                UpdateDistLogInfo(Guid.Parse(log_id), PushStatus.RUNNNING, totalCount, 0, string.Empty, string.Empty, string.Empty);
                LoadActivityData(ActivityList, log_id);
                UpdateDistLogInfo(Guid.Parse(log_id), PushStatus.COMPLETED, totalCount, totalCount, string.Empty, string.Empty, string.Empty);
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

        private void RemoveActivityDefinitionBySupplier(string suppliername, string Activity_Flavour_Id = null)
        {
            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
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
                            collection.DeleteOne(filter);
                            //Call to Generate message static method send Messages.
                            SendToKafka.SendMessage(Activity.CommonProductNameSubType_Id, "ACTIVITY", "DELETE");
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
                using (TLGX_Entities context = new TLGX_Entities())
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
                using (TLGX_Entities context = new TLGX_Entities())
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
                    //_database = null;
                }
            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                using (TLGX_Entities context = new TLGX_Entities())
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
                using (TLGX_Entities context = new TLGX_Entities())
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
            using (TLGX_Entities context = new TLGX_Entities())
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
        //public static int CountDifferences<Int>(this List<Int> list1, List<Int> list2)
        //{
        //    if (list1.Count != list2.Count)
        //        throw new ArgumentException("Lists must have same # elements", "list2");
        //    return list1.Where((t, i) => !Equals(t, list2[i])).Count();
        //}
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
            Supplier_Id = string.IsNullOrEmpty(Supplier_Id) ? Guid.Empty.ToString() : Supplier_Id;

            if (status == PushStatus.INSERT)
            {
                setNewStatus.Append("INSERT INTO DistributionLayerRefresh_Log(Id,Element,Type,Create_Date,Create_User,Status,Supplier_Id) VALUES(");
                setNewStatus.Append("'" + LogId + "','" + Element + "','" + Type + "', GETDATE() " + ", '" + "MONGOPUSH" + "', '" + Status + "', '" + Supplier_Id + "');");
            }
            else if (status == PushStatus.ERROR)
            {
                setNewStatus.Append("UPDATE DistributionLayerRefresh_Log SET Status ='" + Status + "',  Edit_Date = getDate(),  Edit_User='" + EditUser + "' WHERE Id= '" + LogId + "';");
            }
            else
            {
                setNewStatus.Append("UPDATE DistributionLayerRefresh_Log SET TotalCount = " + totalCount.ToString() + " , MongoPushCount = " + insertedCount.ToString() + ", Status ='" + Status + "',  Edit_Date = getDate(),  Edit_User='" + EditUser + "' WHERE Id= '" + LogId + "';");
            }

            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    context.Database.CommandTimeout = 0;
                    context.Database.ExecuteSqlCommand(setNewStatus.ToString());
                }
            }
            catch
            {

            }

            setNewStatus = null;
        }

        #endregion

        #region VisaDefinition

        public void UpdateVisaDefinition(Guid Logid)
        {
            try
            {
                int counter = 0;

                UpdateDistLogInfo(Logid, PushStatus.INSERT, 0, 0, Logid.ToString(), "VISA", "MAPPING");

                UpdateDistLogInfo(Logid, PushStatus.RUNNNING, 0, 0, Logid.ToString(), "VISA", "MAPPING");



                _database = MongoDBHandler.mDatabase();
                _database.DropCollection("VisaMapping");

                var CollecionVisaCountries = _database.GetCollection<BsonDocument>("VisaCountryDetail");
                var VisaMappingCollection = _database.GetCollection<VisaDefinition>("VisaMapping");

                ProjectionDefinition<BsonDocument> project = Builders<BsonDocument>.Projection.Include("SupplierCode");
                project = project.Exclude("_id");
                project = project.Include("SupplierName");
                project = project.Include("CallType");
                project = project.Include("VisaDetail");

                int totalCount = CollecionVisaCountries == null ? 0 : int.Parse(CollecionVisaCountries.CountDocuments(new BsonDocument()).ToString());

                var CollecionVisaCountriesFiltered = CollecionVisaCountries.Find(s => true).Project(project).ToList();

                UpdateDistLogInfo(Logid, PushStatus.RUNNNING, totalCount, 0, Logid.ToString(), "VISA", "MAPPING");

                List<VisaDefinition> ListVisaDefinitions = new List<VisaDefinition>();

                //Transformation
                foreach (var item in CollecionVisaCountriesFiltered)
                {
                    var JsonObject = item.ToJson();

                    JObject VisaJson = JObject.Parse(JsonObject);

                    VisaDefinition objVisaDefinition = new VisaDefinition();

                    objVisaDefinition.SupplierCode = (string)VisaJson["SupplierCode"];
                    objVisaDefinition.CallType = (string)VisaJson["CallType"];
                    objVisaDefinition.SupplierName = (string)VisaJson["SupplierName"];
                    var strVisaDetail = VisaJson["VisaDetail"].ToString();

                    if (!string.IsNullOrEmpty(strVisaDetail))
                    {
                        objVisaDefinition.VisaDetail = new List<VisaDetail>();
                        VisaDetail objVisaDetail = new VisaDetail();

                        JObject JobjectVisaDetail = JObject.Parse(strVisaDetail);

                        objVisaDetail.CountryCode = (string)VisaJson["VisaDetail"]["CountryCode"];
                        objVisaDetail.CountryName = (string)VisaJson["VisaDetail"]["CountryName"];

                        #region Visa
                        objVisaDetail.Visa = new List<Visa>();

                        Visa objVisa = new Visa();

                        objVisa.AdditionalInfo = (string)VisaJson["VisaDetail"]["Visa"]["AdditionalInfo"];
                        var totalVisaInformationNodes = VisaJson["VisaDetail"]["Visa"]["VisaInformation"].ToList().Count;

                        if (totalVisaInformationNodes > 0)
                        {
                            objVisa.VisaInformation = new List<VisaInformation>();
                            for (int i = 0; i < totalVisaInformationNodes; i++)
                            {
                                VisaInformation objVisaInformationNew = new VisaInformation();
                                objVisaInformationNew.TerritoryCity = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["TerritoryCity"];

                                // fill CategoryForms Object
                                objVisaInformationNew.CategoryForms = new CategoryForms();
                                objVisaInformationNew.CategoryForms.CategoryForm = new List<CategoryForm>();
                                if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"] != null &&
                                    VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"].ToList().Count > 0)
                                {
                                    var TypeOfCategoryForm = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"].GetType();
                                    int TotalCategoryForms = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"].ToList().Count;
                                    for (int c = 0; c < TotalCategoryForms; c++)
                                    {
                                        if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"].GetType().Name.ToUpper() == "ARRAY")
                                        {
                                            var TypeOfCategoryCodeNode = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"][c]["CategoryCode"].GetType();

                                            if (TypeOfCategoryCodeNode.Name.ToUpper() == "JARRAY")
                                            {
                                                CategoryForm objCategoryFormNew = new CategoryForm();
                                                objCategoryFormNew.CategoryCode = new List<string>();
                                                objCategoryFormNew.Form = new List<string>();
                                                objCategoryFormNew.FormPath = new List<string>();
                                                int TotalCategoryNodes = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"][c]["CategoryCode"].ToList().Count;
                                                for (int r = 0; r < TotalCategoryNodes; r++)
                                                {
                                                    objCategoryFormNew.CategoryCode.Add(Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"][c]["CategoryCode"][r]));
                                                }
                                                int TotalFormNodes = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"][c]["Form"].ToList().Count;
                                                for (int r = 0; r < TotalFormNodes; r++)
                                                {
                                                    objCategoryFormNew.Form.Add(Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"][c]["Form"][r]));
                                                }
                                                int TotalFormPathNodes = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"][c]["FormPath"].ToList().Count;
                                                for (int r = 0; r < TotalFormPathNodes; r++)
                                                {
                                                    objCategoryFormNew.FormPath.Add(Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"][c]["FormPath"][r]));
                                                }
                                                objVisaInformationNew.CategoryForms.CategoryForm.Add(objCategoryFormNew);
                                            }
                                            else
                                            {  // It is a object containing single value
                                                CategoryForm objCategoryFormNew = new CategoryForm();
                                                objCategoryFormNew.CategoryCode = new List<string>();
                                                objCategoryFormNew.Form = new List<string>();
                                                objCategoryFormNew.FormPath = new List<string>();
                                                objCategoryFormNew.CategoryCode.Add((string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"][c]["CategoryCode"]);
                                                objCategoryFormNew.Form.Add((string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"][c]["Form"]);
                                                objCategoryFormNew.FormPath.Add((string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryForms"]["CategoryForm"][c]["FormPath"]);
                                                objVisaInformationNew.CategoryForms.CategoryForm.Add(objCategoryFormNew);
                                            }
                                        }
                                        else
                                        {

                                        }

                                    }
                                }


                                #region CategoryFees



                                if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"] != null &&
                                    VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"].ToList().Count > 0)
                                {
                                    objVisaInformationNew.CategoryFees = new List<VisaCategoryFees>();
                                    objVisaInformationNew.CategoryFees.Add(new VisaCategoryFees());
                                    objVisaInformationNew.CategoryFees[0].Category = new List<List<VisaCategoryFee>>();



                                    var TypeOfCategoryFees = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"].GetType();

                                    if (TypeOfCategoryFees.Name.ToUpper() == "JARRAY")
                                    {
                                        int TotalCategoryFees = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"].ToList().Count;

                                        for (int p = 0; p < TotalCategoryFees; p++)
                                        {
                                            var TypeOfCategoryFee = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"][p]["CategoryFee"].GetType();
                                            if (TypeOfCategoryFee.Name.ToUpper() == "JARRAY")
                                            {
                                                List<VisaCategoryFee> VisaCategoryfeeList = new List<VisaCategoryFee>();

                                                int TotalCategoryFeesNode = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"][p]["CategoryFee"].ToList().Count;
                                                for (int g = 0; g < TotalCategoryFeesNode; g++)
                                                {
                                                    VisaCategoryfeeList.Add(new VisaCategoryFee
                                                    {
                                                        Category = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"][p]["CategoryFee"][g]["Category"]),
                                                        CategoryCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"][p]["CategoryFee"][g]["CategoryCode"]),
                                                        CategoryFeeAmountINR = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"][p]["CategoryFee"][g]["CategoryFeeAmountINR"]),
                                                        CategoryFeeAmountOther = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"][p]["CategoryFee"][g]["CategoryFeeAmountOther"])
                                                    });
                                                }
                                                objVisaInformationNew.CategoryFees[0].Category.Add(VisaCategoryfeeList);

                                            }
                                            else
                                            {

                                                List<VisaCategoryFee> VisaCategoryfeeList = new List<VisaCategoryFee>();
                                                VisaCategoryfeeList.Add(new VisaCategoryFee
                                                {
                                                    Category = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"][p]["CategoryFee"]["Category"]),
                                                    CategoryCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"][p]["CategoryFee"]["CategoryCode"]),
                                                    CategoryFeeAmountINR = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"][p]["CategoryFee"]["CategoryFeeAmountINR"]),
                                                    CategoryFeeAmountOther = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"][p]["CategoryFee"]["CategoryFeeAmountOther"])
                                                });

                                                objVisaInformationNew.CategoryFees[0].Category.Add(VisaCategoryfeeList);

                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"]["CategoryFee"].GetType().Name.ToUpper() != "JARRAY")
                                        {// it is object
                                            List<VisaCategoryFee> VisaCategoryfeeList = new List<VisaCategoryFee>();
                                            VisaCategoryfeeList.Add(new VisaCategoryFee
                                            {
                                                Category = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"]["CategoryFee"]["Category"]),
                                                CategoryCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"]["CategoryFee"]["CategoryCode"]),
                                                CategoryFeeAmountINR = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"]["CategoryFee"]["CategoryFeeAmountINR"]),
                                                CategoryFeeAmountOther = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["CategoryFees"]["CategoryFee"]["CategoryFeeAmountOther"])
                                            });

                                            objVisaInformationNew.CategoryFees[0].Category.Add(VisaCategoryfeeList);
                                        }
                                        else
                                        {

                                        }
                                    }
                                }

                                #endregion




                                // VisaInfo Object fill
                                if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"] != null &&
                                    VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"].ToList().Count > 0)
                                {
                                    objVisaInformationNew.VisaInfo = new List<VisaInfo>();
                                    VisaInfo objVisaInfoInew = new VisaInfo();
                                    if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"].ToArray().Length > 0)
                                    {
                                        int TotalChildVisaNodes = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"].ToArray().Length;
                                        objVisaInfoInew.VisaInformation = new List<VisaInformationNode>();
                                        objVisaInfoInew.VisaGeneralInformation = new List<VisaGeneralInformation>();

                                        VisaGeneralInformation objVisaGeneralInformationNew = new VisaGeneralInformation();
                                        if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaGeneralInformation"] != null &&
                                            VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaGeneralInformation"]["GeneralInfo"].GetType().Name.ToUpper() == "JOBJECT")
                                        {

                                        }
                                        else
                                        {
                                            if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaGeneralInformation"] != null)
                                            {
                                                objVisaGeneralInformationNew.GeneralInfo = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaGeneralInformation"]["GeneralInfo"];
                                            }

                                        }

                                        objVisaInfoInew.VisaGeneralInformation.Add(objVisaGeneralInformationNew);

                                        VisaInformationNode objVisaInformation2New = new VisaInformationNode();


                                        var TypeOfInformation = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"].GetType();
                                        if (TypeOfInformation.Name.ToUpper() == "JOBJECT")
                                        {
                                            if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"] != null &&
                                                VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["InformationLink"] != null &&
                                                VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["InformationLink"].ToList().Count > 0)
                                            {
                                                objVisaInformation2New.Information = new Information();
                                                int totalInformationLinks = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["InformationLink"].ToList().Count;
                                                objVisaInformation2New.Information.InformationLink = new List<InformationLink>();

                                                var TypeOfInformationLink = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["InformationLink"].GetType();
                                                if (TypeOfInformationLink.Name.ToUpper() == "JOBJECT")
                                                {
                                                    InformationLink objInformationLinkNew = new InformationLink();

                                                    objInformationLinkNew.href = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["InformationLink"]["href"];
                                                    objInformationLinkNew.content = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["InformationLink"]["content"];
                                                    objInformationLinkNew.target = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["InformationLink"]["target"];
                                                    objVisaInformation2New.Information.InformationLink.Add(objInformationLinkNew);
                                                }
                                                else
                                                {
                                                    for (int l = 0; l < totalInformationLinks; l++)
                                                    {
                                                        InformationLink objInformationLinkNew = new InformationLink();

                                                        objInformationLinkNew.href = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["InformationLink"][l]["href"];
                                                        objInformationLinkNew.content = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["InformationLink"][l]["content"];
                                                        objInformationLinkNew.target = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["InformationLink"][l]["target"];

                                                        objVisaInformation2New.Information.InformationLink.Add(objInformationLinkNew);

                                                    }
                                                }

                                            }
                                        }
                                        else
                                        {
                                            //its a jvalue.
                                            objVisaInformation2New.Information = new Information();
                                            //int totalInformationLinks = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["InformationLink"].ToList().Count;
                                            objVisaInformation2New.Information.InformationLink = new List<InformationLink>();
                                            InformationLink objInformationLinkNew = new InformationLink();
                                            objInformationLinkNew.content = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"];
                                            objVisaInformation2New.Information.InformationLink.Add(objInformationLinkNew);

                                        }


                                        if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"].ToList().Count != 0
                                            && VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["content"] != null)
                                        {
                                            var ContentType = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["content"].GetType();
                                            if (ContentType.Name.ToUpper() == "JARRAY")
                                            {
                                                if (objVisaInformation2New.Information == null)
                                                {
                                                    objVisaInformation2New.Information = new Information();
                                                }

                                                objVisaInformation2New.Information.content = new List<string>();
                                                int TotalContentRecords = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["content"].ToList().Count;
                                                for (int t = 0; t < TotalContentRecords; t++)
                                                {
                                                    objVisaInformation2New.Information.content.Add(Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["content"][t]));
                                                }

                                            }
                                            else
                                            {
                                                if (objVisaInformation2New.Information == null)
                                                {
                                                    objVisaInformation2New.Information = new Information();
                                                }
                                                objVisaInformation2New.Information.content = new List<string>();
                                                int TotalContentRecords = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["content"].ToList().Count;
                                                objVisaInformation2New.Information.content.Add(Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"]["content"]));
                                            }
                                        }

                                        //else
                                        //objVisaInformation2New.Information = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["VisaInfo"]["VisaInformation"]["Information"];


                                        objVisaInfoInew.VisaInformation.Add(objVisaInformation2New);


                                    }

                                    #region categories

                                    if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"] != null &&
                                        VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"].ToList().Count > 0)
                                    {
                                        int totalCategoriesCount = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"].ToList().Count;
                                        objVisaInformationNew.Categories = new List<VisaCategories>();
                                        objVisaInformationNew.Categories.Add(new VisaCategories());
                                        objVisaInformationNew.Categories[0].Category = new List<VisaCategoryDetail>();
                                        var TypeOfCategory = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"].GetType();
                                        if (TypeOfCategory.Name.ToUpper() == "JARRAY")
                                        {
                                            for (int m = 0; m < totalCategoriesCount; m++)
                                            {
                                                VisaCategoryDetail objVisaCategoryDetailNew = new VisaCategoryDetail();
                                                objVisaCategoryDetailNew.CategoryInfo = new List<VisaCategoryInfo>();
                                                objVisaCategoryDetailNew.CategoryInfo.Add(new VisaCategoryInfo());
                                                objVisaCategoryDetailNew.CategoryInfo[0].Information = new List<VisaInformationChildNode>();
                                                objVisaCategoryDetailNew.CategoryInfo[0].Information.Add(new VisaInformationChildNode());

                                                objVisaCategoryDetailNew.CategoryCode = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryCode"];
                                                objVisaCategoryDetailNew.Category = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["Category"];
                                                // objVisaCategoryDetailNew.CategoryNotes = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"];



                                                if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"] != null)
                                                {
                                                    var TypeOfCategoryNotes = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"].GetType();
                                                    if (TypeOfCategoryNotes.Name.ToUpper() == "JOBJECT")
                                                    {
                                                        objVisaCategoryDetailNew.CategoryNotes = new CategoryNotes();
                                                        objVisaCategoryDetailNew.CategoryNotes.Notes = new List<string>();

                                                        if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"]["Note"] != null &&
                                                            VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"]["Note"].ToList().Count > 0)
                                                        {
                                                            int TotalNotesTag = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"]["Note"].ToList().Count;
                                                            for (int y = 0; y < TotalNotesTag; y++)
                                                            {
                                                                objVisaCategoryDetailNew.CategoryNotes.Notes.Add(Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]
                                                                                            ["CategoryNotes"][y]["Note"]));
                                                            }

                                                        }
                                                    }
                                                    else
                                                    {     // Its Object
                                                        objVisaCategoryDetailNew.CategoryNotes = new CategoryNotes();
                                                        objVisaCategoryDetailNew.CategoryNotes.Notes = new List<string>();


                                                        if (Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"]) != "")
                                                        {

                                                            if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"].ToList().Count == 0)
                                                            {
                                                                // read it from CategoryNotes Directly
                                                                objVisaCategoryDetailNew.CategoryNotes.Notes.Add(Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]
                                                                                       ["CategoryNotes"]));
                                                            }
                                                            else
                                                            {
                                                                if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"].GetType().Name.ToUpper() != "JARRAY" &&
                                                                    VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"]["Note"] != null &&
                                                                    VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"]["Note"].ToList().Count > 0)
                                                                {
                                                                    int TotalNotesTag = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"]["Note"].ToList().Count;
                                                                    for (int b = 0; b < TotalNotesTag; b++)
                                                                    {
                                                                        objVisaCategoryDetailNew.CategoryNotes.Notes.Add(Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]
                                                                                      ["CategoryNotes"]["Note"][b]));
                                                                    }

                                                                }
                                                                else
                                                                {
                                                                    int TotalCategoryNotesNodes = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"].ToList().Count;

                                                                    for (int d = 0; d < TotalCategoryNotesNodes; d++)
                                                                    {
                                                                        if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"][d] != null &&
                                                                            VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryNotes"][d].ToList().Count > 0)
                                                                        {
                                                                            objVisaCategoryDetailNew.CategoryNotes.Notes.Add(Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]
                                                                                           ["CategoryNotes"][d]));
                                                                        }

                                                                    }

                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                                if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryRequirements"] != null)
                                                {

                                                    var TypeOfCategoryRequirements = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryRequirements"].GetType();
                                                    if (TypeOfCategoryRequirements.Name.ToUpper() == "JARRAY")
                                                    {
                                                        objVisaCategoryDetailNew.CategoryRequirements = new List<VisaCategoryRequirements>();
                                                        objVisaCategoryDetailNew.CategoryRequirements.Add(new VisaCategoryRequirements());
                                                        objVisaCategoryDetailNew.CategoryRequirements[0].Requirements = new Requirements();

                                                        if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryRequirements"] != null)
                                                        {
                                                            int totalRequirements = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryRequirements"].ToList().Count;

                                                            for (int q = 0; q < totalRequirements; q++)
                                                            {
                                                                objVisaCategoryDetailNew.CategoryRequirements[0].Requirements.Line = Convert.ToString(
                                                                    VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryRequirements"][q]["Requirements"]["Line"]);
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        objVisaCategoryDetailNew.CategoryRequirements = new List<VisaCategoryRequirements>();
                                                        objVisaCategoryDetailNew.CategoryRequirements.Add(new VisaCategoryRequirements());
                                                        objVisaCategoryDetailNew.CategoryRequirements[0].Requirements = new Requirements();

                                                        if ((VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryRequirements"] != null))
                                                        {
                                                            if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryRequirements"].ToList().Count > 0)
                                                            {
                                                                if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryRequirements"]["Requirements"].ToList().Count > 0)
                                                                {
                                                                    objVisaCategoryDetailNew.CategoryRequirements[0].Requirements.Line = Convert.ToString(
                                                                        VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryRequirements"]["Requirements"]["Line"]);
                                                                }

                                                            }
                                                        }

                                                    }
                                                }


                                                if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryInfo"]
                                                     ["Information"] != null && VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryInfo"]
                                                     ["Information"].GetType().Name.ToUpper() != "JARRAY")
                                                {
                                                    if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryInfo"]["Information"].ToList().Count != 0)
                                                    {
                                                        objVisaCategoryDetailNew.CategoryInfo[0].Information[0].ProcessingTime = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryInfo"]
                                                                                                        ["Information"]["ProcessingTime"]);
                                                        objVisaCategoryDetailNew.CategoryInfo[0].Information[0].VisaProcedure = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryInfo"]
                                                                                                       ["Information"]["VisaProcedure"]);
                                                        objVisaCategoryDetailNew.CategoryInfo[0].Information[0].content = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryInfo"]
                                                                                                  ["Information"]["content"]);
                                                        if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryInfo"]["Information"]["DocumentsRequired"] != null)
                                                        {
                                                            var TypeOfDocumentRequiredNode = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryInfo"]["Information"]["DocumentsRequired"].GetType();
                                                            if (TypeOfDocumentRequiredNode.Name.ToUpper() == "JOBJECT")
                                                            {

                                                            }
                                                            else
                                                            {
                                                                objVisaCategoryDetailNew.CategoryInfo[0].Information[0].DocumentsRequired = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"][m]["CategoryInfo"]
                                                                                                         ["Information"]["DocumentsRequired"]);
                                                            }
                                                        }
                                                    }
                                                }
                                                else
                                                { // It is Jarray


                                                }

                                                objVisaInformationNew.Categories[0].Category.Add(objVisaCategoryDetailNew);
                                            }
                                        }
                                        else
                                        {
                                            VisaCategoryDetail objVisaCategoryDetailNew = new VisaCategoryDetail();
                                            // it is object

                                            objVisaCategoryDetailNew.CategoryCode = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryCode"];
                                            // Category is object here
                                            var TypeOfCategoryNode = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["Category"].GetType();
                                            if (TypeOfCategoryNode.Name.ToUpper() == "JOBJECT")
                                            {

                                            }
                                            else
                                            {
                                                objVisaCategoryDetailNew.Category = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["Category"];
                                                //objVisaCategoryDetailNew.CategoryNotes = (string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryNotes"];
                                            }

                                            if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryNotes"] != null &&
                                                VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryNotes"].ToList().Count > 0)
                                            {
                                                var TypeOfCategoryNotesNode = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryNotes"].GetType();
                                                if (TypeOfCategoryNotesNode.Name.ToUpper() == "JOBJECT")
                                                {

                                                }
                                                else
                                                {
                                                    //objVisaCategoryDetailNew.CategoryNotes = new List<CategoryNotes>();
                                                    //objVisaCategoryDetailNew.CategoryNotes.Add(new CategoryNotes());
                                                    //objVisaCategoryDetailNew.CategoryNotes[0].Notes = new List<string>();
                                                    //objVisaCategoryDetailNew.CategoryNotes[0].Notes.Add((string)VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryNotes"]);
                                                }

                                            }



                                            #region CategoryInfo

                                            objVisaCategoryDetailNew.CategoryInfo = new List<VisaCategoryInfo>();
                                            objVisaCategoryDetailNew.CategoryInfo.Add(new VisaCategoryInfo());

                                            objVisaCategoryDetailNew.CategoryInfo[0].Information = new List<VisaInformationChildNode>();

                                            if (VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]["Information"] != null &&
                                                VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]["Information"].GetType().Name.ToUpper() != "JARRAY")
                                            {
                                                objVisaCategoryDetailNew.CategoryInfo[0].Information.Add(new VisaInformationChildNode());
                                                if (Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]["Information"]) != "{}" &&
                                                    Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]["Information"]) != "")
                                                {
                                                    objVisaCategoryDetailNew.CategoryInfo[0].Information[0].ProcessingTime = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]
                                                                                                                                          ["Information"]["ProcessingTime"]);
                                                    objVisaCategoryDetailNew.CategoryInfo[0].Information[0].VisaProcedure = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]
                                                                                                   ["Information"]["VisaProcedure"]);
                                                    objVisaCategoryDetailNew.CategoryInfo[0].Information[0].content = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]
                                                                                                 ["Information"]["content"]);
                                                    var TypeOfDocumentRequiredNode = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]["Information"]["DocumentsRequired"].GetType();
                                                    if (TypeOfDocumentRequiredNode.Name.ToUpper() == "JOBJECT")
                                                    {

                                                    }
                                                    else
                                                    {
                                                        objVisaCategoryDetailNew.CategoryInfo[0].Information[0].DocumentsRequired = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]
                                                                                                 ["Information"]["DocumentsRequired"]);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                int TotalInformationNodes = VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]["Information"].ToList().Count;
                                                objVisaCategoryDetailNew.CategoryInfo[0].Information = new List<VisaInformationChildNode>();

                                                for (int q = 0; q < TotalInformationNodes; q++)
                                                {
                                                    if (Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]["Information"][q]) != "{}" &&
                                                        Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]["Information"][q]) != "")
                                                    {


                                                        objVisaCategoryDetailNew.CategoryInfo[0].Information.Add(new VisaInformationChildNode
                                                        {
                                                            ProcessingTime = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]["Information"][q]["ProcessingTime"]),
                                                            DocumentsRequired = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]["Information"][q]["DocumentsRequired"]),
                                                            VisaProcedure = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]["Information"][q]["VisaProcedure"]),
                                                            content = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["VisaInformation"][i]["Categories"]["Category"]["CategoryInfo"]["Information"][q]["content"])
                                                        });
                                                    }
                                                }


                                            }



                                            #endregion



                                            objVisaInformationNew.Categories[0].Category.Add(objVisaCategoryDetailNew);
                                        }




                                    }

                                    #endregion



                                    objVisaInformationNew.VisaInfo.Add(objVisaInfoInew);
                                }





                                objVisa.VisaInformation.Add(objVisaInformationNew);
                            }




                        }


                        #region Initialise collection
                        objVisa.CountryCode = (string)JobjectVisaDetail["CountryCode"];
                        objVisa.CountryDetails = new List<VisaCountryDetails>();
                        objVisa.DiplomaticRepresentation = new List<VisaDiplomaticRepresentation>();
                        objVisa.IndianEmbassy = new List<VisaIndianEmbassy>();
                        objVisa.InternationalAdvisory = new List<VisaInternationalAdvisory>();
                        objVisa.IntlHelpAddress = new VisaIntlHelpAddress();
                        objVisa.IVSAdvisory = new List<VisaIVSAdvisory>();
                        objVisa.ReciprocalVisaInfo = new List<ReciprocalVisaInfo>();
                        objVisa.SAARCInfo = new List<VisaSAARCInfo>();
                        #endregion


                        #region ReciprocalInfo
                        if (VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"] != null &&
                            VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"].ToList().Count > 0)
                        {
                            objVisa.ReciprocalVisaInfo.Add(new ReciprocalVisaInfo());
                            objVisa.ReciprocalVisaInfo[0].Description = new List<VisaDescription>();
                            objVisa.ReciprocalVisaInfo[0].Description.Add(new VisaDescription());
                            objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo = new List<ReciprocalVisaInfoChildNode>();
                            objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo.Add(new ReciprocalVisaInfoChildNode());
                            objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink = new List<VisaInformationLink>();
                            objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].content = new List<string>();
                            var ContentType = VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["content"].GetType();
                            if (ContentType.Name.ToUpper() == "JARRAY")
                            {
                                if (VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["content"] != null)
                                {
                                    int totalContentRecords = VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["content"].ToList().Count;
                                    for (int p = 0; p < totalContentRecords; p++)
                                    {
                                        objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].content.Add((string)VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]
                                                                                 ["content"][p]);
                                    }
                                }
                            }
                            else
                            {
                                objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].content.Add((string)VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["content"]);
                            }

                            if (VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["InformationLink"] != null &&
                                VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["InformationLink"].ToList().Count > 0)
                            {
                                objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink = new List<VisaInformationLink>();
                                objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink.Add(new VisaInformationLink());
                                objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink[0].content = new List<string>();

                                var InformationLinkType = VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["InformationLink"].GetType();
                                if (InformationLinkType.Name.ToUpper() == "JARRAY")
                                {
                                    if (VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["InformationLink"] != null)
                                    {
                                        int TotalInformationLinks = VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["InformationLink"].ToList().Count;
                                        for (int t = 0; t < TotalInformationLinks; t++)
                                        {
                                            VisaInformationLink objVisaInformationLinkNew = new VisaInformationLink();
                                            objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink[t].content = new List<string>();
                                            objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink[t].content.Add((string)VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["InformationLink"][t]["content"]);
                                            objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink[t].href = (string)VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["InformationLink"][t]["href"];
                                            objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink[t].target = (string)VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["InformationLink"][t]["target"];

                                            objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink.Add(objVisaInformationLinkNew);


                                        }
                                    }
                                    if (objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink.Last().content == null)
                                    {
                                        objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink.Remove(objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink.Last());
                                    }
                                }
                                else
                                {
                                    objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink.Add(new VisaInformationLink());
                                    objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink[0].content.Add((string)VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["InformationLink"]["content"]);
                                    objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink[0].href = (string)VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["InformationLink"]["href"];
                                    objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink[0].target = (string)VisaJson["VisaDetail"]["Visa"]["ReciprocalVisaInfo"]["Description"]["ReciprocalVisaInfo"]["InformationLink"]["target"];

                                    if (objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink.Last().content == null)
                                    {
                                        objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink.Remove(objVisa.ReciprocalVisaInfo[0].Description[0].ReciprocalVisaInfo[0].InformationLink.Last());
                                    }

                                }


                            }

                        }
                        #endregion

                        #region InternationalAdvisory

                        if (VisaJson["VisaDetail"]["Visa"]["InternationalAdvisory"] != null && VisaJson["VisaDetail"]["Visa"]["InternationalAdvisory"].ToList().Count > 0)
                        {
                            objVisa.InternationalAdvisory.Add(new VisaInternationalAdvisory());
                            objVisa.InternationalAdvisory[0].Description = new List<VisaDescriptionInnerNode>();
                            objVisa.InternationalAdvisory[0].Description.Add(new VisaDescriptionInnerNode());
                            objVisa.InternationalAdvisory[0].Description[0].VisaInternationalAdvisory = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["InternationalAdvisory"]["Description"]["InternationalAdvisory"]);


                        }


                        #endregion

                        #region IVSAdvisory

                        if (VisaJson["VisaDetail"]["Visa"]["IVSAdvisory"] != null && VisaJson["VisaDetail"]["Visa"]["IVSAdvisory"].ToList().Count > 0)
                        {
                            VisaIVSAdvisory objVisaIVSAdvisoryNew = new VisaIVSAdvisory();
                            objVisaIVSAdvisoryNew.Description = new List<VisaDescriptionNode>();
                            objVisaIVSAdvisoryNew.Description.Add(new VisaDescriptionNode());
                            objVisaIVSAdvisoryNew.Description[0].Heading = new List<VisaHeading>();
                            objVisaIVSAdvisoryNew.Description[0].Heading.Add(new VisaHeading
                            {
                                content = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IVSAdvisory"]["Description"]["Heading"]["content"])
                            });

                            objVisa.IVSAdvisory.Add(objVisaIVSAdvisoryNew);

                        }

                        #endregion


                        #region IndianEmbassy

                        if (VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"] != null && VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"].ToList().Count > 0)
                        {
                            if (VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"] != null && VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"].ToList().Count > 0)
                            {
                                var ContentType = VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"].GetType();
                                if (ContentType.Name.ToUpper() == "JARRAY")
                                {
                                    int TotalOffices = VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"].ToList().Count;
                                    objVisa.IndianEmbassy.Add(new VisaIndianEmbassy
                                    {
                                        Office = new List<VisaOffice>()
                                    });
                                    for (int r = 0; r < TotalOffices; r++)
                                    {
                                        objVisa.IndianEmbassy[0].Office.Add(new VisaOffice
                                        {
                                            Address = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["Address"]),
                                            City = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["City"]),
                                            Country = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["Country"]),
                                            Email = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["Email"]),
                                            Fax = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["Fax"]),
                                            Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["Name"]),
                                            Phone = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["Phone"]),
                                            PinCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["PinCode"]),
                                            SystemCityCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["SystemCityCode"]),
                                            SystemCityName = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["SystemCityName"]),
                                            SystemCountryCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["SystemCountryCode"]),
                                            SystemCountryName = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["SystemCountryName"]),
                                            URL = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["URL"]),
                                            Website = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"][r]["Website"])
                                        });
                                    }
                                }
                                else
                                {
                                    objVisa.IndianEmbassy.Add(new VisaIndianEmbassy
                                    {
                                        Office = new List<VisaOffice>()
                                    });

                                    objVisa.IndianEmbassy[0].Office.Add(new VisaOffice
                                    {
                                        Address = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["Address"]),
                                        City = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["City"]),
                                        Country = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["Country"]),
                                        Email = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["Email"]),
                                        Fax = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["Fax"]),
                                        Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["Name"]),
                                        Phone = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["Phone"]),
                                        PinCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["PinCode"]),
                                        SystemCityCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["SystemCityCode"]),
                                        SystemCityName = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["SystemCityName"]),
                                        SystemCountryCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["SystemCountryCode"]),
                                        SystemCountryName = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["SystemCountryName"]),
                                        URL = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["URL"]),
                                        Website = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IndianEmbassy"]["Office"]["Website"])
                                    });
                                }

                            }

                        }

                        #endregion

                        #region DiplomaticRepresentation

                        if (VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"] != null && VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"].ToList().Count > 0)
                        {
                            if (VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"] != null &&
                                VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"].ToList().Count > 0)
                            {
                                objVisa.DiplomaticRepresentation.Add(new VisaDiplomaticRepresentation());
                                objVisa.DiplomaticRepresentation[0].Offices = new List<VisaOffices>();
                                objVisa.DiplomaticRepresentation[0].Offices.Add(new VisaOffices());
                                objVisa.DiplomaticRepresentation[0].Offices[0].Office = new List<VisaOfficeNode>();


                                var TypeOfDiplomaticOffice = VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"].GetType();
                                if (TypeOfDiplomaticOffice.Name.ToUpper() == "JARRAY")
                                {
                                    if (VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"] != null)
                                    {
                                        int TotalDiplomaticOffices = VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"].ToList().Count;

                                        for (int cnt = 0; cnt < TotalDiplomaticOffices; cnt++)
                                        {
                                            VisaOfficeNode objVisaOffice2New = new VisaOfficeNode()
                                            {
                                                Address = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["Address"]),
                                                City = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["City"]),
                                                CollectionTimings = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["CollectionTimings"]),
                                                Country = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["Country"]),
                                                Email = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["Email"]),
                                                Fax = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["Fax"]),
                                                Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["Name"]),
                                                Notes = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["Notes"]),
                                                Phone = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["Phone"]),
                                                PinCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["PinCode"]),
                                                PublicTimings = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["PublicTimings"]),
                                                Telephone = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["Telephone"]),
                                                Timings = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["Timings"]),
                                                VisaTimings = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["Timings"]),
                                                Website = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"][cnt]["Website"])
                                            };

                                            objVisa.DiplomaticRepresentation[0].Offices[0].Office.Add(objVisaOffice2New);
                                        }
                                    }

                                }
                                else
                                {
                                    // It is Object 
                                    objVisa.DiplomaticRepresentation[0].Offices[0].Office.Add(new VisaOfficeNode
                                    {
                                        Address = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["Address"]),
                                        City = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["City"]),
                                        CollectionTimings = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["CollectionTimings"]),
                                        Country = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["Country"]),
                                        Email = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["Email"]),
                                        Fax = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["Fax"]),
                                        Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["Name"]),
                                        Notes = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["Notes"]),
                                        Phone = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["Phone"]),
                                        PinCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["PinCode"]),
                                        PublicTimings = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["PublicTimings"]),
                                        Telephone = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["Telephone"]),
                                        Timings = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["Timings"]),
                                        VisaTimings = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["Timings"]),
                                        Website = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["DiplomaticRepresentation"]["Offices"]["Office"]["Website"])
                                    });
                                }

                            }

                        }
                        #endregion

                        #region SAARCInfo

                        if (VisaJson["VisaDetail"]["Visa"]["SAARCInfo"] != null && VisaJson["VisaDetail"]["Visa"]["SAARCInfo"].ToList().Count > 0)
                        {
                            if (VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"] != null && VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"].ToList().Count > 0)
                            {
                                objVisa.SAARCInfo.Add(new VisaSAARCInfo());
                                objVisa.SAARCInfo[0].CountryOffices = new List<VisaCountryOffices>();
                                objVisa.SAARCInfo[0].CountryOffices.Add(new VisaCountryOffices());
                                objVisa.SAARCInfo[0].CountryOffices[0].CountryOffice = new List<VisaCountryOffice>();




                                var TypeCountryOffice = VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"].GetType();
                                if (TypeCountryOffice.Name.ToUpper() == "JARRAY")
                                {
                                    if (VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"] != null)
                                    {
                                        int TotalCountryOffices = VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"].ToList().Count;

                                        for (int Cnt = 0; Cnt < TotalCountryOffices; Cnt++)
                                        {
                                            objVisa.SAARCInfo[0].CountryOffices[0].CountryOffice.Add(new VisaCountryOffice
                                            {
                                                Address = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"][Cnt]["Address"]),
                                                City = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"][Cnt]["City"]),
                                                CountryID = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"][Cnt]["CountryID"]),
                                                County = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"][Cnt]["County"]),
                                                Fax = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"][Cnt]["Fax"]),
                                                Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"][Cnt]["Name"]),
                                                PinCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"][Cnt]["PinCode"]),
                                                Telephone = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"][Cnt]["Telephone"]),
                                                VisaRequired = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"][Cnt]["VisaRequired"]),
                                                Website = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"][Cnt]["Website"]),
                                                WhereToApply = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"][Cnt]["WhereToApply"])
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    objVisa.SAARCInfo[0].CountryOffices[0].CountryOffice.Add(new VisaCountryOffice
                                    {
                                        Address = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"]["Address"]),
                                        City = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"]["City"]),
                                        CountryID = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"]["CountryID"]),
                                        County = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"]["County"]),
                                        Fax = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"]["Fax"]),
                                        Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"]["Name"]),
                                        PinCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"]["PinCode"]),
                                        Telephone = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"]["Telephone"]),
                                        VisaRequired = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"]["VisaRequired"]),
                                        Website = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"]["Website"]),
                                        WhereToApply = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["SAARCInfo"]["CountryOffices"]["CountryOffic"]["WhereToApply"])
                                    });

                                }

                            }
                        }
                        #endregion

                        #region IntlHelpAddress

                        if (VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"] != null && VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"].ToList().Count > 0)
                        {
                            if (VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"] != null && VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"].ToList().Count > 0)
                            {
                                objVisa.IntlHelpAddress.HelpAddress = new List<VisaHelpAddress>();


                                var TypeOfHelpAddress = VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"].GetType();
                                if (TypeOfHelpAddress.Name.ToUpper() == "JARRAY")
                                {
                                    int TotalHelpAddress = VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"].ToList().Count;
                                    for (int f = 0; f < TotalHelpAddress; f++)
                                    {
                                        objVisa.IntlHelpAddress.HelpAddress.Add(new VisaHelpAddress
                                        {
                                            Address = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"][f]["Address"]),
                                            City = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"][f]["City"]),
                                            Country = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"][f]["Country"]),
                                            Fax = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"][f]["Fax"]),
                                            Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"][f]["Name"]),
                                            Phone = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"][f]["Phone"]),
                                            PinCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"][f]["PinCode"]),
                                            URL = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"][f]["URL"]),
                                            Website = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"][f]["Website"])
                                        });
                                    }

                                }
                                else
                                {// it is object

                                    objVisa.IntlHelpAddress.HelpAddress.Add(new VisaHelpAddress
                                    {
                                        Address = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"]["Address"]),
                                        City = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"]["City"]),
                                        Country = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"]["Country"]),
                                        Fax = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"]["Fax"]),
                                        Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"]["Name"]),
                                        Phone = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"]["Phone"]),
                                        PinCode = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"]["PinCode"]),
                                        URL = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"]["URL"]),
                                        Website = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["IntlHelpAddress"]["HelpAddress"]["Website"])
                                    });
                                }


                            }
                        }
                        #endregion

                        #region CountryDetails

                        if (VisaJson["VisaDetail"]["Visa"]["CountryDetails"] != null && VisaJson["VisaDetail"]["Visa"]["CountryDetails"].ToList().Count > 0)
                        {
                            if (VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"] != null && VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"].ToList().Count > 0)
                            {
                                var TypeOfGeneralInfo = VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"].GetType();
                                if (TypeOfGeneralInfo.Name.ToUpper() == "JARRAY")
                                {
                                    objVisa.CountryDetails.Add(new VisaCountryDetails());
                                    objVisa.CountryDetails[0].GeneralInfo = new List<VisaGeneralInfo>();

                                    int TotalGeneralInfos = VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"].ToList().Count;
                                    for (int e = 0; e < TotalGeneralInfos; e++)
                                    {
                                        objVisa.CountryDetails[0].GeneralInfo.Add(new VisaGeneralInfo()
                                        {
                                            Area = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["Area"]),
                                            Capital = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["Capital"]),
                                            Code = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["Code"]),
                                            Currency = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["Currency"]),
                                            Flag = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["Flag"]),
                                            Languages = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["Languages"]),
                                            LargeMap = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["LargeMap"]),
                                            Location = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["Location"]),
                                            NationalDay = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["NationalDay"]),
                                            Population = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["Population"]),
                                            SmallMap = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["SmallMap"]),
                                            Time = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["Time"]),
                                            WorldFactBook = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"][e]["WorldFactBook"])
                                        });
                                    }
                                }
                                else
                                {
                                    objVisa.CountryDetails.Add(new VisaCountryDetails());
                                    objVisa.CountryDetails[0].GeneralInfo = new List<VisaGeneralInfo>();
                                    objVisa.CountryDetails[0].GeneralInfo.Add(new VisaGeneralInfo()
                                    {
                                        Area = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["Area"]),
                                        Capital = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["Capital"]),
                                        Code = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["Code"]),
                                        Currency = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["Currency"]),
                                        Flag = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["Flag"]),
                                        Languages = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["Languages"]),
                                        LargeMap = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["LargeMap"]),
                                        Location = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["Location"]),
                                        NationalDay = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["NationalDay"]),
                                        Population = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["Population"]),
                                        SmallMap = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["SmallMap"]),
                                        Time = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["Time"]),
                                        WorldFactBook = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["GeneralInfo"]["WorldFactBook"])
                                    });
                                }

                            }



                            if (VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airports"] != null &&
                                VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airports"].ToList().Count > 0)
                            {
                                if (VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airports"]["Airport"] != null &&
                                    VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airports"]["Airport"].ToList().Count > 0)
                                {
                                    var TypeOfAirport = VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airports"]["Airport"].GetType();
                                    if (TypeOfAirport.Name.ToUpper() == "JARRAY")
                                    {
                                        if (objVisa.CountryDetails.Count == 0)
                                        {
                                            objVisa.CountryDetails.Add(new VisaCountryDetails());
                                        }
                                        if (objVisa.CountryDetails[0].Airports == null)
                                        {
                                            objVisa.CountryDetails[0].Airports = new List<VisaAirports>();
                                        }

                                        int TotalAirports = VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airports"]["Airport"].ToList().Count;

                                        objVisa.CountryDetails[0].Airports = new List<VisaAirports>();
                                        objVisa.CountryDetails[0].Airports.Add(new VisaAirports());
                                        objVisa.CountryDetails[0].Airports[0].Airport = new List<VisaAirport>();

                                        for (int u = 0; u < TotalAirports; u++)
                                        {
                                            objVisa.CountryDetails[0].Airports[0].Airport.Add(new VisaAirport()
                                            {
                                                Code = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airports"]["Airport"][u]["Type"]),
                                                Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airports"]["Airport"][u]["Code"]),
                                                Type = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airports"]["Airport"][u]["Name"])
                                            });
                                        }

                                    }
                                    else
                                    {  // It is object
                                        if (objVisa.CountryDetails.Count == 0)
                                        {
                                            objVisa.CountryDetails.Add(new VisaCountryDetails());
                                        }
                                        if (objVisa.CountryDetails[0].Airports == null)
                                        {
                                            objVisa.CountryDetails[0].Airports = new List<VisaAirports>();
                                        }
                                        objVisa.CountryDetails[0].Airports = new List<VisaAirports>();
                                        objVisa.CountryDetails[0].Airports.Add(new VisaAirports());
                                        objVisa.CountryDetails[0].Airports[0].Airport = new List<VisaAirport>();
                                        objVisa.CountryDetails[0].Airports[0].Airport.Add(new VisaAirport()
                                        {
                                            Code = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airports"]["Airport"]["Type"]),
                                            Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airports"]["Airport"]["Code"]),
                                            Type = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airports"]["Airport"]["Name"])
                                        });
                                    }

                                }
                            }


                            if (VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["CountryName"] != null &&
                                VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["CountryName"].ToList().Count > 0)
                            {
                                if (objVisa.CountryDetails.Count == 0)
                                {
                                    objVisa.CountryDetails.Add(new VisaCountryDetails());
                                }
                                objVisa.CountryDetails[0].CountryName = new List<VisaCountryName>();


                                var TypeOfCounytryName = VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["CountryName"].GetType();
                                if (TypeOfCounytryName.Name.ToUpper() == "JARRAY")
                                {
                                    int TotalCountries = VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["CountryName"].ToList().Count;
                                    for (int y = 0; y < TotalCountries; y++)
                                    {
                                        objVisa.CountryDetails[0].CountryName.Add(new VisaCountryName()
                                        {
                                            Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["CountryName"][y]["Name"])
                                        });
                                    }
                                }
                                else
                                { // It is object

                                    objVisa.CountryDetails[0].CountryName.Add(new VisaCountryName()
                                    {
                                        Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["CountryName"]["Name"])
                                    });
                                }
                            }


                            if (VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airlines"] != null &&
                                VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airlines"].ToList().Count > 0)
                            {
                                if (VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airlines"]["Airline"] != null &&
                                    VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airlines"]["Airline"].ToList().Count > 0)
                                {
                                    if (objVisa.CountryDetails.Count == 0)
                                    {
                                        objVisa.CountryDetails.Add(new VisaCountryDetails());
                                    }

                                    var TypeOfAirlines = VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airlines"]["Airline"].GetType();
                                    if (TypeOfAirlines.Name.ToUpper() == "JARRAY")
                                    {
                                        objVisa.CountryDetails[0].Airlines = new List<VisaAirlines>();
                                        objVisa.CountryDetails[0].Airlines.Add(new VisaAirlines());
                                        objVisa.CountryDetails[0].Airlines[0].Airline = new List<VisaAirline>();

                                        int TotalAirlines = VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airlines"]["Airline"].ToList().Count;

                                        for (int f = 0; f < TotalAirlines; f++)
                                        {
                                            objVisa.CountryDetails[0].Airlines[0].Airline.Add(new VisaAirline()
                                            {
                                                Code = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airlines"]["Airline"][f]["Code"]),
                                                Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airlines"]["Airline"][f]["Name"])
                                            });
                                        }
                                    }
                                    else
                                    { // It is Object
                                        objVisa.CountryDetails[0].Airlines = new List<VisaAirlines>();
                                        objVisa.CountryDetails[0].Airlines.Add(new VisaAirlines());
                                        objVisa.CountryDetails[0].Airlines[0].Airline = new List<VisaAirline>();

                                        objVisa.CountryDetails[0].Airlines[0].Airline.Add(new VisaAirline()
                                        {
                                            Code = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airlines"]["Airline"]["Code"]),
                                            Name = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Airlines"]["Airline"]["Name"])
                                        });
                                    }

                                }
                            }

                            if (VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"] != null &&
                                VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"].ToList().Count > 0)
                            {
                                if (VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"]["Holiday"] != null &&
                                    VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"]["Holiday"].ToList().Count > 0)
                                {
                                    var TypeOfHoliday = VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"]["Holiday"].GetType();

                                    if (objVisa.CountryDetails.Count == 0)
                                    {
                                        objVisa.CountryDetails.Add(new VisaCountryDetails());
                                    }
                                    objVisa.CountryDetails[0].Holidays = new VisaHolidays();
                                    objVisa.CountryDetails[0].Holidays.Holiday = new List<VisaHoliday>();


                                    if (TypeOfHoliday.Name.ToUpper() == "JARRAY")
                                    {
                                        int TotalHolidayRecords = VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"]["Holiday"].ToList().Count;

                                        for (int count = 0; count < TotalHolidayRecords; count++)
                                        {
                                            objVisa.CountryDetails[0].Holidays.Holiday.Add(new VisaHoliday()
                                            {
                                                HolidayName = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"]["Holiday"][count]["HolidayName"]),
                                                Year = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"]["Holiday"][count]["Year"]),
                                                Date = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"]["Holiday"][count]["Date"]),
                                                Month = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"]["Holiday"][count]["Month"])
                                            });
                                        }
                                    }
                                    else
                                    { // it is object
                                        objVisa.CountryDetails[0].Holidays.Holiday.Add(new VisaHoliday()
                                        {
                                            HolidayName = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"]["Holiday"]["HolidayName"]),
                                            Year = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"]["Holiday"]["Year"]),
                                            Date = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"]["Holiday"]["Date"]),
                                            Month = Convert.ToString(VisaJson["VisaDetail"]["Visa"]["CountryDetails"]["Holidays"]["Holiday"]["Month"])
                                        });

                                    }

                                }
                            }

                        }
                        #endregion

                        objVisaDetail.Visa.Add(objVisa);

                        #endregion

                        objVisaDefinition.VisaDetail.Add(objVisaDetail);
                    }

                    //Upsert here
                    //var filter = Builders<VisaDefinition>.Filter.ElemMatch(c => c.VisaDetail.First().CountryCode.ToUpper(), objVisaDefinition.VisaDetail.First().CountryCode.ToUpper());
                    //VisaMappingCollection.ReplaceOne(filter, objVisaDefinition, new UpdateOptions { IsUpsert = true });

                    //var filter1 = Builders<VisaDefinition>.Filter.Eq("VisaDetail.CountryCode", objVisaDefinition.VisaDetail.First().CountryCode);
                    //var update1 = Builders<VisaDefinition>.Update.Set(v => v.VisaDetail, objVisaDefinition.VisaDetail);
                    //VisaMappingCollection.UpdateOne(filter1, update1);

                    //var filter = Builders<VisaDefinition>.Filter.Eq(c => c.VisaDetail.First().CountryCode.ToUpper(), objVisaDefinition.VisaDetail.First().CountryCode.ToUpper());
                    //VisaMappingCollection.ReplaceOne(filter, objVisaDefinition, new UpdateOptions { IsUpsert = true });
                    //ListVisaDefinitions.Add(objVisaDefinition);

                    VisaMappingCollection.InsertOne(objVisaDefinition);

                    counter++;
                    UpdateDistLogInfo(Logid, PushStatus.RUNNNING, totalCount, counter, Logid.ToString(), "VISA", "MAPPING");

                }

                UpdateDistLogInfo(Logid, PushStatus.COMPLETED, totalCount, counter, Logid.ToString(), "VISA", "MAPPING");

            }
            catch (FaultException<DataContracts.ErrorNotifier> ex)
            {
                UpdateDistLogInfo(Logid, PushStatus.ERROR, 0, 0, Logid.ToString(), "VISA", "MAPPING");
            }
        }
        #endregion


        #region HolidayMapping

        public void UpdateHolidayMapping(Guid Logid)
        {
            try
            {
                int counter = 0;

                UpdateDistLogInfo(Logid, PushStatus.INSERT, 0, 0, Logid.ToString(), "HOLIDAY", "MAPPING");

                UpdateDistLogInfo(Logid, PushStatus.RUNNNING, 0, 0, Logid.ToString(), "HOLIDAY", "MAPPING");

                _database = MongoDBHandler.mDatabase();
                // _database.DropCollection("HolidayMapping");

                IMongoCollection<HolidayModel> collectionHolidayModel = _database.GetCollection<HolidayModel>("HolidayMapping");

                var searchResult = collectionHolidayModel.Find(s => true).Project(u => new
                {
                    u.NakshatraHolidayId,
                    u.ClassificationAttributes,
                    u.SupplierProductCode,
                    u.SupplierName,
                    u.TypeOfHoliday,
                    u.TravelFrequency,
                    u.PaceOfHoliday,
                    u.Rating,
                    u.ComfortLevel,
                    u.StayType,
                    u.Interests,
                    u.TravellerType,
                    u.Collections
                }).ToList();

                var CollecionHolidayDetail = _database.GetCollection<BsonDocument>("HolidayDetail");
                var HolidayMappingCollection = _database.GetCollection<HolidayModel>("HolidayMapping");

                ProjectionDefinition<BsonDocument> project = Builders<BsonDocument>.Projection.Include("SupplierCode");
                project = project.Exclude("_id");
                project = project.Include("SupplierName");
                project = project.Include("CallType");
                project = project.Include("Holiday");


                FilterDefinition<BsonDocument> filter;
                filter = Builders<BsonDocument>.Filter.Empty;

                filter = filter & Builders<BsonDocument>.Filter.Regex("CallType", new BsonRegularExpression(new Regex("HolidayMapping", RegexOptions.IgnoreCase)));

                var CollecionHolidayMappingsFiltered = CollecionHolidayDetail.Find(filter).Project(project).ToList();

                UpdateDistLogInfo(Logid, PushStatus.RUNNNING, CollecionHolidayMappingsFiltered.Count, 0, Logid.ToString(), "HOLIDAY", "MAPPING");

                List<HolidayModel> ListHolidayModels = new List<HolidayModel>();

                //Transformation
                foreach (var item in CollecionHolidayMappingsFiltered)
                {
                    var JsonObject = item.ToJson();

                    JObject HolidayJson = JObject.Parse(JsonObject);

                    HolidayModel objHolidayModel = new HolidayModel();

                    objHolidayModel.ClassificationAttributes = new List<ClassificationAttributes>();
                    objHolidayModel.CallType = Convert.ToString(HolidayJson["CallType"]);
                    objHolidayModel.SupplierName = Convert.ToString(HolidayJson["SupplierName"]);
                    objHolidayModel.NakshatraHolidayId = Convert.ToString(HolidayJson["Holiday"]["NakshatraHolidayId"]).ToUpper();
                    objHolidayModel.SupplierHolidayId = Convert.ToString(HolidayJson["Holiday"]["SupplierHolidayId"]);

                    objHolidayModel.SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["SupplierProductCode"]);
                    CultureInfo culture = new CultureInfo("en-US");

                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["EffectiveFromDate"])))
                        objHolidayModel.EffectiveFromDate = Convert.ToString(HolidayJson["Holiday"]["EffectiveFromDate"]);

                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["EffectiveToDate"])))
                        objHolidayModel.EffectiveToDate = Convert.ToString(HolidayJson["Holiday"]["EffectiveToDate"]);

                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["CreateDate"])))
                        objHolidayModel.CreateDate = Convert.ToString(HolidayJson["Holiday"]["CreateDate"]);

                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["EditDate"])))
                        objHolidayModel.EditDate = Convert.ToString(HolidayJson["Holiday"]["EditDate"]);

                    objHolidayModel.ProductCategory = Convert.ToString(HolidayJson["Holiday"]["ProductCategory"]);
                    objHolidayModel.ProductCategorySubtype = Convert.ToString(HolidayJson["Holiday"]["ProductCategorySubtype"]);
                    objHolidayModel.GroupOfCompany = Convert.ToString(HolidayJson["Holiday"]["GroupOfCompany"]);
                    objHolidayModel.Company = Convert.ToString(HolidayJson["Holiday"]["Company"]);
                    objHolidayModel.SBU = Convert.ToString(HolidayJson["Holiday"]["SBU"]);
                    objHolidayModel.BU = Convert.ToString(HolidayJson["Holiday"]["BU"]);
                    objHolidayModel.CompanyProductId = Convert.ToString(HolidayJson["Holiday"]["CompanyProductId"]);
                    objHolidayModel.ProductName = Convert.ToString(HolidayJson["Holiday"]["ProductName"]);
                    objHolidayModel.Overview = Convert.ToString(HolidayJson["Holiday"]["Overview"]);
                    objHolidayModel.TypeOfHoliday = Convert.ToString(HolidayJson["Holiday"]["TypeOfHoliday"]);
                    objHolidayModel.ProductFlavorID = Convert.ToString(HolidayJson["Holiday"]["ProductFlavorID"]);
                    objHolidayModel.ProductFlavorName = Convert.ToString(HolidayJson["Holiday"]["ProductFlavorName"]);
                    objHolidayModel.FlavorType = Convert.ToString(HolidayJson["Holiday"]["FlavorType"]);
                    objHolidayModel.CreateUSer = Convert.ToString(HolidayJson["Holiday"]["CreateUser"]);
                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["UserReviewStatus"])))
                    {
                        objHolidayModel.UserReviewStatus = Convert.ToString(HolidayJson["Holiday"]["UserReviewStatus"]);
                    }
                    else
                    {
                        objHolidayModel.UserReviewStatus = "Not Yet Reviewed";
                    }

                    objHolidayModel.IsActive = true;
                    DateTime FlavourValidFrom;
                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["FlavourValidFrom"])))
                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["FlavourValidFrom"]), out FlavourValidFrom))
                        {
                            objHolidayModel.FlavourValidFrom = FlavourValidFrom;
                        }

                    DateTime FlavourValidTo;
                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["FlavourValidTo"])))
                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["FlavourValidTo"]), out FlavourValidTo))
                        {
                            objHolidayModel.FlavourValidFrom = FlavourValidTo;
                        }
                    objHolidayModel.ProductType = Convert.ToString(HolidayJson["Holiday"]["ProductType"]);
                    objHolidayModel.PackageType = Convert.ToString(HolidayJson["Holiday"]["PackageType"]);
                    objHolidayModel.CompanyPackageCategory = Convert.ToString(HolidayJson["Holiday"]["CompanyPackageCategory"]);
                    objHolidayModel.Rating = Convert.ToString(HolidayJson["Holiday"]["Rating"]);
                    objHolidayModel.StayType = Convert.ToString(HolidayJson["Holiday"]["StayType"]);

                    #region Collections
                    if (HolidayJson["Holiday"]["Collections"] != null)
                    {
                        objHolidayModel.Collections = new List<HolidayCollections>();
                        var TypeOfCollections = HolidayJson["Holiday"]["Collections"].GetType();
                        if (TypeOfCollections.Name.ToUpper() == "JARRAY")
                        {
                            int totalCollections = HolidayJson["Holiday"]["Collections"].ToList().Count;


                            for (int i = 0; i < totalCollections; i++)
                            {
                                objHolidayModel.Collections.Add(new HolidayCollections()
                                {
                                    DisplayName = Convert.ToString(HolidayJson["Holiday"]["Collections"][i]["DisplayName"]),
                                    Type = Convert.ToString(HolidayJson["Holiday"]["Collections"][i]["Type"])
                                });
                            }

                        }
                        else
                        { // its a object
                            objHolidayModel.Collections.Add(new HolidayCollections()
                            {
                                DisplayName = Convert.ToString(HolidayJson["Holiday"]["Collections"]["DisplayName"]),
                                Type = Convert.ToString(HolidayJson["Holiday"]["Collections"]["Type"])
                            });
                        }

                    }
                    #endregion

                    #region TravellerType

                    if (HolidayJson["Holiday"]["TravellerType"] != null)
                    {

                        var TypeOfTravellerType = HolidayJson["Holiday"]["TravellerType"].GetType();
                        if (TypeOfTravellerType.Name.ToUpper() != "JARRAY")
                        {
                            int totalSubType = 0;
                            objHolidayModel.TravellerType = new List<HolidayTypes>();
                            List<SubType> lst = new List<SubType>();
                            if (HolidayJson["Holiday"]["TravellerType"].ToList().Count != 0)
                            {

                                if (HolidayJson["Holiday"]["TravellerType"]["SubType"] != null)
                                    totalSubType = HolidayJson["Holiday"]["TravellerType"]["SubType"].ToList().Count;
                                for (int j = 0; j < totalSubType; j++)
                                {
                                    lst.Add(new SubType()
                                    {
                                        Type = Convert.ToString(HolidayJson["Holiday"]["TravellerType"]["SubType"][j])
                                    });
                                }
                                objHolidayModel.TravellerType.Add(new HolidayTypes()
                                {
                                    Type = Convert.ToString(HolidayJson["Holiday"]["TravellerType"]["Type"]),
                                    SubType = lst
                                });
                            }
                        }
                        else
                        {
                            objHolidayModel.TravellerType = new List<HolidayTypes>();

                            int TotalTravellerType = HolidayJson["Holiday"]["TravellerType"].ToList().Count;
                            for (int k = 0; k < TotalTravellerType; k++)
                            {
                                HolidayTypes objHolidayTypes = new HolidayTypes();
                                objHolidayTypes.SubType = new List<SubType>();
                                objHolidayTypes.Type = Convert.ToString(HolidayJson["Holiday"]["TravellerType"][k]["Type"]);
                                if (HolidayJson["Holiday"]["TravellerType"][k]["SubType"] != null && HolidayJson["Holiday"]["TravellerType"][k]["SubType"].ToList().Count > 0)
                                {
                                    int TotalSubTypes = HolidayJson["Holiday"]["TravellerType"][k]["SubType"].ToList().Count;
                                    for (int p = 0; p < TotalSubTypes; p++)
                                    {
                                        objHolidayTypes.SubType.Add(new SubType() { Type = Convert.ToString(HolidayJson["Holiday"]["TravellerType"][k]["SubType"][p]) });
                                    }
                                }
                                objHolidayModel.TravellerType.Add(objHolidayTypes);
                            }
                        }
                    }
                    #endregion

                    #region Interests

                    if (HolidayJson["Holiday"]["Interests"] != null)
                    {

                        var TypeOfInterests = HolidayJson["Holiday"]["Interests"].GetType();
                        if (TypeOfInterests.Name.ToUpper() != "JARRAY")
                        {
                            int totalSubType = 0;
                            objHolidayModel.Interests = new List<HolidayTypes>();
                            List<SubType> lst = new List<SubType>();
                            if (HolidayJson["Holiday"]["Interests"].ToList().Count != 0)
                            {

                                if (HolidayJson["Holiday"]["Interests"]["SubType"] != null)
                                    totalSubType = HolidayJson["Holiday"]["Interests"]["SubType"].ToList().Count;
                                for (int j = 0; j < totalSubType; j++)
                                {
                                    lst.Add(new SubType()
                                    {
                                        Type = Convert.ToString(HolidayJson["Holiday"]["Interests"]["SubType"][j])
                                    });
                                }
                                objHolidayModel.Interests.Add(new HolidayTypes()
                                {
                                    Type = Convert.ToString(HolidayJson["Holiday"]["Interests"]["Type"]),
                                    SubType = lst
                                });
                            }
                        }
                        else
                        {
                            objHolidayModel.Interests = new List<HolidayTypes>();

                            int TotalInterests = HolidayJson["Holiday"]["Interests"].ToList().Count;
                            for (int k = 0; k < TotalInterests; k++)
                            {
                                HolidayTypes objHolidayTypes = new HolidayTypes();
                                objHolidayTypes.SubType = new List<SubType>();
                                objHolidayTypes.Type = Convert.ToString(HolidayJson["Holiday"]["Interests"][k]["Type"]);
                                if (HolidayJson["Holiday"]["Interests"][k]["SubType"] != null && HolidayJson["Holiday"]["Interests"][k]["SubType"].ToList().Count > 0)
                                {
                                    int TotalSubTypes = HolidayJson["Holiday"]["Interests"][k]["SubType"].ToList().Count;
                                    for (int p = 0; p < TotalSubTypes; p++)
                                    {
                                        objHolidayTypes.SubType.Add(new SubType() { Type = Convert.ToString(HolidayJson["Holiday"]["Interests"][k]["SubType"][p]) });
                                    }
                                }
                                objHolidayModel.Interests.Add(objHolidayTypes);
                            }
                        }
                    }
                    #endregion

                    #region TravelFrequency
                    if (HolidayJson["Holiday"]["TravelFrequency"] != null)
                    {
                        var TypeOfInterests = HolidayJson["Holiday"]["TravelFrequency"].GetType();
                        if (TypeOfInterests.Name.ToUpper() != "JARRAY")
                        {
                            objHolidayModel.TravelFrequency = new List<TravelFrequency>();
                            objHolidayModel.TravelFrequency.Add(new TravelFrequency() { Type = Convert.ToString(HolidayJson["Holiday"]["TravelFrequency"]) });
                        }
                        else
                        {
                            objHolidayModel.TravelFrequency = new List<TravelFrequency>();
                            int TotalTravelFrequency = HolidayJson["Holiday"]["TravelFrequency"].ToList().Count;
                            for (int h = 0; h < TotalTravelFrequency; h++)
                            {
                                objHolidayModel.TravelFrequency.Add(new TravelFrequency() { Type = Convert.ToString(HolidayJson["Holiday"]["TravelFrequency"][h]) });
                            }
                        }
                    }
                    #endregion



                    #region Days
                    if (HolidayJson["Holiday"]["Days"] != null)
                    {
                        int days = 0;
                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["Days"]), out days))
                        {
                            objHolidayModel.Days = days;
                        }
                    }
                    #endregion

                    #region Nights
                    if (HolidayJson["Holiday"]["Nights"] != null)
                    {
                        int nights = 0;
                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["Nights"]), out nights))
                        {
                            objHolidayModel.Nights = nights;
                        }
                    }
                    #endregion

                    #region PerPersonPrice
                    if (HolidayJson["Holiday"]["PerPersonPrice"] != null)
                    {
                        var TypeOfPerPersonPrice = HolidayJson["Holiday"]["PerPersonPrice"].GetType();
                        if (TypeOfPerPersonPrice.Name.ToUpper() == "JARRAY")
                        {
                            objHolidayModel.PerPersonPrice = new List<HolidayPerPersonPrice>();

                            int TotdalPerPersonprice = HolidayJson["Holiday"]["PerPersonPrice"].ToList().Count;
                            for (int g = 0; g < TotdalPerPersonprice; g++)
                            {
                                double amt = 0;
                                if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PerPersonPrice"][g]["Amount"]), out amt))
                                {
                                    // Conversion successfull
                                }

                                objHolidayModel.PerPersonPrice.Add(new HolidayPerPersonPrice()
                                {
                                    Amount = amt,
                                    PriceText = Convert.ToString(HolidayJson["Holiday"]["PerPersonPrice"][g]["PriceText"]),
                                    Currency = Convert.ToString(HolidayJson["Holiday"]["PerPersonPrice"][g]["Currency"]),
                                    PriceBasis = Convert.ToString(HolidayJson["Holiday"]["PerPersonPrice"][g]["PriceBasis"])
                                });
                            }

                        }
                        else
                        { // it is object

                            double amt = 0;
                            if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PerPersonPrice"]["Amount"]), out amt))
                            {
                                // Conversion successfull
                            }

                            objHolidayModel.PerPersonPrice.Add(new HolidayPerPersonPrice()
                            {
                                Amount = amt,
                                PriceText = Convert.ToString(HolidayJson["Holiday"]["PerPersonPrice"]["PriceText"]),
                                Currency = Convert.ToString(HolidayJson["Holiday"]["PerPersonPrice"]["Currency"]),
                                PriceBasis = Convert.ToString(HolidayJson["Holiday"]["PerPersonPrice"]["PriceBasis"])
                            });
                        }

                    }

                    #endregion

                    #region ComfortLevel
                    if (HolidayJson["Holiday"]["PaceOfHoliday"] != null && HolidayJson["Holiday"]["ComfortLevel"].ToList().Count > 0)
                    {

                        int totalComfortLevel = HolidayJson["Holiday"]["ComfortLevel"].ToList().Count;

                        objHolidayModel.ComfortLevel = new List<ComfortLevel>();
                        var TypeOfComfortLevel = HolidayJson["Holiday"]["ComfortLevel"].GetType();
                        if (TypeOfComfortLevel.Name.ToUpper() != "JARRAY")
                        {
                            // It is a single object
                            objHolidayModel.ComfortLevel.Add(new ComfortLevel()
                            {
                                Level = Convert.ToString(HolidayJson["Holiday"]["ComfortLevel"]["Level"]),
                            });
                        }
                        else
                        {
                            for (int f = 0; f < totalComfortLevel; f++)
                            {
                                objHolidayModel.ComfortLevel.Add(new ComfortLevel()
                                {
                                    Level = Convert.ToString(HolidayJson["Holiday"]["ComfortLevel"][f]["Level"])
                                });
                            }
                        }
                    }
                    #endregion

                    #region PaceOfHoliday
                    if (HolidayJson["Holiday"]["PaceOfHoliday"] != null && HolidayJson["Holiday"]["PaceOfHoliday"].ToList().Count > 0)
                    {
                        int totalPaceOfHoliday = HolidayJson["Holiday"]["PaceOfHoliday"].ToList().Count;
                        objHolidayModel.PaceOfHoliday = new List<PaceOfHoliday>();
                        var TypeOfPaceOfHoliday = HolidayJson["Holiday"]["PaceOfHoliday"].GetType();
                        if (TypeOfPaceOfHoliday.Name.ToUpper() != "JARRAY")
                        {
                            // It is a single object
                            objHolidayModel.PaceOfHoliday.Add(new PaceOfHoliday()
                            {
                                Pace = Convert.ToString(HolidayJson["Holiday"]["PaceOfHoliday"]["Pace"]),
                            });
                        }
                        else
                        {
                            for (int f = 0; f < totalPaceOfHoliday; f++)
                            {
                                objHolidayModel.PaceOfHoliday.Add(new PaceOfHoliday()
                                {
                                    Pace = Convert.ToString(HolidayJson["Holiday"]["PaceOfHoliday"][f]["Pace"])
                                });
                            }
                        }
                    }
                    #endregion

                    bool IsRecommended;
                    if (bool.TryParse(Convert.ToString(HolidayJson["Holiday"]["isRecommended"]), out IsRecommended))
                    {
                        objHolidayModel.IsRecommended = IsRecommended;
                    }

                    #region Destinations

                    if (HolidayJson["Holiday"]["Destinations"] != null)
                    {
                        objHolidayModel.Destinations = new List<HolidayDestinations>();
                        var TypeOfdestinations = HolidayJson["Holiday"]["Destinations"].GetType();
                        if (TypeOfdestinations.Name.ToUpper() != "JARRAY")
                        {
                            objHolidayModel.Destinations.Add(new HolidayDestinations()
                            {
                                SupplierCityCode = Convert.ToString(HolidayJson["Holiday"]["Destinations"]["SupplierCityCode"]),
                                SupplierCityName = Convert.ToString(HolidayJson["Holiday"]["Destinations"]["SupplierCityName"]),
                                SupplierCountryCode = Convert.ToString(HolidayJson["Holiday"]["Destinations"]["SupplierCountryCode"]),
                                SupplierCountryName = Convert.ToString(HolidayJson["Holiday"]["Destinations"]["SupplierCountryName"]),
                                TLGXCityCode = Convert.ToString(HolidayJson["Holiday"]["Destinations"]["TLGXCityCode"]),
                                TLGXCityName = Convert.ToString(HolidayJson["Holiday"]["Destinations"]["TLGXCityName"]),
                                TLGXCountryCode = Convert.ToString(HolidayJson["Holiday"]["Destinations"]["TLGXCountryCode"]),
                                TLGXCountryName = Convert.ToString(HolidayJson["Holiday"]["Destinations"]["TLGXCountryName"])
                            });
                        }
                        else
                        {
                            objHolidayModel.Destinations = new List<HolidayDestinations>();
                            int totalDestinations = HolidayJson["Holiday"]["Destinations"].ToList().Count;
                            for (int d = 0; d < totalDestinations; d++)
                            {
                                objHolidayModel.Destinations.Add(new HolidayDestinations()
                                {
                                    SupplierCityCode = Convert.ToString(HolidayJson["Holiday"]["Destinations"][d]["SupplierCityCode"]),
                                    SupplierCityName = Convert.ToString(HolidayJson["Holiday"]["Destinations"][d]["SupplierCityName"]),
                                    SupplierCountryCode = Convert.ToString(HolidayJson["Holiday"]["Destinations"][d]["SupplierCountryCode"]),
                                    SupplierCountryName = Convert.ToString(HolidayJson["Holiday"]["Destinations"][d]["SupplierCountryName"]),
                                    TLGXCityCode = Convert.ToString(HolidayJson["Holiday"]["Destinations"][d]["TLGXCityCode"]),
                                    TLGXCityName = Convert.ToString(HolidayJson["Holiday"]["Destinations"][d]["TLGXCityName"]),
                                    TLGXCountryCode = Convert.ToString(HolidayJson["Holiday"]["Destinations"][d]["TLGXCountryCode"]),
                                    TLGXCountryName = Convert.ToString(HolidayJson["Holiday"]["Destinations"][d]["TLGXCountryName"])
                                });
                            }
                        }
                    }
                    #endregion

                    #region HolidayIncludes
                    if (HolidayJson["Holiday"]["HolidayIncludes"] != null && HolidayJson["Holiday"]["HolidayIncludes"].ToList().Count != 0)
                    {
                        objHolidayModel.HolidayIncludes = new List<HolidayIncludes>();

                        var TypeOfHolidayIncludes = HolidayJson["Holiday"]["HolidayIncludes"].GetType();
                        if (TypeOfHolidayIncludes.Name.ToUpper() != "JARRAY")
                        {
                            objHolidayModel.HolidayIncludes.Add(new HolidayIncludes()
                            {
                                Air = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"]["Air"]),
                                Hotel = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"]["Hotel"]),
                                Insurance = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"]["Insurance"]),
                                Meals = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"]["Meals"]),
                                Sightseeing = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"]["Sightseeing"]),
                                Transfers = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"]["Transfer"]),
                                Visa = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"]["Visa"])
                            });
                        }
                        else
                        {
                            int totalHolidayIncludes = HolidayJson["Holiday"]["HolidayIncludes"].ToList().Count;
                            for (int l = 0; l < totalHolidayIncludes; l++)
                            {
                                objHolidayModel.HolidayIncludes.Add(new HolidayIncludes()
                                {
                                    Air = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"][l]["Air"]),
                                    Hotel = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"][l]["Hotel"]),
                                    Insurance = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"][l]["Insurance"]),
                                    Meals = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"][l]["Meals"]),
                                    Sightseeing = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"][l]["Sightseeing"]),
                                    Transfers = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"][l]["Transfer"]),
                                    Visa = Convert.ToString(HolidayJson["Holiday"]["HolidayIncludes"][l]["Visa"])
                                });
                            }

                        }

                    }
                    #endregion

                    #region UniqueSellingPoints
                    if (HolidayJson["Holiday"]["UniqueSellingPoints"] != null && HolidayJson["Holiday"]["UniqueSellingPoints"].ToList().Count != 0)
                    {
                        var TypeOfUniqueSellingPoints = HolidayJson["Holiday"]["UniqueSellingPoints"].GetType();
                        if (TypeOfUniqueSellingPoints.Name.ToUpper() != "JARRAY")
                        {
                            objHolidayModel.UniqueSellingPoints = new HolidayUniqueSellingPoints();
                            objHolidayModel.UniqueSellingPoints.Id = Convert.ToString(HolidayJson["Holiday"]["UniqueSellingPoints"]["Id"]);
                            objHolidayModel.UniqueSellingPoints.Text = Convert.ToString(HolidayJson["Holiday"]["UniqueSellingPoints"]["Text"]);
                        }
                    }
                    #endregion

                    #region Highlights
                    if (HolidayJson["Holiday"]["Highlights"] != null && HolidayJson["Holiday"]["Highlights"].ToList().Count != 0)
                    {
                        objHolidayModel.Highlights = new List<HolidayHighlights>();
                        var TypeOfHighlights = HolidayJson["Holiday"]["Highlights"].GetType();
                        if (TypeOfHighlights.Name.ToUpper() != "JARRAY")
                        {
                            int day = 0;
                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["Highlights"]["Day"]), out day))
                            {
                                // Conversion successful
                            }
                            DateTime dt;
                            if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["Highlights"]["Date"]), out dt))
                            {
                                // Conversion successful
                            }

                            objHolidayModel.Highlights.Add(new HolidayHighlights()
                            {
                                City = Convert.ToString(HolidayJson["Holiday"]["Highlights"]["City"]),
                                Country = Convert.ToString(HolidayJson["Holiday"]["Highlights"]["Country"]),
                                Date = dt,
                                Day = day,
                                Destination = Convert.ToString(HolidayJson["Holiday"]["Highlights"]["City"]),
                                Text = Convert.ToString(HolidayJson["Holiday"]["Highlights"]["Text"]),
                                Type = Convert.ToString(HolidayJson["Holiday"]["Highlights"]["Type"]),
                            });
                        }
                        else
                        {// It is Array

                            int TotalHighlights = HolidayJson["Holiday"]["Highlights"].ToList().Count;
                            for (int r = 0; r < TotalHighlights; r++)
                            {
                                int day = 0;
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["Highlights"][r]["Day"]), out day))
                                {
                                    // Conversion successful
                                }

                                DateTime? Date = null;
                                DateTime dt;
                                if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["Highlights"][r]["Date"])))
                                {
                                    if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["Highlights"][r]["Date"]), out dt))
                                    {
                                        // Conversion Success
                                        Date = dt;
                                    }
                                }

                                objHolidayModel.Highlights.Add(new HolidayHighlights()
                                {
                                    City = Convert.ToString(HolidayJson["Holiday"]["Highlights"][r]["City"]),
                                    Country = Convert.ToString(HolidayJson["Holiday"]["Highlights"][r]["Country"]),
                                    Date = Date,
                                    Day = day,
                                    Destination = Convert.ToString(HolidayJson["Holiday"]["Highlights"][r]["City"]),
                                    Text = Convert.ToString(HolidayJson["Holiday"]["Highlights"][r]["Text"]),
                                    Type = Convert.ToString(HolidayJson["Holiday"]["Highlights"][r]["Type"]),
                                });
                            }
                        }
                    }
                    #endregion

                    #region Brands
                    if (HolidayJson["Holiday"]["Brands"] != null && HolidayJson["Holiday"]["Brands"].ToList().Count != 0)
                    {
                        objHolidayModel.Brands = new List<HolidayBrandsAndBrochures>();
                        var TypeOfBrands = HolidayJson["Holiday"]["Brands"].GetType();
                        if (TypeOfBrands.Name.ToUpper() != "JARRAY")
                        {
                            objHolidayModel.Brands.Add(new HolidayBrandsAndBrochures()
                            {
                                Code = Convert.ToString(HolidayJson["Holiday"]["Brands"]["Id"]),
                                Id = Convert.ToString(HolidayJson["Holiday"]["Brands"]["Code"]),
                                Name = Convert.ToString(HolidayJson["Holiday"]["Brands"]["Name"]),
                            });
                        }
                        else
                        {
                            int TotalBrands = HolidayJson["Holiday"]["Brands"].ToList().Count;
                            for (int h = 0; h < TotalBrands; h++)
                            {
                                objHolidayModel.Brands.Add(new HolidayBrandsAndBrochures()
                                {
                                    Code = Convert.ToString(HolidayJson["Holiday"]["Brands"][h]["Id"]),
                                    Id = Convert.ToString(HolidayJson["Holiday"]["Brands"][h]["Code"]),
                                    Name = Convert.ToString(HolidayJson["Holiday"]["Brands"][h]["Name"]),
                                });
                            }
                        }
                    }

                    #endregion

                    #region Brochures
                    if (HolidayJson["Holiday"]["Brochures"] != null && HolidayJson["Holiday"]["Brochures"].ToList().Count != 0)
                    {
                        objHolidayModel.Brochures = new List<HolidayBrandsAndBrochures>();
                        var TypeOfBrochures = HolidayJson["Holiday"]["Brochures"].GetType();
                        if (TypeOfBrochures.Name.ToUpper() != "JARRAY")
                        {
                            objHolidayModel.Brochures.Add(new HolidayBrandsAndBrochures()
                            {
                                Code = Convert.ToString(HolidayJson["Holiday"]["Brochures"]["Code"]),
                                Id = Convert.ToString(HolidayJson["Holiday"]["Brochures"]["Id"]),
                                Name = Convert.ToString(HolidayJson["Holiday"]["Brochures"]["Name"])
                            });
                        }
                        else
                        { // It is Jarray
                            int TotalBrochures = HolidayJson["Holiday"]["Brochures"].ToList().Count;
                            for (int d = 0; d < TotalBrochures; d++)
                            {
                                objHolidayModel.Brochures.Add(new HolidayBrandsAndBrochures()
                                {
                                    Code = Convert.ToString(HolidayJson["Holiday"]["Brochures"][d]["Code"]),
                                    Id = Convert.ToString(HolidayJson["Holiday"]["Brochures"][d]["Id"]),
                                    Name = Convert.ToString(HolidayJson["Holiday"]["Brochures"][d]["Name"])
                                });
                            }
                        }
                    }
                    #endregion

                    #region TourNotes
                    if (HolidayJson["Holiday"]["TourNotes"] != null && HolidayJson["Holiday"]["TourNotes"].ToList().Count != 0)
                    {
                        objHolidayModel.TourNotes = new List<HolidayTermsConditions>();
                        var TypeOfTournotes = HolidayJson["Holiday"]["TourNotes"].GetType();
                        if (TypeOfTournotes.Name.ToUpper() != "JARRAY")
                        {
                            int order = 0;
                            if (HolidayJson["Holiday"]["TourNotes"].ToList().Count != 0)
                            {
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["TourNotes"]["Order"]), out order))
                                {
                                    // conversion successful
                                }
                                objHolidayModel.TourNotes.Add(new HolidayTermsConditions()
                                {
                                    Name = Convert.ToString(HolidayJson["Holiday"]["TourNotes"]["Name"]),
                                    Order = order,
                                    Text = Convert.ToString(HolidayJson["Holiday"]["TourNotes"]["Text"]),
                                    ServiceType = Convert.ToString(HolidayJson["Holiday"]["TourNotes"]["ServiceType"]),
                                });
                            }
                        }
                        else
                        {
                            int TotalTourNotes = HolidayJson["Holiday"]["TourNotes"].ToList().Count;
                            for (int s = 0; s < TotalTourNotes; s++)
                            {
                                int order = 0;
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["TourNotes"][s]["Order"]), out order))
                                {
                                    // conversion successful
                                }
                                objHolidayModel.TourNotes.Add(new HolidayTermsConditions()
                                {
                                    Name = Convert.ToString(HolidayJson["Holiday"]["TourNotes"][s]["Name"]),
                                    Order = order,
                                    Text = Convert.ToString(HolidayJson["Holiday"]["TourNotes"][s]["Text"]),
                                    ServiceType = Convert.ToString(HolidayJson["Holiday"]["TourNotes"][s]["ServiceType"]),
                                });
                            }
                        }

                    }
                    #endregion

                    #region Media
                    if (HolidayJson["Holiday"]["Media"] != null && HolidayJson["Holiday"]["Media"].ToList().Count != 0)
                    {
                        objHolidayModel.Media = new List<HolidayMedia>();
                        var TypeOfMedia = HolidayJson["Holiday"]["Media"].GetType();
                        if (TypeOfMedia.Name.ToUpper() != "JARRAY")
                        {
                            int displayOrder = 0;
                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["Media"]["DisplayOrder"]), out displayOrder))
                            {
                                // Conversion Successful
                            }

                            objHolidayModel.Media.Add(new HolidayMedia()
                            {
                                Description = Convert.ToString(HolidayJson["Holiday"]["Media"]["Type"]),
                                ProductDisplayName = Convert.ToString(HolidayJson["Holiday"]["Media"]["ProductDisplayName"]),
                                DisplayOrder = displayOrder,
                                FileCategory = Convert.ToString(HolidayJson["Holiday"]["Media"]["FileCategory"]),
                                LogicalFilePath = Convert.ToString(HolidayJson["Holiday"]["Media"]["LogicalFilePath"]),
                                FileType = Convert.ToString(HolidayJson["Holiday"]["Media"]["FileType"]),
                                Height = Convert.ToString(HolidayJson["Holiday"]["Media"]["Height"]),
                                Name = Convert.ToString(HolidayJson["Holiday"]["Media"]["Name"]),
                                URL = Convert.ToString(HolidayJson["Holiday"]["Media"]["URL"]),
                                City = Convert.ToString(HolidayJson["Holiday"]["Media"]["City"]),
                                Country = Convert.ToString(HolidayJson["Holiday"]["Media"]["Country"]),
                                Width = Convert.ToString(HolidayJson["Holiday"]["Media"]["Width"]),
                                State = Convert.ToString(HolidayJson["Holiday"]["Media"]["State"])
                            });
                        }
                        else
                        {// it is Collection
                            int TotalMedia = HolidayJson["Holiday"]["Media"].ToList().Count;
                            for (int g = 0; g < TotalMedia; g++)
                            {
                                int displayOrder = 0;
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["Media"][g]["DisplayOrder"]), out displayOrder))
                                {
                                    // Conversion Successful
                                }

                                objHolidayModel.Media.Add(new HolidayMedia()
                                {
                                    Description = Convert.ToString(HolidayJson["Holiday"]["Media"][g]["Type"]),
                                    ProductDisplayName = Convert.ToString(HolidayJson["Holiday"]["Media"][g]["ProductDisplayName"]),
                                    DisplayOrder = displayOrder,
                                    FileCategory = Convert.ToString(HolidayJson["Holiday"]["Media"][g]["FileCategory"]),
                                    LogicalFilePath = Convert.ToString(HolidayJson["Holiday"]["Media"][g]["LogicalFilePath"]),
                                    FileType = Convert.ToString(HolidayJson["Holiday"]["Media"][g]["FileType"]),
                                    Height = Convert.ToString(HolidayJson["Holiday"]["Media"][g]["Height"]),
                                    Name = Convert.ToString(HolidayJson["Holiday"]["Media"][g]["Name"]),
                                    URL = Convert.ToString(HolidayJson["Holiday"]["Media"][g]["URL"]),
                                    City = Convert.ToString(HolidayJson["Holiday"]["Media"][g]["City"]),
                                    Country = Convert.ToString(HolidayJson["Holiday"]["Media"][g]["Country"]),
                                    Width = Convert.ToString(HolidayJson["Holiday"]["Media"][g]["Width"]),
                                    State = Convert.ToString(HolidayJson["Holiday"]["Media"][g]["State"])
                                });
                            }

                        }
                    }
                    #endregion

                    #region Inclusions
                    if (HolidayJson["Holiday"]["Inclusions"] != null && HolidayJson["Holiday"]["Inclusions"].ToList().Count != 0)
                    {
                        objHolidayModel.Inclusions = new List<HolidayInclusionExclusion>();
                        var TypeOfInclusions = HolidayJson["Holiday"]["Inclusions"].GetType();
                        if (TypeOfInclusions.Name.ToUpper() != "JARRAY")
                        { // It is a single object
                            int Order = 0;
                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["Inclusions"]["Order"]), out Order))
                            {
                                // Conversion Success
                            }
                            objHolidayModel.Inclusions.Add(new HolidayInclusionExclusion()
                            {
                                Name = Convert.ToString(HolidayJson["Holiday"]["Inclusions"]["Name"]),
                                ServiceType = Convert.ToString(HolidayJson["Holiday"]["Inclusions"]["ServiceType"]),
                                Text = Convert.ToString(HolidayJson["Holiday"]["Inclusions"]["Text"]),
                                Order = Order
                            });
                        }
                        else
                        {
                            int totalInclusions = HolidayJson["Holiday"]["Inclusions"].ToList().Count;
                            for (int f = 0; f < totalInclusions; f++)
                            {
                                int Order = 0;
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["Inclusions"][f]["Order"]), out Order))
                                {
                                    // Conversion Success
                                }
                                objHolidayModel.Inclusions.Add(new HolidayInclusionExclusion()
                                {
                                    Name = Convert.ToString(HolidayJson["Holiday"]["Inclusions"][f]["Name"]),
                                    ServiceType = Convert.ToString(HolidayJson["Holiday"]["Inclusions"][f]["ServiceType"]),
                                    Text = Convert.ToString(HolidayJson["Holiday"]["Inclusions"][f]["Text"]),
                                    Order = Order
                                });
                            }
                        }
                    }

                    #endregion

                    #region ExclusionsObj
                    if (HolidayJson["Holiday"]["ExclusionsObj"] != null && HolidayJson["Holiday"]["ExclusionsObj"].ToList().Count != 0)
                    {
                        objHolidayModel.Exclusions = new List<HolidayInclusionExclusion>();
                        var TypeOfExclusionObj = HolidayJson["Holiday"]["ExclusionsObj"].GetType();
                        if (TypeOfExclusionObj.Name.ToUpper() != "JARRAY")
                        { // It is object
                            int Order = 0;
                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["ExclusionsObj"]["Order"]), out Order))
                            {
                                // Conversion Success
                            }
                            objHolidayModel.Exclusions.Add(new HolidayInclusionExclusion()
                            {
                                Name = Convert.ToString(HolidayJson["Holiday"]["ExclusionsObj"]["Name"]),
                                ServiceType = Convert.ToString(HolidayJson["Holiday"]["ExclusionsObj"]["ServiceType"]),
                                Text = Convert.ToString(HolidayJson["Holiday"]["ExclusionsObj"]["Text"]),
                                Order = Order
                            });
                        }
                        else
                        {  // It is Array
                            int TotalExclusionObj = HolidayJson["Holiday"]["ExclusionsObj"].ToList().Count;
                            for (int a = 0; a < TotalExclusionObj; a++)
                            {
                                int Order = 0;
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["ExclusionsObj"][a]["Order"]), out Order))
                                {
                                    // Conversion Success
                                }
                                objHolidayModel.Exclusions.Add(new HolidayInclusionExclusion()
                                {
                                    Name = Convert.ToString(HolidayJson["Holiday"]["ExclusionsObj"][a]["Name"]),
                                    ServiceType = Convert.ToString(HolidayJson["Holiday"]["ExclusionsObj"][a]["ServiceType"]),
                                    Text = Convert.ToString(HolidayJson["Holiday"]["ExclusionsObj"][a]["Text"]),
                                    Order = Order
                                });
                            }
                        }
                    }
                    #endregion

                    #region TermsConditions
                    if (HolidayJson["Holiday"]["TermsConditions"] != null && HolidayJson["Holiday"]["TermsConditions"].ToList().Count != 0)
                    {
                        objHolidayModel.TermsConditions = new List<HolidayTermsConditions>();
                        var TypeOfTermsConditions = HolidayJson["Holiday"]["TermsConditions"].GetType();
                        if (TypeOfTermsConditions.Name.ToUpper() != "JARRAY")
                        {
                            int Order = 0;
                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["TermsConditions"]["Order"]), out Order))
                            {
                                // Conversion Success
                            }
                            objHolidayModel.TermsConditions.Add(new HolidayTermsConditions()
                            {
                                Name = Convert.ToString(HolidayJson["Holiday"]["TermsConditions"]["Name"]),
                                ServiceType = Convert.ToString(HolidayJson["Holiday"]["TermsConditions"]["ServiceType"]),
                                Text = Convert.ToString(HolidayJson["Holiday"]["TermsConditions"]["Text"]),
                                Order = Order
                            });
                        }
                        else
                        {
                            int TotalTermsConditions = HolidayJson["Holiday"]["TermsConditions"].ToList().Count;
                            for (int g = 0; g < TotalTermsConditions; g++)
                            {
                                int Order = 0;
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["TermsConditions"][g]["Order"]), out Order))
                                {
                                    // Conversion Success
                                }
                                objHolidayModel.TermsConditions.Add(new HolidayTermsConditions()
                                {
                                    Name = Convert.ToString(HolidayJson["Holiday"]["TermsConditions"][g]["Name"]),
                                    ServiceType = Convert.ToString(HolidayJson["Holiday"]["TermsConditions"][g]["ServiceType"]),
                                    Text = Convert.ToString(HolidayJson["Holiday"]["TermsConditions"][g]["Text"]),
                                    Order = Order
                                });
                            }
                        }
                    }
                    #endregion

                    #region BookingPolicy
                    if (HolidayJson["Holiday"]["BookingPolicy"] != null && HolidayJson["Holiday"]["BookingPolicy"].ToList().Count != 0)
                    {
                        objHolidayModel.BookingPolicy = new List<HolidayTermsConditions>();
                        var TypeOfBookingPolicy = HolidayJson["Holiday"]["BookingPolicy"].GetType();
                        if (TypeOfBookingPolicy.Name.ToUpper() != "JARRAY")
                        {
                            int Order = 0;
                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["BookingPolicy"]["Order"]), out Order))
                            {
                                // Conversion Success
                            }
                            objHolidayModel.BookingPolicy.Add(new HolidayTermsConditions()
                            {
                                Name = Convert.ToString(HolidayJson["Holiday"]["BookingPolicy"]["Name"]),
                                ServiceType = Convert.ToString(HolidayJson["Holiday"]["BookingPolicy"]["ServiceType"]),
                                Text = Convert.ToString(HolidayJson["Holiday"]["BookingPolicy"]["Text"]),
                                Order = Order
                            });
                        }
                        else
                        {
                            int TotalBookingPolicy = HolidayJson["Holiday"]["BookingPolicy"].ToList().Count;
                            for (int g = 0; g < TotalBookingPolicy; g++)
                            {
                                int Order = 0;
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["BookingPolicy"][g]["Order"]), out Order))
                                {
                                    // Conversion Success
                                }
                                objHolidayModel.BookingPolicy.Add(new HolidayTermsConditions()
                                {
                                    Name = Convert.ToString(HolidayJson["Holiday"]["BookingPolicy"][g]["Name"]),
                                    ServiceType = Convert.ToString(HolidayJson["Holiday"]["BookingPolicy"][g]["ServiceType"]),
                                    Text = Convert.ToString(HolidayJson["Holiday"]["BookingPolicy"][g]["Text"]),
                                    Order = Order
                                });
                            }
                        }
                    }
                    #endregion

                    #region DayWiseItineraries
                    if (HolidayJson["Holiday"]["DayWiseItineraries"] != null && HolidayJson["Holiday"]["DayWiseItineraries"].ToList().Count != 0)
                    {
                        objHolidayModel.DayWiseItineraries = new List<HolidayDayWiseItineraries>();
                        var TypeOfDayWiseItineraries = HolidayJson["Holiday"]["DayWiseItineraries"].GetType();
                        if (TypeOfDayWiseItineraries.Name.ToUpper() != "JARRAY")
                        {
                            HolidayDayWiseItineraries objHolidayDayWiseItineraries = new HolidayDayWiseItineraries();
                            objHolidayDayWiseItineraries.Dates = new List<HolidayDates>();
                            objHolidayDayWiseItineraries.TagType = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["TagType"]);
                            objHolidayDayWiseItineraries.Destination = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["Destination"]);
                            objHolidayDayWiseItineraries.TLGXCountry = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["TLGXCountry"]);
                            objHolidayDayWiseItineraries.TLGXCity = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["TLGXCity"]);
                            objHolidayDayWiseItineraries.SupplierCountry = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["SupplierCountry"]);
                            objHolidayDayWiseItineraries.SupplierCity = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["SupplierCity"]);
                            objHolidayDayWiseItineraries.ProductCategory = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["ProductCategory"]);
                            objHolidayDayWiseItineraries.ProductCategorySubType = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["ProductCategorySubType"]);
                            objHolidayDayWiseItineraries.ProductName = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["ProductName"]);
                            objHolidayDayWiseItineraries.ProductId = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["ProductId"]);
                            objHolidayDayWiseItineraries.LongDescription = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["LongDescription"]);
                            objHolidayDayWiseItineraries.ShortDescription = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["ShortDescription"]);
                            objHolidayDayWiseItineraries.TagDescription = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["TagDescription"]);
                            objHolidayDayWiseItineraries.StartTime = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["StartTime"]);
                            objHolidayDayWiseItineraries.EndTime = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["EndTime"]);
                            objHolidayDayWiseItineraries.Session = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["Session"]);
                            objHolidayDayWiseItineraries.DynamicAttributes = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["DynamicAttributes"]);

                            #region Dates
                            if (HolidayJson["Holiday"]["DayWiseItineraries"]["Dates"] != null)
                            {
                                int TotdalDates = HolidayJson["Holiday"]["DayWiseItineraries"]["Dates"].ToList().Count;
                                for (int i = 0; i < TotdalDates; i++)
                                {
                                    HolidayDates objHolidayDates = new HolidayDates();
                                    objHolidayDates.Description = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["Dates"][i]["Description"]);
                                    objHolidayDates.TagDescription = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["Dates"][i]["TagDescription"]);
                                    objHolidayDates.DynamicAttributes = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["Dates"][i]["DynamicAttributes"]);
                                    objHolidayDates.Status = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["Dates"][i]["Status"]);
                                    int FromDay = 0;
                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["Dates"][i]["FromDay"]), out FromDay))
                                    {
                                        // Conversion SuccessFul
                                    }
                                    int FromMonth = 0;
                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["Dates"][i]["FromMonth"]), out FromMonth))
                                    {
                                        // Conversion SuccessFul
                                    }
                                    int FromYear = 0;
                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["Dates"][i]["FromYear"]), out FromYear))
                                    {
                                        // Conversion SuccessFul
                                    }
                                    int ToDay = 0;
                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["Dates"][i]["ToDay"]), out ToDay))
                                    {
                                        // Conversion SuccessFul
                                    }
                                    int ToMonth = 0;
                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["Dates"][i]["ToMonth"]), out ToMonth))
                                    {
                                        // Conversion SuccessFul
                                    }
                                    int ToYear = 0;
                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"]["Dates"][i]["ToYear"]), out ToYear))
                                    {
                                        // Conversion SuccessFul
                                    }
                                    objHolidayDates.ToYear = ToYear;
                                    objHolidayDates.ToMonth = ToMonth;
                                    objHolidayDates.ToDay = ToDay;
                                    objHolidayDates.FromYear = FromYear;
                                    objHolidayDates.FromMonth = FromMonth;
                                    objHolidayDates.FromDay = FromDay;

                                    objHolidayDayWiseItineraries.Dates.Add(objHolidayDates);
                                }

                            }
                            #endregion
                        }
                        else
                        {
                            int totalDayWiseItineraries = HolidayJson["Holiday"]["DayWiseItineraries"].ToList().Count;
                            for (int l = 0; l < totalDayWiseItineraries; l++)
                            {
                                HolidayDayWiseItineraries objHolidayDayWiseItineraries = new HolidayDayWiseItineraries();
                                objHolidayDayWiseItineraries.Dates = new List<HolidayDates>();
                                objHolidayDayWiseItineraries.TagType = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["TagType"]);
                                objHolidayDayWiseItineraries.Destination = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["Destination"]);
                                objHolidayDayWiseItineraries.TLGXCountry = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["TLGXCountry"]);
                                objHolidayDayWiseItineraries.TLGXCity = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["TLGXCity"]);
                                objHolidayDayWiseItineraries.SupplierCountry = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["SupplierCountry"]);
                                objHolidayDayWiseItineraries.SupplierCity = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["SupplierCity"]);
                                objHolidayDayWiseItineraries.ProductCategory = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["ProductCategory"]);
                                objHolidayDayWiseItineraries.ProductCategorySubType = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["ProductCategorySubType"]);
                                objHolidayDayWiseItineraries.ProductName = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["ProductName"]);
                                objHolidayDayWiseItineraries.ProductId = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["ProductId"]);
                                objHolidayDayWiseItineraries.LongDescription = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["LongDescription"]);
                                objHolidayDayWiseItineraries.ShortDescription = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["ShortDescription"]);
                                objHolidayDayWiseItineraries.TagDescription = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["TagDescription"]);
                                objHolidayDayWiseItineraries.StartTime = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["StartTime"]);
                                objHolidayDayWiseItineraries.EndTime = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["EndTime"]);
                                objHolidayDayWiseItineraries.Session = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["Session"]);
                                objHolidayDayWiseItineraries.DynamicAttributes = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["DynamicAttributes"]);

                                #region Dates
                                if (HolidayJson["Holiday"]["DayWiseItineraries"][l]["Dates"] != null)
                                {
                                    int TotdalDates = HolidayJson["Holiday"]["DayWiseItineraries"][l]["Dates"].ToList().Count;
                                    for (int i = 0; i < TotdalDates; i++)
                                    {
                                        HolidayDates objHolidayDates = new HolidayDates();
                                        objHolidayDates.Description = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["Dates"][i]["Description"]);
                                        objHolidayDates.TagDescription = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["Dates"][i]["TagDescription"]);
                                        objHolidayDates.DynamicAttributes = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["Dates"][i]["DynamicAttributes"]);
                                        objHolidayDates.Status = Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["Dates"][i]["Status"]);
                                        int FromDay = 0;
                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["Dates"][i]["FromDay"]), out FromDay))
                                        {
                                            // Conversion SuccessFul
                                        }
                                        int FromMonth = 0;
                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["Dates"][i]["FromMonth"]), out FromMonth))
                                        {
                                            // Conversion SuccessFul
                                        }
                                        int FromYear = 0;
                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["Dates"][i]["FromYear"]), out FromYear))
                                        {
                                            // Conversion SuccessFul
                                        }
                                        int ToDay = 0;
                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["Dates"][i]["ToDay"]), out ToDay))
                                        {
                                            // Conversion SuccessFul
                                        }
                                        int ToMonth = 0;
                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["Dates"][i]["ToMonth"]), out ToMonth))
                                        {
                                            // Conversion SuccessFul
                                        }
                                        int ToYear = 0;
                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["DayWiseItineraries"][l]["Dates"][i]["ToYear"]), out ToYear))
                                        {
                                            // Conversion SuccessFul
                                        }
                                        objHolidayDates.ToYear = ToYear;
                                        objHolidayDates.ToMonth = ToMonth;
                                        objHolidayDates.ToDay = ToDay;
                                        objHolidayDates.FromYear = FromYear;
                                        objHolidayDates.FromMonth = FromMonth;
                                        objHolidayDates.FromDay = FromDay;

                                        objHolidayDayWiseItineraries.Dates.Add(objHolidayDates);
                                    }

                                }
                                #endregion

                                objHolidayModel.DayWiseItineraries.Add(objHolidayDayWiseItineraries);
                            }

                        }
                    }
                    #endregion

                    #region Accommodation
                    if (HolidayJson["Holiday"]["Accommodation"] != null && HolidayJson["Holiday"]["Accommodation"].ToList().Count != 0)
                    {
                        objHolidayModel.Accommodation = new List<HolidayAccommodation>();
                        var TypeOfAccommodation = HolidayJson["Holiday"]["Accommodation"].GetType();
                        if (TypeOfAccommodation.Name.ToUpper() != "JARRAY")
                        {
                            int NumberOfNights = 0;
                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["NumberOfNights"]), out NumberOfNights))
                            {
                                // Conversion Successful
                            }
                            objHolidayModel.Accommodation.Add(new HolidayAccommodation()
                            {
                                HostCategory = Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["HostCategory"]),
                                HotelCategory = Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["HotelCategory"]),
                                HotelName = Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["HotelName"]),
                                PackageCategory = Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["PackageCategory"]),
                                SupplierCityCode = Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["SupplierCityCode"]),
                                SupplierCityName = Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["SupplierCityName"]),
                                SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["SupplierProductCode"]),
                                TLGXCityCode = Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["TLGXCityCode"]),
                                TLGXCountryCode = Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["TLGXCountryCode"]),
                                TLGXCountryName = Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["TLGXCountryName"]),
                                TLGXProductCode = Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["TLGXProductCode"]),
                                TLGXCityName = Convert.ToString(HolidayJson["Holiday"]["Accommodation"]["TLGXCityName"]),
                                NumberOfNights = NumberOfNights
                            });
                        }
                        else
                        {
                            int TotalAccommodation = HolidayJson["Holiday"]["Accommodation"].ToList().Count;
                            for (int b = 0; b < TotalAccommodation; b++)
                            {
                                int NumberOfNights = 0;
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["NumberOfNights"]), out NumberOfNights))
                                {
                                    // Conversion Successful
                                }
                                objHolidayModel.Accommodation.Add(new HolidayAccommodation()
                                {
                                    HostCategory = Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["HostCategory"]),
                                    HotelCategory = Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["HotelCategory"]),
                                    HotelName = Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["HotelName"]),
                                    PackageCategory = Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["PackageCategory"]),
                                    SupplierCityCode = Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["SupplierCityCode"]),
                                    SupplierCityName = Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["SupplierCityName"]),
                                    SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["SupplierProductCode"]),
                                    TLGXCityCode = Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["TLGXCityCode"]),
                                    TLGXCountryCode = Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["TLGXCountryCode"]),
                                    TLGXCountryName = Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["TLGXCountryName"]),
                                    TLGXProductCode = Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["TLGXProductCode"]),
                                    TLGXCityName = Convert.ToString(HolidayJson["Holiday"]["Accommodation"][b]["TLGXCityName"]),
                                    NumberOfNights = NumberOfNights
                                });
                            }

                        }
                    }
                    #endregion

                    #region Activity
                    if (HolidayJson["Holiday"]["Activity"] != null && HolidayJson["Holiday"]["Activity"].ToList().Count != 0)
                    {
                        objHolidayModel.Activity = new List<HolidayActivity>();
                        var TypeOfActivity = HolidayJson["Holiday"]["Activity"].GetType();
                        if (TypeOfActivity.Name.ToUpper() != "JARRAY")
                        {
                            DateTime? Date = null;
                            DateTime dt;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["Activity"]["Date"])))
                            {
                                if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["Activity"]["Date"]), out dt))
                                {
                                    // Conversion Success
                                    Date = dt;
                                }
                            }

                            DateTime? DepartureEndDate = null;
                            DateTime dt1;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["Activity"]["DepartureEndDate"])))
                            {
                                if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["Activity"]["DepartureEndDate"]), out dt1))
                                {
                                    // Conversion Success
                                    DepartureEndDate = dt1;
                                }
                            }

                            DateTime? DepartureStartDate = null;
                            DateTime dt2;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["Activity"]["DepartureStartDate"])))
                            {
                                if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["Activity"]["DepartureStartDate"]), out dt2))
                                {
                                    // Conversion Success
                                    DepartureEndDate = dt2;
                                }
                            }

                            DateTime? StartDate = null;
                            DateTime dt3;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["Activity"]["StartDate"])))
                            {
                                if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["Activity"]["StartDate"]), out dt3))
                                {
                                    // Conversion Success
                                    DepartureEndDate = dt3;
                                }
                            }

                            int DayNumber = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["Activity"]["DayNumber"])))
                            {
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["Activity"]["DayNumber"]), out DayNumber))
                                {
                                    // Conversion Success
                                }
                            }

                            objHolidayModel.Activity.Add(new HolidayActivity()
                            {
                                DepartureId = Convert.ToString(HolidayJson["Holiday"]["Activity"]["DepartureId"]),
                                DropDestination = Convert.ToString(HolidayJson["Holiday"]["Activity"]["DropDestination"]),
                                EndTime = Convert.ToString(HolidayJson["Holiday"]["Activity"]["EndTime"]),
                                HostCategory = Convert.ToString(HolidayJson["Holiday"]["Activity"]["HostCategory"]),
                                PickupDestination = Convert.ToString(HolidayJson["Holiday"]["Activity"]["PickupDestination"]),
                                ProductCategory = Convert.ToString(HolidayJson["Holiday"]["Activity"]["ProductCategory"]),
                                Session = Convert.ToString(HolidayJson["Holiday"]["Activity"]["Session"]),
                                StartTime = Convert.ToString(HolidayJson["Holiday"]["Activity"]["StartTime"]),
                                SupplierCityCode = Convert.ToString(HolidayJson["Holiday"]["Activity"]["SupplierCityCode"]),
                                SupplierCityName = Convert.ToString(HolidayJson["Holiday"]["Activity"]["SupplierCityName"]),
                                SupplierCountryCode = Convert.ToString(HolidayJson["Holiday"]["Activity"]["SupplierCountryCode"]),
                                SupplierCountryName = Convert.ToString(HolidayJson["Holiday"]["Activity"]["SupplierCountryName"]),
                                TLGXActivityId = Convert.ToString(HolidayJson["Holiday"]["Activity"]["TLGXActivityId"]),
                                TLGXActivityName = Convert.ToString(HolidayJson["Holiday"]["Activity"]["TLGXActivityName"]),
                                TLGXCityCode = Convert.ToString(HolidayJson["Holiday"]["Activity"]["TLGXCityCode"]),
                                TLGXCityName = Convert.ToString(HolidayJson["Holiday"]["Activity"]["TLGXCityName"]),
                                TLGXCountryCode = Convert.ToString(HolidayJson["Holiday"]["Activity"]["TLGXCountryCode"]),
                                TLGXCountryName = Convert.ToString(HolidayJson["Holiday"]["Activity"]["TLGXCountryName"]),
                                Date = Date,
                                DayNumber = DayNumber,
                                DepartureEndDate = DepartureEndDate,
                                DepartureStartDate = DepartureStartDate,
                                StartDate = StartDate

                            });
                        }
                        else
                        { // Its Array
                            int TotalActivities = HolidayJson["Holiday"]["Activity"].ToList().Count;
                            for (int m = 0; m < TotalActivities; m++)
                            {
                                DateTime? Date = null;
                                DateTime dt;
                                if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["Date"])))
                                {
                                    if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["Date"]), out dt))
                                    {
                                        // Conversion Success
                                        Date = dt;
                                    }
                                }

                                DateTime? DepartureEndDate = null;
                                DateTime dt1;
                                if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["DepartureEndDate"])))
                                {
                                    if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["DepartureEndDate"]), out dt1))
                                    {
                                        // Conversion Success
                                        DepartureEndDate = dt1;
                                    }
                                }

                                DateTime? DepartureStartDate = null;
                                DateTime dt2;
                                if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["DepartureStartDate"])))
                                {
                                    if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["DepartureStartDate"]), out dt2))
                                    {
                                        // Conversion Success
                                        DepartureEndDate = dt2;
                                    }
                                }

                                DateTime? StartDate = null;
                                DateTime dt3;
                                if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["StartDate"])))
                                {
                                    if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["StartDate"]), out dt3))
                                    {
                                        // Conversion Success
                                        DepartureEndDate = dt3;
                                    }
                                }

                                int DayNumber = 0;
                                if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["DayNumber"])))
                                {
                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["DayNumber"]), out DayNumber))
                                    {
                                        // Conversion Success
                                    }
                                }

                                objHolidayModel.Activity.Add(new HolidayActivity()
                                {
                                    DepartureId = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["DepartureId"]),
                                    DropDestination = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["DropDestination"]),
                                    EndTime = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["EndTime"]),
                                    HostCategory = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["HostCategory"]),
                                    PickupDestination = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["PickupDestination"]),
                                    ProductCategory = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["ProductCategory"]),
                                    Session = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["Session"]),
                                    StartTime = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["StartTime"]),
                                    SupplierCityCode = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["SupplierCityCode"]),
                                    SupplierCityName = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["SupplierCityName"]),
                                    SupplierCountryCode = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["SupplierCountryCode"]),
                                    SupplierCountryName = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["SupplierCountryName"]),
                                    TLGXActivityId = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["TLGXActivityId"]),
                                    TLGXActivityName = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["TLGXActivityName"]),
                                    TLGXCityCode = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["TLGXCityCode"]),
                                    TLGXCityName = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["TLGXCityName"]),
                                    TLGXCountryCode = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["TLGXCountryCode"]),
                                    TLGXCountryName = Convert.ToString(HolidayJson["Holiday"]["Activity"][m]["TLGXCountryName"]),
                                    Date = Date,
                                    DayNumber = DayNumber,
                                    DepartureEndDate = DepartureEndDate,
                                    DepartureStartDate = DepartureStartDate,
                                    StartDate = StartDate

                                });
                            }
                        }
                    }

                    #endregion

                    #region Departure
                    if (HolidayJson["Holiday"]["Departure"] != null && HolidayJson["Holiday"]["Departure"].ToList().Count != 0)
                    {
                        objHolidayModel.Departure = new List<HolidayDeparture>();
                        var TypeOfDeparture = HolidayJson["Holiday"]["Departure"].GetType();
                        if (!string.Equals(TypeOfDeparture.Name, "JARRAY", StringComparison.OrdinalIgnoreCase))
                        {
                            objHolidayModel.Departure.Add(new HolidayDeparture()
                            {
                                StartDate = Convert.ToString(HolidayJson["Holiday"]["Departure"]["StartDate"]),
                                DepartureId = Convert.ToString(HolidayJson["Holiday"]["Departure"]["DepartureId"]),
                                TLGXCityCodeEnd = Convert.ToString(HolidayJson["Holiday"]["Departure"]["TLGXCityCodeEnd"]),
                                TLGXCityCodeStart = Convert.ToString(HolidayJson["Holiday"]["Departure"]["TLGXCityCodeStart"]),
                                TLGXCountryCodeEnd = Convert.ToString(HolidayJson["Holiday"]["Departure"]["TLGXCountryCodeEnd"]),
                                TLGXCountryCodeStart = Convert.ToString(HolidayJson["Holiday"]["Departure"]["TLGXCountryCodeStart"]),
                            });

                        }
                        else
                        { // It is Array
                            int TotalDeparture = HolidayJson["Holiday"]["Departure"].ToList().Count;


                            for (int o = 0; o < TotalDeparture; o++)
                            {
                                objHolidayModel.Departure.Add(new HolidayDeparture()
                                {
                                    StartDate = Convert.ToString(HolidayJson["Holiday"]["Departure"][o]["StartDate"]),
                                    DepartureId = Convert.ToString(HolidayJson["Holiday"]["Departure"][o]["DepartureId"]),
                                    TLGXCityCodeEnd = Convert.ToString(HolidayJson["Holiday"]["Departure"][o]["TLGXCityCodeEnd"]),
                                    TLGXCityCodeStart = Convert.ToString(HolidayJson["Holiday"]["Departure"][o]["TLGXCityCodeStart"]),
                                    TLGXCountryCodeEnd = Convert.ToString(HolidayJson["Holiday"]["Departure"][o]["TLGXCountryCodeEnd"]),
                                    TLGXCountryCodeStart = Convert.ToString(HolidayJson["Holiday"]["Departure"][o]["TLGXCountryCodeStart"]),
                                });
                            }
                        }
                    }
                    #endregion

                    #region PreTour
                    if (HolidayJson["Holiday"]["PreTour"] != null && HolidayJson["Holiday"]["PreTour"].ToList().Count != 0)
                    {
                        objHolidayModel.PreTour = new List<PreTourStructure>();
                        var TypeOfPreTour = HolidayJson["Holiday"]["PreTour"].GetType();
                        if (TypeOfPreTour.Name.ToUpper() != "JARRAY")
                        { // it is Object
                            int NumberOfNights = 0;
                            if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTour"]["NumberOfNights"])))
                            {
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTour"]["NumberOfNights"]), out NumberOfNights))
                                {
                                    // Conversion Success
                                }
                            }

                            objHolidayModel.PreTour.Add(new PreTourStructure()
                            {
                                HostCategory = Convert.ToString(HolidayJson["Holiday"]["PreTour"]["HostCategory"]),
                                HotelCategory = Convert.ToString(HolidayJson["Holiday"]["PreTour"]["HotelCategory"]),
                                HotelName = Convert.ToString(HolidayJson["Holiday"]["PreTour"]["HotelName"]),
                                NumberOfNights = NumberOfNights,
                                PackageCategory = Convert.ToString(HolidayJson["Holiday"]["PreTour"]["PackageCategory"]),
                                SupplierCityCode = Convert.ToString(HolidayJson["Holiday"]["PreTour"]["SupplierCityCode"]),
                                SupplierCityName = Convert.ToString(HolidayJson["Holiday"]["PreTour"]["SupplierCityName"]),
                                SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["PreTour"]["SupplierProductCode"]),
                                TLGXCityCode = Convert.ToString(HolidayJson["Holiday"]["PreTour"]["TLGXCityCode"]),
                                TLGXCityName = Convert.ToString(HolidayJson["Holiday"]["PreTour"]["TLGXCityName"]),
                                TLGXCountryCode = Convert.ToString(HolidayJson["Holiday"]["PreTour"]["TLGXCountryCode"]),
                                TLGXCountryName = Convert.ToString(HolidayJson["Holiday"]["PreTour"]["TLGXCountryName"]),
                                TLGXProductCode = Convert.ToString(HolidayJson["Holiday"]["PreTour"]["TLGXProductCode"]),
                            });
                        }
                        else
                        { // It is Array
                            int TotalPreTOurNodes = HolidayJson["Holiday"]["PreTour"].ToList().Count;
                            for (int a = 0; a < TotalPreTOurNodes; a++)
                            {
                                int NumberOfNights = 0;
                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["NumberOfNights"])))
                                {
                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["NumberOfNights"]), out NumberOfNights))
                                    {
                                        // Conversion Success
                                    }
                                }

                                objHolidayModel.PreTour.Add(new PreTourStructure()
                                {
                                    HostCategory = Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["HostCategory"]),
                                    HotelCategory = Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["HotelCategory"]),
                                    HotelName = Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["HotelName"]),
                                    NumberOfNights = NumberOfNights,
                                    PackageCategory = Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["PackageCategory"]),
                                    SupplierCityCode = Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["SupplierCityCode"]),
                                    SupplierCityName = Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["SupplierCityName"]),
                                    SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["SupplierProductCode"]),
                                    TLGXCityCode = Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["TLGXCityCode"]),
                                    TLGXCityName = Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["TLGXCityName"]),
                                    TLGXCountryCode = Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["TLGXCountryCode"]),
                                    TLGXCountryName = Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["TLGXCountryName"]),
                                    TLGXProductCode = Convert.ToString(HolidayJson["Holiday"]["PreTour"][a]["TLGXProductCode"]),
                                });
                            }

                        }

                    }
                    #endregion

                    #region PostTour
                    if (HolidayJson["Holiday"]["PostTour"] != null && HolidayJson["Holiday"]["PostTour"].ToList().Count != 0)
                    {
                        objHolidayModel.PostTour = new List<PreTourStructure>();
                        var TypeOfPostTour = HolidayJson["Holiday"]["PostTour"].GetType();
                        if (TypeOfPostTour.Name.ToUpper() != "JARRAY")
                        { // it is Object
                            int NumberOfNights = 0;
                            if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTour"]["NumberOfNights"])))
                            {
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTour"]["NumberOfNights"]), out NumberOfNights))
                                {
                                    // Conversion Success
                                }
                            }

                            objHolidayModel.PostTour.Add(new PreTourStructure()
                            {
                                HostCategory = Convert.ToString(HolidayJson["Holiday"]["PostTour"]["HostCategory"]),
                                HotelCategory = Convert.ToString(HolidayJson["Holiday"]["PostTour"]["HotelCategory"]),
                                HotelName = Convert.ToString(HolidayJson["Holiday"]["PostTour"]["HotelName"]),
                                NumberOfNights = NumberOfNights,
                                PackageCategory = Convert.ToString(HolidayJson["Holiday"]["PostTour"]["PackageCategory"]),
                                SupplierCityCode = Convert.ToString(HolidayJson["Holiday"]["PostTour"]["SupplierCityCode"]),
                                SupplierCityName = Convert.ToString(HolidayJson["Holiday"]["PostTour"]["SupplierCityName"]),
                                SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["PostTour"]["SupplierProductCode"]),
                                TLGXCityCode = Convert.ToString(HolidayJson["Holiday"]["PostTour"]["TLGXCityCode"]),
                                TLGXCityName = Convert.ToString(HolidayJson["Holiday"]["PostTour"]["TLGXCityName"]),
                                TLGXCountryCode = Convert.ToString(HolidayJson["Holiday"]["PostTour"]["TLGXCountryCode"]),
                                TLGXCountryName = Convert.ToString(HolidayJson["Holiday"]["PostTour"]["TLGXCountryName"]),
                                TLGXProductCode = Convert.ToString(HolidayJson["Holiday"]["PostTour"]["TLGXProductCode"]),
                            });
                        }
                        else
                        { // It is Array
                            int TotalPostTourNodes = HolidayJson["Holiday"]["PostTour"].ToList().Count;
                            for (int a = 0; a < TotalPostTourNodes; a++)
                            {
                                int NumberOfNights = 0;
                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["NumberOfNights"])))
                                {
                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["NumberOfNights"]), out NumberOfNights))
                                    {
                                        // Conversion Success
                                    }
                                }

                                objHolidayModel.PostTour.Add(new PreTourStructure()
                                {
                                    HostCategory = Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["HostCategory"]),
                                    HotelCategory = Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["HotelCategory"]),
                                    HotelName = Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["HotelName"]),
                                    NumberOfNights = NumberOfNights,
                                    PackageCategory = Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["PackageCategory"]),
                                    SupplierCityCode = Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["SupplierCityCode"]),
                                    SupplierCityName = Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["SupplierCityName"]),
                                    SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["SupplierProductCode"]),
                                    TLGXCityCode = Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["TLGXCityCode"]),
                                    TLGXCityName = Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["TLGXCityName"]),
                                    TLGXCountryCode = Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["TLGXCountryCode"]),
                                    TLGXCountryName = Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["TLGXCountryName"]),
                                    TLGXProductCode = Convert.ToString(HolidayJson["Holiday"]["PostTour"][a]["TLGXProductCode"]),
                                });
                            }

                        }
                    }
                    #endregion

                    #region PackagePrice
                    if (HolidayJson["Holiday"]["PackagePrice"] != null && HolidayJson["Holiday"]["PackagePrice"].ToList().Count != 0)
                    {
                        objHolidayModel.PackagePrice = new List<PackagePrice>();

                        var TypeOfPackagePrice = HolidayJson["Holiday"]["PackagePrice"].GetType();
                        if (TypeOfPackagePrice.Name.ToUpper() != "JARRAY")
                        {
                            PackagePrice objPackagePrice = new PackagePrice();
                            objPackagePrice.DisplayName = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["DisplayName"]);
                            objPackagePrice.Category = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["Category"]);
                            objPackagePrice.StarRatingCategory = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["StarRatingCategory"]);
                            objPackagePrice.Notes = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["Notes"]);

                            #region BasePrice

                            objPackagePrice.BasePrice = new List<BasePriceStructure>();

                            if (HolidayJson["Holiday"]["PackagePrice"]["BasePrice"] != null)
                            {
                                var TypeOfBasePrice = HolidayJson["Holiday"]["PackagePrice"]["BasePrice"].GetType();
                                if (TypeOfBasePrice.Name.ToUpper() != "JARRAY")
                                {// it is object

                                    // do nothing
                                }
                                else
                                {  // It is Array
                                    int TotalBasepriceNodes = HolidayJson["Holiday"]["PackagePrice"]["BasePrice"].ToList().Count;

                                    for (int i = 0; i < TotalBasepriceNodes; i++)
                                    {
                                        BasePriceStructure ObjBasePriceStructure = new BasePriceStructure();
                                        ObjBasePriceStructure.HubId = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["HubId"]);
                                        ObjBasePriceStructure.HubName = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["HubName"]);

                                        #region Discount 
                                        if (HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Discount"] != null)
                                        {
                                            ObjBasePriceStructure.Discount = new List<DiscountStructure>();
                                            var TypeOfDiscount = HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Discount"].GetType();
                                            if (TypeOfDiscount.Name.ToUpper() != "JARRAY")
                                            {
                                                Double amount = 0;
                                                DiscountStructure objDiscountStructure = new DiscountStructure();
                                                objDiscountStructure.Name = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Discount"]["Discount"]);
                                                objDiscountStructure.Type = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Discount"]["Type"]);
                                                if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Discount"]["DiscountAmount"])))
                                                {
                                                    if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Discount"]["DiscountAmount"]), out amount))
                                                    {
                                                        objDiscountStructure.DiscountAmount = amount;
                                                    }
                                                }

                                                ObjBasePriceStructure.Discount.Add(objDiscountStructure);
                                            }
                                            else
                                            {  // Discount is Array

                                                int TotalDiscountNodes = HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Discount"].ToList().Count;
                                                for (int n = 0; n < TotalDiscountNodes; n++)
                                                {
                                                    Double amount = 0;
                                                    DiscountStructure objDiscountStructure = new DiscountStructure();
                                                    objDiscountStructure.Name = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Discount"][n]["Discount"]);
                                                    objDiscountStructure.Type = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Discount"][n]["Type"]);
                                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Discount"][n]["DiscountAmount"])))
                                                    {
                                                        if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Discount"][n]["DiscountAmount"]), out amount))
                                                        {
                                                            objDiscountStructure.DiscountAmount = amount;
                                                        }
                                                    }

                                                    ObjBasePriceStructure.Discount.Add(objDiscountStructure);
                                                }

                                            }
                                        }

                                        #endregion

                                        #region Tax

                                        if (HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"] != null)
                                        {
                                            ObjBasePriceStructure.Tax = new List<Tax>();
                                            var TypeOfTax = HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"].GetType();
                                            if (TypeOfTax.Name.ToUpper() != "JARRAY")
                                            {// It is object of Tax
                                                Tax objTax = new Tax();

                                                objTax.ApplicableOn = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"]["ApplicableOn"]);
                                                objTax.TaxCalculatedOnRateType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"]["TaxCalculatedOnRateType"]);
                                                objTax.TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"]["TaxType"]);
                                                objTax.InclusiveOfTaxes = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"]["InclusiveOfTaxes"]);

                                                double TaxValue = 0;
                                                if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"]["TaxValue"])))
                                                {
                                                    if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"]["TaxValue"]), out TaxValue))
                                                    {
                                                        objTax.TaxValue = TaxValue;
                                                    }
                                                }

                                                double TaxRate = 0;
                                                if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"]["TaxRate"])))
                                                {
                                                    if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"]["TaxRate"]), out TaxRate))
                                                    {
                                                        objTax.TaxRate = TaxRate;
                                                    }
                                                }

                                                DateTime ValidTo;
                                                if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"]["ValidTo"])))
                                                {
                                                    if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"]["ValidTo"]), out ValidTo))
                                                    {
                                                        objTax.ValidTo = ValidTo;
                                                    }
                                                }

                                                DateTime ValidFrom;
                                                if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"]["ValidFrom"])))
                                                {
                                                    if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"]["ValidFrom"]), out ValidFrom))
                                                    {
                                                        objTax.ValidFrom = ValidFrom;
                                                    }
                                                }

                                                ObjBasePriceStructure.Tax.Add(objTax);
                                            }
                                            else
                                            {  // It is Array of tax
                                                int TotalTaxNodes = HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"].ToList().Count;
                                                for (int e = 0; e < TotalTaxNodes; e++)
                                                {
                                                    Tax objTax = new Tax();

                                                    objTax.ApplicableOn = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"][e]["ApplicableOn"]);
                                                    objTax.TaxCalculatedOnRateType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"][e]["TaxCalculatedOnRateType"]);
                                                    objTax.TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"][e]["TaxType"]);
                                                    objTax.InclusiveOfTaxes = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"][e]["InclusiveOfTaxes"]);

                                                    double TaxValue = 0;
                                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"][e]["TaxValue"])))
                                                    {
                                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"][e]["TaxValue"]), out TaxValue))
                                                        {
                                                            objTax.TaxValue = TaxValue;
                                                        }
                                                    }

                                                    double TaxRate = 0;
                                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"][e]["TaxRate"])))
                                                    {
                                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"][e]["TaxRate"]), out TaxRate))
                                                        {
                                                            objTax.TaxRate = TaxRate;
                                                        }
                                                    }

                                                    DateTime ValidTo;
                                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"][e]["ValidTo"])))
                                                    {
                                                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"][e]["ValidTo"]), out ValidTo))
                                                        {
                                                            objTax.ValidTo = ValidTo;
                                                        }
                                                    }

                                                    DateTime ValidFrom;
                                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"][e]["ValidFrom"])))
                                                    {
                                                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["Tax"][e]["ValidFrom"]), out ValidFrom))
                                                        {
                                                            objTax.ValidFrom = ValidFrom;
                                                        }
                                                    }

                                                    ObjBasePriceStructure.Tax.Add(objTax);
                                                }
                                            }
                                        }
                                        #endregion

                                        #region PriceDetails
                                        if (HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"] != null)
                                        {
                                            ObjBasePriceStructure.PriceDetails = new List<PriceDetailsStructure>();
                                            var TypeOfPriceDetails = HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"].GetType();
                                            if (TypeOfPriceDetails.Name.ToUpper() != "JARRAY")
                                            {
                                                PriceDetailsStructure objPriceDetailsStructure = new PriceDetailsStructure();
                                                #region PassengerRangeFrom
                                                int PassengerRangeFrom = 0;
                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["PassengerRangeFrom"])))
                                                {
                                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["PassengerRangeFrom"]), out PassengerRangeFrom))
                                                    {
                                                        objPriceDetailsStructure.PassengerRangeFrom = PassengerRangeFrom;
                                                    }
                                                }
                                                #endregion

                                                #region PassengerRangeTo
                                                int PassengerRangeTo = 0;
                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["PassengerRangeTo"])))
                                                {
                                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["PassengerRangeTo"]), out PassengerRangeTo))
                                                    {
                                                        objPriceDetailsStructure.PassengerRangeTo = PassengerRangeTo;
                                                    }
                                                }
                                                #endregion

                                                #region ValidFrom
                                                DateTime ValidFrom;
                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["ValidFrom"])))
                                                {
                                                    if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["ValidFrom"]), out ValidFrom))
                                                    {
                                                        objPriceDetailsStructure.ValidFrom = ValidFrom;
                                                    }
                                                }
                                                #endregion

                                                #region ValidTo
                                                DateTime ValidTo;
                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["ValidTo"])))
                                                {
                                                    if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["ValidTo"]), out ValidTo))
                                                    {
                                                        objPriceDetailsStructure.ValidTo = ValidTo;
                                                    }
                                                }
                                                #endregion

                                                #region Amount
                                                double Amount;
                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Amount"])))
                                                {
                                                    if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Amount"]), out Amount))
                                                    {
                                                        objPriceDetailsStructure.Amount = Amount;
                                                    }
                                                }
                                                #endregion

                                                objPriceDetailsStructure.PersonType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["PersonType"]);
                                                objPriceDetailsStructure.RoomType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["RoomType"]);
                                                objPriceDetailsStructure.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["ExtraBedding"]);
                                                objPriceDetailsStructure.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["ExtraBedding"]);
                                                objPriceDetailsStructure.Currency = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Currency"]);
                                                objPriceDetailsStructure.PriceDescription = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["PriceDescription"]);

                                                #region Tax
                                                if (HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Tax"] != null)
                                                {
                                                    objPriceDetailsStructure.Tax = new List<TaxStructure>();
                                                    var TypeOfTaxNodes = HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Tax"].GetType();
                                                    if (TypeOfTaxNodes.Name.ToUpper() != "JARRAY")
                                                    {// it is single object
                                                        double TaxAmount = 0;
                                                        if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Tax"]["TaxAmount"])))
                                                        {
                                                            if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Tax"]["TaxAmount"]), out TaxAmount))
                                                            {
                                                                // Conversion Success
                                                            }
                                                        }
                                                        objPriceDetailsStructure.Tax.Add(new TaxStructure()
                                                        {
                                                            TaxRate = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Tax"]["TaxRate"]),
                                                            TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Tax"]["TaxType"]),
                                                            TaxAmount = TaxAmount
                                                        });
                                                    }
                                                    else
                                                    { // it is array of Tax
                                                        int TotalTaxNodes = HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Tax"].ToList().Count;
                                                        for (int b = 0; b < TotalTaxNodes; b++)
                                                        {
                                                            double TaxAmount = 0;
                                                            if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Tax"][b]["TaxAmount"])))
                                                            {
                                                                if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Tax"][b]["TaxAmount"]), out TaxAmount))
                                                                {
                                                                    // Conversion Success
                                                                }
                                                            }
                                                            objPriceDetailsStructure.Tax.Add(new TaxStructure()
                                                            {
                                                                TaxRate = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Tax"][b]["TaxRate"]),
                                                                TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"]["Tax"][b]["TaxType"]),
                                                                TaxAmount = TaxAmount
                                                            });
                                                        }
                                                    }
                                                }
                                                #endregion


                                                ObjBasePriceStructure.PriceDetails.Add(objPriceDetailsStructure);
                                            }
                                            else
                                            {
                                                int TotalProceDetails = HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"].ToList().Count;

                                                for (int v = 0; v < TotalProceDetails; v++)
                                                {
                                                    PriceDetailsStructure objPriceDetailsStructure = new PriceDetailsStructure();
                                                    #region PassengerRangeFrom
                                                    int PassengerRangeFrom = 0;
                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["PassengerRangeFrom"])))
                                                    {
                                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["PassengerRangeFrom"]), out PassengerRangeFrom))
                                                        {
                                                            objPriceDetailsStructure.PassengerRangeFrom = PassengerRangeFrom;
                                                        }
                                                    }
                                                    #endregion

                                                    #region PassengerRangeTo
                                                    int PassengerRangeTo = 0;
                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["PassengerRangeTo"])))
                                                    {
                                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["PassengerRangeTo"]), out PassengerRangeTo))
                                                        {
                                                            objPriceDetailsStructure.PassengerRangeTo = PassengerRangeTo;
                                                        }
                                                    }
                                                    #endregion

                                                    #region ValidFrom
                                                    DateTime ValidFrom;
                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["ValidFrom"])))
                                                    {
                                                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["ValidFrom"]), out ValidFrom))
                                                        {
                                                            objPriceDetailsStructure.ValidFrom = ValidFrom;
                                                        }
                                                    }
                                                    #endregion

                                                    #region ValidTo
                                                    DateTime ValidTo;
                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["ValidTo"])))
                                                    {
                                                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["ValidTo"]), out ValidTo))
                                                        {
                                                            objPriceDetailsStructure.ValidTo = ValidTo;
                                                        }
                                                    }
                                                    #endregion

                                                    #region Amount
                                                    double Amount;
                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Amount"])))
                                                    {
                                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Amount"]), out Amount))
                                                        {
                                                            objPriceDetailsStructure.Amount = Amount;
                                                        }
                                                    }
                                                    #endregion

                                                    objPriceDetailsStructure.PersonType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["PersonType"]);
                                                    objPriceDetailsStructure.RoomType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["RoomType"]);
                                                    objPriceDetailsStructure.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["ExtraBedding"]);
                                                    objPriceDetailsStructure.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["ExtraBedding"]);
                                                    objPriceDetailsStructure.Currency = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Currency"]);
                                                    objPriceDetailsStructure.PriceDescription = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["PriceDescription"]);

                                                    #region Tax
                                                    if (HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Tax"] != null)
                                                    {
                                                        objPriceDetailsStructure.Tax = new List<TaxStructure>();
                                                        var TypeOfTaxNodes = HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Tax"].GetType();
                                                        if (TypeOfTaxNodes.Name.ToUpper() != "JARRAY")
                                                        {// it is single object
                                                            double TaxAmount = 0;
                                                            if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Tax"]["TaxAmount"])))
                                                            {
                                                                if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Tax"]["TaxAmount"]), out TaxAmount))
                                                                {
                                                                    // Conversion Success
                                                                }
                                                            }
                                                            objPriceDetailsStructure.Tax.Add(new TaxStructure()
                                                            {
                                                                TaxRate = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Tax"]["TaxRate"]),
                                                                TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Tax"]["TaxType"]),
                                                                TaxAmount = TaxAmount
                                                            });
                                                        }
                                                        else
                                                        { // it is array of Tax
                                                            int TotalTaxNodes = HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Tax"].ToList().Count;
                                                            for (int b = 0; b < TotalTaxNodes; b++)
                                                            {
                                                                double TaxAmount = 0;
                                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Tax"][b]["TaxAmount"])))
                                                                {
                                                                    if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Tax"][b]["TaxAmount"]), out TaxAmount))
                                                                    {
                                                                        // Conversion Success
                                                                    }
                                                                }
                                                                objPriceDetailsStructure.Tax.Add(new TaxStructure()
                                                                {
                                                                    TaxRate = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Tax"][b]["TaxRate"]),
                                                                    TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"]["BasePrice"][i]["PriceDetails"][v]["Tax"][b]["TaxType"]),
                                                                    TaxAmount = TaxAmount
                                                                });
                                                            }
                                                        }
                                                    }
                                                    #endregion


                                                    ObjBasePriceStructure.PriceDetails.Add(objPriceDetailsStructure);
                                                }

                                            }

                                        }


                                        #endregion

                                        objPackagePrice.BasePrice.Add(ObjBasePriceStructure);
                                    }

                                }
                            }

                            #endregion


                            objHolidayModel.PackagePrice.Add(objPackagePrice);


                        }
                        else
                        {
                            int TotalPackagePrice = HolidayJson["Holiday"]["PackagePrice"].ToList().Count;
                            for (int m = 0; m < TotalPackagePrice; m++)
                            {
                                PackagePrice objPackagePrice = new PackagePrice();
                                objPackagePrice.DisplayName = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["DisplayName"]);
                                objPackagePrice.Category = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["Category"]);
                                objPackagePrice.StarRatingCategory = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["StarRatingCategory"]);
                                objPackagePrice.Notes = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["Notes"]);

                                #region BasePrice

                                objPackagePrice.BasePrice = new List<BasePriceStructure>();

                                if (HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"] != null)
                                {
                                    var TypeOfBasePrice = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"].GetType();
                                    if (TypeOfBasePrice.Name.ToUpper() != "JARRAY")
                                    {// it is object


                                    }
                                    else
                                    {  // It is Array
                                        int TotalBasepriceNodes = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"].ToList().Count;

                                        for (int i = 0; i < TotalBasepriceNodes; i++)
                                        {
                                            BasePriceStructure ObjBasePriceStructure = new BasePriceStructure();
                                            ObjBasePriceStructure.HubId = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["HubId"]);
                                            ObjBasePriceStructure.HubName = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["HubName"]);

                                            #region Discount 
                                            if (HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Discount"] != null)
                                            {
                                                ObjBasePriceStructure.Discount = new List<DiscountStructure>();
                                                var TypeOfDiscount = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Discount"].GetType();
                                                if (TypeOfDiscount.Name.ToUpper() != "JARRAY")
                                                {
                                                    Double amount = 0;
                                                    DiscountStructure objDiscountStructure = new DiscountStructure();
                                                    objDiscountStructure.Name = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Discount"]["Discount"]);
                                                    objDiscountStructure.Type = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Discount"]["Type"]);
                                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Discount"]["DiscountAmount"])))
                                                    {
                                                        if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Discount"]["DiscountAmount"]), out amount))
                                                        {
                                                            objDiscountStructure.DiscountAmount = amount;
                                                        }
                                                    }

                                                    ObjBasePriceStructure.Discount.Add(objDiscountStructure);
                                                }
                                                else
                                                {  // Discount is Array

                                                    int TotalDiscountNodes = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Discount"].ToList().Count;
                                                    for (int n = 0; n < TotalDiscountNodes; n++)
                                                    {
                                                        Double amount = 0;
                                                        DiscountStructure objDiscountStructure = new DiscountStructure();
                                                        objDiscountStructure.Name = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Discount"][n]["Discount"]);
                                                        objDiscountStructure.Type = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Discount"][n]["Type"]);
                                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Discount"][n]["DiscountAmount"])))
                                                        {
                                                            if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Discount"][n]["DiscountAmount"]), out amount))
                                                            {
                                                                objDiscountStructure.DiscountAmount = amount;
                                                            }
                                                        }

                                                        ObjBasePriceStructure.Discount.Add(objDiscountStructure);
                                                    }

                                                }
                                            }

                                            #endregion

                                            #region Tax

                                            if (HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"] != null)
                                            {
                                                ObjBasePriceStructure.Tax = new List<Tax>();
                                                var TypeOfTax = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"].GetType();
                                                if (TypeOfTax.Name.ToUpper() != "JARRAY")
                                                {// It is object of Tax
                                                    Tax objTax = new Tax();

                                                    objTax.ApplicableOn = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"]["ApplicableOn"]);
                                                    objTax.TaxCalculatedOnRateType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"]["TaxCalculatedOnRateType"]);
                                                    objTax.TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"]["TaxType"]);
                                                    objTax.InclusiveOfTaxes = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"]["InclusiveOfTaxes"]);

                                                    double TaxValue = 0;
                                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"]["TaxValue"])))
                                                    {
                                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"]["TaxValue"]), out TaxValue))
                                                        {
                                                            objTax.TaxValue = TaxValue;
                                                        }
                                                    }

                                                    double TaxRate = 0;
                                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"]["TaxRate"])))
                                                    {
                                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"]["TaxRate"]), out TaxRate))
                                                        {
                                                            objTax.TaxRate = TaxRate;
                                                        }
                                                    }

                                                    DateTime ValidTo;
                                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"]["ValidTo"])))
                                                    {
                                                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"]["ValidTo"]), out ValidTo))
                                                        {
                                                            objTax.ValidTo = ValidTo;
                                                        }
                                                    }

                                                    DateTime ValidFrom;
                                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"]["ValidFrom"])))
                                                    {
                                                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"]["ValidFrom"]), out ValidFrom))
                                                        {
                                                            objTax.ValidFrom = ValidFrom;
                                                        }
                                                    }

                                                    ObjBasePriceStructure.Tax.Add(objTax);
                                                }
                                                else
                                                {  // It is Array of tax
                                                    int TotalTaxNodes = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"].ToList().Count;
                                                    for (int e = 0; e < TotalTaxNodes; e++)
                                                    {
                                                        Tax objTax = new Tax();

                                                        objTax.ApplicableOn = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"][e]["ApplicableOn"]);
                                                        objTax.TaxCalculatedOnRateType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"][e]["TaxCalculatedOnRateType"]);
                                                        objTax.TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"][e]["TaxType"]);
                                                        objTax.InclusiveOfTaxes = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"][e]["InclusiveOfTaxes"]);

                                                        double TaxValue = 0;
                                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"][e]["TaxValue"])))
                                                        {
                                                            if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"][e]["TaxValue"]), out TaxValue))
                                                            {
                                                                objTax.TaxValue = TaxValue;
                                                            }
                                                        }

                                                        double TaxRate = 0;
                                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"][e]["TaxRate"])))
                                                        {
                                                            if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"][e]["TaxRate"]), out TaxRate))
                                                            {
                                                                objTax.TaxRate = TaxRate;
                                                            }
                                                        }

                                                        DateTime ValidTo;
                                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"][e]["ValidTo"])))
                                                        {
                                                            if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"][e]["ValidTo"]), out ValidTo))
                                                            {
                                                                objTax.ValidTo = ValidTo;
                                                            }
                                                        }

                                                        DateTime ValidFrom;
                                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"][e]["ValidFrom"])))
                                                        {
                                                            if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["Tax"][e]["ValidFrom"]), out ValidFrom))
                                                            {
                                                                objTax.ValidFrom = ValidFrom;
                                                            }
                                                        }

                                                        ObjBasePriceStructure.Tax.Add(objTax);
                                                    }
                                                }
                                            }
                                            #endregion

                                            #region PriceDetails
                                            if (HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"] != null)
                                            {
                                                ObjBasePriceStructure.PriceDetails = new List<PriceDetailsStructure>();
                                                var TypeOfPriceDetails = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"].GetType();
                                                if (TypeOfPriceDetails.Name.ToUpper() != "JARRAY")
                                                {
                                                    PriceDetailsStructure objPriceDetailsStructure = new PriceDetailsStructure();
                                                    #region PassengerRangeFrom
                                                    int PassengerRangeFrom = 0;
                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["PassengerRangeFrom"])))
                                                    {
                                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["PassengerRangeFrom"]), out PassengerRangeFrom))
                                                        {
                                                            objPriceDetailsStructure.PassengerRangeFrom = PassengerRangeFrom;
                                                        }
                                                    }
                                                    #endregion

                                                    #region PassengerRangeTo
                                                    int PassengerRangeTo = 0;
                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["PassengerRangeTo"])))
                                                    {
                                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["PassengerRangeTo"]), out PassengerRangeTo))
                                                        {
                                                            objPriceDetailsStructure.PassengerRangeTo = PassengerRangeTo;
                                                        }
                                                    }
                                                    #endregion

                                                    #region ValidFrom
                                                    DateTime ValidFrom;
                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["ValidFrom"])))
                                                    {
                                                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["ValidFrom"]), out ValidFrom))
                                                        {
                                                            objPriceDetailsStructure.ValidFrom = ValidFrom;
                                                        }
                                                    }
                                                    #endregion

                                                    #region ValidTo
                                                    DateTime ValidTo;
                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["ValidTo"])))
                                                    {
                                                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["ValidTo"]), out ValidTo))
                                                        {
                                                            objPriceDetailsStructure.ValidTo = ValidTo;
                                                        }
                                                    }
                                                    #endregion

                                                    #region Amount
                                                    double Amount;
                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Amount"])))
                                                    {
                                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Amount"]), out Amount))
                                                        {
                                                            objPriceDetailsStructure.Amount = Amount;
                                                        }
                                                    }
                                                    #endregion

                                                    objPriceDetailsStructure.PersonType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["PersonType"]);
                                                    objPriceDetailsStructure.RoomType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["RoomType"]);
                                                    objPriceDetailsStructure.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["ExtraBedding"]);
                                                    objPriceDetailsStructure.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["ExtraBedding"]);
                                                    objPriceDetailsStructure.Currency = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Currency"]);
                                                    objPriceDetailsStructure.PriceDescription = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["PriceDescription"]);

                                                    #region Tax
                                                    if (HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Tax"] != null)
                                                    {
                                                        objPriceDetailsStructure.Tax = new List<TaxStructure>();
                                                        var TypeOfTaxNodes = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Tax"].GetType();
                                                        if (TypeOfTaxNodes.Name.ToUpper() != "JARRAY")
                                                        {// it is single object
                                                            double TaxAmount = 0;
                                                            if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Tax"]["TaxAmount"])))
                                                            {
                                                                if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Tax"]["TaxAmount"]), out TaxAmount))
                                                                {
                                                                    // Conversion Success
                                                                }
                                                            }
                                                            objPriceDetailsStructure.Tax.Add(new TaxStructure()
                                                            {
                                                                TaxRate = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Tax"]["TaxRate"]),
                                                                TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Tax"]["TaxType"]),
                                                                TaxAmount = TaxAmount
                                                            });
                                                        }
                                                        else
                                                        { // it is array of Tax
                                                            int TotalTaxNodes = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Tax"].ToList().Count;
                                                            for (int b = 0; b < TotalTaxNodes; b++)
                                                            {
                                                                double TaxAmount = 0;
                                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Tax"][b]["TaxAmount"])))
                                                                {
                                                                    if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Tax"][b]["TaxAmount"]), out TaxAmount))
                                                                    {
                                                                        // Conversion Success
                                                                    }
                                                                }
                                                                objPriceDetailsStructure.Tax.Add(new TaxStructure()
                                                                {
                                                                    TaxRate = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Tax"][b]["TaxRate"]),
                                                                    TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"]["Tax"][b]["TaxType"]),
                                                                    TaxAmount = TaxAmount
                                                                });
                                                            }
                                                        }
                                                    }
                                                    #endregion


                                                    ObjBasePriceStructure.PriceDetails.Add(objPriceDetailsStructure);
                                                }
                                                else
                                                {
                                                    int TotalProceDetails = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"].ToList().Count;

                                                    for (int v = 0; v < TotalProceDetails; v++)
                                                    {
                                                        if (HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v].GetType().Name.ToUpper() != "JARRAY")
                                                        {
                                                            PriceDetailsStructure objPriceDetailsStructure = new PriceDetailsStructure();
                                                            #region PassengerRangeFrom
                                                            int PassengerRangeFrom = 0;
                                                            if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["PassengerRangeFrom"])))
                                                            {
                                                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["PassengerRangeFrom"]), out PassengerRangeFrom))
                                                                {
                                                                    objPriceDetailsStructure.PassengerRangeFrom = PassengerRangeFrom;
                                                                }
                                                            }
                                                            #endregion

                                                            #region PassengerRangeTo
                                                            int PassengerRangeTo = 0;
                                                            if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["PassengerRangeTo"])))
                                                            {
                                                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["PassengerRangeTo"]), out PassengerRangeTo))
                                                                {
                                                                    objPriceDetailsStructure.PassengerRangeTo = PassengerRangeTo;
                                                                }
                                                            }
                                                            #endregion

                                                            #region ValidFrom
                                                            DateTime ValidFrom;
                                                            if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["ValidFrom"])))
                                                            {
                                                                if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["ValidFrom"]), out ValidFrom))
                                                                {
                                                                    objPriceDetailsStructure.ValidFrom = ValidFrom;
                                                                }
                                                            }
                                                            #endregion

                                                            #region ValidTo
                                                            DateTime ValidTo;
                                                            if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["ValidTo"])))
                                                            {
                                                                if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["ValidTo"]), out ValidTo))
                                                                {
                                                                    objPriceDetailsStructure.ValidTo = ValidTo;
                                                                }
                                                            }
                                                            #endregion

                                                            #region Amount
                                                            double Amount;
                                                            if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Amount"])))
                                                            {
                                                                if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Amount"]), out Amount))
                                                                {
                                                                    objPriceDetailsStructure.Amount = Amount;
                                                                }
                                                            }
                                                            #endregion

                                                            objPriceDetailsStructure.PersonType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["PersonType"]);
                                                            objPriceDetailsStructure.RoomType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["RoomType"]);
                                                            objPriceDetailsStructure.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["ExtraBedding"]);
                                                            objPriceDetailsStructure.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["ExtraBedding"]);
                                                            objPriceDetailsStructure.Currency = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Currency"]);
                                                            objPriceDetailsStructure.PriceDescription = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["PriceDescription"]);

                                                            #region Tax
                                                            if (HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Tax"] != null)
                                                            {
                                                                objPriceDetailsStructure.Tax = new List<TaxStructure>();
                                                                var TypeOfTaxNodes = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Tax"].GetType();
                                                                if (TypeOfTaxNodes.Name.ToUpper() != "JARRAY")
                                                                {// it is single object
                                                                    double TaxAmount = 0;
                                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Tax"]["TaxAmount"])))
                                                                    {
                                                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Tax"]["TaxAmount"]), out TaxAmount))
                                                                        {
                                                                            // Conversion Success
                                                                        }
                                                                    }
                                                                    objPriceDetailsStructure.Tax.Add(new TaxStructure()
                                                                    {
                                                                        TaxRate = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Tax"]["TaxRate"]),
                                                                        TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Tax"]["TaxType"]),
                                                                        TaxAmount = TaxAmount
                                                                    });
                                                                }
                                                                else
                                                                { // it is array of Tax
                                                                    int TotalTaxNodes = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Tax"].ToList().Count;
                                                                    for (int b = 0; b < TotalTaxNodes; b++)
                                                                    {
                                                                        double TaxAmount = 0;
                                                                        if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Tax"][b]["TaxAmount"])))
                                                                        {
                                                                            if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Tax"][b]["TaxAmount"]), out TaxAmount))
                                                                            {
                                                                                // Conversion Success
                                                                            }
                                                                        }
                                                                        objPriceDetailsStructure.Tax.Add(new TaxStructure()
                                                                        {
                                                                            TaxRate = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Tax"][b]["TaxRate"]),
                                                                            TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v]["Tax"][b]["TaxType"]),
                                                                            TaxAmount = TaxAmount
                                                                        });
                                                                    }
                                                                }
                                                            }
                                                            #endregion


                                                            ObjBasePriceStructure.PriceDetails.Add(objPriceDetailsStructure);
                                                        }
                                                        else
                                                        {
                                                            int totalPriceDetails = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v].ToList().Count;

                                                            for (int h = 0; h < totalPriceDetails; h++)
                                                            {
                                                                PriceDetailsStructure objPriceDetailsStructure = new PriceDetailsStructure();
                                                                #region PassengerRangeFrom
                                                                int PassengerRangeFrom = 0;
                                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["PassengerRangeFrom"])))
                                                                {
                                                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["PassengerRangeFrom"]), out PassengerRangeFrom))
                                                                    {
                                                                        objPriceDetailsStructure.PassengerRangeFrom = PassengerRangeFrom;
                                                                    }
                                                                }
                                                                #endregion

                                                                #region PassengerRangeTo
                                                                int PassengerRangeTo = 0;
                                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["PassengerRangeTo"])))
                                                                {
                                                                    if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["PassengerRangeTo"]), out PassengerRangeTo))
                                                                    {
                                                                        objPriceDetailsStructure.PassengerRangeTo = PassengerRangeTo;
                                                                    }
                                                                }
                                                                #endregion

                                                                #region ValidFrom
                                                                DateTime ValidFrom;
                                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["ValidFrom"])))
                                                                {
                                                                    if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["ValidFrom"]), out ValidFrom))
                                                                    {
                                                                        objPriceDetailsStructure.ValidFrom = ValidFrom;
                                                                    }
                                                                }
                                                                #endregion

                                                                #region ValidTo
                                                                DateTime ValidTo;
                                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["ValidTo"])))
                                                                {
                                                                    if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["ValidTo"]), out ValidTo))
                                                                    {
                                                                        objPriceDetailsStructure.ValidTo = ValidTo;
                                                                    }
                                                                }
                                                                #endregion

                                                                #region Amount
                                                                double Amount;
                                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Amount"])))
                                                                {
                                                                    if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Amount"]), out Amount))
                                                                    {
                                                                        objPriceDetailsStructure.Amount = Amount;
                                                                    }
                                                                }
                                                                #endregion

                                                                objPriceDetailsStructure.PersonType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["PersonType"]);
                                                                objPriceDetailsStructure.RoomType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["RoomType"]);
                                                                objPriceDetailsStructure.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["ExtraBedding"]);
                                                                objPriceDetailsStructure.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["ExtraBedding"]);
                                                                objPriceDetailsStructure.Currency = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Currency"]);
                                                                objPriceDetailsStructure.PriceDescription = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["PriceDescription"]);

                                                                #region Tax
                                                                if (HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"] != null)
                                                                {
                                                                    objPriceDetailsStructure.Tax = new List<TaxStructure>();
                                                                    var TypeOfTaxNodes = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"].GetType();
                                                                    if (TypeOfTaxNodes.Name.ToUpper() != "JARRAY")
                                                                    {// it is single object
                                                                        double TaxAmount = 0;
                                                                        if (HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"].ToList().Count != 0 &&
                                                                            !string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"]["TaxAmount"])))
                                                                        {
                                                                            if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"]["TaxAmount"]), out TaxAmount))
                                                                            {
                                                                                // Conversion Success
                                                                            }
                                                                        }
                                                                        if (HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"].ToList().Count != 0)
                                                                        {
                                                                            objPriceDetailsStructure.Tax.Add(new TaxStructure()
                                                                            {
                                                                                TaxRate = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"]["TaxRate"]),
                                                                                TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"]["TaxType"]),
                                                                                TaxAmount = TaxAmount
                                                                            });
                                                                        }
                                                                    }
                                                                    else
                                                                    { // it is array of Tax
                                                                        int TotalTaxNodes = HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"].ToList().Count;
                                                                        for (int b = 0; b < TotalTaxNodes; b++)
                                                                        {
                                                                            if (HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"].ToList().Count != 0)
                                                                            {
                                                                                double TaxAmount = 0;
                                                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"][b]["TaxAmount"])))
                                                                                {
                                                                                    if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"][b]["TaxAmount"]), out TaxAmount))
                                                                                    {
                                                                                        // Conversion Success
                                                                                    }
                                                                                }
                                                                                objPriceDetailsStructure.Tax.Add(new TaxStructure()
                                                                                {
                                                                                    TaxRate = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"][b]["TaxRate"]),
                                                                                    TaxType = Convert.ToString(HolidayJson["Holiday"]["PackagePrice"][m]["BasePrice"][i]["PriceDetails"][v][h]["Tax"][b]["TaxType"]),
                                                                                    TaxAmount = TaxAmount
                                                                                });
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                                #endregion


                                                                ObjBasePriceStructure.PriceDetails.Add(objPriceDetailsStructure);
                                                            }

                                                        }
                                                    }

                                                }

                                            }


                                            #endregion

                                            objPackagePrice.BasePrice.Add(ObjBasePriceStructure);
                                        }

                                    }
                                }

                                #endregion






                                objHolidayModel.PackagePrice.Add(objPackagePrice);
                            }
                        }
                    }
                    #endregion

                    #region PreTourPrice
                    if (HolidayJson["Holiday"]["PreTourPrice"] != null && HolidayJson["Holiday"]["PreTourPrice"].ToList().Count != 0)
                    {
                        objHolidayModel.PreTourPrice = new List<PreTourPrice>();
                        var TypeOfPreTourPrice = HolidayJson["Holiday"]["PreTourPrice"].GetType();
                        if (TypeOfPreTourPrice.Name.ToUpper() != "JARRAY")
                        { // It is Object

                            PreTourPrice objPreTourPrice = new PreTourPrice();
                            objPreTourPrice.Currency = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["Currency"]);
                            objPreTourPrice.DiscountName = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["DiscountAmount"]);
                            objPreTourPrice.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["ExtraBedding"]);
                            objPreTourPrice.PersonType = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["PersonType"]);
                            objPreTourPrice.RoomType = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["RoomType"]);
                            objPreTourPrice.SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["SupplierProductCode"]);

                            #region PassengerRangeFrom
                            double PassengerRangeFrom = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["PassengerRangeFrom"])))
                            {
                                if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["PassengerRangeFrom"]), out PassengerRangeFrom))
                                {
                                    // Conversion Success
                                    objPreTourPrice.PassengerRangeFrom = PassengerRangeFrom;
                                }
                            }
                            #endregion

                            #region PassengerRangeTo
                            double PassengerRangeTo = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["PassengerRangeTo"])))
                            {
                                if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["PassengerRangeTo"]), out PassengerRangeTo))
                                {
                                    // Conversion Success
                                    objPreTourPrice.PassengerRangeTo = PassengerRangeTo;
                                }
                            }
                            #endregion

                            #region ValidFrom
                            DateTime ValidFrom;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["ValidFrom"])))
                            {
                                if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["ValidFrom"]), out ValidFrom))
                                {
                                    // Conversion Success
                                    objPreTourPrice.ValidFrom = ValidFrom;
                                }
                            }
                            #endregion

                            #region ValidTo
                            DateTime ValidTo;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["ValidTo"])))
                            {
                                if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["ValidTo"]), out ValidTo))
                                {
                                    // Conversion Success
                                    objPreTourPrice.ValidTo = ValidTo;
                                }
                            }
                            #endregion

                            #region Amount
                            double Amount = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["Amount"])))
                            {
                                if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["Amount"]), out Amount))
                                {
                                    objPreTourPrice.Amount = Amount;
                                }
                            }
                            #endregion

                            #region AgeFrom
                            int AgeFrom = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["AgeFrom"])))
                            {
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["AgeFrom"]), out AgeFrom))
                                {
                                    objPreTourPrice.AgeFrom = AgeFrom;
                                }
                            }
                            #endregion

                            #region AgeTo
                            int AgeTo = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["AgeTo"])))
                            {
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["AgeTo"]), out AgeTo))
                                {
                                    objPreTourPrice.AgeTo = AgeTo;
                                }
                            }
                            #endregion

                            #region DiscountAmount
                            int DiscountAmount = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["DiscountAmount"])))
                            {
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["DiscountAmount"]), out DiscountAmount))
                                {
                                    objPreTourPrice.DiscountAmount = DiscountAmount;
                                }
                            }
                            #endregion

                            #region Tax
                            if (HolidayJson["Holiday"]["PreTourPrice"]["Tax"] != null)
                            {
                                objPreTourPrice.Tax = new List<TaxStructure>();
                                var TypeOfTaxNodes = HolidayJson["Holiday"]["PreTourPrice"]["Tax"].GetType();
                                if (TypeOfTaxNodes.Name.ToUpper() != "JARRAY")
                                {
                                    double TaxAmt = 0;
                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["Tax"]["TaxAmount"])))
                                    {
                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["Tax"]["TaxAmount"]), out TaxAmt))
                                        {
                                            // Conversion Success
                                        }
                                    }
                                    objPreTourPrice.Tax.Add(new TaxStructure()
                                    {
                                        TaxRate = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["Tax"]["TaxRate"]),
                                        TaxType = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["Tax"]["TaxType"]),
                                        TaxAmount = TaxAmt
                                    });
                                }
                                else
                                {  // it is array
                                    int totalTaxNodes = HolidayJson["Holiday"]["PreTourPrice"]["Tax"].ToList().Count;
                                    for (int p = 0; p < totalTaxNodes; p++)
                                    {
                                        double TaxAmt = 0;
                                        if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["Tax"][p]["TaxAmount"])))
                                        {
                                            if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["Tax"][p]["TaxAmount"]), out TaxAmt))
                                            {
                                                // Conversion Success
                                            }
                                        }
                                        objPreTourPrice.Tax.Add(new TaxStructure()
                                        {
                                            TaxRate = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["Tax"][p]["TaxRate"]),
                                            TaxType = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"]["Tax"][p]["TaxType"]),
                                            TaxAmount = TaxAmt
                                        });
                                    }

                                }

                            }
                            #endregion

                            objHolidayModel.PreTourPrice.Add(objPreTourPrice);
                        }
                        else
                        { // It is Array
                            int TotalPreTourPrice = HolidayJson["Holiday"]["PreTourPrice"].ToList().Count;

                            for (int n = 0; n < TotalPreTourPrice; n++)
                            {
                                if (HolidayJson["Holiday"]["PreTourPrice"][n].GetType().Name.ToUpper() == "JARRAY")
                                {
                                    int TotalPreTourPriceNodes = HolidayJson["Holiday"]["PreTourPrice"][n].ToList().Count;
                                    for (int u = 0; u < TotalPreTourPriceNodes; u++)
                                    {
                                        PreTourPrice objPreTourPrice = new PreTourPrice();

                                        objPreTourPrice.Currency = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["Currency"]);
                                        objPreTourPrice.DiscountName = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["DiscountAmount"]);
                                        objPreTourPrice.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["ExtraBedding"]);
                                        objPreTourPrice.PersonType = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["PersonType"]);
                                        objPreTourPrice.RoomType = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["RoomType"]);
                                        objPreTourPrice.SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["SupplierProductCode"]);
                                        #region PassengerRangeFrom
                                        double PassengerRangeFrom = 0;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["PassengerRangeFrom"])))
                                        {
                                            if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["PassengerRangeFrom"]), out PassengerRangeFrom))
                                            {
                                                // Conversion Success
                                                objPreTourPrice.PassengerRangeFrom = PassengerRangeFrom;
                                            }
                                        }
                                        #endregion

                                        #region PassengerRangeTo
                                        double PassengerRangeTo = 0;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["PassengerRangeTo"])))
                                        {
                                            if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["PassengerRangeTo"]), out PassengerRangeTo))
                                            {
                                                // Conversion Success
                                                objPreTourPrice.PassengerRangeTo = PassengerRangeTo;
                                            }
                                        }
                                        #endregion

                                        #region ValidFrom
                                        DateTime ValidFrom;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["ValidFrom"])))
                                        {
                                            if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["ValidFrom"]), out ValidFrom))
                                            {
                                                // Conversion Success
                                                objPreTourPrice.ValidFrom = ValidFrom;
                                            }
                                        }
                                        #endregion

                                        #region ValidTo
                                        DateTime ValidTo;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["ValidTo"])))
                                        {
                                            if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["ValidTo"]), out ValidTo))
                                            {
                                                // Conversion Success
                                                objPreTourPrice.ValidTo = ValidTo;
                                            }
                                        }
                                        #endregion

                                        #region Amount
                                        double Amount = 0;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["Amount"])))
                                        {
                                            if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["Amount"]), out Amount))
                                            {
                                                objPreTourPrice.Amount = Amount;
                                            }
                                        }
                                        #endregion

                                        #region AgeFrom
                                        int AgeFrom = 0;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["AgeFrom"])))
                                        {
                                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["AgeFrom"]), out AgeFrom))
                                            {
                                                objPreTourPrice.AgeFrom = AgeFrom;
                                            }
                                        }
                                        #endregion

                                        #region AgeTo
                                        int AgeTo = 0;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["AgeTo"])))
                                        {
                                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["AgeTo"]), out AgeTo))
                                            {
                                                objPreTourPrice.AgeTo = AgeTo;
                                            }
                                        }
                                        #endregion

                                        #region DiscountAmount
                                        int DiscountAmount = 0;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["DiscountAmount"])))
                                        {
                                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["DiscountAmount"]), out DiscountAmount))
                                            {
                                                objPreTourPrice.DiscountAmount = DiscountAmount;
                                            }
                                        }
                                        #endregion

                                        #region Tax
                                        if (HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"] != null)
                                        {
                                            objPreTourPrice.Tax = new List<TaxStructure>();
                                            var TypeOfTaxNodes = HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"].GetType();
                                            if (TypeOfTaxNodes.Name.ToUpper() != "JARRAY")
                                            {
                                                double TaxAmt = 0;
                                                if (HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"].ToList().Count != 0
                                                    && !string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"]["TaxAmount"])))
                                                {
                                                    if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"]["TaxAmount"]), out TaxAmt))
                                                    {
                                                        // Conversion Success
                                                    }
                                                }
                                                if (HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"].ToList().Count != 0)
                                                {
                                                    objPreTourPrice.Tax.Add(new TaxStructure()
                                                    {
                                                        TaxRate = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"]["TaxRate"]),
                                                        TaxType = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"]["TaxType"]),
                                                        TaxAmount = TaxAmt
                                                    });
                                                }
                                            }
                                            else
                                            {  // it is array
                                                int totalTaxNodes = HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"].ToList().Count;
                                                for (int p = 0; p < totalTaxNodes; p++)
                                                {
                                                    double TaxAmt = 0;
                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"][p]["TaxAmount"])))
                                                    {
                                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"][p]["TaxAmount"]), out TaxAmt))
                                                        {
                                                            // Conversion Success
                                                        }
                                                    }
                                                    if ((HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"][p].ToList().Count != 0))
                                                    {
                                                        objPreTourPrice.Tax.Add(new TaxStructure()
                                                        {
                                                            TaxRate = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"][p]["TaxRate"]),
                                                            TaxType = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n][u]["Tax"][p]["TaxType"]),
                                                            TaxAmount = TaxAmt
                                                        });
                                                    }
                                                }

                                            }


                                        }
                                        #endregion
                                        objHolidayModel.PreTourPrice.Add(objPreTourPrice);


                                    }

                                }
                                else
                                {
                                    PreTourPrice objPreTourPrice = new PreTourPrice();
                                    objPreTourPrice.Currency = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["Currency"]);
                                    objPreTourPrice.DiscountName = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["DiscountAmount"]);
                                    objPreTourPrice.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["ExtraBedding"]);
                                    objPreTourPrice.PersonType = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["PersonType"]);
                                    objPreTourPrice.RoomType = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["RoomType"]);
                                    objPreTourPrice.SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["SupplierProductCode"]);

                                    #region PassengerRangeFrom
                                    double PassengerRangeFrom = 0;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["PassengerRangeFrom"])))
                                    {
                                        if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["PassengerRangeFrom"]), out PassengerRangeFrom))
                                        {
                                            // Conversion Success
                                            objPreTourPrice.PassengerRangeFrom = PassengerRangeFrom;
                                        }
                                    }
                                    #endregion

                                    #region PassengerRangeTo
                                    double PassengerRangeTo = 0;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["PassengerRangeTo"])))
                                    {
                                        if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["PassengerRangeTo"]), out PassengerRangeTo))
                                        {
                                            // Conversion Success
                                            objPreTourPrice.PassengerRangeTo = PassengerRangeTo;
                                        }
                                    }
                                    #endregion

                                    #region ValidFrom
                                    DateTime ValidFrom;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["ValidFrom"])))
                                    {
                                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["ValidFrom"]), out ValidFrom))
                                        {
                                            // Conversion Success
                                            objPreTourPrice.ValidFrom = ValidFrom;
                                        }
                                    }
                                    #endregion

                                    #region ValidTo
                                    DateTime ValidTo;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["ValidTo"])))
                                    {
                                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["ValidTo"]), out ValidTo))
                                        {
                                            // Conversion Success
                                            objPreTourPrice.ValidTo = ValidTo;
                                        }
                                    }
                                    #endregion

                                    #region Amount
                                    double Amount = 0;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["Amount"])))
                                    {
                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["Amount"]), out Amount))
                                        {
                                            objPreTourPrice.Amount = Amount;
                                        }
                                    }
                                    #endregion

                                    #region AgeFrom
                                    int AgeFrom = 0;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["AgeFrom"])))
                                    {
                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["AgeFrom"]), out AgeFrom))
                                        {
                                            objPreTourPrice.AgeFrom = AgeFrom;
                                        }
                                    }
                                    #endregion

                                    #region AgeTo
                                    int AgeTo = 0;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["AgeTo"])))
                                    {
                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["AgeTo"]), out AgeTo))
                                        {
                                            objPreTourPrice.AgeTo = AgeTo;
                                        }
                                    }
                                    #endregion

                                    #region DiscountAmount
                                    int DiscountAmount = 0;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["DiscountAmount"])))
                                    {
                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["DiscountAmount"]), out DiscountAmount))
                                        {
                                            objPreTourPrice.DiscountAmount = DiscountAmount;
                                        }
                                    }
                                    #endregion

                                    #region Tax
                                    if (HolidayJson["Holiday"]["PreTourPrice"][n]["Tax"] != null)
                                    {
                                        objPreTourPrice.Tax = new List<TaxStructure>();
                                        var TypeOfTaxNodes = HolidayJson["Holiday"]["PreTourPrice"][n]["Tax"].GetType();
                                        if (TypeOfTaxNodes.Name.ToUpper() != "JARRAY")
                                        {
                                            double TaxAmt = 0;
                                            if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["Tax"]["TaxAmount"])))
                                            {
                                                if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["Tax"]["TaxAmount"]), out TaxAmt))
                                                {
                                                    // Conversion Success
                                                }
                                            }
                                            objPreTourPrice.Tax.Add(new TaxStructure()
                                            {
                                                TaxRate = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["Tax"]["TaxRate"]),
                                                TaxType = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["Tax"]["TaxType"]),
                                                TaxAmount = TaxAmt
                                            });
                                        }
                                        else
                                        {  // it is array
                                            int totalTaxNodes = HolidayJson["Holiday"]["PreTourPrice"][n]["Tax"].ToList().Count;
                                            for (int p = 0; p < totalTaxNodes; p++)
                                            {
                                                double TaxAmt = 0;
                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["Tax"][p]["TaxAmount"])))
                                                {
                                                    if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["Tax"][p]["TaxAmount"]), out TaxAmt))
                                                    {
                                                        // Conversion Success
                                                    }
                                                }
                                                objPreTourPrice.Tax.Add(new TaxStructure()
                                                {
                                                    TaxRate = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["Tax"][p]["TaxRate"]),
                                                    TaxType = Convert.ToString(HolidayJson["Holiday"]["PreTourPrice"][n]["Tax"][p]["TaxType"]),
                                                    TaxAmount = TaxAmt
                                                });
                                            }

                                        }


                                    }
                                    #endregion

                                    objHolidayModel.PreTourPrice.Add(objPreTourPrice);
                                }
                            }
                        }

                    }
                    #endregion

                    #region PostTourPrice

                    if (HolidayJson["Holiday"]["PostTourPrice"] != null && HolidayJson["Holiday"]["PostTourPrice"].ToList().Count != 0)
                    {
                        objHolidayModel.PostTourPrice = new List<PreTourPrice>();
                        var TypeOfPostTourPrice = HolidayJson["Holiday"]["PostTourPrice"].GetType();
                        if (TypeOfPostTourPrice.Name.ToUpper() != "JARRAY")
                        { // It is Object

                            PreTourPrice objPostTourPrice = new PreTourPrice();
                            objPostTourPrice.Currency = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["Currency"]);
                            objPostTourPrice.DiscountName = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["DiscountAmount"]);
                            objPostTourPrice.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["ExtraBedding"]);
                            objPostTourPrice.PersonType = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["PersonType"]);
                            objPostTourPrice.RoomType = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["RoomType"]);
                            objPostTourPrice.SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["SupplierProductCode"]);

                            #region PassengerRangeFrom
                            double PassengerRangeFrom = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["PassengerRangeFrom"])))
                            {
                                if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["PassengerRangeFrom"]), out PassengerRangeFrom))
                                {
                                    // Conversion Success
                                    objPostTourPrice.PassengerRangeFrom = PassengerRangeFrom;
                                }
                            }
                            #endregion

                            #region PassengerRangeTo
                            double PassengerRangeTo = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["PassengerRangeTo"])))
                            {
                                if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["PassengerRangeTo"]), out PassengerRangeTo))
                                {
                                    // Conversion Success
                                    objPostTourPrice.PassengerRangeTo = PassengerRangeTo;
                                }
                            }
                            #endregion

                            #region ValidFrom
                            DateTime ValidFrom;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["ValidFrom"])))
                            {
                                if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["ValidFrom"]), out ValidFrom))
                                {
                                    // Conversion Success
                                    objPostTourPrice.ValidFrom = ValidFrom;
                                }
                            }
                            #endregion

                            #region ValidTo
                            DateTime ValidTo;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["ValidTo"])))
                            {
                                if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["ValidTo"]), out ValidTo))
                                {
                                    // Conversion Success
                                    objPostTourPrice.ValidTo = ValidTo;
                                }
                            }
                            #endregion

                            #region Amount
                            double Amount = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["Amount"])))
                            {
                                if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["Amount"]), out Amount))
                                {
                                    objPostTourPrice.Amount = Amount;
                                }
                            }
                            #endregion

                            #region AgeFrom
                            int AgeFrom = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["AgeFrom"])))
                            {
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["AgeFrom"]), out AgeFrom))
                                {
                                    objPostTourPrice.AgeFrom = AgeFrom;
                                }
                            }
                            #endregion

                            #region AgeTo
                            int AgeTo = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["AgeTo"])))
                            {
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["AgeTo"]), out AgeTo))
                                {
                                    objPostTourPrice.AgeTo = AgeTo;
                                }
                            }
                            #endregion

                            #region DiscountAmount
                            int DiscountAmount = 0;
                            if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["DiscountAmount"])))
                            {
                                if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["DiscountAmount"]), out DiscountAmount))
                                {
                                    objPostTourPrice.DiscountAmount = DiscountAmount;
                                }
                            }
                            #endregion

                            #region Tax
                            if (HolidayJson["Holiday"]["PostTourPrice"]["Tax"] != null)
                            {
                                objPostTourPrice.Tax = new List<TaxStructure>();
                                var TypeOfTaxNodes = HolidayJson["Holiday"]["PostTourPrice"]["Tax"].GetType();
                                if (TypeOfTaxNodes.Name.ToUpper() != "JARRAY")
                                {
                                    double TaxAmt = 0;
                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["Tax"]["TaxAmount"])))
                                    {
                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["Tax"]["TaxAmount"]), out TaxAmt))
                                        {
                                            // Conversion Success
                                        }
                                    }
                                    objPostTourPrice.Tax.Add(new TaxStructure()
                                    {
                                        TaxRate = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["Tax"]["TaxRate"]),
                                        TaxType = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["Tax"]["TaxType"]),
                                        TaxAmount = TaxAmt
                                    });
                                }
                                else
                                {  // it is array
                                    int totalTaxNodes = HolidayJson["Holiday"]["PostTourPrice"]["Tax"].ToList().Count;
                                    for (int p = 0; p < totalTaxNodes; p++)
                                    {
                                        double TaxAmt = 0;
                                        if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["Tax"][p]["TaxAmount"])))
                                        {
                                            if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["Tax"][p]["TaxAmount"]), out TaxAmt))
                                            {
                                                // Conversion Success
                                            }
                                        }
                                        objPostTourPrice.Tax.Add(new TaxStructure()
                                        {
                                            TaxRate = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["Tax"][p]["TaxRate"]),
                                            TaxType = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"]["Tax"][p]["TaxType"]),
                                            TaxAmount = TaxAmt
                                        });
                                    }

                                }

                            }
                            #endregion

                            objHolidayModel.PostTourPrice.Add(objPostTourPrice);
                        }
                        else
                        { // It is Array
                            int TotalPreTourPrice = HolidayJson["Holiday"]["PostTourPrice"].ToList().Count;

                            for (int n = 0; n < TotalPreTourPrice; n++)
                            {

                                if (HolidayJson["Holiday"]["PostTourPrice"][n].GetType().Name.ToUpper() == "JARRAY")
                                {
                                    int totalPostTourPrice = HolidayJson["Holiday"]["PostTourPrice"][n].ToList().Count;

                                    for (int q = 0; q < totalPostTourPrice; q++)
                                    {
                                        PreTourPrice objPostTourPrice = new PreTourPrice();
                                        objPostTourPrice.Currency = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["Currency"]);
                                        objPostTourPrice.DiscountName = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["DiscountAmount"]);
                                        objPostTourPrice.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["ExtraBedding"]);
                                        objPostTourPrice.PersonType = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["PersonType"]);
                                        objPostTourPrice.RoomType = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["RoomType"]);
                                        objPostTourPrice.SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["SupplierProductCode"]);

                                        #region PassengerRangeFrom
                                        double PassengerRangeFrom = 0;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["PassengerRangeFrom"])))
                                        {
                                            if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["PassengerRangeFrom"]), out PassengerRangeFrom))
                                            {
                                                // Conversion Success
                                                objPostTourPrice.PassengerRangeFrom = PassengerRangeFrom;
                                            }
                                        }
                                        #endregion

                                        #region PassengerRangeTo
                                        double PassengerRangeTo = 0;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["PassengerRangeTo"])))
                                        {
                                            if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["PassengerRangeTo"]), out PassengerRangeTo))
                                            {
                                                // Conversion Success
                                                objPostTourPrice.PassengerRangeTo = PassengerRangeTo;
                                            }
                                        }
                                        #endregion

                                        #region ValidFrom
                                        DateTime ValidFrom;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["ValidFrom"])))
                                        {
                                            if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["ValidFrom"]), out ValidFrom))
                                            {
                                                // Conversion Success
                                                objPostTourPrice.ValidFrom = ValidFrom;
                                            }
                                        }
                                        #endregion

                                        #region ValidTo
                                        DateTime ValidTo;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["ValidTo"])))
                                        {
                                            if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["ValidTo"]), out ValidTo))
                                            {
                                                // Conversion Success
                                                objPostTourPrice.ValidTo = ValidTo;
                                            }
                                        }
                                        #endregion

                                        #region Amount
                                        double Amount = 0;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["Amount"])))
                                        {
                                            if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["Amount"]), out Amount))
                                            {
                                                objPostTourPrice.Amount = Amount;
                                            }
                                        }
                                        #endregion

                                        #region AgeFrom
                                        int AgeFrom = 0;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["AgeFrom"])))
                                        {
                                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["AgeFrom"]), out AgeFrom))
                                            {
                                                objPostTourPrice.AgeFrom = AgeFrom;
                                            }
                                        }
                                        #endregion

                                        #region AgeTo
                                        int AgeTo = 0;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["AgeTo"])))
                                        {
                                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["AgeTo"]), out AgeTo))
                                            {
                                                objPostTourPrice.AgeTo = AgeTo;
                                            }
                                        }
                                        #endregion

                                        #region DiscountAmount
                                        int DiscountAmount = 0;
                                        if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["DiscountAmount"])))
                                        {
                                            if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["DiscountAmount"]), out DiscountAmount))
                                            {
                                                objPostTourPrice.DiscountAmount = DiscountAmount;
                                            }
                                        }
                                        #endregion

                                        #region Tax
                                        if (HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"] != null)
                                        {
                                            objPostTourPrice.Tax = new List<TaxStructure>();
                                            var TypeOfTaxNodes = HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"].GetType();
                                            if (TypeOfTaxNodes.Name.ToUpper() != "JARRAY")
                                            {
                                                double TaxAmt = 0;
                                                if (HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"].ToList().Count != 0 &&
                                                    !string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"]["TaxAmount"])))
                                                {
                                                    if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"]["TaxAmount"]), out TaxAmt))
                                                    {
                                                        // Conversion Success
                                                    }
                                                }
                                                if (HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"].ToList().Count != 0)
                                                {
                                                    objPostTourPrice.Tax.Add(new TaxStructure()
                                                    {
                                                        TaxRate = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"]["TaxRate"]),
                                                        TaxType = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"]["TaxType"]),
                                                        TaxAmount = TaxAmt
                                                    });
                                                }
                                            }
                                            else
                                            {  // it is array
                                                int totalTaxNodes = HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"].ToList().Count;
                                                for (int p = 0; p < totalTaxNodes; p++)
                                                {
                                                    double TaxAmt = 0;
                                                    if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"][p]["TaxAmount"])))
                                                    {
                                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"][p]["TaxAmount"]), out TaxAmt))
                                                        {
                                                            // Conversion Success
                                                        }
                                                    }
                                                    if (HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"][p].ToList().Count != 0)
                                                    {
                                                        objPostTourPrice.Tax.Add(new TaxStructure()
                                                        {
                                                            TaxRate = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"][p]["TaxRate"]),
                                                            TaxType = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n][q]["Tax"][p]["TaxType"]),
                                                            TaxAmount = TaxAmt
                                                        });
                                                    }
                                                }

                                            }


                                        }
                                        #endregion

                                        objHolidayModel.PostTourPrice.Add(objPostTourPrice);
                                    }

                                }
                                else
                                {

                                    PreTourPrice objPostTourPrice = new PreTourPrice();
                                    objPostTourPrice.Currency = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["Currency"]);
                                    objPostTourPrice.DiscountName = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["DiscountAmount"]);
                                    objPostTourPrice.ExtraBedding = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["ExtraBedding"]);
                                    objPostTourPrice.PersonType = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["PersonType"]);
                                    objPostTourPrice.RoomType = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["RoomType"]);
                                    objPostTourPrice.SupplierProductCode = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["SupplierProductCode"]);

                                    #region PassengerRangeFrom
                                    double PassengerRangeFrom = 0;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["PassengerRangeFrom"])))
                                    {
                                        if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["PassengerRangeFrom"]), out PassengerRangeFrom))
                                        {
                                            // Conversion Success
                                            objPostTourPrice.PassengerRangeFrom = PassengerRangeFrom;
                                        }
                                    }
                                    #endregion

                                    #region PassengerRangeTo
                                    double PassengerRangeTo = 0;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["PassengerRangeTo"])))
                                    {
                                        if (Double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["PassengerRangeTo"]), out PassengerRangeTo))
                                        {
                                            // Conversion Success
                                            objPostTourPrice.PassengerRangeTo = PassengerRangeTo;
                                        }
                                    }
                                    #endregion

                                    #region ValidFrom
                                    DateTime ValidFrom;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["ValidFrom"])))
                                    {
                                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["ValidFrom"]), out ValidFrom))
                                        {
                                            // Conversion Success
                                            objPostTourPrice.ValidFrom = ValidFrom;
                                        }
                                    }
                                    #endregion

                                    #region ValidTo
                                    DateTime ValidTo;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["ValidTo"])))
                                    {
                                        if (DateTime.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["ValidTo"]), out ValidTo))
                                        {
                                            // Conversion Success
                                            objPostTourPrice.ValidTo = ValidTo;
                                        }
                                    }
                                    #endregion

                                    #region Amount
                                    double Amount = 0;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["Amount"])))
                                    {
                                        if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["Amount"]), out Amount))
                                        {
                                            objPostTourPrice.Amount = Amount;
                                        }
                                    }
                                    #endregion

                                    #region AgeFrom
                                    int AgeFrom = 0;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["AgeFrom"])))
                                    {
                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["AgeFrom"]), out AgeFrom))
                                        {
                                            objPostTourPrice.AgeFrom = AgeFrom;
                                        }
                                    }
                                    #endregion

                                    #region AgeTo
                                    int AgeTo = 0;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["AgeTo"])))
                                    {
                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["AgeTo"]), out AgeTo))
                                        {
                                            objPostTourPrice.AgeTo = AgeTo;
                                        }
                                    }
                                    #endregion

                                    #region DiscountAmount
                                    int DiscountAmount = 0;
                                    if (!String.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["DiscountAmount"])))
                                    {
                                        if (int.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["DiscountAmount"]), out DiscountAmount))
                                        {
                                            objPostTourPrice.DiscountAmount = DiscountAmount;
                                        }
                                    }
                                    #endregion

                                    #region Tax
                                    if (HolidayJson["Holiday"]["PostTourPrice"][n]["Tax"] != null)
                                    {
                                        objPostTourPrice.Tax = new List<TaxStructure>();
                                        var TypeOfTaxNodes = HolidayJson["Holiday"]["PostTourPrice"][n]["Tax"].GetType();
                                        if (TypeOfTaxNodes.Name.ToUpper() != "JARRAY")
                                        {
                                            double TaxAmt = 0;
                                            if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["Tax"]["TaxAmount"])))
                                            {
                                                if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["Tax"]["TaxAmount"]), out TaxAmt))
                                                {
                                                    // Conversion Success
                                                }
                                            }
                                            objPostTourPrice.Tax.Add(new TaxStructure()
                                            {
                                                TaxRate = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["Tax"]["TaxRate"]),
                                                TaxType = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["Tax"]["TaxType"]),
                                                TaxAmount = TaxAmt
                                            });
                                        }
                                        else
                                        {  // it is array
                                            int totalTaxNodes = HolidayJson["Holiday"]["PostTourPrice"][n]["Tax"].ToList().Count;
                                            for (int p = 0; p < totalTaxNodes; p++)
                                            {
                                                double TaxAmt = 0;
                                                if (!string.IsNullOrEmpty(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["Tax"][p]["TaxAmount"])))
                                                {
                                                    if (double.TryParse(Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["Tax"][p]["TaxAmount"]), out TaxAmt))
                                                    {
                                                        // Conversion Success
                                                    }
                                                }
                                                objPostTourPrice.Tax.Add(new TaxStructure()
                                                {
                                                    TaxRate = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["Tax"][p]["TaxRate"]),
                                                    TaxType = Convert.ToString(HolidayJson["Holiday"]["PostTourPrice"][n]["Tax"][p]["TaxType"]),
                                                    TaxAmount = TaxAmt
                                                });
                                            }

                                        }


                                    }
                                    #endregion

                                    objHolidayModel.PostTourPrice.Add(objPostTourPrice);
                                }
                            }
                        }

                    }

                    #endregion

                    #region Update
                    //var ObjHolidayMapping = searchResult.Where(x => x.SupplierName.ToLower() == objHolidayModel.SupplierName.ToLower() && x.SupplierProductCode == objHolidayModel.SupplierProductCode).ToList().FirstOrDefault();
                    var ObjHolidayMapping = searchResult.Where(x => x.NakshatraHolidayId.ToUpper() == objHolidayModel.NakshatraHolidayId).ToList().FirstOrDefault();
                    if (ObjHolidayMapping != null)
                    {// it means its a old record so assign Nakid and classificationattributes to our model.
                        objHolidayModel.NakshatraHolidayId = ObjHolidayMapping.NakshatraHolidayId;
                        objHolidayModel.TypeOfHoliday = ObjHolidayMapping.TypeOfHoliday;
                        objHolidayModel.TravelFrequency = ObjHolidayMapping.TravelFrequency;
                        objHolidayModel.PaceOfHoliday = ObjHolidayMapping.PaceOfHoliday;
                        objHolidayModel.Rating = ObjHolidayMapping.Rating;
                        objHolidayModel.ComfortLevel = ObjHolidayMapping.ComfortLevel;
                        objHolidayModel.StayType = ObjHolidayMapping.StayType;
                        objHolidayModel.Interests = ObjHolidayMapping.Interests;
                        objHolidayModel.TravellerType = ObjHolidayMapping.TravellerType;
                        objHolidayModel.Collections = ObjHolidayMapping.Collections;

                        if (ObjHolidayMapping.ClassificationAttributes != null && ObjHolidayMapping.ClassificationAttributes.Count > 0)
                        {
                            objHolidayModel.ClassificationAttributes = ObjHolidayMapping.ClassificationAttributes;
                        }
                    }

                    #endregion

                    // HolidayMappingCollection.InsertOne(objHolidayModel);
                    var filter1 = Builders<HolidayModel>.Filter.Eq(c => c.SupplierProductCode, objHolidayModel.SupplierProductCode);
                    filter1 = filter1 & Builders<HolidayModel>.Filter.Eq(c => c.SupplierName, objHolidayModel.SupplierName);
                    HolidayMappingCollection.ReplaceOneAsync(filter1, objHolidayModel, new UpdateOptions { IsUpsert = true });

                    counter++;
                    UpdateDistLogInfo(Logid, PushStatus.RUNNNING, CollecionHolidayMappingsFiltered.Count, counter, Logid.ToString(), "HOLIDAY", "MAPPING");
                }


                UpdateDistLogInfo(Logid, PushStatus.COMPLETED, CollecionHolidayMappingsFiltered.Count, counter, null, "HOLIDAY", "MAPPING");


            }
            catch (Exception ex)
            {
                UpdateDistLogInfo(Logid, PushStatus.ERROR, 0, 0, null, "HOLIDAY", "MAPPING");
            }
        }
        #endregion

        #region Load Room Type Mapping

        public void UpdateHotelRoomTypeMapping(Guid Logid, Guid Supplier_Id)
        {
            bool Is_IX_SupplierCode_SupplierProductCode_SupplierRoomTypeCode_Exists = false;
            bool Is_IX_MapId_Exists = false;

            _database = MongoDBHandler.mDatabase();
            var collection = _database.GetCollection<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>("RoomTypeMapping");

            try
            {
                int TotalCount = 0;
                List<Supplier> SupplierIds = new List<Supplier>();
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    context.Database.CommandTimeout = 0;

                    SupplierIds = context.Suppliers.Where(w => w.StatusCode == "ACTIVE").Select(s => s).ToList();
                    if (Supplier_Id != Guid.Empty)
                    {
                        SupplierIds = SupplierIds.Where(w => w.Supplier_Id == Supplier_Id).ToList();
                        TotalCount = (from srtm in context.Accommodation_SupplierRoomTypeMapping.AsNoTracking()
                                      join sup in context.Suppliers.AsNoTracking() on srtm.Supplier_Id equals sup.Supplier_Id
                                      join srtmv in context.Accommodation_SupplierRoomTypeMapping_Values.AsNoTracking() on srtm.Accommodation_SupplierRoomTypeMapping_Id equals srtmv.Accommodation_SupplierRoomTypeMapping_Id
                                      where sup.StatusCode == "ACTIVE" && sup.Supplier_Id == Supplier_Id && srtmv.UserMappingStatus == "MAPPED"
                                      select 0).Count();
                    }
                    else
                    {
                        var DistinctRoomSuppliers = context.Accommodation_SupplierRoomTypeMapping.Select(s => s.Supplier_Id).Distinct().ToList();

                        SupplierIds = (from si in SupplierIds
                                       join srs in DistinctRoomSuppliers on si.Supplier_Id equals srs.Value
                                       select si).ToList();

                        TotalCount = (from srtm in context.Accommodation_SupplierRoomTypeMapping.AsNoTracking()
                                      join sup in context.Suppliers.AsNoTracking() on srtm.Supplier_Id equals sup.Supplier_Id
                                      join srtmv in context.Accommodation_SupplierRoomTypeMapping_Values.AsNoTracking() on srtm.Accommodation_SupplierRoomTypeMapping_Id equals srtmv.Accommodation_SupplierRoomTypeMapping_Id
                                      where sup.StatusCode == "ACTIVE" && srtmv.UserMappingStatus == "MAPPED"
                                      select 0).Count();
                    }
                }

                foreach (var SupplierId in SupplierIds)
                {
                    int counter = 0;

                    if (Logid == Guid.Empty)
                    {
                        Logid = Guid.NewGuid();
                        UpdateDistLogInfo(Logid, PushStatus.INSERT, TotalCount, 0, SupplierId.ToString(), "ROOMTYPE", "MAPPING");
                    }

                    UpdateDistLogInfo(Logid, PushStatus.RUNNNING);

                    List<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest> _objHRTM = new List<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>();

                    _objHRTM = GetDataToPushMongo_RTM(SupplierId.Supplier_Id, SupplierId.Code.ToUpper());

                    UpdateDistLogInfo(Logid, PushStatus.RUNNNING, _objHRTM.Count(), 0, SupplierId.ToString(), "ROOMTYPE", "MAPPING");

                    if (_objHRTM.Count > 0)
                    {
                        var SupplierMapIdsInMongo = collection.Find(Builders<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>.Filter.Eq(c => c.supplierCode, SupplierId.Code.ToUpper())).Project(u => new { u.SystemRoomTypeMapId }).ToList();
                        var MapIdsToBeDeleted = (from m in SupplierMapIdsInMongo
                                                 join d in _objHRTM on m.SystemRoomTypeMapId equals d.SystemRoomTypeMapId into ljo
                                                 from lj in ljo.DefaultIfEmpty()
                                                 where lj == null
                                                 select m.SystemRoomTypeMapId).Distinct().ToList();
                        foreach (var mapid in MapIdsToBeDeleted)
                        {
                            var filter = Builders<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>.Filter.Eq(c => c.SystemRoomTypeMapId, mapid);
                            var result = collection.DeleteMany(filter);
                        }

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

        private List<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest> GetDataToPushMongo_RTM(Guid Supplier_id, string SupplierCode)
        {
            List<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest> _objHRTM = new List<DataContracts.Mapping.DC_HotelRoomTypeMappingRequest>();

            try
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    context.Database.CommandTimeout = 0;
                    context.Configuration.AutoDetectChangesEnabled = false;

                    _objHRTM = (from srtm in context.Accommodation_SupplierRoomTypeMapping.AsNoTracking()
                                join srtmv in context.Accommodation_SupplierRoomTypeMapping_Values.AsNoTracking() on srtm.Accommodation_SupplierRoomTypeMapping_Id equals srtmv.Accommodation_SupplierRoomTypeMapping_Id
                                join ari in context.Accommodation_RoomInfo.AsNoTracking() on srtmv.Accommodation_RoomInfo_Id equals ari.Accommodation_RoomInfo_Id
                                join acco in context.Accommodations on ari.Accommodation_Id equals acco.Accommodation_Id
                                // where srtm.SupplierName == "Expedia" && srtm.SupplierProductId == "474569" && srtmv.UserMappingStatus == "MAPPED"
                                where srtm.Supplier_Id == Supplier_id && srtmv.UserMappingStatus == "MAPPED"
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
                                    supplierCode = SupplierCode,
                                    SupplierProductId = srtm.SupplierProductId.ToUpper(),
                                    SupplierRoomCategory = srtm.SupplierRoomCategory.ToUpper(),
                                    SupplierRoomCategoryId = srtm.SupplierRoomCategoryId.ToUpper(),
                                    SupplierRoomId = srtm.SupplierRoomId.ToUpper(),
                                    SupplierRoomName = srtm.SupplierRoomName.ToUpper(),
                                    SupplierRoomTypeCode = srtm.SupplierRoomTypeCode.ToUpper(),
                                    SystemNormalizedRoomType = srtm.TX_RoomName,
                                    SystemProductCode = acco.CompanyHotelID,
                                    SystemRoomCategory = ari.RoomCategory,
                                    SystemRoomTypeCode = ari.RoomId,
                                    SystemRoomTypeMapId = srtmv.MapId,
                                    SystemRoomTypeName = ari.RoomName,
                                    SystemStrippedRoomType = srtm.Tx_StrippedName
                                }).ToList();
                }
            }
            catch (Exception ex)
            {
                return _objHRTM;
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
