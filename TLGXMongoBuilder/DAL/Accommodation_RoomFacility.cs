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
    
    public partial class Accommodation_RoomFacility
    {
        public System.Guid Accommodation_RoomFacility_Id { get; set; }
        public Nullable<System.Guid> Accommodation_Id { get; set; }
        public Nullable<System.Guid> Accommodation_RoomInfo_Id { get; set; }
        public string AmenityType { get; set; }
        public string AmenityName { get; set; }
        public string Description { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public string Create_User { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Edit_user { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}
