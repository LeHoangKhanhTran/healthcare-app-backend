using System.Globalization;
using HealthAppAPI.Entities;
using HealthAppAPI.Enums;
using HealthAppAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HealthAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AppointmentController : ControllerBase
{
    private readonly ILogger<AppointmentController> _logger;
    private readonly IAppointmentRepository _appointmentRepository;
    private readonly IDoctorRepository _doctorRepository;
    private readonly IPatientProfileRepository _patientProfileRepository;
    public AppointmentController(IAppointmentRepository appointmentRepository, IDoctorRepository doctorRepository, IPatientProfileRepository patientProfileRepository, ILogger<AppointmentController> logger)
    {
        _doctorRepository = doctorRepository;
        _appointmentRepository = appointmentRepository;
        _patientProfileRepository = patientProfileRepository;
        _logger = logger;
    }

    [HttpGet(Name = "GetAppointments")]
    public async Task<IEnumerable<AppointmentDto>> GetAppointments([FromQuery] AppointmentQueryParams queryParams)
    {
        var appointments = await _appointmentRepository.GetAppointments(queryParams);
        return appointments.Select(appointment => appointment.AsDto());
    }

    [HttpGet("{id}", Name = "GetAppointmentById")]
    public async Task<AppointmentDto> GetAppointmentId(Guid id)
    {
        return (await _appointmentRepository.GetAppointmentById(id)).AsDto();
    }

    [HttpGet("patient/{patientId}", Name = "GetAppointmentsByPatientId")]
    public async Task<IEnumerable<AppointmentDto>> GetAppointmentsByPatientId(Guid patientId)
    {
        return (await _appointmentRepository.GetAppointmentsByPatientId(patientId)).Select(appointment => appointment.AsDto());
    }

    [HttpPost(Name = "CreateNewAppointment")]
    public async Task<ActionResult<AppointmentDto>> CreateAppointment(CreateAppointmentDto appointmentDto)
    {
        var doctor = await _doctorRepository.GetDoctorById(appointmentDto.DoctorId);
        Calendar calendar = CultureInfo.CurrentCulture.Calendar;
        DateTimeOffset parsedDate = DateTimeOffset.ParseExact(appointmentDto.AppointmentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        DayOfWeek dayOfWeek = calendar.GetDayOfWeek(parsedDate.DateTime);
        string[] appointmentTime = appointmentDto.AppointmentTime.Split("-");
        if (doctor is null) return NotFound("Doctor does not exist");
        var selectedShift = doctor.Shifts.Where(shift => shift.Weekday == dayOfWeek && shift.StartTime == appointmentTime[0] && shift.FinishTime == appointmentTime[1]).SingleOrDefault();
        long appointmentCount = await _appointmentRepository.GetCountByAppointmentTime(doctor.DoctorId, parsedDate.DateTime, appointmentDto.AppointmentTime);
        if (selectedShift is null) {
            return BadRequest("No shift for this appointment");
        }
        if (selectedShift is not null && appointmentCount >= selectedShift.Slots) {
            return BadRequest();
        }
        var patient = await _patientProfileRepository.GetPatientProfileById(appointmentDto.PatientId);
        if (patient is null) 
        {
            return NotFound("Patient not found");
        }
        AppointmentDoctor appointmentDoctor = new() {
            DoctorId = doctor.DoctorId,
            Name = doctor.Name,
            ImageUrl = doctor.DoctorImageUrl
        };
        
        AppointmentPatient appointmentPatient = new() {
            PatientId = patient.PatientProfileId,
            Name = patient.FullName
        };
        Appointment appointment = new()
        {
            AppointmentId = Guid.NewGuid(),
            Reason = appointmentDto.Reason,
            Note = appointmentDto.Note,
            AppointmentDate = DateTime.ParseExact(appointmentDto.AppointmentDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToUniversalTime(),
            AppointmentTime = appointmentDto.AppointmentTime,
            Status = AppointmentStatus.Pending,
            Patient = appointmentPatient,
            Doctor = appointmentDoctor,
            CreatedDate = DateTime.Now   
        };
        await _appointmentRepository.CreateAppointment(appointment);
        return Ok(new {appointment.AppointmentId});
        
    }

    // [HttpPut("{id}", Name = "UpdateAppointment")]
    // public async Task<ActionResult> UpdateAppointment(Guid id, UpdateAppointmentDto appointmentDto)
    // {
    //     var existingAppointment = await _appointmentRepository.GetAppointmentById(id);
    //     if (existingAppointment is null) return NotFound();
    //     await _appointmentRepository.UpdateAppointment(appointmentDto.AsEntity(id, existingAppointment.PatientId));
    //     return Ok();
    // }

    [HttpPatch("{id}/schedule", Name = "ScheduleAppoinment")]
    public async Task<ActionResult> ScheduleAppointment(Guid id)
    {
        var existingAppointment = await _appointmentRepository.GetAppointmentById(id);
        if (existingAppointment is null) return NotFound();
        var doctor = await _doctorRepository.GetDoctorById(existingAppointment.Doctor.DoctorId);
        if (doctor is null) return NotFound("Doctor does not exist");
        Calendar calendar = CultureInfo.CurrentCulture.Calendar;
        DayOfWeek dayOfWeek = calendar.GetDayOfWeek(existingAppointment.AppointmentDate.ToLocalTime().DateTime);
        string[] appointmentTime = existingAppointment.AppointmentTime.Split("-");
        var selectedShift = doctor.Shifts.Where(shift => shift.Weekday == dayOfWeek && shift.StartTime == appointmentTime[0] && shift.FinishTime == appointmentTime[1]).SingleOrDefault();
        if (selectedShift is null) return NotFound("No shift found for this doctor.");
        long appointmentCount = await _appointmentRepository.GetCountByAppointmentTime(doctor.DoctorId, existingAppointment.AppointmentDate.DateTime, existingAppointment.AppointmentTime);
        if (selectedShift is not null && appointmentCount >= selectedShift.Slots) {
            return BadRequest("No slot left.");
        }
        await _appointmentRepository.UpdateAppointmentStatus(id, AppointmentStatus.Scheduled);
        return Ok();
    }

    [HttpPatch("{id}/cancel", Name = "CancelAppoinment")]
    public async Task<ActionResult> CancelAppointment(Guid id)
    {
        var existingAppointment = await _appointmentRepository.GetAppointmentById(id);
        if (existingAppointment is null) return NotFound();
        await _appointmentRepository.UpdateAppointmentStatus(id, AppointmentStatus.Cancelled);
        return Ok();
    }

    // [HttpDelete("{id}",Name = "DeleteAppointment")]
    // public Task<ActionResult> DeleteAppointment(Guid id)
    // {
    //     throw new NotImplementedException();
    // }

    [HttpGet("{status}/count")]
    public async Task<long> GetCountByStatus(AppointmentStatus status) {
        return await _appointmentRepository.GetCountByStatus(status);
    }

}
