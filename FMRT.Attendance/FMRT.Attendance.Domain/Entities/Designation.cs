namespace FMRT.Attendance.Domain.Entities
{
    public class Designation
    {
        public int DesignationId { get; set; }

        public string DesignationName { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}