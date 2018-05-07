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

        public void UpdateActivityCategoryTypes()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.UpdateActivityCategoryTypes();
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


        public void LoadProductMapping()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadProductMapping();
            }
        }

        public void LoadProductMappingLite()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadProductMappingLite();
            }
        }

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

        public void LoadRoomTypeMapping()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadRoomTypeMapping();
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

        public void LoadAccoStaticData(string LogId,string Supplier_Id)
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

    }
}
