using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace DataContracts.Masters
{
    [DataContract]
    [BsonIgnoreExtraElements]
    public class DC_Accomodation
    {
        [BsonId]
        [DataMember]
        public int CommonHotelId { get; set; }

        [DataMember]
        public string HotelName { get; set; }

        [DataMember]
        public string ProductCategory { get; set; }

        [DataMember]
        public string ProductCategorySubType { get; set; }


        [DataMember]
        public string CountryCode { get; set; }

        [DataMember]
        public string CountryName { get; set; }

        [DataMember]
        public string CityCode { get; set; }

        [DataMember]
        public string CityName { get; set; }

        [DataMember]
        public string StateCode { get; set; }

        [DataMember]
        public string StateName { get; set; }

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
        public string SuburbDowntown { get; set; }

        [DataMember]
        public string Location { get; set; }

        [DataMember]
        public string Area { get; set; }

        [DataMember]
        public string TLGXAccoId { get; set; }

        [DataMember]
        public string Brand { get; set; }

        [DataMember]
        public string Chain { get; set; }


        [DataMember]
        public bool IsRoomMappingCompleted { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string Fax { get; set; }

        [DataMember]
        public string WebSiteURL { get; set; }

        [DataMember]
        public string Telephone { get; set; }

        [DataMember]
        public string FullAddress { get; set; }

        [DataMember]
        public string HotelStarRating { get; set; }

        [DataMember]
        public string Latitude { get; set; }

        [DataMember]
        public string Longitude { get; set; }

        [DataMember]
        public string CodeStatus { get; set; }
    }
}
