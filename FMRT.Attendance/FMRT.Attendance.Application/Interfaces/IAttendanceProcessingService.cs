namespace FMRT.Attendance.Application.Interfaces;

public interface IAttendanceProcessingService
{
    Task<int> GenerateAttendanceAsync();
}
