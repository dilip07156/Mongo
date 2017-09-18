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
    
    public partial class Activity_SupplierProductMapping
    {
        public System.Guid ActivitySupplierProductMapping_Id { get; set; }
        public Nullable<System.Guid> Activity_ID { get; set; }
        public System.Guid Supplier_ID { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SuplierProductCode { get; set; }
        public string SupplierProductType { get; set; }
        public string SupplierType { get; set; }
        public string SupplierLocationId { get; set; }
        public string SupplierLocationName { get; set; }
        public string SupplierCountryName { get; set; }
        public string SupplierCityName { get; set; }
        public string SupplierCountryCode { get; set; }
        public string SupplierCityCode { get; set; }
        public string SupplierStateName { get; set; }
        public string SupplierStateCode { get; set; }
        public string SupplierCityIATACode { get; set; }
        public string Duration { get; set; }
        public string SupplierProductName { get; set; }
        public string SupplierDataLangaugeCode { get; set; }
        public string Introduction { get; set; }
        public string Conditions { get; set; }
        public string Inclusions { get; set; }
        public string Exclusions { get; set; }
        public string AdditionalInformation { get; set; }
        public string DeparturePoint { get; set; }
        public string TicketingDetails { get; set; }
        public string Currency { get; set; }
        public string DepartureTime { get; set; }
        public string DepartureDate { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string BlockOutDateFrom { get; set; }
        public string BlockOutDateTo { get; set; }
        public string OptionTitle { get; set; }
        public string OptionCode { get; set; }
        public string OptionDescription { get; set; }
        public string TourActivityLangauageCode { get; set; }
        public string ProductDescription { get; set; }
        public string TourActivityLanguage { get; set; }
        public string ImgURL { get; set; }
        public string ProductValidFor { get; set; }
        public string Address { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string DayPattern { get; set; }
        public string Theme { get; set; }
        public string Distance { get; set; }
        public string SupplierTourType { get; set; }
        public string MappingStatus { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Create_User { get; set; }
        public string Edit_User { get; set; }
        public Nullable<int> MapID { get; set; }
        public Nullable<decimal> Price { get; set; }
        public Nullable<int> TotalActivities { get; set; }
        public string PhysicalIntensity { get; set; }
        public string PassengerNumbers { get; set; }
        public string DurationLength { get; set; }
        public string Timing { get; set; }
        public string Session { get; set; }
        public string Specials { get; set; }
    }
}
