using FMRT.Attendance.Application.DTOs.Attendance;

namespace FMRT.Attendance.Application.Interfaces
{
    public interface IAttendanceService
    {
        Task<IEnumerable<AttendanceDto>> GetEmployeeAttendanceAsync(
            int employeeId,
            DateTime fromDate,
            DateTime toDate);

        Task<AttendanceDto?> GetEmployeeAttendanceByDateAsync(
            int employeeId,
            DateTime date);

        Task<IEnumerable<AttendancePunchDto>> GetEmployeePunchesAsync(
            int employeeId,
            DateTime date);

        Task<MonthlyAttendanceSummaryDto> GetMonthlySummaryAsync(
            int employeeId,
            int year,
            int month);

        Task<int> CreateAsync(
            AttendanceCreateDto dto);

        Task<long> CreatePunchAsync(
            int employeeId,
            AttendancePunchCreateDto dto);
    }
}