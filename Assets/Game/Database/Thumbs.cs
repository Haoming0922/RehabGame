using MongoDB.Bson.Serialization.Attributes;
using Realms;


namespace RehabDB
{

    public class Thumbs : RealmObject
    {

        [BsonElement("id")] public string id  { get; set; }

        [BsonElement("thumbsCount")] public int thumbsCount  { get; set; }
    }
}