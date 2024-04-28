using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class GameTask
{
    [BsonId]
    public string _id { get; set; }
    
    [BsonElement("game")]
    public GameType game;
    
    [BsonElement("sets")]
    public int sets { get; set; }
 
    [BsonElement("status")]
    public string status { get; set; }
    
    [BsonElement("date")]
    public string date { get; set; }
}