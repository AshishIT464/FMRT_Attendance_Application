using FMRT.Attendance.Domain.Enums;

namespace FMRT.Attendance.Domain.Entities;

public class Attendance
{
    public int AttendanceId { get; set; }

    public int EmployeeId { get; set; }

    public DateTime AttendanceDate { get; set; }

    public AttendanceStatus Status { get; set; } = AttendanceStatus.Present; 

    public DateTime? FirstIn { get; set; }

    public DateTime? LastOut { get; set; }

    public int WorkingMinutes { get; set; }

    public int LateMinutes { get; set; }

    public int EarlyMinutes { get; set; }

    public int OvertimeMinutes { get; set; }

    public int? ShiftId { get; set; }

    // eTimeTrackLite mapping
    public string? ExternalAttendanceId { get; set; }

    public bool IsOnLeave { get; set; }

    public int? LeaveTypeId { get; set; }

    public bool WeeklyOff { get; set; }

    public bool Holiday { get; set; }

    public bool MissedInPunch { get; set; }

    public bool MissedOutPunch { get; set; }

    public string? Remarks { get; set; }

    // Navigation properties
    public Employee Employee { get; set; } = null!;

    public Shift? Shift { get; set; }

    public ICollection<AttendancePunch> Punches { get; set; }
        = new List<AttendancePunch>();
}