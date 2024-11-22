using HealthAppAPI.Entities;

public interface IPatientProfileRepository
{
    public Task<object> GetPatientProfiles(PatientProfileQueryParams queryParams);
    public Task<PatientProfile> GetPatientProfileById(Guid id);
    public Task CreatePatientProfile(PatientProfile PatientProfile);
    public Task UpdatePatientProfile(PatientProfile PatientProfile);
    public Task AddDocument(PatientProfile patientProfile, PatientDocument patientDocument);
    public Task RemoveDocument(PatientProfile patientProfile, Guid documentId);
}