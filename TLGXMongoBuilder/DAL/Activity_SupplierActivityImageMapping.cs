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
    
    public partial class Activity_SupplierActivityImageMapping
    {
        public System.Guid Activity_SupplierActivityTypeImageMapping_Id { get; set; }
        public System.Guid ActivitySupplierProductMapping_Id { get; set; }
        public Nullable<System.Guid> Supplier_ID { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SuplierProductCode { get; set; }
        public string SupplierProductName { get; set; }
        public string MediaId { get; set; }
        public string FullURL { get; set; }
        public string ThumbnailURL { get; set; }
        public Nullable<int> SortOrder { get; set; }
        public string MediaDescription { get; set; }
        public string MediaDimension { get; set; }
        public string MediaCaption { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Create_User { get; set; }
        public string Edit_User { get; set; }
        public Nullable<int> MapId { get; set; }
        public string MediaType { get; set; }
    }
}
