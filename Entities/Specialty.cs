using MongoDB.Bson.Serialization.Attributes;

namespace HealthAppAPI.Entities;
public struct Specialty 
{
    [BsonId]
    [BsonElement("SpecialtyId")]
    public Guid SpecialtyId { get; init; }
    public string Name { get; init; }
}