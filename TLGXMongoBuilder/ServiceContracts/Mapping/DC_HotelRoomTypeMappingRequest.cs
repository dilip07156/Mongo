using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts.Mapping
{
    [BsonIgnoreExtraElements]
    public class DC_HotelRoomTypeMappingRequest
    {
        //[BsonIgnoreIfNull]
        public string TLGXAccoId { get; set; }
        [BsonIgnore]
        public Guid Accommodation_SupplierRoomTypeMapping_Id { get; set; }
        //[BsonIgnoreIfNull]
        public string TLGXAccoRoomId { get; set; }
        //[BsonIgnoreIfNull]
        public string supplierCode { get; set; }
        //[BsonIgnoreIfNull]
        public string SupplierProductId { get; set; }
        //[BsonIgnoreIfNull]
        public string SupplierRoomId { get; set; }
        //[BsonIgnoreIfNull]
        public string SupplierRoomTypeCode { get; set; }
        //[BsonIgnoreIfNull]
        public string SupplierRoomName { get; set; }
        //[BsonIgnoreIfNull]
        public string SupplierRoomCategory { get; set; }
        //[BsonIgnoreIfNull]
        public string SupplierRoomCategoryId { get; set; }
        //[BsonIgnoreIfNull]
        public int? MaxAdults { get; set; }
        //[BsonIgnoreIfNull]
        public int? MaxChild { get; set; }
        //[BsonIgnoreIfNull]
        public int? MaxInfants { get; set; }
        //[BsonIgnoreIfNull]
        public int? MaxGuestOccupancy { get; set; }
        //[BsonIgnoreIfNull]
        public int? Quantity { get; set; }
        //[BsonIgnoreIfNull]
        public string RatePlan { get; set; }
        //[BsonIgnoreIfNull]
        public string RatePlanCode { get; set; }
        //[BsonIgnoreIfNull]
        public string RoomSize { get; set; }
        //[BsonIgnoreIfNull]
        public string BathRoomType { get; set; }
        //[BsonIgnoreIfNull]
        public string RoomView { get; set; }
        //[BsonIgnoreIfNull]
        public string FloorName { get; set; }
        //[BsonIgnoreIfNull]
        public int? FloorNumber { get; set; }
        //[BsonIgnoreIfNull]
        public string Amenities { get; set; }
        //[BsonIgnoreIfNull]
        public string RoomLocationCode { get; set; }
        //[BsonIgnoreIfNull]
        public int? ChildAge { get; set; }
        //[BsonIgnoreIfNull]
        public string ExtraBed { get; set; }
        //[BsonIgnoreIfNull]
        public string Bedrooms { get; set; }
        //[BsonIgnoreIfNull]
        public string Smoking { get; set; }
        //[BsonIgnoreIfNull]
        public string BedType { get; set; }
        //[BsonIgnoreIfNull]
        public int? MinGuestOccupancy { get; set; }
        //[BsonIgnoreIfNull]
        public string PromotionalVendorCode { get; set; }
        //[BsonIgnoreIfNull]
        public string BeddingConfig { get; set; }
        //[BsonIgnoreIfNull]
        public string RoomDescription { get; set; }
        //[BsonIgnoreIfNull]
        public int SystemRoomTypeMapId { get; set; }
        //[BsonIgnoreIfNull]
        public int? SystemProductCode { get; set; }
        //[BsonIgnoreIfNull]
        public string SystemRoomTypeCode { get; set; }
        //[BsonIgnoreIfNull]
        public string SystemRoomTypeName { get; set; }
        //[BsonIgnoreIfNull]
        public string SystemNormalizedRoomType { get; set; }
        //[BsonIgnoreIfNull]
        public string SystemStrippedRoomType { get; set; }
        //[BsonIgnoreIfNull]
        public List<DC_RoomTypeMapping_Attributes_HRTM> Attibutes { get; set; }
        //[BsonIgnoreIfNull]
        public string Status { get; set; }
        //[BsonIgnoreIfNull]
        public double? MatchingScore { get; set; }
    }
   public class DC_RoomTypeMapping_Attributes_HRTM
    {
        [BsonIgnore]
        public Guid? RoomTypeMap_Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }



    public class DC_HotelRoomTypeMappingRequest_IM
    {
        //[BsonIgnoreIfNull]
        public string TLGXAccoId { get; set; }
        [BsonIgnore]
        public Guid Accommodation_SupplierRoomTypeMapping_Id { get; set; }
        //[BsonIgnoreIfNull]
        public string TLGXAccoRoomId { get; set; }
        //[BsonIgnoreIfNull]
        public string supplierCode { get; set; }
        //[BsonIgnoreIfNull]
        public string SupplierProductId { get; set; }
        //[BsonIgnoreIfNull]
        public string SupplierRoomId { get; set; }
        //[BsonIgnoreIfNull]
        public string SupplierRoomTypeCode { get; set; }
        //[BsonIgnoreIfNull]
        public string SupplierRoomName { get; set; }
        //[BsonIgnoreIfNull]
        public string SupplierRoomCategory { get; set; }
        //[BsonIgnoreIfNull]
        public string SupplierRoomCategoryId { get; set; }
        //[BsonIgnoreIfNull]
        public int? MaxAdults { get; set; }
        //[BsonIgnoreIfNull]
        public int? MaxChild { get; set; }
        //[BsonIgnoreIfNull]
        public int? MaxInfants { get; set; }
        //[BsonIgnoreIfNull]
        public int? MaxGuestOccupancy { get; set; }
        //[BsonIgnoreIfNull]
        public int? Quantity { get; set; }
        //[BsonIgnoreIfNull]
        public string RatePlan { get; set; }
        //[BsonIgnoreIfNull]
        public string RatePlanCode { get; set; }
        //[BsonIgnoreIfNull]
        public string RoomSize { get; set; }
        //[BsonIgnoreIfNull]
        public string BathRoomType { get; set; }
        //[BsonIgnoreIfNull]
        public string RoomView { get; set; }
        //[BsonIgnoreIfNull]
        public string FloorName { get; set; }
        //[BsonIgnoreIfNull]
        public int? FloorNumber { get; set; }
        //[BsonIgnoreIfNull]
        public string Amenities { get; set; }
        //[BsonIgnoreIfNull]
        public string RoomLocationCode { get; set; }
        //[BsonIgnoreIfNull]
        public int? ChildAge { get; set; }
        //[BsonIgnoreIfNull]
        public string ExtraBed { get; set; }
        //[BsonIgnoreIfNull]
        public string Bedrooms { get; set; }
        //[BsonIgnoreIfNull]
        public string Smoking { get; set; }
        //[BsonIgnoreIfNull]
        public string BedType { get; set; }
        //[BsonIgnoreIfNull]
        public int? MinGuestOccupancy { get; set; }
        //[BsonIgnoreIfNull]
        public string PromotionalVendorCode { get; set; }
        //[BsonIgnoreIfNull]
        public string BeddingConfig { get; set; }
        //[BsonIgnoreIfNull]
        public string RoomDescription { get; set; }
        //[BsonIgnoreIfNull]
        public int SystemRoomTypeMapId { get; set; }
        //[BsonIgnoreIfNull]
        public int? SystemProductCode { get; set; }
        //[BsonIgnoreIfNull]
        public string SystemRoomTypeCode { get; set; }
        //[BsonIgnoreIfNull]
        public string SystemRoomTypeName { get; set; }
        //[BsonIgnoreIfNull]
        public string SystemNormalizedRoomType { get; set; }
        //[BsonIgnoreIfNull]
        public string SystemStrippedRoomType { get; set; }
        //[BsonIgnoreIfNull]
        public List<DC_RoomTypeMapping_Attributes_HRTM_IM> Attibutes { get; set; }
        //[BsonIgnoreIfNull]
        public string Status { get; set; }
        //[BsonIgnoreIfNull]
        public double? MatchingScore { get; set; }
    }

    public class DC_RoomTypeMapping_Attributes_HRTM_IM
    {
        [BsonIgnore]
        public Guid? RoomTypeMap_Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
