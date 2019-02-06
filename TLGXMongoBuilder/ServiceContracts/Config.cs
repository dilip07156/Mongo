using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts
{
    public class KafkaMessage
    {
        public string Payload { get; set; }
        public string Topic { get; set; }
        public List<string> Address { get; set; }
        public SecurityDetail SecurityDetail { get; set; }
    }
    public class SecurityDetail
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
    }

    [DataContract]
    public class DC_M_masterattributevalue
    {
        [DataMember]
        public Guid MasterAttributeValue_Id { get; set; }
        [DataMember]
        public Guid? MasterAttribute_Id { get; set; }
        [DataMember]
        public Guid? ParentAttributeValue_Id { get; set; }
        [DataMember]
        public string ParentAttributeValue { get; set; }
        [DataMember]
        public string AttributeValue { get; set; }
        [DataMember]
        public string MasterAttribute_Name { get; set; }
        [DataMember]
        public string OTA_CodeTableValue { get; set; }
        [DataMember]
        public string IsActive { get; set; }
        [DataMember]
        public string Action { get; set; }
        [DataMember]
        public int TotalCount { get; set; }

    }

    [DataContract]
    public class DC_M_masterattribute
    {
        [DataMember]
        public Guid MasterAttribute_Id { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public string MasterFor { get; set; }
        [DataMember]
        public string ParentAttributeName { get; set; }
        [DataMember]
        public Guid? ParentAttribute_Id { get; set; }
        [DataMember]
        public string OTA_CodeTableCode { get; set; }
        [DataMember]
        public string OTA_CodeTableName { get; set; }
        [DataMember]
        public string IsActive { get; set; }
        [DataMember]
        public string Action { get; set; }
        [DataMember]
        public int TotalRecords { get; set; }
        [DataMember]
        public int? PageSize { get; set; }
        [DataMember]
        public int? PageNo { get; set; }


    }
}
