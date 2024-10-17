
using HealthAppAPI.Enums;
using System.Text.Json.Serialization;

public record PatientDocumentDto(Guid DocumentId, string DocumentType, string DocumentName, string DocumentUrl, string DocumentFormat);
public record CreatePatientDocumentDto(DocumentType DocumentType, IFormFile Document, string DocumentName);
public record UpdatePatientDocumentDto(DocumentType DocumentType, IFormFile Document, string DocumentName); 