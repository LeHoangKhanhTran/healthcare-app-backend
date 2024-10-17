using HealthAppAPI.Entities;
using MongoDB.Driver;

public class UserRepository : IUserRepository
{
    private const string CollectionName = "user";
    private readonly IMongoCollection<User> UserCollection;
    private readonly FilterDefinitionBuilder<User> filterBuilder = Builders<User>.Filter;
    public UserRepository(IMongoDatabase database)
    {
        this.UserCollection = database.GetCollection<User>(CollectionName);
    }
    
    public async Task CreateUser(User user)
    {
        await UserCollection.InsertOneAsync(user);
    }

    public async Task DeleteUser(Guid id)
    {
        var filter = filterBuilder.Eq(user => user.UserId, id);
        await UserCollection.DeleteOneAsync(filter);
    }

    public async Task<User> GetUserByEmail(string email)
    {
        var filter = filterBuilder.Eq(user => user.Email, email);
        return await UserCollection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task<User> GetUserByPhoneNumber(string phoneNumber)
    {
        var filter = filterBuilder.Eq(user => user.PhoneNumber, phoneNumber);
        var user = await UserCollection.Find(filter).SingleOrDefaultAsync();
        return user;
    }

    public async Task UpdateProfile(User user, Guid ProfileId)
    {
        var filter = filterBuilder.Eq(user => user.UserId, user.UserId);
        var updateFilter = Builders<User>.Update.Set(user => (user as Patient).PatientProfileId, ProfileId);
        await UserCollection.UpdateOneAsync(filter, updateFilter);
    }
}