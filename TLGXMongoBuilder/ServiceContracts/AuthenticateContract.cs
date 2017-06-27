using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace DataContracts
{
    [DataContract]
    public class Credentials
    {
        string _User;
        string _Password;
        [DataMember]
        public string User
        {
            get
            {
                return _User;
            }

            set
            {
                _User = value;
            }
        }
        [DataMember]
        public string Password
        {
            get
            {
                return _Password;
            }

            set
            {
                _Password = value;
            }
        }
    }


    //[MongoDB.Bson.Serialization.Attributes.BsonIgnoreExtraElements]
    [DataContract]
    public class TokenContract
    {
        ObjectId _Id;
        string _UserID;
        string _UserName;
        string _Token;
        BsonDateTime _ExpiryDate;

        [DataMember]
        [BsonId]
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
        public string Token
        {
            get
            {
                return _Token;
            }

            set
            {
                _Token = value;
            }
        }

        [DataMember]
        public string UserID
        {
            get
            {
                return _UserID;
            }

            set
            {
                _UserID = value;
            }
        }

        [DataMember]
        public string UserName
        {
            get
            {
                return _UserName;
            }

            set
            {
                _UserName = value;
            }
        }

        [DataMember]
        //[BsonDateTimeOptions(Kind = DateTimeKind.Local, Representation = BsonType.DateTime)]
        public BsonDateTime ExpiryDate
        {
            get
            {
                return _ExpiryDate;
            }

            set
            {
                _ExpiryDate = value;
            }
        }

        
    }

    [DataContract]
    public class UserRoles
    {
        Guid _RoleId;
        string _RoleName;

        [DataMember]
        public Guid RoleId
        {
            get
            {
                return _RoleId;
            }

            set
            {
                _RoleId = value;
            }
        }

        [DataMember]
        public string RoleName
        {
            get
            {
                return _RoleName;
            }

            set
            {
                _RoleName = value;
            }
        }
    }
}
