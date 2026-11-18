using FMRT.Attendance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FMRT.Attendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AttendanceProcessingController
    : ControllerBase
{
    private readonly
        IAttendanceProcessingService
        _service;

    public AttendanceProcessingController(
        IAttendanceProcessingService service)
    {
        _service = service;
    }

    [HttpPost("generate")]
    public async Task<IActionResult>
        GenerateAttendance()
    {
        var count =
            await _service
                .GenerateAttendanceAsync();

        return Ok(new
        {
            Generated = count
        });
    }

    [HttpPost("reprocess")]
    public async Task<IActionResult>
        ReprocessAttendance()
    {
        var count =
            await _service
                .GenerateAttendanceAsync();

        return Ok(new
        {
            Generated = count,
            Message =
                "Attendance reprocessed successfully."
        });
    }
}