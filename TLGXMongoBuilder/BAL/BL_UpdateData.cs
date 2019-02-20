using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    public class BL_UpdateData : IDisposable
    {
        #region Activity Mapping & Lite
        public void Insert_ActivityMapping_ByMapId(string MapId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Insert_ActivityMapping_ByMapId(MapId);
            }
        }

        public void Update_ActivityMapping_ByMapId(string MapId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Update_ActivityMapping_ByMapId(MapId);
            }
        }

        public void Delete_ActivityMapping_ByMapId(string MapId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Delete_ActivityMapping_ByMapId(MapId);
            }
        }

        public void Dispose()
        {
        }
        #endregion

        #region Country Mapping
        public void Insert_CountryMapping_ByMapId(string MapId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Insert_CountryMapping_ByMapId(MapId);
            }
        }

        public void Update_CountryMapping_ByMapId(string MapId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Update_CountryMapping_ByMapId(MapId);
            }
        }

        public void Delete_CountryMapping_ByMapId(string MapId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Delete_CountryMapping_ByMapId(MapId);
            }
        }
        #endregion

        #region City Mapping

        public void Upsert_CityMapping_ByMapId(string MapId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                var mapId = Convert.ToInt32(MapId);
                DL.Upsert_CityMapping_ByMapId(mapId);
            }
        }

        public void Delete_CityMapping_ByMapId(string MapId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Delete_CityMapping_ByMapId(MapId);
            }
        }
        #endregion

        #region Hotel Mapping & Lite
        public void Insert_HotelMapping_ByMapId(string MapId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Insert_HotelMapping_ByMapId(MapId);
            }
        }

        public void Update_HotelMapping_ByMapId(string MapId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Update_HotelMapping_ByMapId(MapId);
            }
        }

        public void Delete_HotelMapping_ByMapId(string MapId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Delete_HotelMapping_ByMapId(MapId);
            }
        }
        #endregion

        #region Country Master
        public void Insert_CountryMaster_ByCode(string Code)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Insert_CountryMaster_ByCode(Code);
            }
        }

        public void Update_CountryMaster_ByCode(string Code, string OldCode)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Update_CountryMaster_ByCode(Code, OldCode);
            }
        }

        public void Delete_CountryMaster_ByCode(string Code)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Delete_CountryMaster_ByCode(Code);
            }
        }
        #endregion

        #region City Master
        public void Insert_CityMaster_ByCode(string Code)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Insert_CityMaster_ByCode(Code);
            }
        }

        public void Update_CityMaster_ByCode(string Code)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Update_CityMaster_ByCode(Code);
            }
        }

        public void Delete_CityMaster_ByCode(string Code)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Delete_CityMaster_ByCode(Code);
            }
        }

        public void Upsert_CityMaster_ByCode(string Code)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Upsert_CityMaster_ByCode(Code);
            }
        }
        #endregion

        #region Supplier Master
        public void Insert_SupplierMaster_ByCode(string Code)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Insert_SupplierMaster_ByCode(Code);
            }
        }

        public void Upsert_SupplierMaster_ByCode(string Code)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Upsert_SupplierMaster_ByCode(Code);
            }
        }

        public void Delete_SupplierMaster_ByCode(string Code)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Delete_SupplierMaster_ByCode(Code);
            }
        }
        #endregion

        #region Hotel Master
        public void Insert_HotelMaster_ByHotelId(string HotelId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Insert_HotelMaster_ByHotelId(HotelId);
            }
        }

        public void Update_HotelMaster_ByHotelId(string HotelId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Update_HotelMaster_ByHotelId(HotelId);
            }
        }

        public void Delete_HotelMaster_ByHotelId(string HotelId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Delete_HotelMaster_ByHotelId(HotelId);
            }
        }
        #endregion

        #region Activity Master
        public void Insert_ActivityMaster_ByActivityId(string ActivityId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Insert_ActivityMaster_ByActivityId(ActivityId);
            }
        }

        public void Update_ActivityMaster_ByActivityId(string ActivityId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Update_ActivityMaster_ByActivityId(ActivityId);
            }
        }

        public void Delete_ActivityMaster_ByActivityId(string ActivityId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Delete_ActivityMaster_ByActivityId(ActivityId);
            }
        }
        #endregion

        #region Activity Definition
        public void Sync_ActivityDefinition_ByActivityFlavourId(string ActivityFlavourId)
        {
            using (DAL.DL_UpdateData DL = new DAL.DL_UpdateData())
            {
                DL.Upsert_ActivityDefinition(Guid.Parse(ActivityFlavourId));
            }
        }
        #endregion
    }
}
