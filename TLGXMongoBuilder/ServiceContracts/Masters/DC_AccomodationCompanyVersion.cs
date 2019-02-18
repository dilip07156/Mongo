using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts.Masters
{

    [DataContract]
    [BsonIgnoreExtraElements]
    public class DC_AccomodationCompanyVersions
    {
        //[DataMember]
        //public System.Guid Accommodation_CompanyVersion_Id { get; set; }

        //[DataMember]
        //public Nullable<System.Guid> Accommodation_Id { get; set; }

        [DataMember]
        public string CommonProductId { get; set; }

        [DataMember]
        public string CompanyProductId { get; set; }

        [DataMember]
        public string CompanyId { get; set; }

        [DataMember]
        public string CompanyName { get; set; }

        [DataMember]
        public string ProductName { get; set; }

        [DataMember]
        public string ProductDisplayName { get; set; }

        [DataMember]
        public string StarRating { get; set; }

        [DataMember]
        public string CompanyRating { get; set; }

        [DataMember]
        public string ProductCatSubType { get; set; }

        [DataMember]
        public string Brand { get; set; }

        [DataMember]
        public string Chain { get; set; }

        [DataMember]
        public string HouseNumber { get; set; }

        [DataMember]
        public string Street { get; set; }

        [DataMember]
        public string Street2 { get; set; }

        [DataMember]
        public string Street3 { get; set; }

        [DataMember]
        public string Street4 { get; set; }

        [DataMember]
        public string Street5 { get; set; }

        [DataMember]
        public string Zone { get; set; }

        [DataMember]
        public string PostalCode { get; set; }

        [DataMember]
        public string Country { get; set; }

        [DataMember]
        public string State { get; set; }

        [DataMember]
        public string City { get; set; }

        [DataMember]
        public string Area { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string Latitude { get; set; }

        [DataMember]
        public string Longitude { get; set; }

        [DataMember]
        public string TLGXAccoId { get; set; }

        [DataMember]
        public string Interest { get; set; }
        
    }
}
