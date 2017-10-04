using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Bson.Serialization.Attributes;

namespace DataContracts.Masters
{
    [BsonIgnoreExtraElements]
    public class DC_Keywords
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string Keyword { get; set; }
        public bool Attribute { get; set; }
        public int Sequence { get; set; }
        public string EntityFor { get; set; }
        public string AttributeType { get; set; }
        public string AttributeLevel { get; set; }
        public string AttributeSubLevel { get; set; }
        public string AttributeSubLevelValue { get; set; }
        public List<DC_Keyword_Aliases> Aliases { get; set; }
    }

    public class DC_Keyword_Aliases
    {
        public string Value { get; set; }
        public int Sequence { get; set; }
        public int NoOfHits { get; set; }
    }
}
