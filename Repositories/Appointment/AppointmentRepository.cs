using HealthAppAPI.Entities;
using HealthAppAPI.Enums;
using MongoDB.Bson;
using MongoDB.Driver;

public class AppointmentRepository : IAppointmentRepository
{
    private const string CollectionName = "appointment";
    private readonly IMongoCollection<Appointment> AppointmentCollection;
    private readonly FilterDefinitionBuilder<Appointment> filterBuilder = Builders<Appointment>.Filter;
    public AppointmentRepository(IMongoDatabase database)
    {
        this.AppointmentCollection = database.GetCollection<Appointment>(CollectionName);
    }

    public async Task CreateAppointment(Appointment appointment)
    {
        await AppointmentCollection.InsertOneAsync(appointment);
    }

    public async Task<Appointment> GetAppointmentById(Guid id)
    {
        var filter = filterBuilder.Eq(appointment => appointment.AppointmentId, id);
        return await AppointmentCollection.Find(filter).SingleOrDefaultAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAppointmentsByPatientId(Guid patientId)
    {
        var filter = filterBuilder.Eq(appointment => appointment.Patient.PatientId, patientId);
        return await AppointmentCollection.Find(filter).ToListAsync();
    }

    public async Task<IEnumerable<Appointment>> GetAppointments(AppointmentQueryParams queryParams)
    {
        FilterDefinition<Appointment> filter = filterBuilder.Empty;
        if (queryParams.Status is not null && queryParams.Status.Length > 0 && Enum.TryParse<AppointmentStatus>(queryParams.Status, out var status))
        {
            filter = filterBuilder.Eq(apppointment => apppointment.Status, status);
        }
        var query = AppointmentCollection.Find(filter);
        if (queryParams.Order is not null)
        {
            if (queryParams.Order == "desc") {
                query.SortByDescending(appoinment => appoinment.CreatedDate);
            }
            else 
            {
                query.SortBy(appoinment => appoinment.CreatedDate);
            }
        }
        if (queryParams.Page is not null && queryParams.PageSize is not null && queryParams.Page > 0) 
        {
            return await query.Skip((queryParams.Page - 1) * queryParams.PageSize).Limit(queryParams.PageSize).ToListAsync();
        }
        return await query.ToListAsync();
    }

    // public async Task UpdateAppointment(Appointment appointment)
    // {
    //     var filter = filterBuilder.Eq(appointment => appointment.AppointmentId, appointment.AppointmentId);
    //     await AppointmentCollection.ReplaceOneAsync(filter, appointment);
    // }

    public async Task UpdateAppointmentStatus(Guid id, AppointmentStatus newStatus)
    {
        var filter = filterBuilder.Eq(appointment => appointment.AppointmentId, id);
        var updateFilter = Builders<Appointment>.Update.Set(appointment => appointment.Status, newStatus);
        await AppointmentCollection.UpdateOneAsync(filter, updateFilter);
    }

    public async Task<long> GetCountByStatus(AppointmentStatus Status)
    {
        var filter = filterBuilder.Eq(appointment => appointment.Status, Status);
        return await AppointmentCollection.Find(filter).CountDocumentsAsync();
    }

    public async Task<long> GetCountByAppointmentTime(Guid doctorId, DateTime appointmentDate, string appointmentTime)
    {
        var filter = filterBuilder.Eq(appointment => appointment.Doctor.DoctorId, doctorId) & 
        filterBuilder.Eq(appointment => appointment.AppointmentTime, appointmentTime) & 
        filterBuilder.Eq(appointment => appointment.AppointmentDate, appointmentDate) & 
        filterBuilder.Ne(appointment => appointment.Status, AppointmentStatus.Cancelled);
        return await AppointmentCollection.Find(filter).CountDocumentsAsync();
    }
}