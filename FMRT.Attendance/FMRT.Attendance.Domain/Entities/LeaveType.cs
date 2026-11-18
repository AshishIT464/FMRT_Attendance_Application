namespace FMRT.Attendance.Domain.Entities;

public class LeaveType
{
    public int LeaveTypeId { get; set; }

    public string LeaveCode { get; set; }
        = string.Empty;

    public string LeaveName { get; set; }
        = string.Empty;
}
