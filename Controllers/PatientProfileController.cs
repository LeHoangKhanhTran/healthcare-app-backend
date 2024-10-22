using System.Globalization;
using HealthAppAPI.Entities;
using HealthAppAPI.Enums;
using HealthAppAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HealthAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PatientProfileController : ControllerBase
{
    private readonly ILogger<PatientProfileController> _logger;
    private readonly IPatientProfileRepository _patientProfileRepository;
    private readonly IUserRepository _userRepository;
    private readonly ICloudinaryUploader _cloudinaryUploader;
    public PatientProfileController(IPatientProfileRepository patientProfileRepository, ICloudinaryUploader cloudinaryUploader, ILogger<PatientProfileController> logger, IUserRepository userRepository)
    {
        _patientProfileRepository = patientProfileRepository;
        _cloudinaryUploader = cloudinaryUploader;
        _userRepository = userRepository;
        _logger = logger;
    }

    [HttpGet(Name = "GetPatientProfiles")]
    public async Task<IEnumerable<PatientProfile>> GetPatientProfiles([FromQuery]PatientProfileQueryParams queryParams)
    {
        return await _patientProfileRepository.GetPatientProfiles(queryParams);
    }

    [HttpGet("{id}", Name = "GetPatientProfileById")]
    public async Task<ActionResult<PatientProfileDto>> GetPatientProfileById(Guid id)
    {
        var patientProfile = await _patientProfileRepository.GetPatientProfileById(id);
        if (patientProfile is null) return NotFound();
        return patientProfile.AsDto();
    }

    [HttpPost(Name = "CreatePatientProfile")]
    public async Task<ActionResult> CreatePatientProfile([FromBody]CreatePatientProfileDto patientProfileDto)
    {
        var existingUser = await _userRepository.GetUserByPhoneNumber(patientProfileDto.UserPhoneNumber);
        if (existingUser is null) return NotFound("User not found");
        PatientProfile patientProfile = new()
        {
            PatientProfileId = Guid.NewGuid(),
            FullName = patientProfileDto.Fullname,
            Email = patientProfileDto.Email,
            PhoneNumber = patientProfileDto.PhoneNumber,
            DateOfBirth = patientProfileDto.DateOfBirth,
            Gender = patientProfileDto.Gender,
            Address = patientProfileDto.Address,
            Occupation = patientProfileDto.Occupation,
            InsuranceNumber = patientProfileDto.InsuranceNumber,
            Allergies = patientProfileDto.Allergies,
            CurrentMedications = patientProfileDto.CurrentMedications,
            PastMedicalHistory = patientProfileDto.PastMedicalHistory
        };
        await _patientProfileRepository.CreatePatientProfile(patientProfile);
        await _userRepository.UpdateProfile(existingUser, patientProfile.PatientProfileId);
        return Ok();
    }

    [HttpPatch("{id}/documents", Name = "AddPatientDocument")]
    public async Task<ActionResult> AddPatientDocument(Guid id, [FromForm] CreatePatientDocumentDto patientDocumentDto)
    {
        var existingPatientProfile = await _patientProfileRepository.GetPatientProfileById(id);
        if (existingPatientProfile is null) return NotFound();
        string format = Path.GetExtension(patientDocumentDto.Document.FileName).Split(".")[1];
        if (Enum.TryParse<DocumentFormat>(format, out var result))
        {
            var uploadResult = result == DocumentFormat.jpg || result == DocumentFormat.png ? await _cloudinaryUploader.UploadImage(patientDocumentDto.Document, "document") : await _cloudinaryUploader.UploadFile(patientDocumentDto.Document, "document");
            PatientDocument document = new()
            {
                DocumentId = Guid.NewGuid(),
                DocumentType = patientDocumentDto.DocumentType,
                DocumentName = patientDocumentDto.DocumentName,
                DocumentUrl = uploadResult.Url.ToString(),
                DocumentFormat = result
            };
                     
            await _patientProfileRepository.AddDocument(existingPatientProfile, document);
            return Ok();
        }
        return BadRequest();    
    }

    [HttpDelete("{id}/documents", Name = "RemovePatientDocument")]
    public async Task<ActionResult> DeleteDocument(Guid id, Guid documentId)
    {
        var existingPatientProfile = await _patientProfileRepository.GetPatientProfileById(id);
        if (existingPatientProfile is null) return NotFound();
        var document = existingPatientProfile.PatientDocuments.Where(document => document.DocumentId == documentId).SingleOrDefault();
        string type = document.DocumentFormat == DocumentFormat.pdf ? "pdf" : "image"; 
        await _cloudinaryUploader.DeleteFile(PublicIdExtractor.Extract(type, document.DocumentUrl), document.DocumentFormat);
        await _patientProfileRepository.RemoveDocument(existingPatientProfile, documentId);
        return Ok();
    }

    [HttpPut("{id}", Name = "UpdatePatientProfile")]
    public async Task<ActionResult> UpdatePatientProfile(Guid id, UpdatePatientProfileDto patientProfileDto)
    {
        var existingPatientProfile = await _patientProfileRepository.GetPatientProfileById(id);
        if (existingPatientProfile is null) return NotFound();
        PatientProfile updatedPatientProfile = new()
        {
            PatientProfileId = existingPatientProfile.PatientProfileId,
            FullName = patientProfileDto.Fullname,
            Email = patientProfileDto.Email,
            PhoneNumber = patientProfileDto.PhoneNumber,
            DateOfBirth = patientProfileDto.DateOfBirth,
            Gender = patientProfileDto.Gender,
            Address = patientProfileDto.Address,
            Occupation = patientProfileDto.Occupation,
            InsuranceNumber = patientProfileDto.InsuranceNumber,
            Allergies = patientProfileDto.Allergies,
            CurrentMedications = patientProfileDto.CurrentMedications,
            PastMedicalHistory = patientProfileDto.PastMedicalHistory, 
            PatientDocuments = existingPatientProfile.PatientDocuments
        };
        await _patientProfileRepository.UpdatePatientProfile(updatedPatientProfile);
        return Ok();
    }

    // [HttpDelete("{id}", Name = "DeletePatientProfile")]
    // public async Task<ActionResult> DeletePatientProfile(Guid id)
    // {
    //     var existingPatientProfile = await _patientProfileRepository.GetPatientProfileById(id);
    //     if (existingPatientProfile is null) return NotFound();
    //     await _patientProfileRepository.
    // }
}
