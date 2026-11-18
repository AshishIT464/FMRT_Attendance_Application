namespace FMRT.Attendance.Application.DTOs.Designation;

public class DesignationDto
{
    public int DesignationId { get; set; }

    public string DesignationName { get; set; }
        = string.Empty;

    public bool IsActive { get; set; }
}