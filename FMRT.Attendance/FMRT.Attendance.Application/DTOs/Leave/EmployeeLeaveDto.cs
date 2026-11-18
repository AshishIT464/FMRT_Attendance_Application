
namespace FMRT.Attendance.Application.DTOs.Leave;

public class EmployeeLeaveDto
{
    public int EmployeeLeaveId { get; set; }

    public string LeaveType { get; set; }
        = string.Empty;

    public DateTime FromDate { get; set; }

    public DateTime ToDate { get; set; }

    public string Reason { get; set; }
        = string.Empty;

    public bool IsApproved { get; set; }
}