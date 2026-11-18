namespace FMRT.Attendance.Application.DTOs.Leave;

public class LeaveBalanceDto
{
    public decimal CasualLeaveBalance { get; set; }

    public decimal SickLeaveBalance { get; set; }

    public decimal EarnedLeaveBalance { get; set; }

    public decimal RestrictedHolidayBalance { get; set; }
}