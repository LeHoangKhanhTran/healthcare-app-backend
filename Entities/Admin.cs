using HealthAppAPI.Enums;
namespace HealthAppAPI.Entities;
public class Admin : User 
{
    public Admin() 
    {
        this.Role = Role.Admin;
    }
}