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
        public string Id { get; set; }
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
        [DataMember]
        public List<DC_Zone_Geography> Zone_GeographyMapping { get; set; }
        [DataMember]
        public List<DC_Zone_LocationMapping> Zone_LocationMapping { get; set; }
        [DataMember]
        public DC_Zone_Geometry loc { get; set; }
        [DataMember]
        public string Zone_Code { get; set; }
        [DataMember]
        public string Zone_House_Number { get; set; }
        [DataMember]
        public string Zone_Street_One { get; set; }
        [DataMember]
        public string Zone_Street_Two { get; set; }
        [DataMember]
        public string Zone_Street_Three { get; set; }
        [DataMember]
        public string Zone_City { get; set; }
        [DataMember]
        public string Zone_City_Area { get; set; }
        [DataMember]
        public string Zone_City_Area_Location { get; set; }
        [DataMember]
        public string Zone_Postal_Code { get; set; }
        [DataMember]
        public string Zone_Full_Adress { get; set; }

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

    [DataContract]
    public class DC_Zone_Geography
    {        
        [DataMember]        
        public string TLGXCityCode { get; set; }
        [DataMember]
        public string TLGXCountryCode { get; set; }
        [DataMember]
        public string TLGXStateCode { get; set; }
        [DataMember]
        public string TLGXCityAreaCode { get; set; }
        [DataMember]
        public string TLGXCityAreaLocationCode { get; set; }
        
    }

    [DataContract]
    public class DC_Zone_Geometry
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public List<double> coordinates { get; set; }
    }

    [DataContract]
    public class DC_Zone_LocationMapping
    {
       
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string ZoneType { get; set; }
        [DataMember]
        public string ZoneSubType { get; set; }
        [DataMember]
        public string HouseNumber { get; set; }
        [DataMember]
        public string StreetName { get; set; }
        [DataMember]
        public string Street2 { get; set; }
        [DataMember]
        public string Street3 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string CityArea { get; set; }
        [DataMember]
        public string CityAreaLocation { get; set; }
        [DataMember]
        public string StateCode { get; set; }
        [DataMember]
        public string StateName { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string PostalCode { get; set; }
        [DataMember]
        public string FullAdress { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public double Distance { get; set; }
        [DataMember]
        public string Supplier_Name { get; set; }
        [DataMember]
        public string Supplier_code { get; set; }

    }

    [DataContract]
    public class DC_Zone_Supplier
    {

        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string ZoneType { get; set; }
        [DataMember]
        public string ZoneSubType { get; set; }
        [DataMember]
        public string HouseNumber { get; set; }
        [DataMember]
        public string StreetName { get; set; }
        [DataMember]
        public string Street2 { get; set; }
        [DataMember]
        public string Street3 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string CityArea { get; set; }
        [DataMember]
        public string CityAreaLocation { get; set; }
        [DataMember]
        public string StateCode { get; set; }
        [DataMember]
        public string StateName { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string PostalCode { get; set; }
        [DataMember]
        public string FullAdress { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }        
        [DataMember]
        public string Supplier_Name { get; set; }
        [DataMember]
        public string Supplier_code { get; set; }

    }



    [DataContract]
    public class DC_Zone_MasterRQ
    {
        [DataMember]
        public string Id { get; set; }
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
        public string Zone_Code { get; set; }
        [DataMember]
        public string Zone_House_Number { get; set; }
        [DataMember]
        public string Zone_Street_One { get; set; }
        [DataMember]
        public string Zone_Street_Two { get; set; }
        [DataMember]
        public string Zone_Street_Three { get; set; }
        [DataMember]
        public string Zone_City { get; set; }
        [DataMember]
        public string Zone_City_Area { get; set; }
        [DataMember]
        public string Zone_City_Area_Location { get; set; }
        [DataMember]
        public string Zone_Postal_Code { get; set; }
        [DataMember]
        public string Zone_Full_Adress { get; set; }
        [DataMember]
        public List<DC_Zone_GeographyRQ> Zone_GeographyMapping { get; set; }
        [DataMember]
        public List<DC_Zone_ProductMappingRQ> Zone_ProductMapping { get; set; }
        [DataMember]
        public List<DC_Zone_CityMappingRQ> Zone_CityMapping { get; set; }
        [DataMember]
        public List<DC_Zone_GeometryRQ> geometry { get; set; }
        [DataMember]
        public List<DC_Zone_LocationMappingRQ> ZoneLocationMapping { get; set; }

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

    [DataContract]
    public class DC_Zone_GeographyRQ
    {
        [DataMember]
        public string TLGXCityCode { get; set; }
        [DataMember]
        public string TLGXCountryCode { get; set; }
        [DataMember]
        public string TLGXStateCode { get; set; }
        [DataMember]
        public string TLGXCityAreaCode { get; set; }
        [DataMember]
        public string TLGXCityAreaLocationCode { get; set; }
        [DataMember]
        public Guid Zone_id { get; set; }
    }

    [DataContract]
    public class DC_Zone_GeometryRQ
    {
        [DataMember]
        public string type { get; set; }
        [DataMember]
        public DC_Zone_CoordinateRQ coordinates { get; set; }

    }

    [DataContract]
    public class DC_Zone_CoordinateRQ
    {
        [DataMember]
        public string Longitude { get; set; }
        [DataMember]
        public string Latitude { get; set; }
        [DataMember]
        public Guid Zone_id { get; set; }
    }

    [DataContract]
    public class DC_ZoneTypeMaster
    {
        [DataMember]
        public string Zone_Type { get; set; }
        [DataMember]
        public List<string> Zone_SubType { get; set; }

    }

    [DataContract]
    public class DC_Zone_LocationMappingRQ
    {
       
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string Code { get; set; }
        [DataMember]
        public string ZoneType { get; set; }
        [DataMember]
        public string ZoneSubType { get; set; }
        [DataMember]
        public string HouseNumber { get; set; }
        [DataMember]
        public string StreetName { get; set; }
        [DataMember]
        public string Street2 { get; set; }
        [DataMember]
        public string Street3 { get; set; }
        [DataMember]
        public string City { get; set; }
        [DataMember]
        public string CityArea { get; set; }
        [DataMember]
        public string CityAreaLocation { get; set; }
        [DataMember]
        public string StateCode { get; set; }
        [DataMember]
        public string StateName { get; set; }
        [DataMember]
        public string CountryCode { get; set; }
        [DataMember]
        public string PostalCode { get; set; }
        [DataMember]
        public string FullAdress { get; set; }
        [DataMember]
        public double Latitude { get; set; }
        [DataMember]
        public double Longitude { get; set; }
        [DataMember]
        public double Distance { get; set; }
        [DataMember]
        public string Supplier_Name { get; set; }
        [DataMember]
        public string Supplier_code { get; set; }
        [DataMember]
        public Guid Zone_id { get; set; }

    }

}
