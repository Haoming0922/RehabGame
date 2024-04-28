
using System;
using System.Collections.Generic;
using Game.Util;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class Performance
{
    [BsonElement("game")]
    public MiniGame game;
    
    [BsonElement("leftInputPerformance")]
    public float leftInputPerformance;
    
    [BsonElement("rightInputPerformance")]
    public float rightInputPerformance;
    
}
