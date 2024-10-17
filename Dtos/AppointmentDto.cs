using HealthAppAPI.Entities;

public record AppointmentDto(Guid AppointmentId, string Reason, string Note, DateTime AppointmentDate, string AppointmentTime, string Status, AppointmentPatient Patient, AppointmentDoctor Doctor, DateTime CreateDate);
public record CreateAppointmentDto(string Reason, string Note, string AppointmentDate, string AppointmentTime, Guid PatientId, Guid DoctorId);
public record UpdateAppointmentDto(string Reason, string Note, string AppointmentDate, string AppointmentTime, Guid DoctorId);

