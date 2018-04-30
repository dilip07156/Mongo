using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts
{
    [DataContract]
    public class Dc_SupplierEntitySingleValue
    {
        [DataMember]
        public string SupplierName { get; set; }
        [DataMember]
        public string SupplierProductCode { get; set; }
        [DataMember]
        public string SupplierValue { get; set; }
    }
}
