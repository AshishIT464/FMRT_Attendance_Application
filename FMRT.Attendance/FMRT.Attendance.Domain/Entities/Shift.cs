namespace FMRT.Attendance.Domain.Entities
{
    public class Shift
    {
        public int ShiftId { get; set; }

        public string ShiftCode { get; set; } = string.Empty;

        public string ShiftName { get; set; } = string.Empty;

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public int GraceMinutes { get; set; }

        public int MinimumWorkingMinutes { get; set; }

        public bool IsActive { get; set; } = true;


        // Navigation Property

        public ICollection<Attendance> Attendances { get; set; } = new List<Attendance>();
    }
}