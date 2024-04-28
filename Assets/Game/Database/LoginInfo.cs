using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

[BsonIgnoreExtraElements]
public class LoginInfo
{
    [BsonId]
    public ObjectId Id { get; set; }
    
    [BsonElement("email")]
    public string Email;
    
    [BsonElement("password")]
    public string Password;
    
}


