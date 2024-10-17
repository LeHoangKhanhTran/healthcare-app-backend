using HealthAppAPI.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

public class SpecialtyRepository : ISpecialtyRepository
{
    private const string CollectionName = "specialty";
    private readonly IMongoCollection<Specialty> SpecialtyCollection;
    private readonly FilterDefinitionBuilder<Specialty> filterBuilder = Builders<Specialty>.Filter;
    public SpecialtyRepository(IMongoDatabase database)
    {
        this.SpecialtyCollection = database.GetCollection<Specialty>(CollectionName);
    }
    public async Task CreateSpecialty(Specialty specialty)
    {
        await SpecialtyCollection.InsertOneAsync(specialty);
    }

    public async Task<IEnumerable<Specialty>> GetSpecialties(SpecialtyQueryParams queryParams)
    {
        var filter = filterBuilder.Empty;
        if (queryParams.SpecialtyName is not null && queryParams.SpecialtyName.Length > 0)
        {
            filter = filterBuilder.Regex(specialty => specialty.Name, new BsonRegularExpression($"(?i){queryParams.SpecialtyName}"));
        }
        return await SpecialtyCollection.Find(filter).ToListAsync();
    }

    public async Task<Specialty> GetSpecialtyById(Guid id)
    {
        var filter = filterBuilder.Eq(specialty => specialty.SpecialtyId, id);
        return await SpecialtyCollection.Find(filter).SingleOrDefaultAsync();
    }

    // public async Task UpdateSpecialty(Specialty specialty)
    // {
    //     var filter = filterBuilder.Eq(specialty => specialty.SpecialtyId, specialty.SpecialtyId);
    //     await SpecialtyCollection.ReplaceOneAsync(filter, specialty);
    // }
    
    public async Task DeleteSpecialty(Guid id)
    {
        var filter = filterBuilder.Eq(specialty => specialty.SpecialtyId, id);
        await SpecialtyCollection.DeleteOneAsync(filter);
    }
}