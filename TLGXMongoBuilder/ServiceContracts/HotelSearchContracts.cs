using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DataContracts
{
    class HotelSearchContracts
    {

    }

    [DataContract]
    public class HotelSearchRequest
    {
        string _HotelName;
        string _HotelCode;
        string _Rating;
        string _AddressLine;
        string _PostalCode;
        string _CityName;
        string _CountryName;
        string[] _Position;

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
    }
}
