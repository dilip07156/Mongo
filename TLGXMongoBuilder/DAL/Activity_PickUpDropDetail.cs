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
    
    public partial class Activity_PickUpDropDetail
    {
        public System.Guid Activity_PickUpDropDetail_Id { get; set; }
        public Nullable<System.Guid> Activity_PickUpDrop_Id { get; set; }
        public Nullable<System.Guid> Activity_Id { get; set; }
        public Nullable<int> Legacy_Product_Id { get; set; }
        public string PickUpDropType { get; set; }
        public string FromToType { get; set; }
        public string LocationType { get; set; }
        public string LocationName { get; set; }
        public string AreaSearchFor { get; set; }
        public string AreaNameofPlace { get; set; }
        public Nullable<System.Guid> Accommodation_Id { get; set; }
        public string Acco_Name { get; set; }
        public string Acco_Address { get; set; }
        public string Acco_PostalCode { get; set; }
        public string Acco_Country { get; set; }
        public string Acco_City { get; set; }
        public string Acco_State { get; set; }
        public string Acco_Area { get; set; }
        public string Acco_Location { get; set; }
        public string Acco_Telephone { get; set; }
        public string Acco_Fax { get; set; }
        public string Acco_Website { get; set; }
        public string Acco_Email { get; set; }
        public string Acco_ContactNotes { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Create_User { get; set; }
        public string Edit_User { get; set; }
    }
}
