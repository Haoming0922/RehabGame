using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Game.Util;

[BsonIgnoreExtraElements]
public class Patient
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("PatientID")]
    public string PatientID { get; set; }
    
    [BsonElement("Name")]
    public string Name { get; set; }
    
    [BsonElement("Email")]
    public string Email { get; set; }
    
    [BsonElement("sexual")]
    public string Sexual { get; set; }
    
    [BsonElement("Avatar")]
    public string Avatar { get; set; }
    
    [BsonElement("progress")]
    public double progress { get; set; }
    
    [BsonElement("Tasks")]
    public List<GameTask> GameTasks { get; set; }
    
    [BsonElement("performance")]
    public Dictionary<MiniGame, Performance> performance;
}

