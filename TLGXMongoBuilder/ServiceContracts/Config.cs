using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts
{
    public class KafkaMessage
    {
        public string Payload { get; set; }
        public string Topic { get; set; }
        public string Address { get; set; }
        public SecurityDetail SecurityDetail { get; set; }
    }
    public class SecurityDetail
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }
}
