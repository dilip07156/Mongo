using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts.Holiday
{
    #region Holiday.Collections
    /// <summary>
    /// Structure for Holiday.Collections
    /// </summary>
    public class HolidayCollections
    {
        /// <summary>
        /// Master values may be retrived from a Master Service.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Name of the Supplier.
        /// </summary>
        public string DisplayName { get; set; }
    }
    #endregion

    /// <summary>
    /// Structure for Holiday.Interests,Holiday.TravellerType.subType
    /// </summary>
    public class SubType
    {
        /// <summary>
        /// Contain Type 
        /// </summary>
        public string Type { get; set; }
    }

    /// <summary>
    /// Structure for Holiday.TravelFrequency.Type
    /// </summary>
    public class TravelFrequency
    {
        /// <summary>
        /// Contain Type 
        /// </summary>
        public string Type { get; set; }
    }



    #region Holiday.Interests & Holiday.TravellerType
    /// <summary>
    /// Structure for Holiday.Interests,Holiday.TravellerType,Holiday.TravelFrequency
    /// </summary>
    public class HolidayTypes
    {
        /// <summary>
        ///  Values may be retrived from Master Service.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        ///   SubTypes for Type.Values may be retrived from Master Service.
        /// </summary>
        public List<SubType> SubType { get; set; }
    }
    #endregion


    public class PaceOfHoliday
    {
        public string Type { get; set; }
    }

    #region Holiday.Interests & Holiday.TravellerType
    /// <summary>
    /// Structure for Per person
    /// </summary>
    public class HolidayPerPersonPrice
    {
        /// <summary>
        /// Currency name.
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        ///   Amount in decimal
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        ///   contain Price basis
        /// </summary>
        public string PriceBasis { get; set; }
        /// <summary>
        ///   contain Price text
        /// </summary>
        public string PriceText { get; set; }
    }
    #endregion

    #region Holiday.Destinations
    /// <summary>
    /// Structure for Holiday.Destinations
    /// </summary>
    public class HolidayDestinations
    {
        /// <summary>
        ///  TLGX Country Code.Values may be retrived from Master Service.
        /// </summary>
        public string TLGXCountryCode { get; set; }
        /// <summary>
        ///  TLGX Country Name.Values may be retrived from Master Service.
        /// </summary>
        public string TLGXCountryName { get; set; }
        /// <summary>
        ///  TLGX City Code.Values may be retrived from Master Service.
        /// </summary>
        public string TLGXCityCode { get; set; }
        /// <summary>
        ///  TLGX City Name.Values may be retrived from Master Service.
        /// </summary>
        public string TLGXCityName { get; set; }
        /// <summary>
        ///  CountryCode given by Supplier
        /// </summary>
        public string SupplierCountryCode { get; set; }
        /// <summary>
        ///  CountryName given by Supplier
        /// </summary>
        public string SupplierCountryName { get; set; }
        /// <summary>
        ///  CityCode given by Supplier
        /// </summary>
        public string SupplierCityCode { get; set; }
        /// <summary>
        ///  CityName given by Supplier
        /// </summary>
        public string SupplierCityName { get; set; }
    }
    #endregion

    #region Holiday.HolidayIncludes
    /// <summary>
    /// Structure for Holiday.HolidayIncludes
    /// </summary>
    public class HolidayIncludes
    {
        /// <summary>
        /// If Air service is included in Holiday or not.
        /// </summary>
        public string Air { get; set; }
        /// <summary>
        /// If Hotels are included in Holiday or not.
        /// </summary>
        public string Hotel { get; set; }
        /// <summary>
        ///  If Transfer services are included in Holiday or not.
        /// </summary>
        public string Transfers { get; set; }
        /// <summary>
        /// If Sightseeing is included in Holiday or not.
        /// </summary>
        public string Sightseeing { get; set; }
        /// <summary>
        /// If Meals are included in Holiday or not.
        /// </summary>
        public string Meals { get; set; }
        /// <summary>
        /// If Visa service is provided in Holiday or not.
        /// </summary>
        public string Visa { get; set; }
        /// <summary>
        /// if Insurance is included in holiday or not.
        /// </summary>
        public string Insurance { get; set; }
    }
    #endregion

    #region Holiday.UniqueSellingPoints
    /// <summary>
    /// Structure for Holiday.UniqueSellingPoints
    /// </summary>
    public class HolidayUniqueSellingPoints
    {
        /// <summary>
        ///  Id of unique sellinf Points
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        ///  Description about attractions/UniqueSellingPoints.
        /// </summary>
        public string Text { get; set; }
    }
    #endregion

    #region Holiday.Highlights
    /// <summary>
    /// Structure for Holiday.Highlights
    /// </summary>
    public class HolidayHighlights
    {
        /// <summary>
        /// Name of the destination provided by supplier.
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        ///  City Name  provided by supplier.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        ///  Country Name  provided by supplier.
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Day
        /// </summary>
        public int Day { get; set; }
        /// <summary>
        /// Date on which this information is valid.
        /// </summary>
        public DateTime? Date { get; set; }
        /// <summary>
        /// Type of the attraction.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Description for the attraction.
        /// </summary>
        public string Text { get; set; }

    }
    #endregion

    #region Holiday.Brands & Holiday.Brochures
    /// <summary>
    /// Structure for Holiday.Brands
    /// </summary>
    public class HolidayBrandsAndBrochures
    {
        /// <summary>
        ///  Id. 
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        ///  Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///  Code provided by supplier.
        /// </summary>
        public string Code { get; set; }
    }
    #endregion

    #region Holiday.Media
    /// <summary>
    /// Structure for Holiday.Media
    /// </summary>
    public class HolidayMedia
    {
        /// <summary>
        /// Url to access the media file.
        /// </summary>
        public string URL { get; set; }
        /// <summary>
        /// width of the media file.
        /// </summary>
        public string Width { get; set; }
        /// <summary>
        /// Height of the media file.
        /// </summary>
        public string Height { get; set; }
        /// <summary>
        /// name of the media file.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Display name for media file.
        /// </summary>
        public string ProductDisplayName { get; set; }
        /// <summary>
        /// Contain Country Name
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// Contain State
        /// </summary>
        public string State { get; set; }
        /// <summary>
        /// Contain city
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// category of  media .
        /// </summary>
        public string FileCategory { get; set; }
        /// <summary>
        /// Type of  media.
        /// </summary>
        public string FileType { get; set; }
        /// <summary>
        /// path to access media.
        /// </summary>
        public string LogicalFilePath { get; set; }
        ///// <summary>
        ///// from which date media information is valid.
        ///// </summary>
        //public string ValidFromDate { get; set; }
        ///// <summary>
        ///// upto which date media information is valid.
        ///// </summary>
        //public string ValidToDate { get; set; }
        /// <summary>
        /// Display order for media file.
        /// </summary>
        public int DisplayOrder { get; set; }
        /// <summary>
        /// detail description about media.
        /// </summary>
        public string Description { get; set; }
    }
    #endregion

    #region Holiday.TermsConditions & Holiday.BookingPolicy & Holiday.TourNotes
    /// <summary>
    /// Structure for Holiday.TermsConditions,Holiday.BookingPolicy,Holiday.TourNotes
    /// </summary>
    public class HolidayTermsConditions
    {
        /// <summary>
        /// Type of information
        /// </summary>
        public string ServiceType { get; set; }
        /// <summary>
        /// name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// order number.
        /// </summary>
        public int Order { get; set; }

    }
    #endregion

    #region Holiday.Inclusions & Holiday.Exclusions
    /// <summary>
    /// Structure for Holiday.Inclusions,Holiday.Exclusions
    /// </summary>
    public class HolidayInclusionExclusion
    {
        /// <summary>
        /// Inclusion/exclusion type.
        /// </summary>
        public string ServiceType { get; set; }
        /// <summary>
        /// Name for inclusion/exclusion.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// description of inclusion/exclusion.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// order of inclusion/exclusion.
        /// </summary>
        public int Order { get; set; }
        ///// <summary>
        ///// IsMandatoryPaid for the inclusion/exclusion.
        ///// </summary>
        //public string IsMandatoryPaid { get; set; }

    }
    #endregion

    #region Holiday.StartingPrice 
    /// <summary>
    /// Structure for Holiday.StartingPrice
    /// </summary>
    public class HolidayStartingPrice
    {
        /// <summary>
        /// Currency in which price is given by supplier.
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Price for Holiday.
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        ///  Information about on which basis price is calculated.
        /// </summary>
        public string PriceBasis { get; set; }
        /// <summary>
        /// Description about price.
        /// </summary>
        public string PriceText { get; set; }

    }
    #endregion Holiday.StartingPrice

    #region Holiday.HubDetails 
    /// <summary>
    /// Structure for Holiday.HubDetails
    /// </summary>
    public class HolidayHubDetails
    {
        /// <summary>
        /// Id of the Hub.
        /// </summary>
        public string HubId { get; set; }
        /// <summary>
        /// Name of the Hub.
        /// </summary>
        public string HubName { get; set; }

    }
    #endregion Holiday.HubDetails

    #region Holiday.DayWiseItineraries 
    /// <summary>
    /// Structure for Holiday.DayWiseItineraries
    /// </summary>
    public class HolidayDayWiseItineraries
    {
        /// <summary>
        /// Contain TagType
        /// </summary>
        public string TagType { get; set; }
        /// <summary>
        /// Contain Destination
        /// </summary>
        public string Destination { get; set; }
        /// <summary>
        /// Contain TLGXCountry
        /// </summary>
        public string TLGXCountry { get; set; }
        /// <summary>
        /// Contain TLGXCity
        /// </summary>
        public string TLGXCity { get; set; }
        /// <summary>
        /// Contain Supplier Country
        /// </summary>
        public string SupplierCountry { get; set; }
        /// <summary>
        /// Contain Supplier City
        /// </summary>
        public string SupplierCity { get; set; }
        /// <summary>
        /// Contain Product Category
        /// </summary>
        public string ProductCategory { get; set; }
        /// <summary>
        /// Contain Product Category Sub Type
        /// </summary>
        public string ProductCategorySubType { get; set; }
        /// <summary>
        /// Contain Product Name
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Contain Product ID
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// Contain Long Description
        /// </summary>
        public string LongDescription { get; set; }
        /// <summary>
        /// Contain Short Description
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// Contain Tag Description
        /// </summary>
        public string TagDescription { get; set; }
        /// <summary>
        /// Contain start time
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// Contain End Time 
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// Contain Session
        /// </summary>
        public string Session { get; set; }
        /// <summary>
        /// Contain Dynamic Attributes
        /// </summary>
        public string DynamicAttributes { get; set; }
        /// <summary>
        /// Contain Date Structure
        /// </summary>
        public List<HolidayDates> Dates { get; set; }


        ///// <summary>
        ///// Date from which Itinerary details are valid.
        ///// </summary>
        //public string FromDate { get; set; }
        ///// <summary>
        ///// Date upto  which Itinerary details are valid.
        ///// </summary>
        //public string ToDate { get; set; }
        ///// <summary>
        ///// Package category is the Holiday category(e.g 2*,4* ).
        ///// </summary>
        //public string PackageCategory { get; set; }
        ///// <summary>
        ///// Host category is type of Holiday(e.g economy, delux,premium etc..).
        ///// </summary>
        //public string HostCategory { get; set; }
        ///// <summary>
        /// Day wise Itinerary
        /// </summary>
        // public List<HolidayDay> Day { get; set; }


    }

    /// <summary>
    /// Holiday.DayWiseItineraries.Dates
    /// </summary>
    public class HolidayDates
    {
        /// <summary>
        /// Contain from day Number
        /// </summary>
        public int FromDay { get; set; }
        /// <summary>
        /// Day from month number.
        /// </summary>
        public int FromMonth { get; set; }
        /// <summary>
        /// From year Number.
        /// </summary>
        public int FromYear { get; set; }
        /// <summary>
        /// Contain till day information.
        /// </summary>
        public int ToDay { get; set; }
        /// <summary>
        /// Contain till month information
        /// </summary>
        public int ToMonth { get; set; }
        /// <summary>
        /// contain till year information
        /// </summary>
        public int ToYear { get; set; }
        /// <summary>
        /// Contain Description
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Contain tag description.
        /// </summary>
        public string TagDescription { get; set; }
        /// <summary>
        /// Contain dynamic attributes.
        /// </summary>
        public string DynamicAttributes { get; set; }
        /// <summary>
        /// Contain status.
        /// </summary>
        public string Status { get; set; }


    }

    #region Holiday.DayWiseItineraries.Day
    /// <summary>
    /// Holiday.DayWiseItineraries.Day
    /// </summary>
    public class HolidayDay
    {
        /// <summary>
        /// Day Number for Itinerary.
        /// </summary>
        public string DayNumber { get; set; }
        /// <summary>
        /// Date for itinerary details .
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// header info for itinearary
        /// </summary>
        public string DayHeader { get; set; }
        /// <summary>
        /// Country Name given by supplier.
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// CountryCode given by supplier.
        /// </summary>
        public string CountryCode { get; set; }
        /// <summary>
        /// City Name given by supplier.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        ///  City code given by supplier.
        /// </summary>
        public string CityCode { get; set; }
        /// <summary>
        /// Type of the itinerary.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// overninght stay for the day.
        /// </summary>
        public string OvernightStay { get; set; }
        /// <summary>
        /// Short Description for itinerary on the day.
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// Long Description for itinerary on the day.
        /// </summary>
        public string LongDescription { get; set; }
        /// <summary>
        /// Note.
        /// </summary>
        public string DayNote { get; set; }
        /// <summary>
        /// information about meals
        /// </summary>
        public List<HolidayMeals> Meals { get; set; }
        /// <summary>
        /// Information about different services(Itinerary items) on this day.
        /// </summary>
        public List<HolidayItineraryItems> ItineraryItems { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<HolidayMedia> Media { get; set; }
    }

    #region Holiday.DayWiseItineraries.Day.Meals
    /// <summary>
    /// Holiday.DayWiseItineraries.Day.Meals
    /// </summary>
    public class HolidayMeals
    {
        /// <summary>
        /// Type of the meal.
        /// </summary>
        public string MealType { get; set; }
        /// <summary>
        /// Description about meal.
        /// </summary>
        public string MealDescription { get; set; }
    }
    #endregion Holiday.DayWiseItineraries.Day.Meals

    #region Holiday.DayWiseItineraries.Day.ItineraryItems
    /// <summary>
    /// Holiday.DayWiseItineraries.Day.ItineraryItems
    /// </summary>
    public class HolidayItineraryItems
    {
        /// <summary>
        /// Type of Service detail
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Name of Service detail
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Country name provided by supplier
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// City name provided by supplier
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Information about overnight stay for this service.
        /// </summary>
        public string OvernightStay { get; set; }
        /// <summary>
        /// Long description about service.
        /// </summary>
        public string LongDescription { get; set; }
        /// <summary>
        ///  short description about service.
        /// </summary>
        public string ShortDescription { get; set; }
        /// <summary>
        /// Pick up point for the Itinerary service.
        /// </summary>
        public string PickupPoint { get; set; }
        /// <summary>
        /// Drop point for the Itinerary service.
        /// </summary>
        public string DropPoint { get; set; }
        /// <summary>
        /// Pick up time for the Itinerary service.
        /// </summary>
        public string PickupTime { get; set; }
        /// <summary>
        /// Drop time for the Itinerary service.
        /// </summary>
        public string DropTime { get; set; }
        /// <summary>
        /// Total duration for the completion.
        /// </summary>
        public string Duration { get; set; }
    }
    #endregion Holiday.DayWiseItineraries.Day.ItineraryItems

    #endregion Holiday.DayWiseItineraries.Day

    #endregion Holiday.DayWiseItineraries

    #region Holiday.Accommodation 
    /// <summary>
    /// Structure for Holiday.Accommodation
    /// </summary>
    public class HolidayAccommodation
    {
        /// <summary>
        /// TLGX Code for Product given by supplier.
        /// </summary>
        public string TLGXProductCode { get; set; }
        /// <summary>
        /// Code provided by Supplier for Product.
        /// </summary>
        public string SupplierProductCode { get; set; }
        /// <summary>
        ///  TLGX CountryName.Master values may be retrived from a Master Service.
        /// </summary>
        public string TLGXCountryName { get; set; }
        /// <summary>
        ///TLGX country code.
        /// </summary>
        public string TLGXCountryCode { get; set; }
        /// <summary>
        /// TLGX City Name given by Supplier
        /// </summary>
        public string TLGXCityName { get; set; }
        /// <summary>
        /// TLGX Code for Product given by supplier.
        /// </summary>
        public string TLGXCityCode { get; set; }
        /// <summary>
        /// City Name provided by Supplier.
        /// </summary>
        public string SupplierCityName { get; set; }
        /// <summary>
        ///  City Code provided by Supplier.
        /// </summary>
        public string SupplierCityCode { get; set; }
        /// <summary>
        /// Number of Nights.
        /// </summary>
        public int NumberOfNights { get; set; }
        /// <summary>
        ///  Tour Package Category under which this accomodation is provided. values may be 2*,3* etc..
        /// </summary>
        public string PackageCategory { get; set; }
        /// <summary>
        /// Tour SubCategory values may be Economy,Delux,Premium etc..
        /// </summary>
        public string HostCategory { get; set; }
        /// <summary>
        /// Category of Hotel.
        /// </summary>
        public string HotelCategory { get; set; }
        /// <summary>
        ///  Name of the Hotel.
        /// </summary>
        public string HotelName { get; set; }
        /// <summary>
        /// star Rtaing of Hotel (e.g 3*,4*)
        /// </summary>
        //public string StarRating { get; set; }
        ///// <summary>
        /////  Date on which this accomodation is valid.
        ///// </summary>
        //public string StartDate { get; set; }
        ///// <summary>
        ///// Date upto which this accomodation is valid.
        ///// </summary>
        //public string FinishDate { get; set; }
        ///// <summary>
        ///// Departure Id
        ///// </summary>
        //public string DepartureId { get; set; }
        ///// <summary>
        /////  Date for Departure.
        ///// </summary>
        //public string DepartureStartDate { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public string DepartureEndDate { get; set; }
        ///// <summary>
        /////  Rating by Trip Advisor for the hotel.
        ///// </summary>
        //public string TripAdvisorRating { get; set; }
        ///// <summary>
        ///// Description for Hotel.
        ///// </summary>
        //public string HotelDescription { get; set; }
        ///// <summary>
        /////  information of Facilities at Hotel.
        ///// </summary>
        //public List<HolidayFacility> Facilities { get; set; }
        ///// <summary>
        /////  Media for Hotel(images/Videos)
        ///// </summary>
        //public List<HolidayMedia> Media { get; set; }
    }
    #endregion Holiday.Accommodation

    #region Holiday.Accommodation.Facilities
    /// <summary>
    /// Structure for Holiday.Accommodation.Facility
    /// </summary>
    public class HolidayFacility
    {
        /// <summary>
        /// Facility Type
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// Name of the Facility.
        /// </summary>
        public string Name { get; set; }
    }
    #endregion Holiday.Accommodation.Facilities

    #region Holiday.Activity 
    /// <summary>
    /// Structure for Holiday.Activity
    /// </summary>
    public class HolidayActivity
    {
        /// <summary>
        /// TLGX Code for Product given by supplier.
        /// </summary>
        public string TLGXActivityId { get; set; }
        /// <summary>
        /// Code provided by Supplier for Product.
        /// </summary>
        public string TLGXActivityName { get; set; }
        /// <summary>
        /// Country Code provided by Supplier for Product.
        /// </summary>
        public string TLGXCountryCode { get; set; }
        /// <summary>
        /// Country Name provided by Supplier for Product.
        /// </summary>
        public string TLGXCountryName { get; set; }
        /// <summary>
        /// City Name provided by Supplier for Product.
        /// </summary>
        public string TLGXCityName { get; set; }
        /// <summary>
        /// City Code provided by Supplier for Product.
        /// </summary>
        public string TLGXCityCode { get; set; }
        /// <summary>
        /// Supplier Country Name provided by Supplier for Product.
        /// </summary>
        public string SupplierCountryName { get; set; }
        /// <summary>
        /// Supplier Country Code provided by Supplier for Product.
        /// </summary>
        public string SupplierCountryCode { get; set; }
        /// <summary>
        /// Supplier City Code provided by Supplier for Product.
        /// </summary>
        public string SupplierCityCode { get; set; }
        /// <summary>
        /// Supplier City Name provided by Supplier for Product.
        /// </summary>
        public string SupplierCityName { get; set; }
        /// <summary>
        /// Contain number of days.
        /// </summary>
        public int DayNumber { get; set; }
        /// <summary>
        /// Contain date
        /// </summary>
        public DateTime? Date { get; set; }
        /// <summary>
        /// Contain start date
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// Date upto which this accomodation is valid.
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// Contain Session Information
        /// </summary>
        public string Session { get; set; }
        /// <summary>
        ///  Date on which this accomodation is valid.
        /// </summary>
        public DateTime? StartDate { get; set; }
        /// <summary>
        /// Contain Departure id
        /// </summary>
        public string DepartureId { get; set; }
        /// <summary>
        /// Contain Departure start date
        /// </summary>
        public DateTime? DepartureStartDate { get; set; }
        /// <summary>
        /// Contain Departure end date
        /// </summary>
        public DateTime? DepartureEndDate { get; set; }
        /// <summary>
        /// Contain product category
        /// </summary>
        public string ProductCategory { get; set; }
        /// <summary>
        /// Contain host category  Values may be (Economy,Delux,Premium etc..)
        /// </summary>
        public string HostCategory { get; set; }
        /// <summary>
        /// Contain pick up destination
        /// </summary>
        public string PickupDestination { get; set; }
        /// <summary>
        /// Contain drop destination
        /// </summary>
        public string DropDestination { get; set; }




        ///// <summary>
        ///// TLGX Code for Product given by supplier.
        ///// </summary>
        //public string ID { get; set; }
        ///// <summary>
        ///// Code provided by Supplier for Product.
        ///// </summary>
        //public string ActivityName { get; set; }
        ///// <summary>
        /////  TLGX CountryName.Master values may be retrived from a Master Service.
        ///// </summary>
        //public string ActivityDescription { get; set; }
        ///// <summary>
        /////TLGX country code.
        ///// </summary>
        //public string CountryName { get; set; }
        ///// <summary>
        ///// TLGX Code for Product given by supplier.
        ///// </summary>
        //public string CityName { get; set; }
        ///// <summary>
        ///// City Name provided by Supplier.
        ///// </summary>
        //public string SupplierCityCode { get; set; }
        ///// <summary>
        /////  City Code provided by Supplier.
        ///// </summary>
        //public string TLGXCountryName { get; set; }
        ///// <summary>
        ///// Number of Nights.
        ///// </summary>
        //public string TLGXCountryCode { get; set; }
        ///// <summary>
        /////  Tour Package Category under which this accomodation is provided. values may be 2*,3* etc..
        ///// </summary>
        //public string TLGXCityName { get; set; }
        ///// <summary>
        ///// Tour SubCategory values may be Economy,Delux,Premium etc..
        ///// </summary>
        //public string TLGXCityCode { get; set; }
        ///// <summary>
        ///// Category of Hotel.
        ///// </summary>
        //public string DayNumber { get; set; }
        ///// <summary>
        /////  Name of the Hotel.
        ///// </summary>
        //public string Date { get; set; }
        ///// <summary>
        ///// star Rtaing of Hotel (e.g 3*,4*)
        ///// </summary>
        //public string StartTime { get; set; }
        ///// <summary>
        /////  Date on which this accomodation is valid.
        ///// </summary>
        //public string EndTime { get; set; }
        ///// <summary>
        ///// Date upto which this accomodation is valid.
        ///// </summary>
        //public string Lat { get; set; }
        ///// <summary>
        ///// Departure Id
        ///// </summary>
        //public string Lon { get; set; }
        ///// <summary>
        /////  Date for Departure.
        ///// </summary>
        //public string StartDate { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public string FinishDate { get; set; }
        ///// <summary>
        /////  Rating by Trip Advisor for the hotel.
        ///// </summary>
        //public string DepartureId { get; set; }
        ///// <summary>
        ///// Description for Hotel.
        ///// </summary>
        //public string DepartureStartDate { get; set; }
        ///// <summary>
        /////  information of Facilities at Hotel.
        ///// </summary>
        //public List<HolidayFacility> DepartureEndDate { get; set; }
        ///// <summary>
        /////  Package category for Holiday
        ///// </summary>
        //public List<HolidayMedia> PackageCategory { get; set; }
        ///// <summary>
        ///// Host Category of Holiday. Values may be (Economy,Delux,Premium etc..)
        ///// </summary>
        //public string HostCategory { get; set; }
        ///// <summary>
        /////  Pick Up Point.
        ///// </summary>
        //public string PickupDestination { get; set; }
        ///// <summary>
        ///// Drop point.
        ///// </summary>
        //public string DropDestination { get; set; }
        ///// <summary>
        /////  Media details.
        ///// </summary>
        //public List<HolidayMedia> Media { get; set; }
        ///// <summary>
        /////  what is included in Holiday.
        ///// </summary>
        //public List<HolidayInclusionExclusion> Inclusion { get; set; }
        ///// <summary>
        /////  what is excluded in Holiday.
        ///// </summary>
        //public List<HolidayInclusionExclusion> Exclusion { get; set; }
        ///// <summary>
        /////  Booking policies
        ///// </summary>
        //public List<HolidayTermsConditions> BookingPolicies { get; set; }
        ///// <summary>
        /////  Review for Activity.
        ///// </summary>
        //public List<HolidayReview> Review { get; set; }
    }
    #endregion Holiday.Activity

    #region Review
    /// <summary>
    /// Structure for Holiday.Activity.Review
    /// </summary>
    public class ReviewDetails
    {
        /// <summary>
        /// Title for review.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Description of review.
        /// </summary>
        public string Text { get; set; }
        /// <summary>
        /// Name for review.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Review score.
        /// </summary>
        public string Score { get; set; }
        /// <summary>
        /// Image URL for review.
        /// </summary>
        public string ImageURL { get; set; }
        /// <summary>
        /// Date on which review is given.
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// Who has given the review.
        /// </summary>
        public string ReviewProvider { get; set; }

    }
    #endregion Review

    #region Holiday.Flights
    /// <summary>
    /// Structure for Holiday.Flights
    /// </summary>
    public class HolidayFlights
    {
        /// <summary>
        /// Name of the Airline Company.
        /// </summary>
        public string Airline { get; set; }
        /// <summary>
        /// Flight Number.
        /// </summary>
        public string FlightNumber { get; set; }
        /// <summary>
        /// Flight is included in holiday or not.
        /// </summary>
        public string IncludedInTrip { get; set; }
        /// <summary>
        /// Airport Code from which Departure will take place.
        /// </summary>
        public string DepartureAirportCode { get; set; }
        /// <summary>
        /// Airport Name from which Departure will take place.
        /// </summary>
        public string DepartureAirportName { get; set; }
        /// <summary>
        /// Name of the terminal.
        /// </summary>
        public string DepartureTerminal { get; set; }
        /// <summary>
        /// Time for Departure.
        /// </summary>
        public string DepartureTime { get; set; }
        /// <summary>
        /// Date of Departure.
        /// </summary>
        public string DepartureDate { get; set; }
        /// <summary>
        /// Duration.
        /// </summary>
        public string Duration { get; set; }
        /// <summary>
        /// Stops for Flight.
        /// </summary>
        public string FlightStops { get; set; }
        /// <summary>
        /// Airport Code .
        /// </summary>
        public string ArrivalAirportCode { get; set; }
        /// <summary>
        /// Airport name.
        /// </summary>
        public string ArrivalAirportName { get; set; }
        /// <summary>
        /// Terminal information.
        /// </summary>
        public string ArrivalTerminal { get; set; }
        /// <summary>
        /// Time of Flight arrival.
        /// </summary>
        public string ArrivalTime { get; set; }
        /// <summary>
        /// date for Flight Arrival.
        /// </summary>
        public string ArrivalDate { get; set; }
        /// <summary>
        /// Class of Flight ticket(e.g Economy,Buisness)
        /// </summary>
        public string TicketClass { get; set; }
        /// <summary>
        /// Information about Baggage Allowance.
        /// </summary>
        public string BaggageAllowance { get; set; }
        /// <summary>
        /// Information about meals provided.
        /// </summary>
        public string Meals { get; set; }
        /// <summary>
        /// Information about Refunds.
        /// </summary>
        public string RefundPolicy { get; set; }
        /// <summary>
        /// Flight Booking Policies
        /// </summary>
        public string BookingPolicyText { get; set; }
        /// <summary>
        /// Inclusion details.
        /// </summary>
        public List<HolidayInclusionExclusion> Inclusion { get; set; }
        /// <summary>
        /// Exclusion details.
        /// </summary>
        public List<HolidayInclusionExclusion> Exclusion { get; set; }

    }
    #endregion Holiday.Flights

    #region Holiday.Departure
    /// <summary>
    /// Structure for Holiday.Departure
    public class HolidayDeparture
    {
        ///// <summary>
        ///// contain departure Id
        ///// </summary>
        public string DepartureId { get; set; }
        ///// <summary>
        ///// contain Country code start
        ///// </summary>
        public string TLGXCountryCodeStart { get; set; }
        ///// <summary>
        ///// contain Tlgx City Code start date
        ///// </summary>
        public string TLGXCityCodeStart { get; set; }
        ///// <summary>
        ///// contain start date
        ///// </summary>
        public DateTime? StartDate { get; set; }
        ///// <summary>
        ///// Tlgx Country code end
        ///// </summary>
        public string TLGXCountryCodeEnd { get; set; }
        ///// <summary>
        ///// contain TLGX city code end
        ///// </summary>
        public string TLGXCityCodeEnd { get; set; }

        ///// <summary>
        /////
        ///// </summary>
        //public string DepartureId { get; set; }
        ///// <summary>
        /////
        ///// </summary>
        //public string DepartureName { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public string StartDate { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public string EndDate { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public string StartCity { get; set; }
        ///// <summary>
        ///// 
        ///// </summary>
        //public string EndCity { get; set; }

    }
    #endregion Holiday.Departure

    /// <summary>
    /// Structure for Holiday.PackagePrice.BasePrice.Discount
    /// 
    public class DiscountStructure
    {
        ///// <summary>
        ///// Contain name of Discount
        ///// </summary>
        public string Name { get; set; }
        ///// <summary>
        ///// Contain Type of discount
        ///// </summary>
        public string Type { get; set; }
        ///// <summary>
        ///// Contain discount amount
        ///// </summary>
        public double DiscountAmount { get; set; }



    }

    /// <summary>
    /// Structure for Holiday.PackagePrice.BasePrice.Tax
    /// 
    public class Tax
    {
        ///// <summary>
        ///// Inclusive of taxes Yes or No
        ///// </summary>
        public string InclusiveOfTaxes { get; set; }
        ///// <summary>
        ///// Date from which it is valid
        ///// </summary>
        public DateTime? ValidFrom { get; set; }
        ///// <summary>
        ///// Date upto which it is valid
        ///// </summary>
        public DateTime? ValidTo { get; set; }
        ///// <summary>
        ///// Contain Tax type
        ///// </summary>
        public string TaxType { get; set; }
        ///// <summary>
        ///// Contain tax rate
        ///// </summary>
        public double TaxRate { get; set; }
        ///// <summary>
        ///// Contain tax value
        ///// </summary>
        public double TaxValue { get; set; }
        ///// <summary>
        ///// Contain tax calculated on rate Type
        ///// </summary>
        public string TaxCalculatedOnRateType { get; set; }
        ///// <summary>
        ///// Contain Applicable on
        ///// </summary>
        public string ApplicableOn { get; set; }

    }

    /// <summary>
    /// Structure for Holiday.PackagePrice.BasePrice.PriceDetails.tax
    /// 
    public class TaxStructure
    {
        ///// <summary>
        ///// Contain Tax Rate
        ///// </summary>
        public string TaxRate { get; set; }
        ///// <summary>
        ///// contain Tax Type
        ///// </summary>
        public string TaxType { get; set; }
        ///// <summary>
        ///// Contain Tax amount
        ///// </summary>
        public double TaxAmount { get; set; }
    }

    /// <summary>
    /// Structure for Holiday.PackagePrice.BasePrice.PriceDetails
    /// 
    public class PriceDetailsStructure
    {
        public int PassengerRangeFrom { get; set; }

        public int PassengerRangeTo { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }

        public string Currency { get; set; }

        public double Amount { get; set; }

        public string PersonType { get; set; }

        public string RoomType { get; set; }

        public string ExtraBedding { get; set; }

        public int AgeFrom { get; set; }

        public int AgeTo { get; set; }

        public string PriceDescription { get; set; }

        public List<TaxStructure> Tax { get; set; }


    }

    /// <summary>
    /// Structure for Holiday.PackagePrice.BasePrice
    /// 
    public class BasePriceStructure
    {
        ///// <summary>
        ///// Contain hub Id
        ///// </summary>
        public string HubId { get; set; }
        /// <summary>
        /// Contain hub name
        /// </summary>
        public string HubName { get; set; }
        /// <summary>
        /// hold list of price details
        /// </summary>
        public List<PriceDetailsStructure> PriceDetails { get; set; }
        /// <summary>
        /// hold a list of discount 
        /// </summary>
        public List<DiscountStructure> Discount { get; set; }
        /// <summary>
        /// hold a list of tax applied
        /// </summary>
        public List<Tax> Tax { get; set; }
    }

    /// <summary>
    /// Structure for Holiday.PreTourPrice and Holiday.PostTourPrice
    /// 
    public class PreTourPrice
    {
        /// <summary>
        /// Code provided by Supplier for Product.
        /// </summary>
        public string SupplierProductCode { get; set; }
        /// <summary>
        /// Contain passenger range from
        /// </summary>
        public double PassengerRangeFrom { get; set; }
        /// <summary>
        /// contain passenger range to
        /// </summary>
        public double PassengerRangeTo { get; set; }
        /// <summary>
        /// contain date from which it is valid.
        /// </summary>
        public DateTime? ValidFrom { get; set; }
        /// <summary>
        /// contain date upto which it is valid
        /// </summary>
        public DateTime? ValidTo { get; set; }
        /// <summary>
        /// Contain Currency Name 
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Contain Amount 
        /// </summary>
        public double Amount { get; set; }
        /// <summary>
        /// Contain Person type
        /// </summary>
        public string PersonType { get; set; }
        /// <summary>
        /// Contain Room Type
        /// </summary>
        public string RoomType { get; set; }
        /// <summary>
        /// Contain extra bedding information
        /// </summary>
        public string ExtraBedding { get; set; }
        /// <summary>
        /// Contain age from information
        /// </summary>
        public int AgeFrom { get; set; }
        /// <summary>
        /// contain Age to Information
        /// </summary>
        public int AgeTo { get; set; }
        /// <summary>
        /// Contain Discount name information
        /// </summary>
        public string DiscountName { get; set; }
        /// <summary>
        /// cotain disocunt Amount
        /// </summary>
        public int DiscountAmount { get; set; }
        /// <summary>
        /// Hold a list of Tax Structure
        /// </summary>
        public List<TaxStructure> Tax { get; set; }
    }



    /// <summary>
    /// Structure for Holiday.PackagePrice
    /// 
    public class PackagePrice
    {
        /// <summary>
        /// Display Name
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// Category Name
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        /// Star Rating category
        /// </summary>
        public string StarRatingCategory { get; set; }
        /// <summary>
        /// Notes if any
        /// </summary>
        public string Notes { get; set; }

        public List<BasePriceStructure> BasePrice { get; set; }

    }

    /// <summary>
    /// Structure for Holiday.PreTourStructure , Holiday.PostTourStructure
    /// 
    public class PreTourStructure
    {
        /// <summary>
        /// TLGX Code for Product given by supplier.
        /// </summary>
        public string TLGXProductCode { get; set; }
        /// <summary>
        /// Code provided by Supplier for Product.
        /// </summary>
        public string SupplierProductCode { get; set; }
        /// <summary>
        ///  TLGX Country Name.Values may be retrived from Master Service.
        /// </summary>
        public string TLGXCountryName { get; set; }
        /// <summary>
        ///  TLGX Country Code.Values may be retrived from Master Service.
        /// </summary>
        public string TLGXCountryCode { get; set; }
        /// <summary>
        ///  TLGX City Name.Values may be retrived from Master Service.
        /// </summary>
        public string TLGXCityName { get; set; }
        /// <summary>
        ///  TLGX City Code.Values may be retrived from Master Service.
        /// </summary>
        public string TLGXCityCode { get; set; }
        /// <summary>
        ///  CityName given by Supplier
        /// </summary>
        public string SupplierCityName { get; set; }
        /// <summary>
        ///  CityCode given by Supplier
        /// </summary>
        public string SupplierCityCode { get; set; }
        /// <summary>
        /// Number Of Nights in the Hotel.
        /// </summary>
        public int NumberOfNights { get; set; }
        ///// <summary>
        ///// Host category is type of Holiday(e.g economy, delux,premium etc..).
        ///// </summary>
        public string HostCategory { get; set; }
        /// <summary>
        /// Category of Hotel.
        /// </summary>
        public string HotelCategory { get; set; }
        ///// <summary>
        ///// Package category is the Holiday category(e.g 2*,4* ).
        ///// </summary>
        public string PackageCategory { get; set; }
        /// <summary>
        ///  Name of the Hotel.
        /// </summary>
        public string HotelName { get; set; }

    }




    #region Holiday.Pre 
    /// <summary>
    /// Structure for Holiday.Pre, Holiday.Post
    /// </summary>
    public class HolidayPrePost
    {
        /// <summary>
        /// Type of information.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// CountryN ame provided by Supplier.
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// City Name provided by Supplier.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// TLGX HotelCode.
        /// </summary>
        public string TLGXProductCode { get; set; }
        /// <summary>
        ///  Supplier HotelCode.
        /// </summary>
        public string SupplierProductCode { get; set; }
        /// <summary>
        /// Name of the Hotel.
        /// </summary>
        public string HotelName { get; set; }
        /// <summary>
        /// Hotel Star  Rating.
        /// </summary>
        public string StarRating { get; set; }
        /// <summary>
        ///  Hotel Description
        /// </summary>
        public string HotelDescription { get; set; }
        /// <summary>
        /// Currency in which price is given.
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// Price.
        /// </summary>
        public string Amount { get; set; }
        /// <summary>
        /// Description about price.
        /// </summary>
        public string PriceFor { get; set; }
        /// <summary>
        /// Number Of Nights in the Hotel.
        /// </summary>
        public string NumberOfNights { get; set; }
        /// <summary>
        /// Number Of Days in the Hotel.
        /// </summary>
        public string DayNo { get; set; }
        /// <summary>
        /// Rating by Trip Advisor.
        /// </summary>
        public string TripAdvisorRating { get; set; }
        /// <summary>
        /// Pax description for hotel.
        /// </summary>
        public string PaxDescription { get; set; }

    }
    #endregion Holiday.PrePost

    #region Holiday.Optionals
    /// <summary>
    /// Structure for Holiday.Pre, Holiday.Post
    /// </summary>
    public class HolidayOptionals
    {
        /// <summary>
        /// Country name given by supplier.
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// City Name provided by Supplier.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// TLGX Country Name.
        /// </summary>
        public string TLGXCountryName { get; set; }
        /// <summary>
        /// TLGX Country Code
        /// </summary>
        public string TLGXCountryCode { get; set; }
        /// <summary>
        ///  TLGX City Name
        /// </summary>
        public string TLGXCityName { get; set; }
        /// <summary>
        /// TLGX City Code
        /// </summary>
        public string TLGXCityCode { get; set; }
        /// <summary>
        ///On which day Optional  Activity is available .
        /// </summary>
        public string AvailableonDays { get; set; }
        /// <summary>
        ///  Date for  optional Activity.
        /// </summary>
        public string Date { get; set; }
        /// <summary>
        /// Starting Time.
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// End Time.
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Session { get; set; }
        /// <summary>
        ///Total  Duration of this optional activity.
        /// </summary>
        public string Duration { get; set; }
        /// <summary>
        /// Name of Activity.
        /// </summary>
        public string ActivityName { get; set; }
        /// <summary>
        /// Description for activity.
        /// </summary>
        public string ActivityDescription { get; set; }
        /// <summary>
        /// Image for activity.
        /// </summary>
        public string ActivityImage { get; set; }
        /// <summary>
        /// Review ratings for activity.
        /// </summary>
        public string AverageReviewRating { get; set; }
        /// <summary>
        /// In which Currency price is given by supplier.
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// price for this optional activity.
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// Description about no. of peoples.
        /// </summary>
        public string PaxDescription { get; set; }
        /// <summary>
        /// On which day this Optional Activity is valid.
        /// </summary>
        public string ItienraryDay { get; set; }
        /// <summary>
        /// What is Included in Activity.
        /// </summary>
        public List<HolidayInclusionExclusion> Inclusion { get; set; }
        /// <summary>
        /// What is excluded from activity
        /// </summary>
        public List<HolidayInclusionExclusion> Exclusion { get; set; }

    }
    #endregion Holiday.optionals

    #region Holiday.Extension
    /// <summary>
    /// Structure for Holiday.Extension
    /// </summary>
    public class HolidayExtension
    {
        /// <summary>
        /// Country name given by supplier.
        /// </summary>
        public string Country { get; set; }
        /// <summary>
        /// City Name provided by Supplier.
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// No. of days extended.
        /// </summary>
        public string Days { get; set; }
        /// <summary>
        ///  No. of nights extended.
        /// </summary>
        public string Nights { get; set; }
        /// <summary>
        /// Detail description for extension.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        ///What is included.
        /// </summary>
        public string Inclusions { get; set; }
        /// <summary>
        /// price for this.
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// in which currency price is given by Supplier.
        /// </summary>
        public string Currency { get; set; }
        /// <summary>
        /// On what basis price is valid.( Detail Information about price.)
        /// </summary>
        public string PriceBasis { get; set; }
        /// <summary>
        /// Holiday Reference Number
        /// </summary>
        public string HolidayReferenceNumber { get; set; }
    }
    #endregion Holiday.Extension

    #region Holiday.Review
    /// <summary>
    /// Structure for Holiday.Extension
    /// </summary>
    public class HolidayReview
    {
        /// <summary>
        /// Summary of Reviews.
        /// </summary>
        public HolidayReviewSummary ReviewSummary { get; set; }
        /// <summary>
        /// Details of Reviews
        /// </summary>
        public ReviewDetails ReviewDetails { get; set; }

    }
    #endregion Holiday.Review

    #region Holiday.ReviewSummary
    /// <summary>
    /// Structure for Holiday.ReviewSummary
    /// </summary>
    public class HolidayReviewSummary
    {
        /// <summary>
        /// Source of Reviews.
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// Type of Reviews.
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        ///Review Score.
        /// </summary>
        public string score { get; set; }

    }

    #endregion Holiday.ReviewSummary
}
