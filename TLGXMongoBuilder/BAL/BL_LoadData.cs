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

        public void LoadCountryMaster()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadCountryMaster();
            }
        }

        public void LoadCityMaster()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadCityMaster();
            }
        }

        public void LoadSupplierMaster()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadSupplierMaster();
            }
        }

        public void LoadCountryMapping()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadCountryMapping();
            }
        }

        public void LoadCityMapping()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadCityMapping();
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

        public void LoadActivityMapping()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadActivityMapping();
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

        public void LoadStates()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadStates();
            }
        }

        public void LoadPorts()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadPorts();
            }
        }

        public void LoadAccoStaticData()
        {
            using (DAL.DL_LoadData obj = new DAL.DL_LoadData())
            {
                obj.LoadAccoStaticData();
            }
        }
    }
}
