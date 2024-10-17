using HealthAppAPI.Entities;
using HealthAppAPI.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace HealthAppAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SpecialtyController : ControllerBase
{
    private readonly ILogger<SpecialtyController> _logger;
    private readonly ISpecialtyRepository _specialtyRepository;
    public SpecialtyController(ISpecialtyRepository specialtyRepository, ILogger<SpecialtyController> logger)
    {
        _logger = logger;
        _specialtyRepository = specialtyRepository;
    }

    [HttpGet(Name = "GetSpecialties")]
    public async Task<IEnumerable<SpecialtyDto>> GetAllSpecialties([FromQuery] SpecialtyQueryParams queryParams)
    {
        return (await _specialtyRepository.GetSpecialties(queryParams)).Select(specialty => specialty.AsDto());
    }

    [HttpPost(Name = "CreateNewSpecialty")]
    public async Task CreateSpecialty(CreateSpecialtyDto specialtyDto)
    {
        await _specialtyRepository.CreateSpecialty(new Specialty {SpecialtyId = Guid.NewGuid() , Name = specialtyDto.Name});
    }

    // [HttpPut("{id}", Name = "UpdateSpecilaty")]
    // public async Task<ActionResult> UpdateSpecialty(Guid id, UpdateSpecialtyDto specialtyDto)
    // {
    //     var existingSpecialty = _specialtyRepository.GetSpecialtyById(id);
    //     if (existingSpecialty is null) return NotFound();
    //     await _specialtyRepository.UpdateSpecialty(new Specialty() {SpecialtyId = id , Name = specialtyDto.Name});
    //     return Ok();
    // }

    [HttpDelete("{id}", Name = "DeleteSpecialty")]
    public async Task<ActionResult> DeleteSpecialty(Guid id)
    {
        var existingSpecialty = _specialtyRepository.GetSpecialtyById(id);
        if (existingSpecialty is null) return NotFound();
        await _specialtyRepository.DeleteSpecialty(id);
        return Ok();
    }
}
