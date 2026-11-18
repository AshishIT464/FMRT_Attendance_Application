namespace FMRT.Attendance.Domain.Entities;

public class EmployeeLeave
{
    public int EmployeeLeaveId { get; set; }

    public int EmployeeId { get; set; }

    public int LeaveTypeId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string Reason { get; set; }
        = string.Empty;

    public bool IsApproved { get; set; }

    public bool IsHalfDay { get; set; }

    public Employee Employee { get; set; }
        = null!;

    public LeaveType LeaveType { get; set; }
        = null!;
}