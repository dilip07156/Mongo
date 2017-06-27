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
    public class DC_CrossSystemMapping
    {
        string _Source_System_Name;
        string _Source_System_Code;
        string _Source_Entity;
        string _Source_Value_Code;
        string _Source_Value_Name;
        string _Target_System_Name;
        string _Target_System_Code;
        string _Target_Value_Code;
        string _Target_Value_Name;

        [DataMember]
        public string Source_System_Name
        {
            get
            {
                return _Source_System_Name;
            }

            set
            {
                _Source_System_Name = value;
            }
        }

        [DataMember]
        public string Source_System_Code
        {
            get
            {
                return _Source_System_Code;
            }

            set
            {
                _Source_System_Code = value;
            }
        }

        [DataMember]
        public string Source_Entity
        {
            get
            {
                return _Source_Entity;
            }

            set
            {
                _Source_Entity = value;
            }
        }

        [DataMember]
        public string Source_Value_Code
        {
            get
            {
                return _Source_Value_Code;
            }

            set
            {
                _Source_Value_Code = value;
            }
        }

        [DataMember]
        public string Source_Value_Name
        {
            get
            {
                return _Source_Value_Name;
            }

            set
            {
                _Source_Value_Name = value;
            }
        }

        [DataMember]
        public string Target_System_Name
        {
            get
            {
                return _Target_System_Name;
            }

            set
            {
                _Target_System_Name = value;
            }
        }

        [DataMember]
        public string Target_System_Code
        {
            get
            {
                return _Target_System_Code;
            }

            set
            {
                _Target_System_Code = value;
            }
        }

        [DataMember]
        public string Target_Value_Code
        {
            get
            {
                return _Target_Value_Code;
            }

            set
            {
                _Target_Value_Code = value;
            }
        }

        [DataMember]
        public string Target_Value_Name
        {
            get
            {
                return _Target_Value_Name;
            }

            set
            {
                _Target_Value_Name = value;
            }
        }
    }
}
