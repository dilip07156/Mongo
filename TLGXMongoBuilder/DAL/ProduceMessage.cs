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

                KafkaMessage newMsg = new KafkaMessage
                {
                    Address = System.Configuration.ConfigurationManager.AppSettings["KafkaAddress"],
                    SecurityDetail = new SecurityDetail
                    {
                        UserName = "1213",
                        PassWord = "4646"
                    },
                    Payload = rss.ToString()
                };

                if(Entity.ToUpper() == "ACTIVITY")
                {
                    newMsg.Topic = System.Configuration.ConfigurationManager.AppSettings["ActivityTopic"];
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

        public void Dispose()
        {
        }
    }
}
