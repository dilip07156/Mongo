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
    
    public partial class Activity_Ancillary
    {
        public System.Guid Activity_Ancillary_Id { get; set; }
        public string Ancillary_Type { get; set; }
        public string Ancillary_Name { get; set; }
        public string Ancillary_Description { get; set; }
        public string Ancillary_Status { get; set; }
        public Nullable<System.DateTime> Ancillary_FromDate { get; set; }
        public Nullable<System.DateTime> Ancillary_ToDate { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Create_User { get; set; }
        public string Edit_User { get; set; }
    }
}
