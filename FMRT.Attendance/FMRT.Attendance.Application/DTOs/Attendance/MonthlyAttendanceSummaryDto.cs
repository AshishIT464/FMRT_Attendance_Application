namespace FMRT.Attendance.Application.DTOs.Attendance
{
    public class MonthlyAttendanceSummaryDto
    {
        public int Year { get; set; }

        public int Month { get; set; }

        public int TotalDays { get; set; }

        public int PresentDays { get; set; }

        public int AbsentDays { get; set; }

        public int LeaveDays { get; set; }

        public int WeeklyOffDays { get; set; }

        public int HolidayDays { get; set; }

        public int LateDays { get; set; }

        public int TotalWorkingMinutes { get; set; }

        public int TotalOvertimeMinutes { get; set; }

        public double AttendancePercentage { get; set; }
    }
}