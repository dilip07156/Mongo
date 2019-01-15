using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public static class Proxy
    {
        public enum ProxyFor
        {
            ProducerMessage
        }
        public static string ProdMessageURL
        {
            get
            {
                return System.Configuration.ConfigurationManager.AppSettings["ProdMessageURL"];
            }
        }
        public static bool PostData(string URI, object Param, Type RequestType, Type ResponseType, out object ReturnValue)
        {
            try
            {
                string AbsPath = string.Empty;
               
                AbsPath = ProdMessageURL;
              
                HttpWebRequest request;
                request = (HttpWebRequest)WebRequest.Create(AbsPath + URI);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.KeepAlive = false;
                DataContractJsonSerializer serializerToUpload = new DataContractJsonSerializer(RequestType);

                using (var memoryStream = new MemoryStream())
                {
                    using (var reader = new StreamReader(memoryStream))
                    {
                        serializerToUpload.WriteObject(memoryStream, Param);
                        memoryStream.Position = 0;
                        string body = reader.ReadToEnd();

                        using (var streamWriter = new StreamWriter(request.GetRequestStream()))
                        {
                            streamWriter.Write(body);
                        }
                    }
                }

                var response = request.GetResponse();

                if (((System.Net.HttpWebResponse)response).StatusCode != HttpStatusCode.OK)
                {
                    ReturnValue = null;
                }
                else
                {
                    var stream = response.GetResponseStream();

                    var obj = new DataContractJsonSerializer(ResponseType);
                    ReturnValue = obj.ReadObject(stream);

                    obj = null;
                    stream = null;
                }

                serializerToUpload = null;

                response.Dispose();
                response = null;
                request = null;

                if (ReturnValue != null)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                ReturnValue = null;
                return false;
            }
        }

    }
}
