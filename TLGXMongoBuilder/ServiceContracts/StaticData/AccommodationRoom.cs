using MongoDB.Bson;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts.StaticData
{
    public class AccomodationRoom
    {
        public ObjectId _id { get; set; }
        public string AccomodationId { get; set; }
        public bool IsMysteryRoom { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Category { get; set; }

        public string View { get; set; }

        [JsonProperty(Required = Required.Always)]
        public int NoOfRooms { get; set; }

        public int NoOfInterconnectingRooms { get; set; }
        public string Name { get; set; }
        public string NumberAndNameOfUnit { get; set; }
        public string FloorsNameAndNameOfUnit { get; set; }
        public string NumberAndNameOfRoom { get; set; }
        public Amenities Amenities { get; set; }
        public string CompanyRoomCategory { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string RoomDescription { get; set; }

        public int RoomSizeInSquareFeet { get; set; }
        public int RoomSizeInSquareMeter { get; set; }
        public string BathroomType { get; set; }
        public string BedType { get; set; }
        public string BedTypeByUnit { get; set; }
        public string RoomDecor { get; set; }
        public string RoomSubType { get; set; }
        public string FloorName { get; set; }
        public int FloorNo { get; set; }
    }

    public class Amenities
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Desc { get; set; }
        public bool IsChargeable { get; set; }
    }
}
