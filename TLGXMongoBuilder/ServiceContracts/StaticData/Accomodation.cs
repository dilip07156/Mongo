using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace DataContracts.StaticData
{
    [DataContract]
    public class Accomodation
    {
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();
        public List<string> RoomIds { get; set; }

        public AccomodationInfo AccomodationInfo { get; set; }
        public Overview Overview { get; set; }
        public StaffContactInfo StaffContactInfo { get; set; }
        public List<Facility> Facility { get; set; }
        public PassengerOccupancy PassengerOccupancy { get; set; }
        public List<Directions> Directions { get; set; }
        public List<InAndAround> InAndAround { get; set; }
        public List<Landmark> Landmark { get; set; }
        public List<Rule> Rule { get; set; }
        public List<Media> Media { get; set; }
        public List<Updates> Updates { get; set; }
        public List<SafetyRegulations> SafetyRegulations { get; set; }
        public List<Ancillary> Ancillary { get; set; }
        public ProductStatus ProductStatus { get; set; }
        //New field Added
        public List<Rooms> Rooms { get; set; }
        public List<Policies> Policies { get; set; }
        //public SupplierDetails SupplierDetails { get; set; }

        //public string CreatedBy { get; set; }
        //public string UpdatedBy { get; set; }
        //public string CreatedAt { get; set; }
        //public string LastUpdated { get; set; }
        //public bool Deleted { get; set; }

        //public Lock Lock { get; set; }
    }

    //public class SupplierDetails
    //{
    //    public string SupplierCode { get; set; }
    //    public string SupplierProductCode { get; set; }
    //}

    //public class Lock
    //{
    //    public bool Enabled { get; set; }
    //    public string User { get; set; }
    //}

    public class AccomodationInfo
    {
        public string CompanyId { get; set; }
        public string CompanyName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ProductCatSubType { get; set; }

        public bool IsMysteryProduct { get; set; }
        public string CommonProductId { get; set; }
        public string CompanyProductId { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        public string FinanceControlId { get; set; }
        public string Chain { get; set; }
        public string Brand { get; set; }
        public string Affiliations { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Rating { get; set; }

        public string CompanyRating { get; set; }
        public Date RatingDatedOn { get; set; }
        public int NoOfFloors { get; set; }
        public int NoOfRooms { get; set; }
        public List<string> RecommendedFor { get; set; }
        public string DisplayName { get; set; }
        public bool IsTwentyFourHourCheckout { get; set; }

        //New field Added
        public bool IsBookWithoutCC { get; set; }
        public bool IsRecommended { get; set; }

        public string CheckinCloseTime { get; set; }
        public string CheckoutCloseTime { get; set; }
        public bool IsRatingEstimatedAutomatically { get; set; }
        public bool IsCreditcardRequired { get; set; }
        public string currency { get; set; }

        public string DefaultLanguage { get; set; }
        public string ExactRating { get; set; }
        public string ProductCategorySubTypeId { get; set; }
        public string HotelMessage { get; set; }
        public string MaxPersonReservation { get; set; }
        public string MaxRoomReservation { get; set; }
        public string noOfReviews { get; set; }
        public string HotelRanking { get; set; }
        public string ReviewScore { get; set; }
        public string SpokenLanguages { get; set; }
        public string Facilities { get; set; }

        //public string ChildrenPolicy { get; set; }
        //public string InternetPolicy { get; set; }
        //public string ParkingPolicy { get; set; }
        //public string PetsPolicy { get; set; }
        //public string GroupsPolicy { get; set; }
        //public string Policies { get; set; }
        //public string RecreationPolicy { get; set; }
        //public string TermsAndConditions { get; set; }
        public string Stars { get; set; }


        //New field Added


        [JsonProperty(Required = Required.Always)]
        public string CheckInTime { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string CheckOutTime { get; set; }

        public FamilyDetails FamilyDetails { get; set; }
        public string ResortType { get; set; }
        public Address Address { get; set; }
        public List<ContactDetails> ContactDetails { get; set; }
        public General General { get; set; }


        //public string ShortDescription { get; set; }
        //public string LongDescription { get; set; }
    }

    public class Date
    {
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
    }

    public class FamilyDetails
    {
        [JsonProperty(Required = Required.Always)]
        public string FamilyName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string FamilyDescription { get; set; }
    }

    public class Address
    {
        public string HouseNumber { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Street { get; set; }

        public string Street2 { get; set; }
        public string Street3 { get; set; }
        public string Street4 { get; set; }
        public string Street5 { get; set; }
        public string Zone { get; set; }


        //New field Added
        public string CityCode { get; set; }
        public string CountryCode { get; set; }
        public string StateCode { get; set; }
        public string DistrictId { get; set; }
        public string DistrictName { get; set; }
        public string Region { get; set; }
        //New field Added

        [JsonProperty(Required = Required.Always)]
        public string PostalCode { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Country { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string State { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string City { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Area { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Location { get; set; }

        public Geometry Geometry { get; set; }
    }

    public class Geometry
    {
        public string Type { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<decimal> Coordinates { get; set; }
    }

    public class ContactDetails
    {
        public TelephoneFormat Phone { get; set; }
        public TelephoneFormat Fax { get; set; }
        public string Website { get; set; }
        //New field Added
        public string MobileAppUrl { get; set; }
        public string EmailAddress { get; set; }

    }

    public class TelephoneFormat
    {
        public string CountryCode { get; set; }
        public string CityCode { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Number { get; set; }
    }

    public class General
    {
        public string Cfp { get; set; }
        public string YearBuilt { get; set; }
        public string YearRenovated { get; set; }
        public string AwardsReceived { get; set; }
        public List<Extras> Extras { get; set; }
        public string InternalRemarks { get; set; }
        public string CopiedFrom { get; set; }
    }

    public class Extras
    {
        public string Label { get; set; }
        public string Description { get; set; }
    }

    public class Overview
    {
        public List<Duration> Duration { get; set; }

        [JsonProperty(Required = Required.Always)]
        public List<string> Interest { get; set; }

        public string Usp { get; set; }
        public List<string> HashTag { get; set; }
        public string Highlights { get; set; }
        public string SellingTips { get; set; }
        public bool IsCompanyRecommended { get; set; }
    }

    public class Duration
    {
        public Date From { get; set; }
        public Date To { get; set; }
        public string TypeOfDesc { get; set; }
        public string Description { get; set; }
    }

    public class StaffContactInfo
    {
        public HostFamilyDetails HostFamilyDetails { get; set; }
        public CertificationDetails CertificationDetails { get; set; }
        public LocationDetails LocationDetails { get; set; }
    }

    public class HostFamilyDetails
    {
        public string description { get; set; }
        public int MemberCount { get; set; }
        public List<Members> Members { get; set; }
        public Childrens Childrens { get; set; }
        public Pets Pets { get; set; }
        public bool IsNonSmokinghouseHold { get; set; }

    }

    public class Members
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string Description { get; set; }
        public string LanguageSpoken { get; set; }
        public string Interest { get; set; }
    }

    public class Childrens
    {
        public bool IsPresent { get; set; }
        public int Count { get; set; }
    }

    public class Pets
    {
        public bool IsPresent { get; set; }
        public int Count { get; set; }
        public string PetTypes { get; set; }
    }

    public class CertificationDetails
    {
        public string CertifiedHostId { get; set; }
        public string UserCertificationDescription { get; set; }
        public string CriminalRecordDescription { get; set; }
    }

    public class LocationDetails
    {
        public string NeighbourHoodDescription { get; set; }
        public string DistanceFromCenter { get; set; }
        public string DistanceFromPublicTransportation { get; set; }
    }

    public class Facility
    {

        public string Category { get; set; }
        public string Type { get; set; }
        public string Desc { get; set; }
        public string OperationalTimeFrom { get; set; }
        public string OperationalTimeTo { get; set; }
        public string Duration { get; set; }
        public bool IsChargeable { get; set; }
        //New field Added
        public string Facility_Id { get; set; }
        public string FacilityCategoryID { get; set; }
        public string ExtraCharge { get; set; }

        //New field Added
    }

    public class PassengerOccupancy
    {
        public List<Type1> Type1 { get; set; }
        public List<Type2> Type2 { get; set; }
    }

    public class Type1
    {
        public string RoomCategory { get; set; }
        public string RoomType { get; set; }
        public int MaxAdults { get; set; }
        public AgeFor AgeForPaxExtraBed { get; set; }
        public int MaxPaxWithExtraBeds { get; set; }
        public AgeFor AgeBracketForCnb { get; set; }
        public int MaxCnb { get; set; }
        public AgeFor AgeForCior { get; set; }
        public int MaxCior { get; set; }
        public int MaxPax { get; set; }
    }

    public class AgeFor
    {
        public int From { get; set; }
        public int To { get; set; }
    }

    public class Type2
    {
        public string RoomCategory { get; set; }
        public int MaxAdults { get; set; }
        public int MaxChild { get; set; }
        public int MaxPax { get; set; }
    }

    public class Distance
    {
        public string Value { get; set; }
        public string Unit { get; set; }
    }

    public class Directions
    {
        [JsonProperty(Required = Required.Always)]
        public string From { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string NameOfPlace { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string ModeOfTransport { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string TransportType { get; set; }

        public Distance DistanceFromProperty { get; set; }
        public Distance ApproxDuration { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Description { get; set; }

        public string DrivingDirection { get; set; }
        public Date ValidityFrom { get; set; }
        public Date ValidityTo { get; set; }
    }

    public class InAndAround
    {
        public string PlaceCategory { get; set; }
        public string PlaceName { get; set; }
        public Distance DistanceFromProperty { get; set; }
        public string LocationHighlights { get; set; }
        public string Description { get; set; }
    }

    public class Landmark
    {
        public string PlaceCategory { get; set; }
        public string PlaceName { get; set; }
        public Distance DistanceFromProperty { get; set; }
        public string Description { get; set; }
    }

    public class Rule
    {
        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Description { get; set; }
    }

    public class Media
    {
        [JsonProperty(Required = Required.Always)]
        public string MediaId { get; set; }

        public string FileCategory { get; set; }
        public string FileType { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public string MediaPosition { get; set; }
        public Date From { get; set; }
        public Date To { get; set; }

        //New field Added
        public string MediaWidth { get; set; }
        public string MediaHeight { get; set; }
        public string ThumbnailUrl { get; set; }
        public string LargeImageURL { get; set; }
        public string MediaFileMaster { get; set; }
        public string SmallImageURL { get; set; }
        public string MediaFileFormat { get; set; }
        //New field Added


    }

    public class Updates
    {
        public string UpdateCategory { get; set; }
        public DescriptionType DescriptionType { get; set; }
        public string Description { get; set; }
        public Display Display { get; set; }
        public string Source { get; set; }
        public bool SendUpdates { get; set; }
        public ModeOfCommunication ModeOfCommunication { get; set; }
        public bool DisplayUpdatesOnVoucher { get; set; }
    }

    public class DescriptionType
    {
        public bool Internal { get; set; }
        public bool External { get; set; }
    }

    public class Display
    {
        public Date From { get; set; }
        public Date To { get; set; }
    }

    public class ModeOfCommunication
    {
        public bool Email { get; set; }
        public bool Sms { get; set; }
    }

    public class SafetyRegulations
    {
        public string Category { get; set; }
        public string Name { get; set; }
        public string Remarks { get; set; }
        public string Value { get; set; }
        public Date LastUpdated { get; set; }
    }

    public class Ancillary
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public Date From { get; set; }
        public Date To { get; set; }
    }

    public class ProductStatus
    {
        public List<Deactivated> Deactivated { get; set; }
        public string Status { get; set; }
        public Date From { get; set; }
        public Date To { get; set; }
        public string SubmitFor { get; set; }
        public string Reason { get; set; }
        public string Remark { get; set; }

    }

    public class Deactivated
    {
        public string MarketId { get; set; }
        public string MarketName { get; set; }
        public Date From { get; set; }
        public Date To { get; set; }
        public string Reason { get; set; }
    }

    //New field Added

    public class Policies
    {
        public string Type { get; set; }
        public string Description { get; set; }
    }

    public class Rooms
    {
        public string RoomTypeId { get; set; }
        public string RoomTypeName { get; set; }
        public string RoomCategoryCode { get; set; }
        public string RoomCategoryName { get; set; }
        public string RoomCode { get; set; }
        public string CompanyRoomCategory { get; set; }
        public string RoomDescription { get; set; }
        public int BathRoomCount { get; set; }
        public string BathRoomType { get; set; }
        public string Size { get; set; }
        public string View { get; set; }
        public string RoomRanking { get; set; }
        public bool IsRoomBookable { get; set; }
        public string MaxPrice { get; set; }
        public string MinPrice { get; set; }
        public string AmenitiesType { get; set; }
        public PassengerOccupancy PassengerOccupancy { get; set; }
        public List<RoomAmenities> RoomAmenities { get; set; }
        public List<BedRooms> BedRooms { get; set; }
        public List<Media> RoomMedia { get; set; }


    }


    public class BedRooms
    {
        public string BedTypeID { get; set; }
        public string BedType { get; set; }
        public string BeddingConfiguration { get; set; }
        
        public string BedRoomCount { get; set; }
        public string MaxAdultWithExtraBed { get; set; }
        public string MaxChildWithExtraBed { get; set; }
        public string NoOfExreaBeds { get; set; }

    }

    public class RoomAmenities
    {
        public string AmenityId { get; set; }
        public string RoomAmenityType { get; set; }
        public string AmenityCategoryID { get; set; }
        public string AmenityCategory { get; set; }
    }

}
