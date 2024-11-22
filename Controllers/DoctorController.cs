using HealthAppAPI.Entities;
using HealthAppAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HealthAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoctorController : ControllerBase
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly ICloudinaryUploader _cloudinaryUploader;
    private readonly ILogger<DoctorController> _logger;
    public DoctorController(IDoctorRepository doctorRepository, ICloudinaryUploader cloudinaryUploader, ILogger<DoctorController> logger)
    {
        _doctorRepository = doctorRepository;
        _cloudinaryUploader = cloudinaryUploader;
        _logger = logger;
    }

    [HttpGet(Name = "GetDoctors")]
    public async Task<object> GetDoctors([FromQuery] DoctorQueryParams queryParams)
    {
        var result= await _doctorRepository.GetDoctors(queryParams);
        if (result is PaginatedList<Doctor> doctors) 
        {
            return new PaginatedList<DoctorDto>(
                ((PaginatedList<Doctor>)result).ListItems.Select(d => d.AsDto()),
                ((PaginatedList<Doctor>)result).CurrentPage,
                ((PaginatedList<Doctor>)result).TotalPage
            );
        }
        return ((IEnumerable<Doctor>)result).Select(d => d.AsDto());
    }

    [HttpGet("{id}", Name = "GetDoctorById")]
    public async Task<ActionResult<DoctorDto>> GetDoctorById(Guid id)
    {
        var doctor =  await _doctorRepository.GetDoctorById(id);
        if (doctor is null) return NotFound();
        return doctor.AsDto();  
    }
    
    [HttpPost(Name = "CreateNewDoctor")]
    public async Task<ActionResult<DoctorDto>> CreateDoctor([FromForm] CreateDoctorDto doctorDto)
    {
        string imageUrl = "";
        try {
            if (doctorDto.DoctorImage is not null)
            {
                var uploadResult = await _cloudinaryUploader.UploadImage(doctorDto.DoctorImage, "doctor");
                imageUrl = uploadResult.Url.ToString();
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            return BadRequest(exception.Message);
        }
        Doctor doctor = new()
        {
            DoctorId = Guid.NewGuid(),
            Name = doctorDto.Name,
            DoctorInfo = doctorDto.DoctorInfo,
            DoctorImageUrl = imageUrl,
            Specialties = doctorDto.Specialties,
        };
        await _doctorRepository.CreateDoctor(doctor);
        return Ok();
    }

    [HttpPut("{id}", Name = "UpdateDoctor")]
    public async Task<ActionResult> UpdateDoctor(Guid id, [FromForm] UpdateDoctorDto doctorDto)
    {
        var existingDoctor = await _doctorRepository.GetDoctorById(id);
        if (existingDoctor is null) return NotFound();
        string imageUrl = "";
        try {
            if (doctorDto.DoctorImage is not null)
            {
                var uploadResult = await _cloudinaryUploader.UploadImage(doctorDto.DoctorImage, "doctor");
                imageUrl = uploadResult.Url.ToString();
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception.Message);
            return BadRequest(exception.Message);
        }
        Doctor updatedDoctor = new()
        {
            DoctorId = id, 
            Name = doctorDto.Name, 
            DoctorInfo = doctorDto.DoctorInfo, 
            DoctorImageUrl = imageUrl.Length > 0 ? imageUrl : existingDoctor.DoctorImageUrl, 
            Specialties = doctorDto.Specialties,
            Shifts = existingDoctor.Shifts
        };
        await _doctorRepository.UpdateDoctor(updatedDoctor);
        return Ok();
    }

    [HttpPatch("{id}/shifts", Name = "AddShift")]
    public async Task<ActionResult> AddShift(Guid id, CreateShiftDto shiftDto)
    {
        var existingDoctor = await _doctorRepository.GetDoctorById(id);
        if (existingDoctor is null) return NotFound("This doctor does not exist");
        Time start = TimeExtractor.Extract(shiftDto.StartTime);
        Time finish = TimeExtractor.Extract(shiftDto.FinishTime);
        TimeOnly startTime = new(start.Hour, start.Min);
        TimeOnly finishTime = new(finish.Hour, finish.Min);
        if (finishTime < startTime) return BadRequest("Finish time must be after start time");
        Shift newShift = new(shiftDto.Weekday, startTime.ToString("HH:mm"), finishTime.ToString("HH:mm"), shiftDto.Slots);
        if (existingDoctor.Shifts is not null)
        {
            foreach (var shift in existingDoctor.Shifts)
            {
                if (Shift.IsOverlap(shift, newShift)) return BadRequest($"This shift overlaps with shift of id {shift.ShiftId}");
            };
        }
        await _doctorRepository.AddShift(existingDoctor, newShift);
        return Ok();
    }

    [HttpDelete("{id}/shifts", Name = "RemoveShift")]
    public async Task<ActionResult> RemoveShift(Guid id, Guid shiftId)
    {
        var existingDoctor = await _doctorRepository.GetDoctorById(id);
        if (existingDoctor is null) return NotFound("This doctor does not exist");
        await _doctorRepository.RemoveShift(existingDoctor, shiftId);
        return Ok();
    }

    [HttpDelete("{id}",Name = "RemoveDoctor")]
    public async Task<ActionResult> DeleteDoctor(Guid id)
    {
        var existingDoctor = await _doctorRepository.GetDoctorById(id);
        if (existingDoctor is null) return NotFound();
        await _cloudinaryUploader.DeleteImage(new List<string>() {PublicIdExtractor.Extract("image", existingDoctor.DoctorImageUrl)});
        await _doctorRepository.DeleteDoctor(id);
        return Ok();
    }
}
