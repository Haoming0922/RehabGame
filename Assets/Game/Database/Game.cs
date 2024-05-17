using MongoDB.Bson.Serialization.Attributes;
using Realms;

namespace RehabDB
{
    public class Game : RealmObject
    {
        [BsonElement("_id")] public string _id { get; set; }
        [BsonElement("type")] public string type  { get; set; }
        [BsonElement("equippment")] public string equipment  { get; set; }
        [BsonElement("slots")] public int slots  { get; set; }
        [BsonElement("img")] public string img  { get; set; }
        [BsonElement("icon")] public string icon  { get; set; }

    }
}