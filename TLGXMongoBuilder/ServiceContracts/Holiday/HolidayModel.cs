using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataContracts.Holiday
{
    /// <summary>
    /// Structure for Holiday Model
    /// </summary>
    public class HolidayModel
    {        
        /// <summary>
        /// Unique Supplier Holiday Id for each Tour.
        /// </summary>
        [Required]
        public string SupplierHolidayId { get; set; }
        /// <summary>
        /// Call Type Like HotelMapping
        /// </summary>
        public string CallType { get; set; }

        /// <summary>
        /// Unique Supplier Holiday Id for each Tour.
        /// </summary>
        [Required]
        public string NakshatraHolidayId { get; set; }

        /// <summary>
        /// Name of the supplier providing this holiday.
        /// </summary>
        [Required]
        public string SupplierName { get; set; }
        /// <summary>
        /// Product Code provided by supplier. may be required in the booking process.
        /// </summary>
        public string SupplierProductCode { get; set; }
        /// <summary>
        /// Holiday is effective from this date.
        /// </summary>
        public string EffectiveFromDate { get; set; }
        /// <summary>
        /// Holiday is effective To this date.
        /// </summary>
        public string EffectiveToDate { get; set; }
        /// <summary>
        /// Date on which Holiday is created on system.
        /// </summary>
        [Required]
        public string CreateDate { get; set; }
        /// <summary>
        /// The user who created the product.
        /// </summary>
        public string CreateUSer { get; set; }
        /// <summary>
        ///Date on which Holiday was last edited.
        /// </summary>
        public string EditDate { get; set; }
        /// <summary>
        /// is active or not
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// The user review status.
        /// </summary>
        public string UserReviewStatus { get; set; }
        /// <summary>
        /// The user Remarks if any
        /// </summary>
        public string UserRemarks { get; set; }
        /// <summary>
        /// The user who edited Holiday.
        /// </summary>
        public string EditUser { get; set; }
        /// <summary>
        /// The Product category for this Holiday.Master values may be retrived from a Master Service.
        /// </summary>
        public string ProductCategory { get; set; }
        /// <summary>
        /// The Product SubCategory for this Holiday.Master values may be retrived from a Master Service.
        /// </summary>
        public string ProductCategorySubtype { get; set; }
        /// <summary>
        /// The group of company name.
        /// </summary>
        public string GroupOfCompany { get; set; }
        /// <summary>
        /// The group of company.
        /// </summary>
        public string GroupCompany { get; set; }
        /// <summary>
        /// contains the company name 
        /// </summary>
        public string Company { get; set; }
        /// <summary>
        /// contains the SBU 
        /// </summary>
        public string SBU { get; set; }
        /// <summary>
        /// contains the BU 
        /// </summary>
        public string BU { get; set; }
        /// <summary>
        /// contains the Company Product Id 
        /// </summary>
        public string CompanyProductId { get; set; }
        /// <summary>
        /// Name of the Product
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// Name of the Holiday
        /// </summary>
        public string Overview { get; set; }
        ///// <summary>
        ///// Name of the Holiday
        ///// </summary>
        //public string Name { get; set; }
        /// <summary>
        /// Holiday Type can be all,group,Individual.Master values may be retrived from a Master Service.
        /// </summary>
        public string TypeOfHoliday { get; set; }
        /// <summary>
        /// Id of the holiday flavor.Flavor is any change in activity,travelType for the Holiday.
        /// </summary>
        public string ProductFlavorID { get; set; }
        /// <summary>
        /// Name of the flavor. 
        /// </summary>
        public string ProductFlavorName { get; set; }
        /// <summary>
        /// TourOptionID. 
        /// </summary>
        public string TourOptionID { get; set; }
        /// <summary>
        /// It is a type of flavour. e.g self drive package ,cruise package.
        /// </summary>
        public string FlavorType { get; set; }
        /// <summary>
        /// It is a Date from which flavour is valid.
        /// </summary>
        public DateTime? FlavourValidFrom { get; set; }
        /// <summary>
        /// It is a Date To which flavour is valid.
        /// </summary>
        public DateTime? FlavourValidTo { get; set; }
        /// <summary>
        /// This will have value either FIT(Fully Independant Traveller) or Group
        /// </summary>
        public string ProductType { get; set; }
        /// <summary>
        /// This will have value either 'SET Packages' or 'Customized Packages'.
        /// </summary>
        public string PackageType { get; set; }
        /// <summary>
        /// This has values like Standard,Delux,Executive.
        /// </summary>
        public string CompanyPackageCategory { get; set; }
        /// <summary>
        /// Collection is a name of Supplier providing holiday.
        /// </summary>
        public List<HolidayCollections> Collections { get; set; }
        /// <summary>
        /// Biforgation of Holiday by types of traveller such as Couple ,Men,Women etc..Values may be retrived from Master Service.   
        /// </summary>
        public List<HolidayTypes> TravellerType { get; set; }
        /// <summary>
        /// Biforgation of Holiday by interests such as Beach ,Backpack,Hills etc..Values may be retrived from Master Service.   
        /// </summary>
        public List<HolidayTypes> Interests { get; set; }
        /// <summary>
        /// Biforgation of Holiday by Frequency of travel  such as First Time ,Been there before etc..Values may be retrived from Master Service. 
        /// </summary>
        public List<TravelFrequency> TravelFrequency { get; set; }
        /// <summary>
        /// Rating based on entire holiday.
        /// </summary>
        public string Rating { get; set; }
        /// <summary>
        /// This field gives you pace of holiday such as Relaxing,Moderate etc..
        /// </summary>
        public List<PaceOfHoliday> PaceOfHoliday { get; set; }
        /// <summary>
        /// Should not be mandatory as this data point will not come via API in most cases 
        /// </summary>
        public string StayType { get; set; }
        /// <summary>
        /// Number of days included in holiday.( e.g 3 days)
        /// </summary>
        public int Days { get; set; }
        /// <summary>
        /// Number of nights included in holiday.(e.g 2 Nights)
        /// </summary>
        public int Nights { get; set; }
        /// <summary>
        /// hold a list of Per Person Structure
        /// </summary>
        public List<HolidayPerPersonPrice> PerPersonPrice { get; set; }
        /// <summary>
        /// Total duration of holiday( e.g 3 days this field takes the total no. of days of Holiday as duration).
        /// </summary>
        public string Duration { get; set; }
        /// <summary>
        /// Starting price of the holiday
        /// </summary>
        public List<HolidayStartingPrice> StartingPrice { get; set; }
        /// <summary>
        /// Comfort level of the holiday.
        /// </summary>
        public List<ComfortLevel> ComfortLevel { get; set; }
        /// <summary>
        /// Is this Holiday recommended.
        /// </summary>
        public bool IsRecommended { get; set; }
        /// <summary>
        /// Different destinations included in holiday.
        /// </summary>
        public List<HolidayDestinations> Destinations { get; set; }
        /// <summary>
        /// what is included in the holiday. values are'Y' /'N'.
        /// </summary>
        public List<HolidayIncludes> HolidayIncludes { get; set; }
        /// <summary>
        /// Main attractions for holiday.
        /// </summary>
        public HolidayUniqueSellingPoints UniqueSellingPoints { get; set; }
        /// <summary>
        /// Information about Highlights of the holiday.
        /// </summary>
        public List<HolidayHighlights> Highlights { get; set; }
        /// <summary>
        /// Brand information of holiday.
        /// </summary>
        public List<HolidayBrandsAndBrochures> Brands { get; set; }
        /// <summary>
        /// Documment of details of holiday
        /// </summary>
        public List<HolidayBrandsAndBrochures> Brochures { get; set; }
        /// <summary>
        /// Information about images,videos for holidays
        /// </summary>
        public List<HolidayMedia> Media { get; set; }
        /// <summary>
        /// What is included in holiday.
        /// </summary>
        public List<HolidayInclusionExclusion> Inclusions { get; set; }
        /// <summary>
        /// What is excluded from package.
        /// </summary>
        public List<HolidayInclusionExclusion> Exclusions { get; set; }
        /// <summary>
        /// Terms and conditions for Holiday.
        /// </summary>
        public List<HolidayTermsConditions> TermsConditions { get; set; }
        /// <summary>
        /// policies about booking of holiday
        /// </summary>
        public List<HolidayTermsConditions> BookingPolicy { get; set; }
        /// <summary>
        /// Key notes about holiday.
        /// </summary>
        public List<HolidayTermsConditions> TourNotes { get; set; }
        ///// <summary>
        ///// Information about Starting cities for holiday
        ///// </summary>
        //public List<HolidayHubDetails> HubDetails { get; set; }
        /// <summary>
        /// Day wise itinerary information.
        /// </summary>
        public List<HolidayDayWiseItineraries> DayWiseItineraries { get; set; }
        /// <summary>
        /// Details about Accomodations provided in Holiday.
        /// </summary>
        public List<HolidayAccommodation> Accommodation { get; set; }
        /// <summary>
        /// Details about Sightseeings/Activities included in Holiday.
        /// </summary>
        public List<HolidayActivity> Activity { get; set; }
        /// <summary>
        /// Flight Details for Holiday.
        /// </summary>
        //public List<HolidayFlights> Flights { get; set; }
        /// <summary>
        /// Departure Details for Holiday.
        /// </summary>
        public List<HolidayDeparture> Departure { get; set; }
        /// <summary>
        /// Pre Tour Details.
        /// </summary>
        public List<PreTourStructure> PreTour { get; set; }
        /// <summary>
        /// Post Tour Details.
        /// </summary>
        public List<PreTourStructure> PostTour { get; set; }

        /// <summary>
        /// Holds list of PackagePrice
        /// </summary>
        public List<PackagePrice> PackagePrice { get; set; }

        /// <summary>
        /// Holds list of PreTourPrice
        /// </summary>
        public List<PreTourPrice> PreTourPrice { get; set; }

        /// <summary>
        /// Holds list of PostTourPrice
        /// </summary>
        public List<PreTourPrice> PostTourPrice { get; set; }

        /// <summary>
        /// Holds list of ClassificationAttributes
        /// </summary>
        public List<ClassificationAttributes> ClassificationAttributes { get; set; }


        ///// <summary>
        ///// Post Tour Details.
        ///// </summary>
        //public List<HolidayOptionals> Optionals { get; set; }
        ///// <summary>
        ///// Informatio about Extension to holiday.
        ///// </summary>
        //public List<HolidayExtension> Extension { get; set; }
        ///// <summary>
        ///// Information about Extension to holiday.
        ///// </summary>
        //public List<HolidayReview> Review { get; set; }
        ///// <summary>
        ///// Detailed information about Price  for holiday.
        ///// </summary>
        //public List<Price> Price { get; set; }
    }
}
