using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts.Masters
{
    [DataContract(Namespace = "Port")]
    [BsonIgnoreExtraElements]
    public class DC_Port
    {
        [DataMember]
        public string CountryName { get; set; }

        [DataMember]
        public string CountryCode { get; set; }

        [DataMember]
        public string CityName { get; set; }

        [DataMember]
        public string CityCode { get; set; }

        [DataMember]
        public string PortName { get; set; }

        [DataMember]
        public string PortCode  { get; set; }

        [DataMember]
        public string Latitude { get; set; }

        [DataMember]
        public string Longitude { get; set; }
    }
}
