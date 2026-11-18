using FMRT.Attendance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FMRT.Attendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ImportController : ControllerBase
{
    private readonly IDeviceLogImportService _importService;

    public ImportController(
        IDeviceLogImportService importService)
    {
        _importService = importService;
    }

    [HttpPost("devicelogs")]
    public async Task<IActionResult> ImportDeviceLogs()
    {
        var mdbPath =
            @"D:\eTimeTrackLite1.mdb";

        var importedCount =
            await _importService
                .ImportDeviceLogsAsync(mdbPath);

        return Ok(new
        {
            Success = true,
            ImportedRecords = importedCount
        });
    }
}