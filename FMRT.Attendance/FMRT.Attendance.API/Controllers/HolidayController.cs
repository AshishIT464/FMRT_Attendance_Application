using FMRT.Attendance.Domain.Entities;
using FMRT.Attendance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMRT.Attendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HolidayController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public HolidayController(
        ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var holidays =
            await _context.Holidays
                .OrderBy(x => x.HolidayDate)
                .ToListAsync();

        return Ok(holidays);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromBody] Holiday holiday)
    {
        _context.Holidays.Add(holiday);

        await _context.SaveChangesAsync();

        return Ok(holiday);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        [FromBody] Holiday holiday)
    {
        var existingHoliday =
            await _context.Holidays
                .FirstOrDefaultAsync(x =>
                    x.HolidayId == id);

        if (existingHoliday == null)
        {
            return NotFound();
        }

        existingHoliday.HolidayName =
            holiday.HolidayName;

        existingHoliday.HolidayDate =
            holiday.HolidayDate;

        existingHoliday.IsRestricted =
            holiday.IsRestricted;

        await _context.SaveChangesAsync();

        return Ok(existingHoliday);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var holiday =
            await _context.Holidays
                .FirstOrDefaultAsync(x =>
                    x.HolidayId == id);

        if (holiday == null)
        {
            return NotFound();
        }

        _context.Holidays.Remove(holiday);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}