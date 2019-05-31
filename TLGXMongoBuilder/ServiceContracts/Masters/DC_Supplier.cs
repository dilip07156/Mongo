using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;
using DataContracts.Mapping;

namespace DataContracts.Masters
{
    [DataContract(Namespace = "Supplier")]
    [BsonIgnoreExtraElements]
    public class DC_Supplier
    {
        [DataMember]
        public string SupplierName { get; set; }

        [DataMember]
        public string SupplierCode { get; set; }

        [DataMember]
        public string SupplierOwner { get; set; }

        [DataMember]
        public string SupplierType { get; set; }


        [DataMember]
        public DC_SupplierMCONS MCON { get; set; }
    }

    public class DC_Supplier_ShortVersion
    {
        public Guid Supplier_Id { get; set; }
        public string SupplierName { get; set; }
        public string SupplierCode { get; set; }
    }
}
