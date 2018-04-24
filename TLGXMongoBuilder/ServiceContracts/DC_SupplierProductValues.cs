using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts
{
    [DataContract]
    public class DC_SupplierProductValues
    {
        //[DataMember]
        //public System.Guid SupplierEntity_Id { get; set; }

        [DataMember]
        public string SupplierProperty { get; set; }

        [DataMember]
        public string Value { get; set; }

        [DataMember]
        public Nullable<System.Guid> AttributeMap_Id { get; set; }

        [DataMember]
        public string SystemAttribute { get; set; }

        
    }
}
