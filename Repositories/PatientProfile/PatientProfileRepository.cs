using HealthAppAPI.Entities;
using MongoDB.Bson;
using MongoDB.Driver;

public class PatientProfileRepository : IPatientProfileRepository
{
    private const string CollectionName = "patientProfile";
    private readonly IMongoCollection<PatientProfile> PatientProfileCollection;
    private readonly FilterDefinitionBuilder<PatientProfile> filterBuilder = Builders<PatientProfile>.Filter;
    public PatientProfileRepository(IMongoDatabase database)
    {
        this.PatientProfileCollection = database.GetCollection<PatientProfile>(CollectionName);
    }

    public async Task CreatePatientProfile(PatientProfile patientProfile)
    {
        await PatientProfileCollection.InsertOneAsync(patientProfile);
    }

    public async Task<PatientProfile> GetPatientProfileById(Guid id)
    {
        var filter = filterBuilder.Eq(patientProfile => patientProfile.PatientProfileId, id);
        return await PatientProfileCollection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<PatientProfile>> GetPatientProfiles(PatientProfileQueryParams queryParams)
    {
        var filter = filterBuilder.Empty;
        if (queryParams.FullName is not null && queryParams.FullName.Length > 0)
        {
            filter = filterBuilder.Regex(patientProfile => patientProfile.FullName, new BsonRegularExpression($"(?i){queryParams.FullName}"));
        }
        if (queryParams.Page is not null && queryParams.PageSize is not null && queryParams.Page > 0)
        {
            return await PatientProfileCollection.Find(filter).Skip((queryParams.Page - 1) * queryParams.PageSize).Limit(queryParams.PageSize).ToListAsync();
        }
        return await PatientProfileCollection.Find(filter).ToListAsync();
    }

    public async Task UpdatePatientProfile(PatientProfile patientProfile)
    {
        var filter = filterBuilder.Eq(patientProfile => patientProfile.PatientProfileId, patientProfile.PatientProfileId);
        await PatientProfileCollection.ReplaceOneAsync(filter, patientProfile);
    }

    public async Task AddDocument(PatientProfile patientProfile, PatientDocument patientDocument)
    {
        var filter = filterBuilder.Eq(patientProfile => patientProfile.PatientProfileId, patientProfile.PatientProfileId);
        var newDocuments = patientProfile.PatientDocuments is null ? new List<PatientDocument>() { patientDocument } : 
        patientProfile.PatientDocuments.Append(patientDocument);
        var updateFilter = Builders<PatientProfile>.Update.Set(patientProfile => patientProfile.PatientDocuments, newDocuments);
        await PatientProfileCollection.UpdateOneAsync(filter, updateFilter);
    }

    public async Task RemoveDocument(PatientProfile patientProfile, Guid documentId)
    {
        var filter = filterBuilder.Eq(patientProfile => patientProfile.PatientProfileId, patientProfile.PatientProfileId);
        var newDocuments = patientProfile.PatientDocuments.Where(patientDocument => patientDocument.DocumentId != documentId);
        var updateFilter = Builders<PatientProfile>.Update.Set(patientProfile => patientProfile.PatientDocuments, newDocuments);
        await PatientProfileCollection.UpdateOneAsync(filter, updateFilter);
    }
}
