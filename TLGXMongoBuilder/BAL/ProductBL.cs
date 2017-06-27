using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAL
{
    public class ProductBL : IDisposable
    {
        public void Dispose()
        {
        }

        public DataContracts.ProductDetails GetProductDetails(string ID)
        {
            using (DAL.ProductDAL obj = new DAL.ProductDAL())
            {
                return obj.GetProductDetails(ID);
            }
        }

        public void LoadHotelDefinitions()
        {
            using (DAL.ProductDAL obj = new DAL.ProductDAL())
            {
                obj.LoadHotelDefinitions();
            }
        }

        public List<DataContracts.HotelDescriptiveInfo> HotelSearch(DataContracts.HotelSearchRequest objHSRQ)
        {
            using (DAL.ProductDAL obj = new DAL.ProductDAL())
            {
                return obj.HotelSearch(objHSRQ);
            }
        }
    }
}
