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
    
    public partial class Activity_InclusionDetails
    {
        public System.Guid Activity_InclusionDetails_Id { get; set; }
        public Nullable<System.Guid> Activity_Inclusions_Id { get; set; }
        public Nullable<System.Guid> Activity_Id { get; set; }
        public Nullable<int> Legacy_Product_Id { get; set; }
        public Nullable<System.Guid> Activity_Flavour_Id { get; set; }
        public string InclusionDetailFor { get; set; }
        public string GuideLanguage { get; set; }
        public string GuideLanguageCode { get; set; }
        public string InclusionDetailType { get; set; }
        public string InclusionDetailName { get; set; }
        public string InclusionDetailDescription { get; set; }
        public Nullable<System.DateTime> InclusionDetailFrom { get; set; }
        public Nullable<System.DateTime> InclusionDetailTo { get; set; }
        public string DaysofWeek { get; set; }
        public string FromTime { get; set; }
        public string ToTime { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Create_User { get; set; }
        public string Edit_User { get; set; }
    }
}
