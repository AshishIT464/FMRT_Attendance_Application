namespace FMRT.Attendance.Application.DTOs.Employee
{
    public class EmployeeCreateDto
    {
        public string EmployeeCode { get; set; } = string.Empty;

        public string? BiometricId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? MobileNumber { get; set; }

        public int DepartmentId { get; set; }

        public int DesignationId { get; set; }

        public DateTime? JoiningDate { get; set; }

        public bool IsActive { get; set; } = true;
    }
}