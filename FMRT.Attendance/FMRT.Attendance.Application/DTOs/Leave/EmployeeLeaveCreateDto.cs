namespace FMRT.Attendance.Application.DTOs.Leave;

public class EmployeeLeaveCreateDto
{
    public int EmployeeId { get; set; }

    public int LeaveTypeId { get; set; }

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string Reason { get; set; }
        = string.Empty;

    public bool IsHalfDay { get; set; }
}