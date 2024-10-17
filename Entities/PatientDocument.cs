using HealthAppAPI.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthAppAPI.Entities;
public class PatientDocument 
{
    [BsonId]
    [BsonElement("DocumentId")]
    public Guid DocumentId { get; init; }
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public DocumentType DocumentType { get; init; }
    public string DocumentName { get; init; }
    public string DocumentUrl { get; init; }
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public DocumentFormat DocumentFormat { get; init; }
}