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
    
    public partial class Accommodation_Status
    {
        public System.Guid Accommodation_Status_Id { get; set; }
        public Nullable<System.Guid> Accommodation_Id { get; set; }
        public Nullable<System.DateTime> From { get; set; }
        public Nullable<System.DateTime> To { get; set; }
        public string DeactivationReason { get; set; }
        public string Status { get; set; }
        public string CompanyMarket { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public string Create_User { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public string Edit_User { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
    }
}
