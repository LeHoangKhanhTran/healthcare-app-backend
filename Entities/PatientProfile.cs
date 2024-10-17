using HealthAppAPI.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthAppAPI.Entities;
public class PatientProfile
{
    [BsonId]
    [BsonElement("PatientProfileId")]
    public Guid PatientProfileId { get; init; }
    public string FullName { get; init; }
    public string Email { get; init; }
    public string PhoneNumber { get; init; }
    public string DateOfBirth { get; init; }
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public Gender Gender { get; init; }
    public string Address { get; init; }
    public string Occupation { get; init; }
    public string InsuranceNumber { get; init; }
    public string Allergies { get; init; }
    public string CurrentMedications { get; init; }
    public string PastMedicalHistory { get; init; }
    public IEnumerable<PatientDocument> PatientDocuments { get; set; }

}