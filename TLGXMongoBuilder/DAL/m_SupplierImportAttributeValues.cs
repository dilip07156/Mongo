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
    
    public partial class m_SupplierImportAttributeValues
    {
        public System.Guid SupplierImportAttributeValue_Id { get; set; }
        public System.Guid SupplierImportAttribute_Id { get; set; }
        public string AttributeType { get; set; }
        public string AttributeName { get; set; }
        public string AttributeValue { get; set; }
        public string STATUS { get; set; }
        public Nullable<System.DateTime> CREATE_DATE { get; set; }
        public string CREATE_USER { get; set; }
        public Nullable<System.DateTime> EDIT_DATE { get; set; }
        public string EDIT_USER { get; set; }
        public Nullable<int> Priority { get; set; }
        public string Description { get; set; }
        public Nullable<System.Guid> AttributeValue_ID { get; set; }
        public string AttributeValueType { get; set; }
        public string Comparison { get; set; }
    }
}
