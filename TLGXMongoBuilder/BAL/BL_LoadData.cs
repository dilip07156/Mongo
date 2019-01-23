using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    public class BL_LoadData : IDisposable
    {
        public void Dispose()
        {
        }

        public void LoadActivityDefinition()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadActivityDefinition(Guid.Empty);
            }
        }
        public void UpdateVisaDefinition(string logId)
        {
            Guid gLogId;
            if (Guid.TryParse(logId, out gLogId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.UpdateVisaDefinition(gLogId);
                }
            }
        }

        public void LoadActivityDefinitionBySupplier(string log_id, string suppliername)
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadActivityDefinitionBySupplier(log_id, suppliername);
            }
        }

        public void UpdateActivityCategoryTypes()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.UpdateActivityCategoryTypes();
            }
        }

        public void UpdateActivityInterestType()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.UpdateActivityInterestType();
            }
        }
        public void UpdateActivityDOW()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.UpdateActivityDOW();
            }
        }
        public void UpdateActivitySpecial()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.UpdateActivitySpecial();
            }
        }
        public void UpdateActivityPrices()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.UpdateActivityPrices();
            }
        }
        public void UpdateActivityDescription()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.UpdateActivityDescription();
            }
        }

        public void LoadActivityMasters()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadActivityMasters();
            }
        }

        public void LoadCountryMaster(string LogId)
        {
            Guid gLogId;
            if (Guid.TryParse(LogId, out gLogId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadCountryMaster(gLogId);
                }
            }
        }

        public void LoadCityMaster(string LogId)
        {
            Guid gLogId;
            if (Guid.TryParse(LogId, out gLogId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadCityMaster(gLogId);
                }
            }
        }

        public void LoadSupplierMaster(string LogId)
        {
            Guid gLogId;
            if (Guid.TryParse(LogId, out gLogId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadSupplierMaster(gLogId);
                }
            }
        }

        public void LoadCountryMapping(string LogId)
        {
            Guid gLogId;
            if (Guid.TryParse(LogId, out gLogId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadCountryMapping(gLogId);
                }
            }
        }

        public void LoadCityMapping(string LogId)
        {
            Guid gLogId;
            if (Guid.TryParse(LogId, out gLogId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadCityMapping(gLogId);
                }
            }
        }

        #region Product Mapping Push
        public void LoadProductMapping(string LogId, string ProdMapId)
        {
            Guid gLogId;
            Guid gProdMapId;
            if (Guid.TryParse(LogId, out gLogId) && Guid.TryParse(ProdMapId, out gProdMapId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadProductMapping(gLogId, gProdMapId);
                }
            }
        }

        public void LoadProductMappingLite(string LogId, string ProdMapId)
        {
            Guid gLogId;
            Guid gProdMapId;
            if (Guid.TryParse(LogId, out gLogId) && Guid.TryParse(ProdMapId, out gProdMapId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadProductMappingLite(gLogId, gProdMapId);
                }
            }
        }
        #endregion
        public void LoadActivityMapping(string LogId)
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                Guid gLogId;
                if (Guid.TryParse(LogId, out gLogId))
                {
                    obj.LoadActivityMapping(gLogId);
                }
            }
        }

        public void LoadActivityMappingLite()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadActivityMappingLite();
            }
        }
        
        public void LoadKeywords()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadKeywords();
            }
        }

        public void LoadStates(string LogId)
        {
            Guid gLogId;
            if (Guid.TryParse(LogId, out gLogId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadStates(gLogId);
                }
            }
        }

        public void LoadPorts(string LogId)
        {
            Guid gLogId;
            if (Guid.TryParse(LogId, out gLogId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadPorts(gLogId);
                }
            }
        }

        public void LoadAccoStaticData(string LogId, string Supplier_Id)
        {
            Guid logid = new Guid();
            Guid gSupplier_Id;
            if (Guid.TryParse(LogId, out logid) && Guid.TryParse(Supplier_Id, out gSupplier_Id))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadAccoStaticData(logid, gSupplier_Id);
                }
            }
        }

        public void LoadHotelMapping(string LogId)
        {
            Guid gLogId;
            if (Guid.TryParse(LogId, out gLogId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadHotelMapping(gLogId);
                }
            }
        }

        public void UpdateAccoStaticDataSingleColumn()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.UpdateAccoStaticDataSingleColumn();
            }
        }

        #region ZoneMaster
        public void LoadZoneMaster(string LogId)
        {
            Guid gLogId;
            if (Guid.TryParse(LogId, out gLogId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadZoneMaster(gLogId);
                }
            }
        }
        #endregion


        public void UpdateHotelRoomTypeMapping(string LogId, string Supplier_Id)
        {
            Guid gLogId;
            Guid gSupplier_Id;
            if (Guid.TryParse(LogId, out gLogId) && Guid.TryParse(Supplier_Id, out gSupplier_Id))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.UpdateHotelRoomTypeMapping(gLogId, gSupplier_Id);
                }
            }
        }

        #region ZoneType Master
        public void LoadZoneTypeMaster(string LogId)
        {
            Guid gLogId;
            if (Guid.TryParse(LogId, out gLogId))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadZoneTypeMaster(gLogId);
                }
            }
        }
        #endregion


        public void LoadMasterAccommodation(string LogId, string Accommodation_Id)
        {
            if (Guid.TryParse(LogId, out Guid gLogId) && Guid.TryParse(Accommodation_Id, out Guid gAccommodation_Id))
            {
                using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
                {
                    obj.LoadMasterAccommodation(gLogId, gAccommodation_Id);
                }
            }
        }

    }
}
