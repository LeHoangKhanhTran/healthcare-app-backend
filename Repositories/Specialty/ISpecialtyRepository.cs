using HealthAppAPI.Entities;

public interface ISpecialtyRepository 
{
    public Task<IEnumerable<Specialty>> GetSpecialties(SpecialtyQueryParams queryParams);
    public Task<Specialty> GetSpecialtyById(Guid id);
    public Task CreateSpecialty(Specialty specialty);
    // public Task UpdateSpecialty(Specialty specialty);
    public Task DeleteSpecialty(Guid id);
}