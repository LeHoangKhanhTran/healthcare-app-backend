using System.Runtime.CompilerServices;

public record AppointmentQueryParams(int? Page, int? PageSize, string? Status, string? Order);
public record DoctorQueryParams(int? Page, int? PageSize, string? Name, string? Specialty);
public record PatientProfileQueryParams(int? Page, int? PageSize, string? FullName);
public record SpecialtyQueryParams(string? SpecialtyName); 