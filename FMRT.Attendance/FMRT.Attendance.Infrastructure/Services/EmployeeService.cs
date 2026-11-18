using FMRT.Attendance.Application.DTOs.Employee;
using FMRT.Attendance.Application.Interfaces;
using FMRT.Attendance.Infrastructure.Data;
using FMRT.Attendance.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FMRT.Attendance.Infrastructure.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationDbContext _context;

        public EmployeeService(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================================
        // GET ALL
        // =====================================================

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.IsActive)
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeCode = e.EmployeeCode,
                    BiometricId = e.BiometricId,
                    FullName = e.FullName,
                    Email = e.Email,
                    MobileNumber = e.MobileNumber,
                    DepartmentId = e.DepartmentId,
                    DesignationId = e.DesignationId,
                    DepartmentName = e.Department != null
                        ? e.Department.DepartmentName
                        : null,

                    DesignationName = e.Designation != null
                        ? e.Designation.DesignationName
                        : null,
                    Role = e.Role,
                    ReportingManagerId = e.ReportingManagerId,
                    JoiningDate = e.JoiningDate,
                    IsActive = e.IsActive
                })
                .OrderBy(e => e.FullName)
                .ToListAsync();
        }


        // =====================================================
        // GET BY ID
        // =====================================================

        public async Task<EmployeeDto?> GetByIdAsync(int employeeId)
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.EmployeeId == employeeId)
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeCode = e.EmployeeCode,
                    BiometricId = e.BiometricId,
                    FullName = e.FullName,
                    Email = e.Email,
                    MobileNumber = e.MobileNumber,
                    DepartmentId = e.DepartmentId,
                    DesignationId = e.DesignationId,
                    DepartmentName = e.Department != null
                        ? e.Department.DepartmentName
                        : null,

                    DesignationName = e.Designation != null
                        ? e.Designation.DesignationName
                        : null,
                    Role = e.Role,
                    ReportingManagerId = e.ReportingManagerId,
                    JoiningDate = e.JoiningDate,
                    IsActive = e.IsActive
                })
                .FirstOrDefaultAsync();
        }


        // =====================================================
        // GET BY BIOMETRIC ID
        // =====================================================

        public async Task<EmployeeDto?> GetByBiometricIdAsync(string biometricId)
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e => e.BiometricId == biometricId)
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeCode = e.EmployeeCode,
                    BiometricId = e.BiometricId,
                    FullName = e.FullName,
                    Email = e.Email,
                    MobileNumber = e.MobileNumber,
                    DepartmentId = e.DepartmentId,
                    DesignationId = e.DesignationId,
                    DepartmentName = e.Department != null
                        ? e.Department.DepartmentName
                        : null,

                    DesignationName = e.Designation != null
                        ? e.Designation.DesignationName
                        : null,
                    Role = e.Role,
                    ReportingManagerId = e.ReportingManagerId,
                    JoiningDate = e.JoiningDate,
                    IsActive = e.IsActive
                })
                .FirstOrDefaultAsync();
        }


        public async Task<EmployeeDto?> GetByEmailAsync(string email)
        {
            return await _context.Employees
                .AsNoTracking()
                .Where(e =>
                    e.IsActive &&
                    e.Email != null &&
                    e.Email.ToLower() == email.ToLower())
                .Select(e => new EmployeeDto
                {
                    EmployeeId = e.EmployeeId,
                    EmployeeCode = e.EmployeeCode,
                    BiometricId = e.BiometricId,
                    FullName = e.FullName,
                    Email = e.Email,
                    MobileNumber = e.MobileNumber,
                    DepartmentId = e.DepartmentId,
                    DesignationId = e.DesignationId,
                    DepartmentName = e.Department != null
                        ? e.Department.DepartmentName
                        : null,

                    DesignationName = e.Designation != null
                        ? e.Designation.DesignationName
                        : null,
                    Role = e.Role,
                    ReportingManagerId = e.ReportingManagerId,
                    JoiningDate = e.JoiningDate,
                    IsActive = e.IsActive
                })
                .FirstOrDefaultAsync();
        }

        // =====================================================
        // CREATE
        // =====================================================

        public async Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto)
        {
            var employee = new Employee
            {
                EmployeeCode = dto.EmployeeCode,
                BiometricId = dto.BiometricId,
                FullName = dto.FullName,
                Email = dto.Email,
                MobileNumber = dto.MobileNumber,
                DepartmentId = dto.DepartmentId,
                DesignationId = dto.DesignationId,
                Role = "Employee",
                ReportingManagerId = null,
                JoiningDate = dto.JoiningDate,
                IsActive = dto.IsActive
            };

            _context.Employees.Add(employee);

            await _context.SaveChangesAsync();

            return await GetByIdAsync(employee.EmployeeId)
                ?? throw new InvalidOperationException(
                    "Employee was created but could not be retrieved.");
        }


        // =====================================================
        // UPDATE
        // =====================================================

        public async Task<EmployeeDto?> UpdateAsync(
            int employeeId,
            EmployeeUpdateDto dto)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
            {
                return null;
            }

            employee.EmployeeCode = dto.EmployeeCode;
            employee.BiometricId = dto.BiometricId;
            employee.FullName = dto.FullName;
            employee.Email = dto.Email;
            employee.MobileNumber = dto.MobileNumber;
            employee.DepartmentId = dto.DepartmentId;
            employee.DesignationId = dto.DesignationId;
            employee.Role = dto.Role;
            employee.ReportingManagerId = dto.ReportingManagerId;
            employee.JoiningDate = dto.JoiningDate;
            employee.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(employeeId);
        }


        // =====================================================
        // DELETE
        // =====================================================

        public async Task<bool> DeleteAsync(int employeeId)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
            {
                return false;
            }

            // Soft delete
            employee.IsActive = false;

            await _context.SaveChangesAsync();

            return true;
        }
    }
}