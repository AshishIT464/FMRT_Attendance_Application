using FMRT.Attendance.Application.DTOs.Employee;
using FMRT.Attendance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FMRT.Attendance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeesController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }


        // =====================================================
        // GET ALL
        // =====================================================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var employees = await _employeeService.GetAllAsync();

            return Ok(employees);
        }

        [HttpGet("by-email")]
        public async Task<IActionResult> GetByEmail(
    [FromQuery] string email)
        {
            var employee =
                await _employeeService
                    .GetByEmailAsync(email);

            if (employee == null)
            {
                return NotFound(new
                {
                    message =
                        "Employee not found."
                });
            }

            return Ok(employee);
        }


        // =====================================================
        // GET BY ID
        // =====================================================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);

            if (employee == null)
            {
                return NotFound(new
                {
                    message = "Employee not found."
                });
            }

            return Ok(employee);
        }


        // =====================================================
        // CREATE
        // =====================================================

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] EmployeeCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.EmployeeCode))
            {
                return BadRequest(new
                {
                    message = "Employee code is required."
                });
            }

            if (string.IsNullOrWhiteSpace(dto.FullName))
            {
                return BadRequest(new
                {
                    message = "Employee name is required."
                });
            }

            var employee = await _employeeService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetById),
                new { id = employee.EmployeeId },
                employee);
        }


        // =====================================================
        // UPDATE
        // =====================================================

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(
            int id,
            [FromBody] EmployeeUpdateDto dto)
        {
            var employee = await _employeeService.UpdateAsync(id, dto);

            if (employee == null)
            {
                return NotFound(new
                {
                    message = "Employee not found."
                });
            }

            return Ok(employee);
        }


        // =====================================================
        // DELETE / DEACTIVATE
        // =====================================================

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _employeeService.DeleteAsync(id);

            if (!deleted)
            {
                return NotFound(new
                {
                    message = "Employee not found."
                });
            }

            return Ok(new
            {
                message = "Employee deactivated successfully."
            });
        }
    }
}