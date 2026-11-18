namespace FMRT.Attendance.Domain.Entities
{
    public class BiometricDevice
    {
        public int DeviceId { get; set; }

        public string DeviceName { get; set; } = string.Empty;

        public string? SerialNumber { get; set; }

        public string? IPAddress { get; set; }

        public string? Location { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastSync { get; set; }


        // Navigation Property

        public ICollection<AttendancePunch> AttendancePunches { get; set; }
            = new List<AttendancePunch>();
    }
}