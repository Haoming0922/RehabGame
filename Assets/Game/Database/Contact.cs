using MongoDB.Bson.Serialization.Attributes;
using Realms;

public class Contact : RealmObject
{
    [BsonElement("fullName")]
    public string fullName { get; set; }
    [BsonElement("email")]
    public string email { get; set; }
    [BsonElement("phoneNumber")]
    public string phoneNumber { get; set; }
}
