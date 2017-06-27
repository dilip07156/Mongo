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
    
    public partial class Activity
    {
        public System.Guid Acivity_Id { get; set; }
        public Nullable<int> CommonProductID { get; set; }
        public Nullable<int> Legacy_Product_ID { get; set; }
        public string Product_Name { get; set; }
        public string Display_Name { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string ProductType { get; set; }
        public string ProductSubType { get; set; }
        public string ProductCategory { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Create_User { get; set; }
        public string Edit_User { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string StartingPoint { get; set; }
        public string EndingPoint { get; set; }
        public string Duration { get; set; }
        public Nullable<bool> CompanyRecommended { get; set; }
        public Nullable<double> Latitude { get; set; }
        public Nullable<double> Longitude { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public Nullable<bool> MealsYN { get; set; }
        public Nullable<bool> GuideYN { get; set; }
        public string TransferYN { get; set; }
        public string PhysicalLevel { get; set; }
        public string Advisory { get; set; }
        public string ThingsToCarry { get; set; }
        public string DeparturePoint { get; set; }
        public string TourType { get; set; }
        public Nullable<int> Parent_Legacy_Id { get; set; }
    }
}
