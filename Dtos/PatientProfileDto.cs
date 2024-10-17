using HealthAppAPI.Enums;

public record PatientProfileDto(Guid PatientProfileId, string Fullname, string Email, string PhoneNumber, string DateOfBirth, 
                                string Gender, string Address, string Occupation, string InsuranceNumber, string Allergies, 
                                string CurrentMedications, string PastMedicalHistory, IEnumerable<PatientDocumentDto> PatientDocuments);
public record CreatePatientProfileDto(string Fullname, string Email, string PhoneNumber, string DateOfBirth, Gender Gender, string Address, string UserPhoneNumber,
                                      string Occupation, string InsuranceNumber, string Allergies, string CurrentMedications, string PastMedicalHistory);
public record UpdatePatientProfileDto(string Fullname, string Email, string PhoneNumber, string DateOfBirth, Gender Gender, string Address,
                                      string Occupation, string InsuranceNumber, string Allergies, string CurrentMedications, string PastMedicalHistory);