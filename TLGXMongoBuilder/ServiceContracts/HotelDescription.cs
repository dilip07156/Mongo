using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace DataContracts
{
    class HotelDescription
    {
    }

    [DataContract]
    public class HotelDescriptiveInfo
    {
        [BsonId]
        ObjectId _Id;

        string _HotelCode;
        string _HotelName;
        string _Rating;
        string _URLs;
        string _AddressLine;
        string _PostalCode;
        string _CityName;
        string _CountryName;
        string[] _Position;
        string _PhoneNumber;

        [DataMember]
        public ObjectId Id
        {
            get
            {
                return _Id;
            }

            set
            {
                _Id = value;
            }
        }

        [DataMember]
        public string HotelCode
        {
            get
            {
                return _HotelCode;
            }

            set
            {
                _HotelCode = value;
            }
        }

        [DataMember]
        public string HotelName
        {
            get
            {
                return _HotelName;
            }

            set
            {
                _HotelName = value;
            }
        }

        [DataMember]
        public string Rating
        {
            get
            {
                return _Rating;
            }

            set
            {
                _Rating = value;
            }
        }

        [DataMember]
        public string URLs
        {
            get
            {
                return _URLs;
            }

            set
            {
                _URLs = value;
            }
        }

        [DataMember]
        public string AddressLine
        {
            get
            {
                return _AddressLine;
            }

            set
            {
                _AddressLine = value;
            }
        }

        [DataMember]
        public string PostalCode
        {
            get
            {
                return _PostalCode;
            }

            set
            {
                _PostalCode = value;
            }
        }

        [DataMember]
        public string CityName
        {
            get
            {
                return _CityName;
            }

            set
            {
                _CityName = value;
            }
        }

        [DataMember]
        public string CountryName
        {
            get
            {
                return _CountryName;
            }

            set
            {
                _CountryName = value;
            }
        }

        [DataMember]
        public string[] Position
        {
            get
            {
                return _Position;
            }

            set
            {
                _Position = value;
            }
        }

        [DataMember]
        public string PhoneNumber
        {
            get
            {
                return _PhoneNumber;
            }

            set
            {
                _PhoneNumber = value;
            }
        }
    }


    /// <remarks/>
    [BsonIgnoreExtraElements]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class HotelsHotel
    {

        private HotelsHotelStarRating starRatingField;

        private HotelsHotelAddress addressField;

        private HotelsHotelImage[] imageField;

        //private object videoField;

        private HotelsHotelFacility[] hotelFacilityField;

        private HotelsHotelHotelAmenity hotelAmenityField;

        private HotelsHotelHotelDistance hotelDistanceField;

        private string supplierHotelIDField;

        private string hotelIdField;

        private string nameField;

        private string credicardsField;

        private string areatransportationField;

        private string restaurantsField;

        private string meetingfacilityField;

        private string descriptionField;

        private string highlightField;

        private string overviewField;

        private string checkintimeField;

        private string checkouttimeField;

        private string emailField;

        private string websiteField;

        private string roomsField;

        private string landmarkCategoryField;

        private string landmarkField;

        private string themeField;

        private string hotelChainField;

        private string brandNameField;

        private string recommendsField;

        private string latitudeField;

        private string longitudeField;

        private string landmarkDescriptionField;

        private string thumbField;

        /// <remarks/>
        public HotelsHotelStarRating StarRating
        {
            get
            {
                return this.starRatingField;
            }
            set
            {
                this.starRatingField = value;
            }
        }

        /// <remarks/>
        public HotelsHotelAddress Address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("image")]
        public HotelsHotelImage[] image
        {
            get
            {
                return this.imageField;
            }
            set
            {
                this.imageField = value;
            }
        }

        /// <remarks/>
        //public object video
        //{
        //    get
        //    {
        //        return this.videoField;
        //    }
        //    set
        //    {
        //        this.videoField = value;
        //    }
        //}

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("Facility", IsNullable = false)]
        public HotelsHotelFacility[] HotelFacility
        {
            get
            {
                return this.hotelFacilityField;
            }
            set
            {
                this.hotelFacilityField = value;
            }
        }

        /// <remarks/>
        public HotelsHotelHotelAmenity HotelAmenity
        {
            get
            {
                return this.hotelAmenityField;
            }
            set
            {
                this.hotelAmenityField = value;
            }
        }

        /// <remarks/>
        public HotelsHotelHotelDistance HotelDistance
        {
            get
            {
                return this.hotelDistanceField;
            }
            set
            {
                this.hotelDistanceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string SupplierHotelID
        {
            get
            {
                return this.supplierHotelIDField;
            }
            set
            {
                this.supplierHotelIDField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string HotelId
        {
            get
            {
                return this.hotelIdField;
            }
            set
            {
                this.hotelIdField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string credicards
        {
            get
            {
                return this.credicardsField;
            }
            set
            {
                this.credicardsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string areatransportation
        {
            get
            {
                return this.areatransportationField;
            }
            set
            {
                this.areatransportationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string restaurants
        {
            get
            {
                return this.restaurantsField;
            }
            set
            {
                this.restaurantsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string meetingfacility
        {
            get
            {
                return this.meetingfacilityField;
            }
            set
            {
                this.meetingfacilityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string description
        {
            get
            {
                return this.descriptionField;
            }
            set
            {
                this.descriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string highlight
        {
            get
            {
                return this.highlightField;
            }
            set
            {
                this.highlightField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string overview
        {
            get
            {
                return this.overviewField;
            }
            set
            {
                this.overviewField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string checkintime
        {
            get
            {
                return this.checkintimeField;
            }
            set
            {
                this.checkintimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string checkouttime
        {
            get
            {
                return this.checkouttimeField;
            }
            set
            {
                this.checkouttimeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string email
        {
            get
            {
                return this.emailField;
            }
            set
            {
                this.emailField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string website
        {
            get
            {
                return this.websiteField;
            }
            set
            {
                this.websiteField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string rooms
        {
            get
            {
                return this.roomsField;
            }
            set
            {
                this.roomsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LandmarkCategory
        {
            get
            {
                return this.landmarkCategoryField;
            }
            set
            {
                this.landmarkCategoryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Landmark
        {
            get
            {
                return this.landmarkField;
            }
            set
            {
                this.landmarkField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string theme
        {
            get
            {
                return this.themeField;
            }
            set
            {
                this.themeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string HotelChain
        {
            get
            {
                return this.hotelChainField;
            }
            set
            {
                this.hotelChainField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string BrandName
        {
            get
            {
                return this.brandNameField;
            }
            set
            {
                this.brandNameField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string recommends
        {
            get
            {
                return this.recommendsField;
            }
            set
            {
                this.recommendsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string latitude
        {
            get
            {
                return this.latitudeField;
            }
            set
            {
                this.latitudeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string longitude
        {
            get
            {
                return this.longitudeField;
            }
            set
            {
                this.longitudeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string LandmarkDescription
        {
            get
            {
                return this.landmarkDescriptionField;
            }
            set
            {
                this.landmarkDescriptionField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string thumb
        {
            get
            {
                return this.thumbField;
            }
            set
            {
                this.thumbField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public partial class HotelsHotelStarRating
    {

        private string levelField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string Level
        {
            get
            {
                return this.levelField;
            }
            set
            {
                this.levelField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class HotelsHotelAddress
    {

        private string addressField;

        private string cityField;

        private string stateField;

        private string countryField;

        private string pincodeField;

        private string locationField;

        private string phoneField;

        private string faxField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string address
        {
            get
            {
                return this.addressField;
            }
            set
            {
                this.addressField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string city
        {
            get
            {
                return this.cityField;
            }
            set
            {
                this.cityField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string state
        {
            get
            {
                return this.stateField;
            }
            set
            {
                this.stateField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string country
        {
            get
            {
                return this.countryField;
            }
            set
            {
                this.countryField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string pincode
        {
            get
            {
                return this.pincodeField;
            }
            set
            {
                this.pincodeField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string location
        {
            get
            {
                return this.locationField;
            }
            set
            {
                this.locationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string phone
        {
            get
            {
                return this.phoneField;
            }
            set
            {
                this.phoneField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string fax
        {
            get
            {
                return this.faxField;
            }
            set
            {
                this.faxField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class HotelsHotelImage
    {

        private string pathField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string path
        {
            get
            {
                return this.pathField;
            }
            set
            {
                this.pathField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class HotelsHotelFacility
    {

        private string nameField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string name
        {
            get
            {
                return this.nameField;
            }
            set
            {
                this.nameField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class HotelsHotelHotelAmenity
    {

        private bool restaurantField;

        private bool conferenceField;

        private bool fitnessField;

        private bool travelField;

        private bool forexField;

        private bool shoppingField;

        private bool banquetField;

        private bool gamesField;

        private bool barField;

        private bool coffee_ShopField;

        private bool room_ServiceField;

        private bool internet_AccessField;

        private bool business_CentreField;

        private bool swimming_PoolField;

        private bool petsField;

        private bool tennis_CourtField;

        private bool golfField;

        private bool air_ConditioningField;

        private bool parkingField;

        private bool wheel_ChairField;

        private bool health_ClubField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Restaurant
        {
            get
            {
                return this.restaurantField;
            }
            set
            {
                this.restaurantField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool conference
        {
            get
            {
                return this.conferenceField;
            }
            set
            {
                this.conferenceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool fitness
        {
            get
            {
                return this.fitnessField;
            }
            set
            {
                this.fitnessField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool travel
        {
            get
            {
                return this.travelField;
            }
            set
            {
                this.travelField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool forex
        {
            get
            {
                return this.forexField;
            }
            set
            {
                this.forexField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool shopping
        {
            get
            {
                return this.shoppingField;
            }
            set
            {
                this.shoppingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool banquet
        {
            get
            {
                return this.banquetField;
            }
            set
            {
                this.banquetField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool games
        {
            get
            {
                return this.gamesField;
            }
            set
            {
                this.gamesField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Bar
        {
            get
            {
                return this.barField;
            }
            set
            {
                this.barField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Coffee_Shop
        {
            get
            {
                return this.coffee_ShopField;
            }
            set
            {
                this.coffee_ShopField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Room_Service
        {
            get
            {
                return this.room_ServiceField;
            }
            set
            {
                this.room_ServiceField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Internet_Access
        {
            get
            {
                return this.internet_AccessField;
            }
            set
            {
                this.internet_AccessField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Business_Centre
        {
            get
            {
                return this.business_CentreField;
            }
            set
            {
                this.business_CentreField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Swimming_Pool
        {
            get
            {
                return this.swimming_PoolField;
            }
            set
            {
                this.swimming_PoolField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Pets
        {
            get
            {
                return this.petsField;
            }
            set
            {
                this.petsField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Tennis_Court
        {
            get
            {
                return this.tennis_CourtField;
            }
            set
            {
                this.tennis_CourtField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Golf
        {
            get
            {
                return this.golfField;
            }
            set
            {
                this.golfField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Air_Conditioning
        {
            get
            {
                return this.air_ConditioningField;
            }
            set
            {
                this.air_ConditioningField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Parking
        {
            get
            {
                return this.parkingField;
            }
            set
            {
                this.parkingField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Wheel_Chair
        {
            get
            {
                return this.wheel_ChairField;
            }
            set
            {
                this.wheel_ChairField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public bool Health_Club
        {
            get
            {
                return this.health_ClubField;
            }
            set
            {
                this.health_ClubField = value;
            }
        }
    }

    /// <remarks/>
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType = true)]
    public class HotelsHotelHotelDistance
    {

        private string distancefromAirportField;

        private string distancefromStationField;

        private string distancefromBusField;

        private string distancefromCityCenterField;

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DistancefromAirport
        {
            get
            {
                return this.distancefromAirportField;
            }
            set
            {
                this.distancefromAirportField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DistancefromStation
        {
            get
            {
                return this.distancefromStationField;
            }
            set
            {
                this.distancefromStationField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DistancefromBus
        {
            get
            {
                return this.distancefromBusField;
            }
            set
            {
                this.distancefromBusField = value;
            }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string DistancefromCityCenter
        {
            get
            {
                return this.distancefromCityCenterField;
            }
            set
            {
                this.distancefromCityCenterField = value;
            }
        }
    }

}
