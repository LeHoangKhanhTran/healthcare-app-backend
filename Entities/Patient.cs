using HealthAppAPI.Enums;
using MongoDB.Bson.Serialization.Attributes;

namespace HealthAppAPI.Entities;
[BsonDiscriminator]
public class Patient : User 
{


    public Patient(Guid userId, string email, string phoneNumber, string password)
    {
        UserId = userId;
        Email = email;
        Password = password;
        PhoneNumber = phoneNumber;
        Role = Role.Patient;
    }
}