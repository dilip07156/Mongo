using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace DataContracts.Mapping
{
    [DataContract]
    [BsonIgnoreExtraElements]
    public class DC_AccomodationMasterMapping
    {
        [BsonId]
        [DataMember]
        public int CommonHotelId { get; set; }

        [DataMember]
        public string HotelName { get; set; }

        [DataMember]
        public string Country { get; set; }


        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string StreetName { get; set; }

        [DataMember]
        public string StreetNumber { get; set; }

        [DataMember]
        public string Street3 { get; set; }

        [DataMember]
        public string Street4 { get; set; }

        [DataMember]
        public string Street5 { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public string Town { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string Area { get; set; }

        [DataMember]
        public string TLGXAccoId { get; set; }


        [DataMember]
        public string ProductCategory { get; set; }


        [DataMember]
        public string ProductCategorySubType { get; set; }

        [DataMember]
        public bool IsRoomMappingCompleted { get; set; }
   
    }
}
