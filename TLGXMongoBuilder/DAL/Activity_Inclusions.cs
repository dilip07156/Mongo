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
    
    public partial class Activity_Inclusions
    {
        public System.Guid Activity_Inclusions_Id { get; set; }
        public Nullable<System.Guid> Activity_Id { get; set; }
        public Nullable<int> Legacy_Product_Id { get; set; }
        public Nullable<System.Guid> Activity_Flavour_Id { get; set; }
        public Nullable<bool> IsInclusion { get; set; }
        public string InclusionFor { get; set; }
        public Nullable<bool> IsDriver { get; set; }
        public Nullable<bool> IsAudioCommentary { get; set; }
        public string InclusionDescription { get; set; }
        public string InclusionName { get; set; }
        public string RestaurantStyle { get; set; }
        public string InclusionType { get; set; }
        public Nullable<System.DateTime> InclusionFrom { get; set; }
        public Nullable<System.DateTime> InclusionTo { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Create_User { get; set; }
        public string Edit_User { get; set; }
    }
}