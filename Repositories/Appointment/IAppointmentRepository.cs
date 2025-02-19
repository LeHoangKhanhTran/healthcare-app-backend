
using HealthAppAPI.Entities;
using HealthAppAPI.Enums;

public interface IAppointmentRepository 
{
    public Task<object> GetAppointments(AppointmentQueryParams queryParams);
    public Task<Appointment> GetAppointmentById(Guid id);
    public Task<IEnumerable<Appointment>> GetAppointmentsByPatientId(Guid patientId);
    public Task CreateAppointment(Appointment appointment);
    // public Task UpdateAppointment(Appointment appointment);
    public Task UpdateAppointmentStatus(Guid id, AppointmentStatus newStatus);
    public Task<long> GetCountByStatus(AppointmentStatus Status);
    public Task<long> GetCountByAppointmentTime(Guid doctorId, DateTime appointmentDate, string AppointmentTime);
    // public Task DeleteAppointment(Guid id);
}