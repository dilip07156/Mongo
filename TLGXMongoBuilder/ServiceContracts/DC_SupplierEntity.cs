using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts
{
    [DataContract]
    public class DC_SupplierEntity
    {
        [DataMember]
        public System.Guid SupplierEntity_Id { get; set; }
        [DataMember]
        public string Parent_Id { get; set; }
        [DataMember]
        public System.Guid Supplier_Id { get; set; }
        [DataMember]
        public string SupplierName { get; set; }
        [DataMember]
        public string SupplierProductCode { get; set; }
        [DataMember]
        public string Entity { get; set; }
        [DataMember]
        public Nullable<System.DateTime> Create_Date { get; set; }
        [DataMember]
        public string Create_User { get; set; }
    }
}
