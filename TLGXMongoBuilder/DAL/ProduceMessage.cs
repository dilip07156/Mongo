using DataContracts;
using DataContracts.Activity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL
{
    public class SendToKafka : IDisposable
    {
        /// <summary>   
        /// This Method Calling to Kafka Producer API for producing Messages.
        ///  <param name="SerializeContent">
        ///     A collection of object to send serialized Message.         
        /// </param>       
        /// </summary>

        public static void SendMessage<TMessage>(TMessage val, string Entity, string Method)
        {
            try
            {
                JObject rss = new JObject(new JProperty("METHOD", Method),
                                          new JProperty("ENTITY", Entity),
                                          new JProperty("DATA", JObject.FromObject(val)));

                Console.WriteLine(rss.ToString());

                var requestObject = new DC_M_masterattribute
                {
                    MasterFor = "Kafka",
                    PageNo = 0,
                    PageSize = int.MaxValue
                };
                IList<DC_M_masterattribute> Kafka = GetMasterAttributes(requestObject);

                IList<DC_M_masterattributevalue> KafkaVariables = new List<DC_M_masterattributevalue>();
                if (Kafka != null)
                {
                    Guid MasterAttribute_Id = Kafka.Where(w => w.Name == "KafkaVariables").Select(s => s.MasterAttribute_Id).FirstOrDefault();
                    KafkaVariables = GetKafkaConfigurationAttributesValues(MasterAttribute_Id.ToString(), int.MaxValue, 0);
                }
                
                List<string> addresslist = new List<string>();
                string username = string.Empty;
                string password = string.Empty;
                string topics = string.Empty;
                if (KafkaVariables != null)
                {
                    KafkaVariables = KafkaVariables.Where(w => w.IsActive == "Y").ToList();
                                      

                    if (KafkaVariables != null)
                    {
                        KafkaVariables = KafkaVariables.Where(w => w.IsActive == "Y").ToList();
                        foreach (var bootstrap in KafkaVariables.Where(w => w.AttributeValue.StartsWith("bootstrap.servers")).Select(s => s.OTA_CodeTableValue))
                        {
                            addresslist.Add(bootstrap);
                        }
                        username= KafkaVariables.Where(w => w.AttributeValue == "Username").Select(s => s.OTA_CodeTableValue).FirstOrDefault();
                        password= KafkaVariables.Where(w => w.AttributeValue == "Password").Select(s => s.OTA_CodeTableValue).FirstOrDefault();

                        topics = KafkaVariables.Where(w => w.AttributeValue.StartsWith("topic_activity")).Select(s => s.OTA_CodeTableValue).FirstOrDefault();
                    }
                }

               
                                
                KafkaMessage newMsg = new KafkaMessage
                {
                    Address = addresslist,  //System.Configuration.ConfigurationManager.AppSettings["ActivityTopic"],
                    SecurityDetail = new SecurityDetail
                    {
                        UserName = username,
                        PassWord = password
                    },
                    Payload = rss.ToString()
                };

                if(Entity.ToUpper() == "ACTIVITY")
                {
                    newMsg.Topic = topics;
                }
                
                object result = null;
                Proxy.PostData("/Kafka/Produce", newMsg, typeof(KafkaMessage), typeof(List<string>), out result);
                var Msg = result;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public static IList<DC_M_masterattribute> GetMasterAttributes(DC_M_masterattribute _obj)
        {
            using (TLGX_Entities context = new TLGX_Entities())
            {
                var search = (from d in context.m_masterattribute select d).AsQueryable();

                bool isActive = Convert.ToBoolean(Convert.ToInt32(_obj.IsActive));
                if (!string.IsNullOrWhiteSpace(_obj.MasterFor))
                {
                    search = (from x in search where x.MasterFor == _obj.MasterFor select x);
                }
                if (!string.IsNullOrWhiteSpace(_obj.OTA_CodeTableCode))
                    search = (from x in search where x.OTA_CodeTableCode == _obj.OTA_CodeTableCode select x);
                if (!string.IsNullOrWhiteSpace(_obj.OTA_CodeTableName))
                    search = (from x in search where x.OTA_CodeTableName == _obj.OTA_CodeTableName select x);
                if (!string.IsNullOrWhiteSpace(_obj.Name))
                    search = (from x in search where x.Name.ToString().ToLower().Contains(_obj.Name.ToLower()) select x);
               
                if (_obj.ParentAttribute_Id.HasValue)
                    if (_obj.ParentAttribute_Id != Guid.Empty)
                        search = (from x in search where x.ParentAttribute_Id == _obj.ParentAttribute_Id select x);
                if (!String.IsNullOrWhiteSpace(_obj.IsActive))
                    search = (from x in search where x.IsActive == isActive select x);

                int total = search.Count();
                int skip = (_obj.PageNo ?? 0) * (_obj.PageSize ?? 0);


                var result = from d in search
                             join pa in context.m_masterattribute on d.ParentAttribute_Id equals pa.MasterAttribute_Id into ps
                             from pa in ps.DefaultIfEmpty()
                             orderby d.Name
                             select new DC_M_masterattribute
                             {
                                 MasterAttribute_Id = d.MasterAttribute_Id,
                                 Name = d.Name,
                                 MasterFor = d.MasterFor ?? "",
                                 OTA_CodeTableCode = d.OTA_CodeTableCode ?? "",
                                 OTA_CodeTableName = d.OTA_CodeTableName ?? "",
                                 ParentAttributeName = pa == null ? "" : pa.Name,
                                 ParentAttribute_Id = d.ParentAttribute_Id ?? Guid.Empty,
                                 IsActive = d.IsActive ?? false == true ? "Y" : "N",
                                 TotalRecords = total
                             };
                return result.OrderBy(p => p.Name).Skip(skip).Take((_obj.PageSize ?? total)).ToList();
            }
        }

        public static IList<DC_M_masterattributevalue> GetKafkaConfigurationAttributesValues(string MasterFor, int PageNo,int PageSize)
        {
            try
            {
                Guid gMasterAttribute_Id = Guid.Parse(MasterFor);
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    var total = (from mav in context.m_masterattributevalue
                                 where mav.MasterAttribute_Id == gMasterAttribute_Id
                                 select mav).Count();

                    int skip = (PageNo) * (PageSize);

                    var search = from mav in context.m_masterattributevalue
                                 join pav in context.m_masterattributevalue on mav.ParentAttributeValue_Id equals pav.MasterAttributeValue_Id into paval
                                 from pavalid in paval.DefaultIfEmpty()
                                 orderby mav.AttributeValue
                                 where mav.MasterAttribute_Id == gMasterAttribute_Id
                                 select new DC_M_masterattributevalue
                                 {
                                     MasterAttribute_Id = mav.MasterAttribute_Id,
                                     MasterAttributeValue_Id = mav.MasterAttributeValue_Id,
                                     AttributeValue = mav.AttributeValue ?? "",
                                     OTA_CodeTableValue = mav.OTA_CodeTableValue ?? "",
                                     IsActive = mav.IsActive ?? false == true ? "Y" : "N",
                                     ParentAttributeValue_Id = pavalid.MasterAttributeValue_Id,
                                     ParentAttributeValue = pavalid.AttributeValue,
                                     TotalCount = total
                                 };


                    return search.OrderBy(p => p.AttributeValue).Skip(skip).Take(PageSize == 0 ? total : PageSize).ToList();
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void Dispose()
        {
        }
    }
}
