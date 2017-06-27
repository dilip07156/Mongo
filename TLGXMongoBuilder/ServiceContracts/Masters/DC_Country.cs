using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace DataContracts.Masters
{

    [DataContract(Namespace = "Country")]
    [BsonIgnoreExtraElements]
    public class DC_Country
    {
        [DataMember]
        public string CountryName { get; set; }

        [DataMember]
        public string CountryCode { get; set; }
    }
}
