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
        
        public string TLGXAccoId { get; set; }
        [BsonIgnore]
        public Guid Accommodation_SupplierRoomTypeMapping_Id { get; set; }
        
        public string TLGXAccoRoomId { get; set; }
        
        public string supplierCode { get; set; }
        
        public string SupplierProductId { get; set; }
        
        public string SupplierRoomId { get; set; }
        
        public string SupplierRoomTypeCode { get; set; }
        
        public string SupplierRoomName { get; set; }
        
        public string SupplierRoomCategory { get; set; }
        
        public string SupplierRoomCategoryId { get; set; }
        
        public int? MaxAdults { get; set; }
        
        public int? MaxChild { get; set; }
        
        public int? MaxInfants { get; set; }
        
        public int? MaxGuestOccupancy { get; set; }
        
        public int? Quantity { get; set; }
        
        public string RatePlan { get; set; }
        
        public string RatePlanCode { get; set; }
        
        public string RoomSize { get; set; }
        
        public string BathRoomType { get; set; }
        
        public string RoomView { get; set; }
        
        public string FloorName { get; set; }
        
        public int? FloorNumber { get; set; }
        
        public string Amenities { get; set; }
        
        public string RoomLocationCode { get; set; }
        
        public int? ChildAge { get; set; }
        
        public string ExtraBed { get; set; }
        
        public string Bedrooms { get; set; }
        
        public string Smoking { get; set; }
        
        public string BedType { get; set; }
        
        public int? MinGuestOccupancy { get; set; }
        
        public string PromotionalVendorCode { get; set; }
        
        public string BeddingConfig { get; set; }
        
        public string RoomDescription { get; set; }
        
        public int SystemRoomTypeMapId { get; set; }
        
        public int? SystemProductCode { get; set; }
        
        public string SystemRoomTypeCode { get; set; }
        
        public string SystemRoomTypeName { get; set; }
        
        public string SystemNormalizedRoomType { get; set; }
        
        public string SystemStrippedRoomType { get; set; }
        
        public List<DC_RoomTypeMapping_Attributes_HRTM> Attibutes { get; set; }
        
        public string Status { get; set; }
        
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
        
        public string TLGXAccoId { get; set; }
        [BsonIgnore]
        public Guid Accommodation_SupplierRoomTypeMapping_Id { get; set; }
        
        public string TLGXAccoRoomId { get; set; }
        
        public string supplierCode { get; set; }
        
        public string SupplierProductId { get; set; }
        
        public string SupplierRoomId { get; set; }
        
        public string SupplierRoomTypeCode { get; set; }
        
        public string SupplierRoomName { get; set; }
        
        public string SupplierRoomCategory { get; set; }
        
        public string SupplierRoomCategoryId { get; set; }
        
        public int? MaxAdults { get; set; }
        
        public int? MaxChild { get; set; }
        
        public int? MaxInfants { get; set; }
        
        public int? MaxGuestOccupancy { get; set; }
        
        public int? Quantity { get; set; }
        
        public string RatePlan { get; set; }
        
        public string RatePlanCode { get; set; }
        
        public string RoomSize { get; set; }
        
        public string BathRoomType { get; set; }
        
        public string RoomView { get; set; }
        
        public string FloorName { get; set; }
        
        public int? FloorNumber { get; set; }
        
        public string Amenities { get; set; }
        
        public string RoomLocationCode { get; set; }
        
        public int? ChildAge { get; set; }
        
        public string ExtraBed { get; set; }
        
        public string Bedrooms { get; set; }
        
        public string Smoking { get; set; }
        
        public string BedType { get; set; }
        
        public int? MinGuestOccupancy { get; set; }
        
        public string PromotionalVendorCode { get; set; }
        
        public string BeddingConfig { get; set; }
        
        public string RoomDescription { get; set; }
        
        public int SystemRoomTypeMapId { get; set; }
        
        public int? SystemProductCode { get; set; }
        
        public string SystemRoomTypeCode { get; set; }
        
        public string SystemRoomTypeName { get; set; }
        
        public string SystemNormalizedRoomType { get; set; }
        
        public string SystemStrippedRoomType { get; set; }
        
        public List<DC_RoomTypeMapping_Attributes_HRTM_IM> Attibutes { get; set; }
        
        public string Status { get; set; }
        
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
