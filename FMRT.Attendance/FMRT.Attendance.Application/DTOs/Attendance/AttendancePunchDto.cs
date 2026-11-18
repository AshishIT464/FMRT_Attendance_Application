namespace FMRT.Attendance.Application.DTOs.Attendance
{
    public class AttendancePunchDto
    {
        public long PunchId { get; set; }

        public DateTime PunchDateTime { get; set; }

        public string? PunchType { get; set; }

        public string Source { get; set; } = string.Empty;

        public string? DeviceName { get; set; }
    }
}