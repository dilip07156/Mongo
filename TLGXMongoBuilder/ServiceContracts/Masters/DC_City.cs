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
    [DataContract(Namespace = "City")]
    [BsonIgnoreExtraElements]
    public class DC_City
    {
        [DataMember]
        public string CityName { get; set; }

        [DataMember]
        public string CityCode { get; set; }

        [DataMember]
        public string StateName { get; set; }

        [DataMember]
        public string StateCode { get; set; }

        [DataMember]
        public string CountryName { get; set; }

        [DataMember]
        public string CountryCode { get; set; }
    }
}
