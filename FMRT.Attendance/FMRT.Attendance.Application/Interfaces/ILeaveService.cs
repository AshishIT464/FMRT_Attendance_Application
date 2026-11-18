using FMRT.Attendance.Application.DTOs.Leave;

namespace FMRT.Attendance.Application.Interfaces;

public interface ILeaveService
{
    Task<int> ApplyLeaveAsync(
        EmployeeLeaveCreateDto dto);

    Task<IEnumerable<EmployeeLeaveDto>>
        GetEmployeeLeavesAsync(
            int employeeId);

    Task ApproveLeaveAsync(
        int employeeLeaveId);

    Task RejectLeaveAsync(
        int leaveId);

    Task DeleteLeaveAsync(
        int leaveId);

    Task<LeaveBalanceDto>
        GetLeaveBalanceAsync(
            int employeeId);
}