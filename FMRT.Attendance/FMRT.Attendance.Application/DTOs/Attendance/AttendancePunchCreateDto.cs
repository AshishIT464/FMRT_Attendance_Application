namespace FMRT.Attendance.Application.DTOs.Attendance
{
    public class AttendancePunchCreateDto
    {
        public DateTime PunchDateTime { get; set; }

        public string? PunchType { get; set; }

        public string Source { get; set; } = "Manual";

        public int? DeviceId { get; set; }

        public string? RawRecordId { get; set; }

        public string? ExternalPunchId { get; set; }

        public string? UserId { get; set; }

        public string? Direction { get; set; }

        public string? AttDirection { get; set; }

        public DateTime? DownloadDate { get; set; }

        public string? WorkCode { get; set; }

        public bool? IsApproved { get; set; }
    }
}