namespace HealthAppAPI.Enums;
public enum Role 
{
    Patient,
    Admin
}

public enum AppointmentStatus 
{
    Scheduled,
    Pending,
    Cancelled
}


public enum Gender
{
    Male,
    Female,
    Other
}

public enum DocumentType
{
    MedicalDocument,
    AdministrativeDocument,
    IdentificationDocument,
    Other
}

public enum DocumentFormat 
{
    jpg,
    png,
    pdf
}