//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DAL
{
    using System;
    using System.Collections.Generic;
    
    public partial class Accommodation
    {
        public System.Guid Accommodation_Id { get; set; }
        public string CompanyName { get; set; }
        public int CompanyHotelID { get; set; }
        public Nullable<int> FinanceControlID { get; set; }
        public Nullable<System.DateTime> OnlineDate { get; set; }
        public Nullable<System.DateTime> OfflineDate { get; set; }
        public string HotelRating { get; set; }
        public string CompanyRating { get; set; }
        public string HotelName { get; set; }
        public string DisplayName { get; set; }
        public string ProductCategory { get; set; }
        public string ProductCategorySubType { get; set; }
        public Nullable<System.DateTime> RatingDate { get; set; }
        public string TotalFloors { get; set; }
        public string TotalRooms { get; set; }
        public string CheckInTime { get; set; }
        public string CheckOutTime { get; set; }
        public string InternalRemarks { get; set; }
        public Nullable<bool> CompanyRecommended { get; set; }
        public string RecommendedFor { get; set; }
        public string Hashtag { get; set; }
        public string Chain { get; set; }
        public string Brand { get; set; }
        public string Affiliation { get; set; }
        public string Reason { get; set; }
        public string Remarks { get; set; }
        public string CarbonFootPrint { get; set; }
        public string YearBuilt { get; set; }
        public string AwardsReceived { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<bool> IsMysteryProduct { get; set; }
        public string Create_User { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public string Edit_User { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string StreetName { get; set; }
        public string StreetNumber { get; set; }
        public string Street3 { get; set; }
        public string Street4 { get; set; }
        public string Street5 { get; set; }
        public string PostalCode { get; set; }
        public string Town { get; set; }
        public string Location { get; set; }
        public string Area { get; set; }
        public string city { get; set; }
        public string country { get; set; }
        public string SuburbDowntown { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string LEGACY_COUNTRY { get; set; }
        public string LEGACY_CITY { get; set; }
        public string Country_ISO { get; set; }
        public string City_ISO { get; set; }
        public string State_Name { get; set; }
        public string State_ISO { get; set; }
        public string LEGACY_STATE { get; set; }
        public Nullable<int> Legacy_HTL_ID { get; set; }
        public string Google_Place_Id { get; set; }
        public string FullAddress { get; set; }
        public Nullable<System.Guid> Country_Id { get; set; }
        public Nullable<System.Guid> City_Id { get; set; }
        public Nullable<bool> InsertFrom { get; set; }
        public string Address_Tx { get; set; }
        public string Telephone_Tx { get; set; }
        public string HotelName_Tx { get; set; }
        public string Latitude_Tx { get; set; }
        public string Longitude_Tx { get; set; }
        public System.Data.Entity.Spatial.DbGeography GeoLocation { get; set; }
        public string gArea { get; set; }
        public string gLocation { get; set; }
        public Nullable<System.Guid> Area_Id { get; set; }
        public Nullable<System.Guid> Location_Id { get; set; }
        public string TLGXAccoId { get; set; }
        public Nullable<int> Priority { get; set; }
    }
}
