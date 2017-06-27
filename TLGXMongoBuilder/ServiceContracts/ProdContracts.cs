using System.Collections.Generic;
using System.Runtime.Serialization;

namespace DataContracts
{
    [DataContract]
    public class ProductDetails
    {
        string _Hotel_Id;
        string _ProdType;
        string _ProdName;
        string _Country;
        string _CountryCode;
        string _CityCode;
        string _City;
        string _StarRating;
        string _Address;
        string _Street;
        string _PostCode;
        string _WebSite;
        string _Contact;
        string _Email;
        string _Long;
        string _Lat;
        List<ProductCategories> _ProdCat;

        [DataMember]
        public string Hotel_Id
        {
            get
            {
                return _Hotel_Id;
            }

            set
            {
                _Hotel_Id = value;
            }
        }

        [DataMember]
        public string ProdType
        {
            get
            {
                return _ProdType;
            }

            set
            {
                _ProdType = value;
            }
        }

        [DataMember]
        public string ProdName
        {
            get
            {
                return _ProdName;
            }

            set
            {
                _ProdName = value;
            }
        }

        [DataMember]
        public string Country
        {
            get
            {
                return _Country;
            }

            set
            {
                _Country = value;
            }
        }

        [DataMember]
        public string City
        {
            get
            {
                return _City;
            }

            set
            {
                _City = value;
            }
        }

        [DataMember]
        public string StarRating
        {
            get
            {
                return _StarRating;
            }

            set
            {
                _StarRating = value;
            }
        }

        [DataMember]
        public string Address
        {
            get
            {
                return _Address;
            }

            set
            {
                _Address = value;
            }
        }

        [DataMember]
        public string WebSite
        {
            get
            {
                return _WebSite;
            }

            set
            {
                _WebSite = value;
            }
        }

        [DataMember]
        public List<ProductCategories> ProdCat
        {
            get
            {
                return _ProdCat;
            }

            set
            {
                _ProdCat = value;
            }
        }

        [DataMember]
        public string Street
        {
            get
            {
                return _Street;
            }

            set
            {
                _Street = value;
            }
        }

        [DataMember]
        public string PostCode
        {
            get
            {
                return _PostCode;
            }

            set
            {
                _PostCode = value;
            }
        }

        [DataMember]
        public string CountryCode
        {
            get
            {
                return _CountryCode;
            }

            set
            {
                _CountryCode = value;
            }
        }

        [DataMember]
        public string CityCode
        {
            get
            {
                return _CityCode;
            }

            set
            {
                _CityCode = value;
            }
        }

        [DataMember]
        public string Long
        {
            get
            {
                return _Long;
            }

            set
            {
                _Long = value;
            }
        }

        [DataMember]
        public string Lat
        {
            get
            {
                return _Lat;
            }

            set
            {
                _Lat = value;
            }
        }

        [DataMember]
        public string Contact
        {
            get
            {
                return _Contact;
            }

            set
            {
                _Contact = value;
            }
        }

        [DataMember]
        public string Email
        {
            get
            {
                return _Email;
            }

            set
            {
                _Email = value;
            }
        }
    }

    public class ProductCategories
    {
        string _CategoryID;
        string _RoomCategory;
        string _RoomName;

        public string CategoryID
        {
            get
            {
                return _CategoryID;
            }

            set
            {
                _CategoryID = value;
            }
        }

        public string RoomCategory
        {
            get
            {
                return _RoomCategory;
            }

            set
            {
                _RoomCategory = value;
            }
        }

        public string RoomName
        {
            get
            {
                return _RoomName;
            }

            set
            {
                _RoomName = value;
            }
        }
    }

}
