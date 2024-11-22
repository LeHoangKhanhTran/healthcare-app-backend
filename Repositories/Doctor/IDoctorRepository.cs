using HealthAppAPI.Entities;
using Microsoft.AspNetCore.Mvc;

public interface IDoctorRepository
{
    public Task<object> GetDoctors(DoctorQueryParams queryParams);
    public Task<Doctor> GetDoctorById(Guid id);
    public Task CreateDoctor(Doctor doctor);
    public Task UpdateDoctor(Doctor doctor);
    public Task DeleteDoctor(Guid id);
    public Task AddShift(Doctor doctor, Shift shift);
    public Task RemoveShift(Doctor doctor, Guid shiftId);
    public Task UpdateShift(Doctor doctor, Shift shift);
} 