namespace FMRT.Attendance.Application.DTOs.Attendance
{
    public class AttendanceDto
    {
        public int AttendanceId { get; set; }

        public DateTime AttendanceDate { get; set; }

        public string Status { get; set; } = string.Empty;

        public DateTime? FirstIn { get; set; }

        public DateTime? LastOut { get; set; }

        public int WorkingMinutes { get; set; }

        public int LateMinutes { get; set; }

        public int EarlyMinutes { get; set; }

        public int OvertimeMinutes { get; set; }

        public string? ShiftName { get; set; }
    }
}