using FMRT.Attendance.Application.DTOs.Designation;
using FMRT.Attendance.Infrastructure.Data;
using FMRT.Attendance.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMRT.Attendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DesignationsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DesignationsController(
        ApplicationDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // GET ALL
    // ==========================================
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var designations =
        await _context.Designations
         .OrderBy(x => x.DesignationName)
         .Select(x => new
         {
             x.DesignationId,
             x.DesignationName,
             x.IsActive
         })
         .ToListAsync();

        return Ok(designations);
    }

    // ==========================================
    // GET BY ID
    // ==========================================
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(
        int id)
    {
        var designation =
            await _context.Designations
                .FirstOrDefaultAsync(x =>
                    x.DesignationId == id);

        if (designation == null)
        {
            return NotFound();
        }

        return Ok(designation);
    }

    // ==========================================
    // CREATE
    // ==========================================
    [HttpPost]
    public async Task<IActionResult> Create(
        DesignationDto dto)
    {
        var designation =
            new Designation
            {
                DesignationName =
                    dto.DesignationName,

                IsActive =
                    dto.IsActive
            };

        _context.Designations
            .Add(designation);

        await _context.SaveChangesAsync();

        return Ok(designation);
    }

    // ==========================================
    // UPDATE
    // ==========================================
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(
        int id,
        DesignationDto dto)
    {
        var designation =
            await _context.Designations
                .FirstOrDefaultAsync(x =>
                    x.DesignationId == id);

        if (designation == null)
        {
            return NotFound();
        }

        designation.DesignationName =
            dto.DesignationName;

        designation.IsActive =
            dto.IsActive;

        await _context.SaveChangesAsync();

        return Ok(designation);
    }

    // ==========================================
    // DELETE
    // ==========================================
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var isUsed =
            await _context.Employees
                .AnyAsync(x =>
                    x.DesignationId == id);

        if (isUsed)
        {
            return BadRequest(
                "Designation is assigned to employees.");
        }

        var designation =
            await _context.Designations
                .FirstOrDefaultAsync(x =>
                    x.DesignationId == id);

        if (designation == null)
        {
            return NotFound();
        }

        _context.Designations
            .Remove(designation);

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message =
                "Designation deleted successfully."
        });
    }
}