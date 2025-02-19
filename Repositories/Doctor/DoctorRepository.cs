using HealthAppAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Bson;
using MongoDB.Driver;

public class DoctorRepository : IDoctorRepository
{
    private const string CollectionName = "doctor";
    private readonly IMongoCollection<Doctor> DoctorCollection;
    private readonly FilterDefinitionBuilder<Doctor> filterBuilder = Builders<Doctor>.Filter;
    public DoctorRepository(IMongoDatabase database)
    {
        this.DoctorCollection = database.GetCollection<Doctor>(CollectionName);
    }

    public async Task<object> GetDoctors(DoctorQueryParams queryParams)
    {
        var filter = filterBuilder.Empty;
        List<FilterDefinition<Doctor>> filterList = new(); 
        if (queryParams.Name is not null && queryParams.Name.Length > 0)
        {
            var nameFilter = filterBuilder.Regex(doctor => doctor.Name, new BsonRegularExpression($"(?i){queryParams.Name}"));
            filterList.Add(nameFilter);
        }
        if (queryParams.Specialty is not null && queryParams.Specialty.Length > 0)
        {
            var specialtyFilter = filterBuilder.AnyEq(doctor => doctor.Specialties, queryParams.Specialty.ToString());
            filterList.Add(specialtyFilter);
        }
        if (filterList.Count > 0) 
        {
            filter = filterBuilder.And(filterList);
        }
        if (queryParams.Page is not null && queryParams.PageSize is not null && queryParams.Page > 0)
        {
            long count = await DoctorCollection.CountDocumentsAsync(filter);
            int totalPage = (int)Math.Ceiling(count / (double)queryParams.PageSize);
            var results = await DoctorCollection.Find(filter).Skip((queryParams.Page - 1) * queryParams.PageSize).Limit(queryParams.PageSize).ToListAsync();
            return new PaginatedList<Doctor>(results, (int)queryParams.Page, totalPage);
        }
        var list = await DoctorCollection.Find(filter).ToListAsync();
        return list;
    }

    public async Task CreateDoctor(Doctor doctor)
    {
        await DoctorCollection.InsertOneAsync(doctor);
    }

    public async Task DeleteDoctor(Guid id)
    {
        var filter = filterBuilder.Eq(doctor => doctor.DoctorId, id);
        await DoctorCollection.DeleteOneAsync(filter);
    }

    public async Task<Doctor> GetDoctorById(Guid id)
    {
        var filter = filterBuilder.Eq(doctor => doctor.DoctorId, id);
        return await DoctorCollection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task UpdateDoctor(Doctor doctor)
    {
        var filter = filterBuilder.Eq(doctor => doctor.DoctorId, doctor.DoctorId);
        await DoctorCollection.ReplaceOneAsync(filter, doctor);
    }

    public async Task AddShift(Doctor doctor, Shift shift)
    {
        var filter = filterBuilder.Eq(doctor => doctor.DoctorId, doctor.DoctorId);
        var updateFilter = Builders<Doctor>.Update.Set(doctor => doctor.Shifts, doctor.Shifts is null ? new List<Shift>() {shift} : doctor.Shifts.Append(shift));
        await DoctorCollection.UpdateOneAsync(filter, updateFilter);
    }

    public async Task RemoveShift(Doctor doctor, Guid shiftId)
    {
        var filter = filterBuilder.Eq(doctor => doctor.DoctorId, doctor.DoctorId);
        var updateFilter = Builders<Doctor>.Update.Set(doctor => doctor.Shifts, doctor.Shifts.Where(shift => shift.ShiftId != shiftId));
        await DoctorCollection.UpdateOneAsync(filter, updateFilter);
    }

    public async Task UpdateShift(Doctor doctor, Shift shift)
    {
        var filter = filterBuilder.Eq(doctor => doctor.DoctorId, doctor.DoctorId);
        var newShifts = doctor.Shifts.Where(shift => shift.ShiftId != shift.ShiftId).Append(shift);
        var updateFilter = Builders<Doctor>.Update.Set(doctor => doctor.Shifts, doctor.Shifts is null ? new List<Shift>() : newShifts);
        await DoctorCollection.UpdateOneAsync(filter, updateFilter);
    }

}