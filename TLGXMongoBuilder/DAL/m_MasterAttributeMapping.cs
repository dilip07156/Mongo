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
    
    public partial class m_MasterAttributeMapping
    {
        public System.Guid MasterAttributeMapping_Id { get; set; }
        public System.Guid Supplier_Id { get; set; }
        public System.Guid SystemMasterAttribute_Id { get; set; }
        public string SupplierMasterAttribute { get; set; }
        public string Status { get; set; }
        public bool IsActive { get; set; }
        public string Create_User { get; set; }
        public System.DateTime Create_Date { get; set; }
        public string Edit_User { get; set; }
        public Nullable<System.DateTime> Edit_Date { get; set; }
    }
}
