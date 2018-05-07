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
        public string oag_location_code { get; set; }

        [DataMember]
        public string oag_multi_airport_citycode { get; set; }

        [DataMember]
        public string oag_location_type_code { get; set; }

        [DataMember]
        public string oag_location_type { get; set; }

        [DataMember]
        public string oag_location_subtype_code { get; set; }

        [DataMember]
        public string oag_location_subtype { get; set; }

        [DataMember]
        public string oag_location_name { get; set; }

        [DataMember]
        public string oag_port_name  { get; set; }

        [DataMember]
        public string oag_country_code { get; set; }

        [DataMember]
        public string oag_country_subcode { get; set; }

        [DataMember]
        public string oag_country_name { get; set; }

        [DataMember]
        public string oag_state_code { get; set; }

        [DataMember]
        public string oag_state_subcode { get; set; }

        [DataMember]
        public string oag_time_division { get; set; }

        [DataMember]
        public string oag_latitiude { get; set; }

        [DataMember]
        public string oag_longitude { get; set; }

        [DataMember]
        public string oag_inactive_indicator { get; set; }

        [DataMember]
        public string tlgx_country_code { get; set; }

        [DataMember]
        public string tlgx_country_name { get; set; }

        [DataMember]
        public string tlgx_state_code { get; set; }

        [DataMember]
        public string tlgx_state_name { get; set; }

        [DataMember]
        public string tlgx_city_code { get; set; }

        [DataMember]
        public string tlgx_city_name { get; set; }

    }
}
