using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using DataContracts;
using Newtonsoft.Json;
using ServiceContracts;
using System.Security.Permissions;
using MongoDB.Bson;
using System.IO;

namespace MapperSvc
{
    public partial class MapperSvc : IMapSvs
    {
        #region Activity Mapping & Lite
        public void Insert_ActivityMapping_ByMapId(string MapId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Insert_ActivityMapping_ByMapId(MapId);
            }
        }

        public void Update_ActivityMapping_ByMapId(string MapId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Update_ActivityMapping_ByMapId(MapId);
            }
        }

        public void Delete_ActivityMapping_ByMapId(string MapId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Delete_ActivityMapping_ByMapId(MapId);
            }
        }
        #endregion

        #region Country Mapping
        public void Insert_CountryMapping_ByMapId(string MapId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Insert_CountryMapping_ByMapId(MapId);
            }
        }

        public void Update_CountryMapping_ByMapId(string MapId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Update_CountryMapping_ByMapId(MapId);
            }
        }

        public void Delete_CountryMapping_ByMapId(string MapId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Delete_CountryMapping_ByMapId(MapId);
            }
        }
        #endregion

        #region City Mapping
        public void Insert_CityMapping_ByMapId(string MapId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Insert_CityMapping_ByMapId(MapId);
            }
        }

        public void Update_CityMapping_ByMapId(string MapId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Update_CityMapping_ByMapId(MapId);
            }
        }

        public void Delete_CityMapping_ByMapId(string MapId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Delete_CityMapping_ByMapId(MapId);
            }
        }
        #endregion

        #region Hotel Mapping & Lite
        public void Insert_HotelMapping_ByMapId(string MapId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Insert_HotelMapping_ByMapId(MapId);
            }
        }

        public void Update_HotelMapping_ByMapId(string MapId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Update_HotelMapping_ByMapId(MapId);
            }
        }

        public void Delete_HotelMapping_ByMapId(string MapId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Delete_HotelMapping_ByMapId(MapId);
            }
        }
        #endregion

        #region Country Master
        public void Insert_CountryMaster_ByCode(string Code)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Insert_CountryMaster_ByCode(Code);
            }
        }

        public void Update_CountryMaster_ByCode(string Code)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Update_CountryMaster_ByCode(Code);
            }
        }

        public void Delete_CountryMaster_ByCode(string Code)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Delete_CountryMaster_ByCode(Code);
            }
        }
        #endregion

        #region City Master
        public void Insert_CityMaster_ByCode(string Code)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Insert_CityMaster_ByCode(Code);
            }
        }

        public void Update_CityMaster_ByCode(string Code)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Update_CityMaster_ByCode(Code);
            }
        }

        public void Delete_CityMaster_ByCode(string Code)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Delete_CityMaster_ByCode(Code);
            }
        }

        public void Upsert_CityMaster_ByCode(string Code)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Upsert_CityMaster_ByCode(Code);
            }
        }
        #endregion

        #region Supplier Master
        public void Insert_SupplierMaster_ByCode(string Code)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Insert_SupplierMaster_ByCode(Code);
            }
        }

        public void Update_SupplierMaster_ByCode(string Code)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Update_SupplierMaster_ByCode(Code);
            }
        }

        public void Delete_SupplierMaster_ByCode(string Code)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Delete_SupplierMaster_ByCode(Code);
            }
        }
        #endregion

        #region Hotel Master
        public void Insert_HotelMaster_ByHotelId(string HotelId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Insert_HotelMaster_ByHotelId(HotelId);
            }
        }

        public void Update_HotelMaster_ByHotelId(string HotelId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Update_HotelMaster_ByHotelId(HotelId);
            }
        }

        public void Delete_HotelMaster_ByHotelId(string HotelId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Delete_HotelMaster_ByHotelId(HotelId);
            }
        }
        #endregion

        #region Activity Master
        public void Insert_ActivityMaster_ByActivityId(string ActivityId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Insert_ActivityMaster_ByActivityId(ActivityId);
            }
        }

        public void Update_ActivityMaster_ByActivityId(string ActivityId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Update_ActivityMaster_ByActivityId(ActivityId);
            }
        }

        public void Delete_ActivityMaster_ByActivityId(string ActivityId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Delete_ActivityMaster_ByActivityId(ActivityId);
            }
        }
        #endregion

        #region Activity Definition
        public void Sync_ActivityDefinition_ByActivityFlavourId(string ActivityFlavourId)
        {
            using (BAL.BL_UpdateData BL = new BAL.BL_UpdateData())
            {
                BL.Sync_ActivityDefinition_ByActivityFlavourId(ActivityFlavourId);
            }
        }
        #endregion
    }
}