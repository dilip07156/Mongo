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
    
    public partial class Activity_Prices
    {
        public System.Guid Activity_Prices_Id { get; set; }
        public Nullable<System.Guid> Activity_Flavour_Id { get; set; }
        public Nullable<System.Guid> Activity_Id { get; set; }
        public string PriceCode { get; set; }
        public string PriceBasis { get; set; }
        public Nullable<decimal> Price { get; set; }
        public string PriceCurrency { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public string Create_User { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Edit_User { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Price_For { get; set; }
        public string Price_Type { get; set; }
        public string Price_OptionCode { get; set; }
        public Nullable<System.Guid> Activity_FlavourOptions_Id { get; set; }
        public Nullable<System.DateTime> Price_ValidFrom { get; set; }
        public Nullable<System.DateTime> Price_ValidTo { get; set; }
        public string Market { get; set; }
        public string FromPax { get; set; }
        public string ToPax { get; set; }
        public string PersonType { get; set; }
        public string AgeFrom { get; set; }
        public string AgeTo { get; set; }
    }
}
