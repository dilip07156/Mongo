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
    
    public partial class m_CountryMapping
    {
        public System.Guid CountryMapping_Id { get; set; }
        public Nullable<System.Guid> Country_Id { get; set; }
        public Nullable<System.Guid> Supplier_Id { get; set; }
        public string SupplierName { get; set; }
        public string CountryName { get; set; }
        public string CountryCode { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public string Create_User { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Edit_User { get; set; }
        public Nullable<int> MapID { get; set; }
        public string Remarks { get; set; }
        public string actionType { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string ContinentCode { get; set; }
        public string ContinentName { get; set; }
        public Nullable<System.Guid> stg_Country_Id { get; set; }
        public Nullable<int> MatchedBy { get; set; }
    }
}
