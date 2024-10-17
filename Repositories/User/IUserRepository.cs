using HealthAppAPI.Entities;

public interface IUserRepository
{
    public Task<User> GetUserByEmail(string email);
    public Task<User> GetUserByPhoneNumber(string phoneNumber);
    public Task CreateUser(User user);
    public Task UpdateProfile(User user, Guid ProfileId); 
    public Task DeleteUser(Guid id);
}