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
    
    public partial class Activity_Tips
    {
        public System.Guid Activity_Tips_Id { get; set; }
        public Nullable<System.Guid> Activity_Id { get; set; }
        public Nullable<int> Legacy_Product_Id { get; set; }
        public string TipsType { get; set; }
        public string TipsName { get; set; }
        public string TipsDescription { get; set; }
        public Nullable<System.DateTime> Create_Date { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
        public string Create_User { get; set; }
        public string Edit_User { get; set; }
    }
}
