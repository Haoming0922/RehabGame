using MongoDB.Bson.Serialization.Attributes;
using Realms;

namespace RehabDB
{
    public class Performance :RealmObject
    {
        [BsonElement("gameType")]
        public string gameType { get; set; }
    
        [BsonElement("leftInputPerformance")]
        public float leftInputPerformance { get; set; }
    
        [BsonElement("rightInputPerformance")]
        public float rightInputPerformance { get; set; }
    }

}
  