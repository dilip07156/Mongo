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
    
    public partial class ZoneProduct_Mapping
    {
        public System.Guid ZoneProductMapping_Id { get; set; }
        public Nullable<System.Guid> Zone_Id { get; set; }
        public Nullable<System.Guid> Product_Id { get; set; }
        public string ProductType { get; set; }
        public string Unit { get; set; }
        public Nullable<decimal> Distance { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public Nullable<bool> Included { get; set; }
        public string Create_User { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public string Edit_User { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
    }
}
