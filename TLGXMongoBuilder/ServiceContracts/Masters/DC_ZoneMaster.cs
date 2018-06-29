using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;

namespace DataContracts.Masters
{
    /// <summary>
    /// get all Zones
    /// </summary>
    /// 
    [DataContract]
    [BsonIgnoreExtraElements]
    public class DC_Zone_Master
    {
        [DataMember]
        public string Zone_Type { get; set; }
        [DataMember]
        public string Zone_SubType { get; set; }
        [DataMember]
        public string Zone_Name { get; set; }
        [DataMember]
        public string Latitude { get; set; }
        [DataMember]
        public string Longitude { get; set; }
        [DataMember]
        public decimal? Zone_Radius { get; set; }
        [DataMember]
        public string TLGXCountryCode { get; set; }
        [DataMember]
        public List<DC_Zone_CityMapping> Zone_CityMapping { get; set; }
        [DataMember]
        public List<DC_Zone_ProductMapping> Zone_ProductMapping { get; set; }

    }
    [DataContract]
    public class DC_Zone_ProductMapping
    {
        [DataMember]
        public int? TLGXCompanyHotelID { get; set; }
        [DataMember]
        public string TLGXHotelName { get; set; }
        [DataMember]
        public string TLGXProductType { get; set; }
        [DataMember]
        public decimal? Distance { get; set; }
        [DataMember]
        public string Unit { get; set; }
        [DataMember]
        public bool? IsIncluded { get; set; }
    }
    [DataContract]
    public class DC_Zone_CityMapping
    {
        [DataMember]
        public string TLGXCityCode { get; set; }
    }


    public class DC_Zone_MasterRQ
    {
        [DataMember]
        public Guid Zone_id { get; set; }
        [DataMember]
        public string Zone_Type { get; set; }
        [DataMember]
        public string Zone_SubType { get; set; }
        [DataMember]
        public string Zone_Name { get; set; }
        [DataMember]
        public string Latitude { get; set; }
        [DataMember]
        public string Longitude { get; set; }
        [DataMember]
        public decimal? Zone_Radius { get; set; }
        [DataMember]
        public string TLGXCountryCode { get; set; }
        [DataMember]
        public List<DC_Zone_ProductMappingRQ> Zone_ProductMapping { get; set; }
        [DataMember]
        public List<DC_Zone_CityMappingRQ> Zone_CityMapping { get; set; }

    }
    [DataContract]
    public class DC_Zone_ProductMappingRQ
    {
        [DataMember]
        public Guid Zone_id { get; set; }
        [DataMember]
        public int? TLGXCompanyHotelID { get; set; }
        [DataMember]
        public string TLGXProductType { get; set; }
        [DataMember]
        public string TLGXHotelName { get; set; }
        [DataMember]
        public string Unit { get; set; }
        [DataMember]
        public decimal? Distance { get; set; }
        [DataMember]
        public bool? IsIncluded { get; set; }
    }
    [DataContract]
    public class DC_Zone_CityMappingRQ
    {
        [DataMember]
        public string TLGXCityCode { get; set; }
        [DataMember]
        public Guid Zone_id { get; set; }
    }

}
