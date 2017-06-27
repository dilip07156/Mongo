using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DataContracts
{
    [DataContract]
    public class GetMasterData
    {
        List<string> _MasterRequest;
        List<Resorts> _countries;
        List<Resorts> _cities;
        List<Types_Master> _producttype_master;

        [DataMember]
        public List<string> MasterRequest
        {
            get
            {
                return _MasterRequest;
            }

            set
            {
                _MasterRequest = value;
            }
        }

        [DataMember]
        public List<Resorts> Country
        {
            get
            {
                return _countries;
            }

            set
            {
                _countries = value;
            }
        }

        [DataMember]
        public List<Resorts> City
        {
            get
            {
                return _cities;
            }

            set
            {
                _cities = value;
            }
        }

        [DataMember]
        public List<Types_Master> ProdutType_Master
        {
            get
            {
                return _producttype_master;
            }

            set
            {
                _producttype_master = value;
            }
        }
    }

    public class Resorts
    {
        Guid _resort_id;
        string _resort_name;
        Guid? _parent_resort_id;
        string _parent_resort_name;
        int _resort_type_id;
        string _resort_type;

        public Guid Resort_ID
        {
            get
            {
                return _resort_id;
            }

            set
            {
                _resort_id = value;
            }
        }

        public string Resort_Name
        {
            get
            {
                return _resort_name;
            }

            set
            {
                _resort_name = value;
            }
        }

        public Guid? Parent_Resort_ID
        {
            get
            {
                return _parent_resort_id;
            }

            set
            {
                _parent_resort_id = value;
            }
        }

        public string Parent_Resort_Name
        {
            get
            {
                return _parent_resort_name;
            }

            set
            {
                _parent_resort_name = value;
            }
        }

        public int Resort_Type_ID
        {
            get
            {
                return _resort_type_id;
            }

            set
            {
                _resort_type_id = value;
            }
        }

        public string Resort_Type
        {
            get
            {
                return _resort_type;
            }

            set
            {
                _resort_type = value;
            }
        }
    }

    public class Types_Master
    {
        Guid _type_id;
        string _type_name;
        int _int_type_id;
        string _status;

        public Guid Type_ID
        {
            get
            {
                return _type_id;
            }
            set
            {
                _type_id = value;
            }
        }

        public string Type_Namne
        {
            get
            {
                return _type_name;
            }
            set
            {
                _type_name = value;
            }
        }

        public int Type_Int_ID
        {
            get
            {
                return _int_type_id;
            }
            set
            {
                _int_type_id = value;
            }
        }

        public string Type_Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
    }
}
