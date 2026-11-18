using FMRT.Attendance.Application.DTOs.Leave;
using FMRT.Attendance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FMRT.Attendance.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaveController
    : ControllerBase
{
    private readonly ILeaveService
        _service;

    public LeaveController(
        ILeaveService service)
    {
        _service = service;
    }

    [HttpPost]
    public async Task<IActionResult>
        ApplyLeave(
            EmployeeLeaveCreateDto dto)
    {
        var id =
            await _service
                .ApplyLeaveAsync(dto);

        return Ok(
            new
            {
                LeaveId = id
            });
    }

    [HttpGet("{employeeId}")]
    public async Task<IActionResult>
        GetLeaves(
            int employeeId)
    {
        return Ok(
            await _service
                .GetEmployeeLeavesAsync(
                    employeeId));
    }

    [HttpPost("{leaveId}/approve")]
    public async Task<IActionResult>
        Approve(
            int leaveId)
    {
        await _service
            .ApproveLeaveAsync(
                leaveId);

        return Ok();
    }

    [HttpGet("{employeeId}/balance")]
    public async Task<IActionResult>
    GetBalance(int employeeId)
    {
        return Ok(
            await _service
                .GetLeaveBalanceAsync(
                    employeeId));
    }

    [HttpPost("{leaveId}/reject")]
    public async Task<IActionResult> Reject(
    int leaveId)
    {
        await _service
            .RejectLeaveAsync(leaveId);

        return Ok();
    }

    [HttpDelete("{leaveId}")]
    public async Task<IActionResult> Delete(
    int leaveId)
    {
        await _service
            .DeleteLeaveAsync(leaveId);

        return NoContent();
    }

}

