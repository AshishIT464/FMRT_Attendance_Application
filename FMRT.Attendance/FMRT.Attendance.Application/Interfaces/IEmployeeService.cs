using FMRT.Attendance.Application.DTOs.Employee;

namespace FMRT.Attendance.Application.Interfaces
{
    public interface IEmployeeService
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync();

        Task<EmployeeDto?> GetByIdAsync(int employeeId);

        Task<EmployeeDto?> GetByBiometricIdAsync(string biometricId);

        Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto);

        Task<EmployeeDto?> UpdateAsync(int employeeId, EmployeeUpdateDto dto);

        Task<EmployeeDto?> GetByEmailAsync(string email);

        Task<bool> DeleteAsync(int employeeId);
    }
}