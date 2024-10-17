using MongoDB.Bson.Serialization.Attributes;

namespace HealthAppAPI.Entities;

public class Doctor 
{
    [BsonId]
    [BsonElement("DoctorId")]
    public Guid DoctorId { get; init; }
    public string Name { get; init; }
    public IEnumerable<string> Specialties { get; init; }
    public string DoctorInfo { get; init; }
    public string DoctorImageUrl { get; init; }
    public IEnumerable<Shift> Shifts { get; set; }
}