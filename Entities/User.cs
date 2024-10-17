using HealthAppAPI.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthAppAPI.Entities;
public class User 
{
    [BsonId]
    [BsonElement("UserId")]
    public Guid UserId { get; init; }
    public string Email {get; init; }
    public string PhoneNumber {get; init; }
    public string Password { get; init; }

    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Role Role;
    public Guid? ProfileId;
}

