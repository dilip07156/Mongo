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
        

        public List<DataContracts.Masters.DC_Country> Master_GetCountries()
        {
            //if(WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.AbsoluteUri.EndsWith("xml"))
            //{
            //    WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Xml;
            //}
            //else if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.AbsoluteUri.EndsWith("json"))
            //{
            //    WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
            //}
            //else if (WebOperationContext.Current.IncomingRequest.UriTemplateMatch.RequestUri.AbsoluteUri.EndsWith("jsonp"))
            //{
            //    WebOperationContext.Current.OutgoingResponse.ContentType = "application/javascript";
            //}

            using (BAL.BL_Masters objBL = new BAL.BL_Masters())
            {
                var result = objBL.Master_GetCountries();
                //var stream = new MemoryStream(Encoding.UTF8.GetBytes());
                return result;
            }
        }

        //public List<DataContracts.Masters.DC_Country> Master_GetCountriesXml()
        //{
        //    using (BAL.BL_Masters objBL = new BAL.BL_Masters())
        //    {
        //        return objBL.Master_GetCountries();
        //    }
        //}

        //public List<DataContracts.Masters.DC_Country> Master_GetCountriesJsonp()
        //{
        //    WebOperationContext.Current.OutgoingResponse.Format = WebMessageFormat.Json;
        //    using (BAL.BL_Masters objBL = new BAL.BL_Masters())
        //    {
        //        return objBL.Master_GetCountries();
        //    }
        //}


        public List<DataContracts.Masters.DC_Country> Master_GetCountries_ByCountryCode(string CountryCode)
        {
            using (BAL.BL_Masters objBL = new BAL.BL_Masters())
            {
                var result = objBL.Master_GetCountries_ByCountryCode(CountryCode);
                return result;
            }
        }

        public List<DataContracts.Masters.DC_Country> Master_GetCountries_ByCountryName(string CountryName)
        {
            using (BAL.BL_Masters objBL = new BAL.BL_Masters())
            {
                var result = objBL.Master_GetCountries_ByCountryName(CountryName);
                return result;
            }
        }

        public List<DataContracts.Masters.DC_City> Master_GetCities_ByCountryCode(string CountryCode)
        {
            using (BAL.BL_Masters objBL = new BAL.BL_Masters())
            {
                var result = objBL.Master_GetCities_ByCountryCode(CountryCode);
                return result;
            }
        }

        public List<DataContracts.Masters.DC_City> Master_GetCities_ByCountryName(string CountryName)
        {
            using (BAL.BL_Masters objBL = new BAL.BL_Masters())
            {
                var result = objBL.Master_GetCities_ByCountryName(CountryName);
                return result;
            }
        }

        public List<DataContracts.Masters.DC_Supplier> Master_GetSupplier_ByCode(string Code)
        {
            using (BAL.BL_Masters objBL = new BAL.BL_Masters())
            {
                var result = objBL.Master_GetSupplier_ByCode(Code);
                return result;
            }
        }

        public List<DataContracts.Masters.DC_Supplier> Master_GetSupplier_ByName(string Name)
        {
            using (BAL.BL_Masters objBL = new BAL.BL_Masters())
            {
                var result = objBL.Master_GetSupplier_ByName(Name);
                return result;
            }
        }

        public List<DataContracts.Masters.DC_Supplier> Master_GetAllSupplier()
        {
            using (BAL.BL_Masters objBL = new BAL.BL_Masters())
            {
                var result = objBL.Master_GetAllSupplier();
                return result;
            }
        }

    }
}