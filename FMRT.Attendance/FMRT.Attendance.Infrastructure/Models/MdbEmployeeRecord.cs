namespace FMRT.Attendance.Infrastructure.Models;

public class MdbEmployeeRecord
{
    public int EmployeeId { get; set; }

    public string? EmployeeName { get; set; }

    public string? EmployeeCode { get; set; }

    public string? EmployeeCodeInDevice { get; set; }
}