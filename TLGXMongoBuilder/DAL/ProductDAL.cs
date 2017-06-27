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
    public class ProductDAL : IDisposable
    {
        protected static IMongoDatabase _database;

        public void Dispose()
        {
        }


        public ProductDetails GetProductDetails(string ID)
        {
            throw new NotImplementedException();
            //try
            //{
            //    using (ETNSupportEntities context = new ETNSupportEntities())
            //    {
            //        var ProdDetails = (from p in context.Products
            //                           join pt in context.def_ProductType on p.ProductType_Id equals pt.ProductType_Id
            //                           join ct in context.Resorts on p.Resort_Id equals ct.Resort_Id
            //                           join cn in context.Resorts on ct.Parent_Resort_Id equals cn.Resort_Id
            //                           join prodAttr in context.ProductAttributes on p.Product_Id equals prodAttr.Product_Id
            //                           join dProdAttr in context.Def_ProductAttribute on prodAttr.Attribute_id equals dProdAttr.Attribute_Id
            //                           join dAttr in context.Def_AttributeValues on prodAttr.AttributeValues_Id equals dAttr.AttributeValues_Id
            //                           where p.Ezeego_Id == ID && dProdAttr.AttributeName == "STARRATING"
            //                           select new ProductDetails
            //                           {
            //                               Hotel_Id = p.Ezeego_Id,
            //                               Address = p.Address,
            //                               City = ct.RESORT1,
            //                               Country = cn.RESORT1,
            //                               ProdName = p.PRODNAME,
            //                               ProdType = pt.PRODTYPE,
            //                               StarRating = dAttr.Value,
            //                               WebSite = p.SUPPWEB
            //                           }).FirstOrDefault();

            //        if (ProdDetails != null)
            //        {
            //            var ProdCategory = (from p in context.Products
            //                                join pc in context.ProductCategories on p.Product_Id equals pc.Product_Id
            //                                join dpc in context.Def_ProductCategory on pc.Def_ProductCategory_Id equals dpc.Def_ProductCategory_Id
            //                                where p.Ezeego_Id == ID
            //                                select new ProductCategories
            //                                {
            //                                    CategoryID = "",
            //                                    RoomCategory = dpc.Name,
            //                                    RoomName = ""
            //                                }).ToList();

            //            ProdDetails.ProdCat = ProdCategory;
            //        }
            //        else
            //        {
            //            throw new FaultException<DataContracts.ErrorNotifier>(new ErrorNotifier { ErrorMessage = "No data found.", ErrorStatusCode = System.Net.HttpStatusCode.NotFound });
            //        }

            //        return ProdDetails;
            //    }
            //}
            //catch (FaultException<DataContracts.ErrorNotifier> ex)
            //{
            //    throw ex;
            //}
        }

        //public void LoadHotelDefinitions()
        //{
        //    try
        //    {
        //        using (TLGX_DEVEntities context = new TLGX_DEVEntities())
        //        {

        //            _database = MongoDBHandler.mDatabase();

        //            _database.DropCollection("Accommodations");

        //            var collection = _database.GetCollection<BsonDocument>("Accommodations");
        //            var AccoList = (from a in context.Accommodations
        //                           where a.CompanyHotelID != null
        //                           select a).Take(100);


        //            List<BsonDocument> docs = new List<BsonDocument>();

        //            foreach (var Acco in AccoList)
        //            {

        //                var document = new BsonDocument
        //                {
        //                    //{
        //                        //Acco.CompanyHotelID.ToString(), new BsonDocument
        //                        //{
        //                            {"SupplierHotelID",string.Empty},
        //                            {"HotelId",Acco.CompanyHotelID.ToString()},
        //                            {"name",Acco.HotelName},
        //                            //{"starrating",(Acco.HotelRating ?? string.Empty)},
        //                            {"credicards",string.Empty},
        //                            {"areatransportation",string.Empty},
        //                            {"restaurants",string.Empty},
        //                            {"meetingfacility",string.Empty},
        //                            {"description",string.Empty},
        //                            {"highlight", string.Empty},
        //                            {"overview", string.Empty},
        //                            {"checkintime", (Acco.CheckInTime ?? string.Empty)},
        //                            {"checkouttime", (Acco.CheckOutTime ?? string.Empty)},
        //                            {"email", string.Empty},
        //                            {"website", string.Empty},
        //                            {"rooms", (Acco.TotalRooms ?? string.Empty)},
        //                            {"LandmarkCategory",string.Empty},
        //                            {"Landmark",string.Empty},
        //                            {"theme",string.Empty},
        //                            {"HotelChain",(Acco.Chain ?? string.Empty)},
        //                            {"BrandName",(Acco.Brand ?? string.Empty)},
        //                            {"recommends", (Acco.CompanyRecommended ?? false)},
        //                            {"latitude",(Acco.Latitude ?? string.Empty)},
        //                            {"longitude", (Acco.Longitude ?? string.Empty)},
        //                            {"LandmarkDescription",string.Empty},
        //                            {"starrating", new BsonDocument
        //                                {
        //                                    {"level", (Acco.HotelRating ?? string.Empty) }
        //                                }
        //                            },
        //                            //{"Address", new BsonDocument
        //                            //    {
        //                            //        {"address",(Acco.StreetName ?? string.Empty)},
        //                            //        {"city", (Acco.city ?? string.Empty)},
        //                            //        {"state",(Acco.State_Name ?? string.Empty)},
        //                            //        {"country", (Acco.country ?? string.Empty)},
        //                            //        {"pincode", (Acco.PostalCode ?? string.Empty)},
        //                            //        {"location",string.Empty}
        //                            //        //{"phone",string.Empty},
        //                            //        //{"fax",string.Empty}
        //                            //    }
        //                            //},
        //                            {"thumb",string.Empty},
        //                            //{"image", new BsonDocument
        //                            //    {
        //                            //        {"image0",string.Empty},
        //                            //        {"image1", string.Empty}
        //                            //    }
        //                            //},
        //                            {"video",string.Empty},
        //                            //{"HotelFacility", new BsonDocument
        //                            //    {
        //                            //        {"hotelfacility0",string.Empty},
        //                            //        {"hotelfacility1",string.Empty}
        //                            //    }
        //                            //},
        //                            {"HotelAmenity", new BsonDocument
        //                                {
        //                                    {"Restaurant",1},
        //                                    {"conference",0}
        //                                }
        //                            },
        //                            {"HotelDistance",new BsonDocument
        //                                {
        //                                    {"DistancefromAirport",12},
        //                                    {"DistancefromStation",5}
        //                                }
        //                            },
        //                            {"type1","Standard"},
        //                            {"facility1", new BsonDocument
        //                                {
        //                                    {"facility0","Ac Room"},
        //                                    {"facility1","Attached Bath"}
        //                                }
        //                            },
        //                            {"type2","Premium Suite"},
        //                            {"facility2", new BsonDocument
        //                                {
        //                                    {"facility0","Ac Room"},
        //                                    {"facility1","Attached Bath"}
        //                                }
        //                            }

        //                        //}
        //                    //}
        //                };

        //                var AccoContact = (from c in context.Accommodation_Contact
        //                                   orderby c.Create_Date descending
        //                                   where c.Accommodation_Id == Acco.Accommodation_Id
        //                                   select c).FirstOrDefault();
        //                if (AccoContact != null)
        //                {
        //                    document.Add(new BsonElement("Address", new BsonDocument {{"address",(Acco.StreetName ?? string.Empty)},
        //                                    {"city", (Acco.city ?? string.Empty)},
        //                                    {"state",(Acco.State_Name ?? string.Empty)},
        //                                    {"country", (Acco.country ?? string.Empty)},
        //                                    {"pincode", (Acco.PostalCode ?? string.Empty)},
        //                                    {"location",string.Empty},
        //                                    { "phone", AccoContact.Telephone ?? string.Empty },
        //                                    { "fax", AccoContact.Fax ?? string.Empty } }));
        //                }
        //                else
        //                {
        //                    document.Add(new BsonElement("Address", new BsonDocument {{"address",(Acco.StreetName ?? string.Empty)},
        //                                    {"city", (Acco.city ?? string.Empty)},
        //                                    {"state",(Acco.State_Name ?? string.Empty)},
        //                                    {"country", (Acco.country ?? string.Empty)},
        //                                    {"pincode", (Acco.PostalCode ?? string.Empty)},
        //                                    {"location",string.Empty}
        //                                     }));
        //                }


        //                var AccoImages = from m in context.Accommodation_Media
        //                                 where m.Accommodation_Id == Acco.Accommodation_Id
        //                                 select m;
        //                var bHM = new BsonDocument(true);
        //                foreach (var media in AccoImages)
        //                {
        //                    bHM.Add("path", media.Media_URL);
        //                }
        //                document.Add("image", bHM);

        //                var AccoFacility = from f in context.Accommodation_Facility
        //                                   where f.Accommodation_Id == Acco.Accommodation_Id
        //                                   select f;
        //                var bHF = new BsonDocument(true);
        //                foreach (var facility in AccoFacility)
        //                {
        //                    bHF.Add("name", facility.FacilityType);
        //                }
        //                document.Add("HotelFacility", bHF);

        //                docs.Add(document);
        //                document = null;



        //                //    //Address
        //                //    if (objProducts.Address != null)
        //                //    {
        //                //        BsonArray ba = new BsonArray();

        //                //        ba.Add(objProducts.Address);

        //                //        var tx_details = (from t in Transformations
        //                //                          where t.TX_Column == "AddressLine"
        //                //                          select new
        //                //                          {
        //                //                              t.Keyword,
        //                //                              t.Value
        //                //                          }).ToList();

        //                //        if (tx_details.Count > 0)
        //                //        {
        //                //            foreach (var txval in tx_details)
        //                //            {
        //                //                if (Regex.IsMatch(objProducts.Address, txval.Keyword, RegexOptions.IgnoreCase))
        //                //                {
        //                //                    ba.Add(Regex.Replace(objProducts.Address, txval.Keyword, txval.Value, RegexOptions.IgnoreCase));
        //                //                }

        //                //            }
        //                //        }

        //                //        document.Add("AddressLine", ba);
        //                //        ba = null;
        //                //    }



        //                //    docs.Add(document);
        //                //    docs = null;

        //                //}





        //            }

        //            collection.InsertMany(docs);
        //            docs = null;
        //            collection = null;
        //            _database = null;
        //        }
        //    }
        //    catch (FaultException<DataContracts.ErrorNotifier> ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void LoadHotelDefinitions()
        {
            using (TLGX_DEVEntities context = new TLGX_DEVEntities())
            {
                _database = MongoDBHandler.mDatabase();
                
                //_database.DropCollection("Accommodations");

                var collection = _database.GetCollection<HotelsHotel>("Accommodations");
                var AccoList = from a in context.Accommodations
                                where a.CompanyHotelID != null
                                select a;

                List<HotelsHotel> docs = new List<HotelsHotel>();

                foreach (var Acco in AccoList)
                {
                    //if hotel id is null then don't insert else check for duplicates
                    if (Acco.CompanyHotelID == null)
                    {
                        continue;
                    }
                    else
                    {
                        //check if record is already exists
                        var searchResultCount = collection.Find(f => f.HotelId == Acco.CompanyHotelID.ToString()).Count();
                        if (searchResultCount > 0)
                        {
                            continue;
                        }
                    }

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

                docs = null;
                collection = null;
                _database = null;
            }
        }

        public List<HotelDescriptiveInfo> HotelSearch(HotelSearchRequest objHSRQ)
        {
            _database = MongoDBHandler.mDatabase();

            var collection = _database.GetCollection<BsonDocument>("HotelDefinitions");

            FilterDefinition<BsonDocument> filter;
            filter = Builders<BsonDocument>.Filter.Empty;
            //Hotel Name filter
            if (objHSRQ.HotelName != null)
            {
                filter = filter & Builders<BsonDocument>.Filter.Regex("HotelName", new BsonRegularExpression(new Regex(objHSRQ.HotelName, RegexOptions.IgnoreCase)));
            }

            //City Name Filter
            if (objHSRQ.CityName != null)
            {
                filter = filter & Builders<BsonDocument>.Filter.Eq("CityName", objHSRQ.CityName);
            }

            //Country Name filter
            if (objHSRQ.CountryName != null)
            {
                filter = filter & Builders<BsonDocument>.Filter.Eq("CountryName", objHSRQ.CountryName);
            }

            var searchResult = collection.Find(filter).ToList();

            List<HotelDescriptiveInfo> result = new List<HotelDescriptiveInfo>();
            foreach (var docs in searchResult)
            {
                HotelDescriptiveInfo subResult = new HotelDescriptiveInfo();
                subResult.Id = docs["_id"].AsObjectId;
                subResult.HotelCode = docs["HotelCode"].AsString;
                subResult.HotelName = docs["HotelName"].AsString;
                subResult.Rating = docs["Rating"].AsString;
                subResult.URLs = docs["URLs"].AsString;
                subResult.AddressLine = docs["AddressLine"].AsString;
                subResult.PostalCode = docs["PostalCode"].AsString;
                subResult.CityName = docs["CityName"].AsString;
                subResult.CountryName = docs["CountryName"].AsString;
                var position = docs["Position"].AsBsonArray.Select(p => p.AsString).ToArray();
                string[] actualPosition = new string[position.Length];
                int i = 0;
                foreach (string pos in position)
                {
                    actualPosition[i] = pos;
                    i++;
                }
                subResult.Position = actualPosition;
                subResult.PhoneNumber = docs["PhoneNumber"].AsString;
                result.Add(subResult);
            }

            collection = null;
            _database = null;

            return result;
        }
    }
}
