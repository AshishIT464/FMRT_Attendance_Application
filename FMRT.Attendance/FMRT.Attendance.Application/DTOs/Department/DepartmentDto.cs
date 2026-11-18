namespace FMRT.Attendance.Application.DTOs.Department;

public class DepartmentDto
{
    public int DepartmentId { get; set; }

    public string DepartmentName { get; set; }
        = string.Empty;

    public string Code { get; set; }
        = string.Empty;

    public bool IsActive { get; set; }
}