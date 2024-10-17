using HealthAppAPI.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthAppAPI.Entities;
[BsonDiscriminator]
public class Patient : User 
{
    public Guid PatientProfileId { get; init; } 
    public Patient()
    {
        this.Role = Role.Patient;
    }

    public Patient(Guid userId, string email, string phoneNumber, Role role)
    {
        UserId = userId;
        Email = email;
        PhoneNumber = phoneNumber;
        Role = role;
    }
}