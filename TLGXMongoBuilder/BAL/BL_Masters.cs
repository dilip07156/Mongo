using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace BAL
{
    public class BL_Masters : IDisposable
    {
        public void Dispose()
        { }

        public List<DataContracts.Masters.DC_Country> Master_GetCountries()
        {
            using (DAL.DL_Masters obj = new DAL.DL_Masters())
            {
                return obj.Master_GetCountries();
            }
        }

        public List<DataContracts.Masters.DC_Country> Master_GetCountries_ByCountryCode(string CountryCode)
        {
            using (DAL.DL_Masters obj = new DAL.DL_Masters())
            {
                var result = obj.Master_GetCountries_ByCountryCode(CountryCode);
                return result;
            }
        }

        public List<DataContracts.Masters.DC_Country> Master_GetCountries_ByCountryName(string CountryName)
        {
            using (DAL.DL_Masters obj = new DAL.DL_Masters())
            {
                var result = obj.Master_GetCountries_ByCountryName(CountryName);
                return result;
            }
        }

        public List<DataContracts.Masters.DC_City> Master_GetCities_ByCountryCode(string CountryCode)
        {
            using (DAL.DL_Masters objBL = new DAL.DL_Masters())
            {
                var result = objBL.Master_GetCities_ByCountryCode(CountryCode);
                return result;
            }
        }

        public List<DataContracts.Masters.DC_City> Master_GetCities_ByCountryName(string CountryName)
        {
            using (DAL.DL_Masters objBL = new DAL.DL_Masters())
            {
                var result = objBL.Master_GetCities_ByCountryName(CountryName);
                return result;
            }
        }

        public List<DataContracts.Masters.DC_Supplier> Master_GetSupplier_ByCode(string Code)
        {
            using (DAL.DL_Masters objBL = new DAL.DL_Masters())
            {
                var result = objBL.Master_GetSupplier_ByCode(Code);
                return result;
            }
        }

        public List<DataContracts.Masters.DC_Supplier> Master_GetSupplier_ByName(string Name)
        {
            using (DAL.DL_Masters objBL = new DAL.DL_Masters())
            {
                var result = objBL.Master_GetSupplier_ByName(Name);
                return result;
            }
        }

        public List<DataContracts.Masters.DC_Supplier> Master_GetAllSupplier()
        {
            using (DAL.DL_Masters objBL = new DAL.DL_Masters())
            {
                var result = objBL.Master_GetAllSupplier();
                return result;
            }
        }
    }
}
