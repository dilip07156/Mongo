using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts.Masters
{
    [DataContract]
    [BsonIgnoreExtraElements]
    public class DC_AccomodationRoomInfo
    {
        [BsonId]
        [DataMember]
        public string CommonRoomId { get; set; }

        [DataMember]
        public int? CommonHotelId { get; set; }

        [DataMember]
        public string RoomView { get; set; }

        [DataMember]
        public int? NoOfRooms { get; set; }

        [DataMember]
        public string RoomName { get; set; }

        [DataMember]
        public string Smoking { get; set; }

        [DataMember]
        public string BathRoomType { get; set; }

        [DataMember]
        public string BedType { get; set; }

        [DataMember]
        public string CompanyRoomCategory { get; set; }

        [DataMember]
        public string RoomCategory { get; set; }

        [DataMember]
        public string Category { get; set; }

       

        [BsonIgnore]
        [DataMember]
        public Guid Accommodation_RoomInfo_Id { get; set; }


        [DataMember]
        public List<DC_AccomodationRoomInfoCompanyVersion> AccomodationRoomInfoCompanyVersions { get; set; }
        /* 
        Child Table
        
  */
    }
    [DataContract]
    [BsonIgnoreExtraElements]
    public class DC_AccomodationRoomInfoCompanyVersion
    {
        [DataMember]
        public string CompanyId { get; set; }

        [DataMember]
        public string CompanyName { get; set; }

        [DataMember]
        public string TLGXAccoRoomID { get; set; }

        [DataMember]
        public string TLGXAccoId { get; set; }

        [DataMember]
        public string RoomView { get; set; }

        [DataMember]
        public string RoomDescription { get; set; }

        [DataMember]
        public string RoomName { get; set; }

        [DataMember]
        public string Smoking { get; set; }


        [DataMember]
        public string BedType { get; set; }

        [DataMember]
        public string CompanyRoomCategory { get; set; }

        [DataMember]
        public string RoomCategory { get; set; }
    }
}
