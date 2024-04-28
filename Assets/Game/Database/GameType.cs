using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class GameType
{
    [BsonElement("type")]
    public string type;
    
    [BsonElement("equippment")]
    public string equipment;
    
    [BsonElement("slots")]
    public int slots;
}
