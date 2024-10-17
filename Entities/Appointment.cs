using HealthAppAPI.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthAppAPI.Entities;
public class Appointment 
{
    [BsonId]
    [BsonElement("AppointmentId")]
    public Guid AppointmentId { get; init; }
    public string Reason { get; init; }
    public string Note { get; init; }
    public DateTimeOffset AppointmentDate { get; init; }
    public string AppointmentTime { get; init; }
    
    [BsonRepresentation(MongoDB.Bson.BsonType.String)]
    public AppointmentStatus Status { get; init; }
    public AppointmentPatient Patient { get; init; }
    public AppointmentDoctor Doctor { get; init; }
    public DateTime CreatedDate { get; init; }
}

public record AppointmentDoctor {
    public Guid DoctorId { get; init; }
    public string Name { get; init; }
    public string ImageUrl { get; init; }
}

public record AppointmentPatient {
    public Guid PatientId { get; init; }
    public string Name { get; init; }
}