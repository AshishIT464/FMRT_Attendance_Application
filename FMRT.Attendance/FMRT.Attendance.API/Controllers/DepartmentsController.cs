using FMRT.Attendance.Application.DTOs.Department;
using FMRT.Attendance.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FMRT.Attendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DepartmentsController(
        ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var departments =
         await _context.Departments
        .OrderBy(x => x.DepartmentName)
        .Select(x => new
        {
            x.DepartmentId,
            x.DepartmentName,
            x.Code,
            x.IsActive
        })
        .ToListAsync();

        return Ok(departments);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        DepartmentDto dto)
    {
        var department =
            new Domain.Entities.Department
            {
                DepartmentName =
                    dto.DepartmentName,

                Code =
                    dto.Code,

                IsActive =
                    dto.IsActive
            };

        _context.Departments.Add(
            department);

        await _context.SaveChangesAsync();

        return Ok(department);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var department =
            await _context.Departments
                .FirstOrDefaultAsync(x =>
                    x.DepartmentId == id);

        if (department == null)
        {
            return NotFound();
        }

        return Ok(department);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(
        int id,
        DepartmentDto dto)
    {
        var department =
            await _context.Departments
                .FirstOrDefaultAsync(x =>
                    x.DepartmentId == id);

        if (department == null)
        {
            return NotFound();
        }

        department.DepartmentName =
            dto.DepartmentName;

        department.Code =
            dto.Code;

        department.IsActive =
            dto.IsActive;

        await _context.SaveChangesAsync();

        return Ok(department);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(
        int id)
    {
        var isUsed =
            await _context.Employees
                .AnyAsync(x =>
                    x.DepartmentId == id);

        if (isUsed)
        {
            return BadRequest(
                "Department assigned to employees.");
        }

        var department =
            await _context.Departments
                .FirstOrDefaultAsync(x =>
                    x.DepartmentId == id);

        if (department == null)
        {
            return NotFound();
        }

        _context.Departments.Remove(
            department);

        await _context.SaveChangesAsync();

        return Ok();
    }
}