using HealthAppAPI.Entities;

public record DoctorDto(Guid DoctorId, string Name, string DoctorInfo, string ImageUrl, IEnumerable<string> Specialties, IEnumerable<ShiftDto>  Shifts);
public record CreateDoctorDto(string Name, string DoctorInfo, IFormFile DoctorImage, IEnumerable<string> Specialties);
public record UpdateDoctorDto(string? Name, string? DoctorInfo, IFormFile? DoctorImage, IEnumerable<string>? Specialties);