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
    
    public partial class DistributionLayerRefresh_Log
    {
        public System.Guid Id { get; set; }
        public string Element { get; set; }
        public string Type { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public string Create_User { get; set; }
        public string Status { get; set; }
        public Nullable<System.Guid> Supplier_Id { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Edit_User { get; set; }
        public Nullable<int> TotalCount { get; set; }
        public Nullable<int> MongoPushCount { get; set; }
    }
}
