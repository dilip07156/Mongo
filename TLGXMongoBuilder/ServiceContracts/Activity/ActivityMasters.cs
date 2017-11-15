using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataContracts.Activity
{

    public class ActivityMasters
    {
        public string Type { get; set; }

        [BsonIgnoreIfNull]
        public string ParentType { get; set; }
        public List<ActivityMasterValues> Values { get; set; }
    }
    public class ActivityMasterValues
    {
        public string Value { get; set; }

        [BsonIgnoreIfNull]
        public string ParentValue { get; set; }
    }

}
