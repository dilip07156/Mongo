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
    
    public partial class Activity_SupplierProductMapping_CA
    {
        public System.Guid Activity_SupplierProductMapping_CA_Id { get; set; }
        public Nullable<System.Guid> Supplier_ID { get; set; }
        public string SupplierCode { get; set; }
        public string SupplierName { get; set; }
        public string SuplierProductCode { get; set; }
        public string SupplierProductName { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Create_User { get; set; }
        public string Edit_User { get; set; }
        public string AttributeType { get; set; }
        public string AttributeSubType { get; set; }
        public string AttributeValue { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
