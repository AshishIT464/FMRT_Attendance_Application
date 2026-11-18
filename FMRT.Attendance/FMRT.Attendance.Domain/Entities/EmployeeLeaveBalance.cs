namespace FMRT.Attendance.Domain.Entities;

public class EmployeeLeaveBalance
{
    public int EmployeeLeaveBalanceId { get; set; }

    public int EmployeeId { get; set; }

    public decimal CasualLeaveBalance { get; set; }

    public decimal SickLeaveBalance { get; set; }

    public decimal EarnedLeaveBalance { get; set; }

    public decimal RestrictedHolidayBalance { get; set; }

    public Employee Employee { get; set; }
        = null!;
}