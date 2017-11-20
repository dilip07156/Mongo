using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts.Masters
{
    [DataContract(Namespace = "State")]
    [BsonIgnoreExtraElements]
    public class DC_State
    {
        public string StateName { get; set; }

        [DataMember]
        public string StateCode { get; set; }

        [DataMember]
        public string CountryName { get; set; }

        [DataMember]
        public string CountryCode { get; set; }
    }
}
