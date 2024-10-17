using HealthAppAPI.Entities;
using HealthAppAPI.Enums;
using Swashbuckle.AspNetCore.SwaggerGen;
namespace HealthAppAPI.Extensions;
public static class Extension 
{
    // Specialty extensions
    public static SpecialtyDto AsDto(this Specialty specialty)
    {
        return new(specialty.SpecialtyId, specialty.Name);
    }

    public static Specialty AsEntity(this SpecialtyDto specialtyDto)
    {
        return new Specialty {SpecialtyId = specialtyDto.SpecialtyId, Name = specialtyDto.Name};
    }
    public static Specialty AsEntity(this UpdateSpecialtyDto specialtyDto, Guid id)
    {
        return new Specialty {SpecialtyId = id, Name = specialtyDto.Name};
    }

    // Doctor extensions
    public static DoctorDto AsDto(this Doctor doctor)
    {
        var specialties = doctor.Specialties;
        if (doctor.Shifts is not null) {
            var shifts = doctor.Shifts.Select(shift => shift.AsDto());
            return new DoctorDto(doctor.DoctorId, doctor.Name, doctor.DoctorInfo, doctor.DoctorImageUrl, specialties, shifts);
        }
        return new DoctorDto(doctor.DoctorId, doctor.Name, doctor.DoctorInfo, doctor.DoctorImageUrl, specialties, new List<ShiftDto>() {});
    }


    public static ShiftDto AsDto(this Shift shift)
    {
        return new ShiftDto(shift.ShiftId, shift.Weekday, shift.StartTime, shift.FinishTime, shift.Slots);
    }

    // PatientProfile extensions
    public static PatientProfileDto AsDto(this PatientProfile patientProfile)
    {
        var documents = patientProfile.PatientDocuments != null && patientProfile.PatientDocuments.Count() > 0 ? patientProfile.PatientDocuments.Select(document => document.AsDto()) : null;
        return new(patientProfile.PatientProfileId, patientProfile.FullName, patientProfile.Email, patientProfile.PhoneNumber, patientProfile.DateOfBirth, patientProfile.Gender.ToString(), patientProfile.Address, 
                    patientProfile.Occupation, patientProfile.InsuranceNumber, patientProfile.Allergies, patientProfile.CurrentMedications, 
                    patientProfile.PastMedicalHistory, documents);
    }


    //PatientDocument extensions
    public static PatientDocumentDto AsDto(this PatientDocument patientDocument)
    {
        return new(patientDocument.DocumentId, patientDocument.DocumentType.ToString(), patientDocument.DocumentName, patientDocument.DocumentUrl, patientDocument.DocumentFormat.ToString());
    }
    public static PatientDocument AsEntity(this CreatePatientDocumentDto patientDocumentDto, string url, DocumentFormat format)
    {
        return new PatientDocument
        {
            DocumentId = Guid.NewGuid(),
            DocumentType = patientDocumentDto.DocumentType,
            DocumentName = patientDocumentDto.DocumentName,
            DocumentUrl = url,
            DocumentFormat = format
        };
    }

     public static PatientDocument AsEntity(this UpdatePatientDocumentDto patientDocumentDto, string url, DocumentFormat format)
    {
        return new PatientDocument
        {
            DocumentType = patientDocumentDto.DocumentType,
            DocumentName = patientDocumentDto.DocumentName,
            DocumentUrl = url,
            DocumentFormat = format
        };
    }

    // Appointment extensions
    public static AppointmentDto AsDto(this Appointment appointment)
    {
        return new(appointment.AppointmentId, appointment.Reason, appointment.Note, appointment.AppointmentDate.ToLocalTime().DateTime, appointment.AppointmentTime, appointment.Status.ToString(), appointment.Patient, appointment.Doctor, appointment.CreatedDate);
    }

    // User extensions
    public static UserDto AsDto(this User user)
    {
        if (user.Role == Role.Admin) return new(user.UserId, user.Email, user.PhoneNumber, user.Role.ToString(), null);
        return new(user.UserId, user.Email, user.PhoneNumber, user.Role.ToString(), user.ProfileId);
    }
}