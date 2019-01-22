using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DataContracts.Visa
{
    /// <summary>
    /// This is the request format for Country-based Visas Mapping Searches. It is a paged request/response service.
    /// </summary>

    public class InformationLink
    {
        public string href { get; set; }
        public string content { get; set; }
        public string target { get; set; }
    }

    public class Information
    {
        public List<InformationLink> InformationLink { get; set; }
        public List<string> content { get; set; }
    }

    public class VisaInformationNode
    {
        public Information Information { get; set; }
    }

    public class VisaGeneralInformation
    {
        public string GeneralInfo { get; set; }
    }

    public class VisaInfo
    {
        public List<VisaInformationNode> VisaInformation { get; set; }
        public List<VisaGeneralInformation> VisaGeneralInformation { get; set; }
    }

    public class VisaInformationChildNode
    {
        public string ProcessingTime { get; set; }
        public string VisaProcedure { get; set; }
        public string DocumentsRequired { get; set; }
        public string content { get; set; }
    }

    public class VisaCategoryInfo
    {
        public List<VisaInformationChildNode> Information { get; set; }
    }

    public class Requirements
    {
        public string Line { get; set; }
    }

    public class VisaCategoryRequirements
    {
        public Requirements Requirements { get; set; }
    }

    public class CategoryNotes
    {
        public List<string> Notes { get; set; }
    }

    public class VisaCategoryDetail
    {
        public string CategoryCode { get; set; }
        public List<VisaCategoryInfo> CategoryInfo { get; set; }
        public List<VisaCategoryRequirements> CategoryRequirements { get; set; }
        public string Category { get; set; }
        public CategoryNotes CategoryNotes { get; set; }
    }

    public class VisaCategoryFee
    {
        public string CategoryCode { get; set; }
        public string Category { get; set; }
        public string CategoryFeeAmountINR { get; set; }
        public string CategoryFeeAmountOther { get; set; }
    }

    public class VisaCategoryFees
    {
        public List<List<VisaCategoryFee>> Category { get; set; }
    }

    public class VisaCategories
    {
        public List<VisaCategoryDetail> Category { get; set; }
    }

    public class VisaInformation
    {
        public string TerritoryCity { get; set; }
        public List<VisaInfo> VisaInfo { get; set; }
        public List<VisaCategories> Categories { get; set; }
        public List<VisaCategoryFees> CategoryFees { get; set; }
        public CategoryForms CategoryForms { get; set; }
    }

    public class CategoryForm
    {
        public List<string> CategoryCode { get; set; }
        public List<string> Form { get; set; }
        public List<string> FormPath { get; set; }
    }

    public class CategoryForms
    {
        public List<CategoryForm> CategoryForm { get; set; }
    }

    public class VisaWebsite
    {
    }

    public class VisaCountryOffice
    {
        public string CountryID { get; set; }
        public string VisaRequired { get; set; }
        public string WhereToApply { get; set; }
        public string Website { get; set; }
        public string City { get; set; }
        public string County { get; set; }
        public string Name { get; set; }
        public string PinCode { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
    }

    public class VisaCountryOffices
    {
        public List<VisaCountryOffice> CountryOffice { get; set; }
    }

    public class VisaSAARCInfo
    {
        public List<VisaCountryOffices> CountryOffices { get; set; }
    }

    public class VisaInformationLink
    {
        public string href { get; set; }
        public List<string> content { get; set; }
        public string target { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class ReciprocalVisaInfoChildNode
    {
        public List<VisaInformationLink> InformationLink { get; set; }
        public List<string> content { get; set; }
    }

    public class VisaDescription
    {
        public List<ReciprocalVisaInfoChildNode> ReciprocalVisaInfo { get; set; }
    }

    public class ReciprocalVisaInfo
    {
        public List<VisaDescription> Description { get; set; }
    }

    public class VisaDescriptionInnerNode
    {
        public string VisaInternationalAdvisory { get; set; }
    }

    public class VisaInternationalAdvisory
    {
        public List<VisaDescriptionInnerNode> Description { get; set; }
    }

    public class VisaDescriptionSubNode
    {
    }

    public class VisaHeading
    {
        public List<VisaDescriptionSubNode> Description { get; set; }
        public string content { get; set; }
    }

    public class VisaDescriptionNode
    {
        public List<VisaHeading> Heading { get; set; }
    }

    public class VisaIVSAdvisory
    {
        public List<VisaDescriptionNode> Description { get; set; }
    }

    public class VisaHelpAddress
    {
        public string Phone { get; set; }
        public string Country { get; set; }
        public string Website { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string URL { get; set; }
        public string Name { get; set; }
        public string PinCode { get; set; }
        public string Address { get; set; }
    }

    public class VisaIntlHelpAddress
    {
        public List<VisaHelpAddress> HelpAddress { get; set; }
    }

    public class VisaOffice
    {
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Country { get; set; }
        public string SystemCountryCode { get; set; }
        public string SystemCountryName { get; set; }
        public string Website { get; set; }
        public string City { get; set; }
        public string SystemCityCode { get; set; }
        public string SystemCityName { get; set; }
        public string Fax { get; set; }
        public string URL { get; set; }
        public string Name { get; set; }
        public string PinCode { get; set; }
    }

    public class VisaIndianEmbassy
    {
        public List<VisaOffice> Office { get; set; }
    }

    public class VisaClimate
    {
    }

    public class VisaGeneralInfo
    {
        //public Climate Climate { get; set; }
        public string SmallMap { get; set; }
        public string Languages { get; set; }
        public string Time { get; set; }
        public string Capital { get; set; }
        public string Flag { get; set; }
        public string Code { get; set; }
        public string Area { get; set; }
        public string Currency { get; set; }
        public string LargeMap { get; set; }
        public string Population { get; set; }
        public string WorldFactBook { get; set; }
        public string NationalDay { get; set; }
        public string Location { get; set; }
    }

    public class VisaMonth
    {
    }

    public class VisaDate
    {
    }

    public class VisaHoliday
    {
        public string Month { get; set; }
        public string HolidayName { get; set; }
        public string Year { get; set; }
        public string Date { get; set; }
    }

    public class VisaHolidays
    {
        public List<VisaHoliday> Holiday { get; set; }
    }

    public class VisaAirline
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class VisaAirlines
    {
        public List<VisaAirline> Airline { get; set; }
    }

    public class VisaCountryName
    {
        public string Name { get; set; }
    }

    public class VisaAirport
    {
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class VisaAirports
    {
        public List<VisaAirport> Airport { get; set; }
    }

    public class VisaCountryDetails
    {
        public List<VisaGeneralInfo> GeneralInfo { get; set; }
        public VisaHolidays Holidays { get; set; }
        public List<VisaAirlines> Airlines { get; set; }
        public List<VisaCountryName> CountryName { get; set; }
        public List<VisaAirports> Airports { get; set; }
    }

    public class VisaOfficeNode
    {
        public string Timings { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Telephone { get; set; }
        public string VisaTimings { get; set; }
        public string Website { get; set; }
        public string City { get; set; }
        public string Name { get; set; }
        public string PinCode { get; set; }
        public string Phone { get; set; }
        public string CollectionTimings { get; set; }
        public string Country { get; set; }
        public string PublicTimings { get; set; }
        public string Fax { get; set; }
        public string Notes { get; set; }
    }

    public class VisaOffices
    {
        public List<VisaOfficeNode> Office { get; set; }
    }

    public class VisaDiplomaticRepresentation
    {
        public List<VisaOffices> Offices { get; set; }
    }

    public class Visa
    {
        public string AdditionalInfo { get; set; }
        public List<VisaInformation> VisaInformation { get; set; }
        public List<VisaSAARCInfo> SAARCInfo { get; set; }
        public List<ReciprocalVisaInfo> ReciprocalVisaInfo { get; set; }
        public List<VisaInternationalAdvisory> InternationalAdvisory { get; set; }
        public List<VisaIVSAdvisory> IVSAdvisory { get; set; }
        public VisaIntlHelpAddress IntlHelpAddress { get; set; }
        public string CountryCode { get; set; }
        public List<VisaIndianEmbassy> IndianEmbassy { get; set; }
        public List<VisaCountryDetails> CountryDetails { get; set; }
        public List<VisaDiplomaticRepresentation> DiplomaticRepresentation { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class VisaDetail
    {
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public List<Visa> Visa { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class VisaDefinition
    {
        //[BsonId]
        //[Newtonsoft.Json.JsonProperty("_id")]
        //public object _id { get; set; }
        
            /// <summary>
        /// Mapping System Code for End Supplier. Full List of Codes can be retrieved from the Supplier Code API
        /// </summary>
        public string SupplierCode { get; set; }

        public string CallType { get; set; }
        /// <summary>
        /// End Supplier System Product Code. This code should be used in all communication with the End Supplier
        /// </summary>
        public string SupplierName { get; set; }

        public List<VisaDetail> VisaDetail { get; set; }

    }
}
