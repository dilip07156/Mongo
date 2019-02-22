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
                IList<DC_M_masterattributevalue> KafkaVariables = GetMasterAttributesValues("Kafka", "KafkaVariables");

                if (KafkaVariables != null)
                {
                    JObject rss = new JObject(new JProperty("METHOD", Method),
                                          new JProperty("ENTITY", Entity));
//                                          new JProperty("DATA", (val.GetType() == typeof(string) ? JValue.FromObject(val) : JObject.FromObject(val))));
                    
                    if(val.GetType() == typeof(string) || val.GetType() == typeof(int) || val.GetType() == typeof(Int32) || val.GetType() == typeof(long) || val.GetType() == typeof(Int64))
                    {
                        rss.Add("DATA", JValue.FromObject(val));
                    }
                    else
                    {
                        rss.Add("DATA", JObject.FromObject(val));
                    }

                    KafkaMessage newMsg = new KafkaMessage
                    {
                        Address = KafkaVariables.Where(w => w.AttributeValue.StartsWith("bootstrap.servers")).Select(s => s.OTA_CodeTableValue).ToList(),
                        SecurityDetail = new SecurityDetail
                        {
                            UserName = KafkaVariables.Where(w => w.AttributeValue == "Username").Select(s => s.OTA_CodeTableValue).FirstOrDefault(),
                            PassWord = KafkaVariables.Where(w => w.AttributeValue == "Password").Select(s => s.OTA_CodeTableValue).FirstOrDefault()
                        },
                        Payload = rss.ToString()
                    };

                    if (Entity.ToUpper() == "ACTIVITY")
                    {
                        newMsg.Topic = KafkaVariables.Where(w => w.AttributeValue.StartsWith("topic_activity")).Select(s => s.OTA_CodeTableValue).FirstOrDefault();
                    }

                    object result = null;
                    Proxy.PostData("/Kafka/Produce", newMsg, typeof(KafkaMessage), typeof(string), out result);
                    var Msg = result;
                }
            }
            catch (Exception ex)
            {
            }
        }

        public static IList<DC_M_masterattributevalue> GetMasterAttributesValues(string MasterFor, string Name)
        {

            List<DC_M_masterattributevalue> result = new List<DC_M_masterattributevalue>();

            using (var scope = new System.Transactions.TransactionScope(System.Transactions.TransactionScopeOption.RequiresNew, new System.Transactions.TransactionOptions()
            {
                IsolationLevel = System.Transactions.IsolationLevel.ReadUncommitted,
                Timeout = new TimeSpan(0, 2, 0)
            }))
            {
                using (TLGX_Entities context = new TLGX_Entities())
                {
                    result = (from ma in context.m_masterattribute
                              join mav in context.m_masterattributevalue on ma.MasterAttribute_Id equals mav.MasterAttribute_Id
                              join pav in context.m_masterattributevalue on mav.ParentAttributeValue_Id equals pav.MasterAttributeValue_Id into paval
                              from pavalid in paval.DefaultIfEmpty()
                              where ma.MasterFor == MasterFor && ma.Name == Name && (mav.IsActive ?? false) == true
                              select new DC_M_masterattributevalue
                              {
                                  MasterAttribute_Id = mav.MasterAttribute_Id,
                                  MasterAttributeValue_Id = mav.MasterAttributeValue_Id,
                                  AttributeValue = mav.AttributeValue ?? "",
                                  OTA_CodeTableValue = mav.OTA_CodeTableValue ?? "",
                                  IsActive = mav.IsActive ?? false == true ? "Y" : "N",
                                  ParentAttributeValue_Id = pavalid.MasterAttributeValue_Id,
                                  ParentAttributeValue = pavalid.AttributeValue,
                              }).ToList();
                }
                scope.Complete();
                scope.Dispose();
            }
            return result;
        }

        public void Dispose()
        {
        }
    }
}
