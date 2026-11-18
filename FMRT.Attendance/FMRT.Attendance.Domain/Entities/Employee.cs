namespace FMRT.Attendance.Domain.Entities;

public class Employee
{
    public int EmployeeId { get; set; }

    public string EmployeeCode { get; set; } = string.Empty;

    public string? BiometricId { get; set; }

    public string FullName { get; set; } = string.Empty;

    public string? Email { get; set; }

    public string? MobileNumber { get; set; }

    public int DepartmentId { get; set; }

    public int DesignationId { get; set; }

    public DateTime? JoiningDate { get; set; }

    public bool IsActive { get; set; } = true;


    // Navigation Properties

    public Department? Department { get; set; }

    public Designation? Designation { get; set; }

    public ICollection<Attendance> Attendances { get; set; }
        = new List<Attendance>();

    public ICollection<AttendancePunch> AttendancePunches { get; set; }
        = new List<AttendancePunch>();

    public string Role { get; set; }
    = "Employee";

    public int? ReportingManagerId { get; set; }
}