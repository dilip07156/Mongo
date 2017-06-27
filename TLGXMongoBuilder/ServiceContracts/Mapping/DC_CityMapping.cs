using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace DataContracts.Mapping
{
    [DataContract]
    [BsonIgnoreExtraElements]
    public class DC_CityMapping
    {
        //string _CityMapping_Id;
        string _CountryName;
        string _CountryCode;
        string _CityName;
        string _CityCode;
        string _SupplierName;
        string _SupplierCode;
        string _SupplierCountryName;
        string _SupplierCountryCode;
        string _SupplierCityName;
        string _SupplierCityCode;
        int _MapId;

        //[DataMember]
        //public string CityMapping_Id
        //{
        //    get
        //    {
        //        return _CityMapping_Id;
        //    }

        //    set
        //    {
        //        _CityMapping_Id = value;
        //    }
        //}

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
        public string SupplierName
        {
            get
            {
                return _SupplierName;
            }

            set
            {
                _SupplierName = value;
            }
        }

        [DataMember]
        public string SupplierCode
        {
            get
            {
                return _SupplierCode;
            }

            set
            {
                _SupplierCode = value;
            }
        }

        [DataMember]
        public string SupplierCountryName
        {
            get
            {
                return _SupplierCountryName;
            }

            set
            {
                _SupplierCountryName = value;
            }
        }

        [DataMember]
        public string SupplierCountryCode
        {
            get
            {
                return _SupplierCountryCode;
            }

            set
            {
                _SupplierCountryCode = value;
            }
        }

        [DataMember]
        public string SupplierCityName
        {
            get
            {
                return _SupplierCityName;
            }

            set
            {
                _SupplierCityName = value;
            }
        }

        [DataMember]
        public string SupplierCityCode
        {
            get
            {
                return _SupplierCityCode;
            }

            set
            {
                _SupplierCityCode = value;
            }
        }

        [DataMember]
        public int MapId
        {
            get
            {
                return _MapId;
            }

            set
            {
                _MapId = value;
            }
        }
    }

    [DataContract]
    public class CityMappingRQ
    {
        string _SourceCode;
        string _TargetCode;
        string _SourceCountryCode;
        string _SourceCountryName;
        string _SourceCityCode;
        string _SourceCityName;

        [DataMember]
        public string SourceCode
        {
            get
            {
                return _SourceCode;
            }

            set
            {
                _SourceCode = value;
            }
        }

        [DataMember]
        public string TargetCode
        {
            get
            {
                return _TargetCode;
            }

            set
            {
                _TargetCode = value;
            }
        }

        [DataMember]
        public string SourceCountryCode
        {
            get
            {
                return _SourceCountryCode;
            }

            set
            {
                _SourceCountryCode = value;
            }
        }

        [DataMember]
        public string SourceCountryName
        {
            get
            {
                return _SourceCountryName;
            }

            set
            {
                _SourceCountryName = value;
            }
        }

        [DataMember]
        public string SourceCityCode
        {
            get
            {
                return _SourceCityCode;
            }

            set
            {
                _SourceCityCode = value;
            }
        }

        [DataMember]
        public string SourceCityName
        {
            get
            {
                return _SourceCityName;
            }

            set
            {
                _SourceCityName = value;
            }
        }
    }

    [DataContract]
    public class CityMappingRS
    {
        string _SourceCode;
        string _SourceName;
        string _SourceCountryCode;
        string _SourceCountryName;
        string _SourceCityCode;
        string _SourceCityName;
        string _TargetCode;
        string _TargetName;
        string _TargetCountryCode;
        string _TargetCountryName;
        string _TargetCityCode;
        string _TargetCityName;

        [DataMember]
        public string SourceCode
        {
            get
            {
                return _SourceCode;
            }

            set
            {
                _SourceCode = value;
            }
        }

        [DataMember]
        public string SourceName
        {
            get
            {
                return _SourceName;
            }

            set
            {
                _SourceName = value;
            }
        }

        [DataMember]
        public string SourceCountryCode
        {
            get
            {
                return _SourceCountryCode;
            }

            set
            {
                _SourceCountryCode = value;
            }
        }

        [DataMember]
        public string SourceCountryName
        {
            get
            {
                return _SourceCountryName;
            }

            set
            {
                _SourceCountryName = value;
            }
        }

        [DataMember]
        public string SourceCityCode
        {
            get
            {
                return _SourceCityCode;
            }

            set
            {
                _SourceCityCode = value;
            }
        }

        [DataMember]
        public string SourceCityName
        {
            get
            {
                return _SourceCityName;
            }

            set
            {
                _SourceCityName = value;
            }
        }

        [DataMember]
        public string TargetCode
        {
            get
            {
                return _TargetCode;
            }

            set
            {
                _TargetCode = value;
            }
        }

        [DataMember]
        public string TargetName
        {
            get
            {
                return _TargetName;
            }

            set
            {
                _TargetName = value;
            }
        }

        [DataMember]
        public string TargetCountryCode
        {
            get
            {
                return _TargetCountryCode;
            }

            set
            {
                _TargetCountryCode = value;
            }
        }

        [DataMember]
        public string TargetCountryName
        {
            get
            {
                return _TargetCountryName;
            }

            set
            {
                _TargetCountryName = value;
            }
        }

        [DataMember]
        public string TargetCityCode
        {
            get
            {
                return _TargetCityCode;
            }

            set
            {
                _TargetCityCode = value;
            }
        }

        [DataMember]
        public string TargetCityName
        {
            get
            {
                return _TargetCityName;
            }

            set
            {
                _TargetCityName = value;
            }
        }
    }
}
