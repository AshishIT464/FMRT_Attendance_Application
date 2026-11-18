using FMRT.Attendance.Application.DTOs.Attendance;
using FMRT.Attendance.Application.Interfaces;
using FMRT.Attendance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FMRT.Attendance.Infrastructure.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationDbContext _context;

        public AttendanceService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AttendanceDto>> GetEmployeeAttendanceAsync(
            int employeeId,
            DateTime fromDate,
            DateTime toDate)
        {
            return await _context.Attendances
                .AsNoTracking()
                .Where(a =>
                    a.EmployeeId == employeeId &&
                    a.AttendanceDate >= fromDate.Date &&
                    a.AttendanceDate < toDate.Date.AddDays(1))
                .OrderBy(a => a.AttendanceDate)
                .Select(a => new AttendanceDto
                {
                    AttendanceId = a.AttendanceId,
                    AttendanceDate = a.AttendanceDate,
                    Status = a.Status.ToString(),
                    FirstIn = a.FirstIn,
                    LastOut = a.LastOut,
                    WorkingMinutes = a.WorkingMinutes,
                    LateMinutes = a.LateMinutes,
                    EarlyMinutes = a.EarlyMinutes,
                    OvertimeMinutes = a.OvertimeMinutes,
                    ShiftName = a.Shift != null
                        ? a.Shift.ShiftName
                        : null
                })
                .ToListAsync();
        }

        public async Task<AttendanceDto?> GetEmployeeAttendanceByDateAsync(
            int employeeId,
            DateTime date)
        {
            return await _context.Attendances
                .AsNoTracking()
                .Where(a =>
                    a.EmployeeId == employeeId &&
                    a.AttendanceDate == date.Date)
                .Select(a => new AttendanceDto
                {
                    AttendanceId = a.AttendanceId,
                    AttendanceDate = a.AttendanceDate,
                    Status = a.Status.ToString(),
                    FirstIn = a.FirstIn,
                    LastOut = a.LastOut,
                    WorkingMinutes = a.WorkingMinutes,
                    LateMinutes = a.LateMinutes,
                    EarlyMinutes = a.EarlyMinutes,
                    OvertimeMinutes = a.OvertimeMinutes,
                    ShiftName = a.Shift != null
                        ? a.Shift.ShiftName
                        : null
                })
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AttendancePunchDto>> GetEmployeePunchesAsync(
            int employeeId,
            DateTime date)
        {
            return await _context.AttendancePunches
                .AsNoTracking()
                .Where(p =>
                    p.EmployeeId == employeeId &&
                    p.PunchDateTime >= date.Date &&
                    p.PunchDateTime < date.Date.AddDays(1))
                .OrderBy(p => p.PunchDateTime)
                .Select(p => new AttendancePunchDto
                {
                    PunchId = p.PunchId,
                    PunchDateTime = p.PunchDateTime,
                    PunchType = p.PunchType,
                    Source = p.Source,
                    DeviceName = p.Device != null
                        ? p.Device.DeviceName
                        : null
                })
                .ToListAsync();
        }

        public async Task<MonthlyAttendanceSummaryDto> GetMonthlySummaryAsync(
            int employeeId,
            int year,
            int month)
        {
            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var records = await _context.Attendances
                .AsNoTracking()
                .Where(a =>
                    a.EmployeeId == employeeId &&
                    a.AttendanceDate >= startDate &&
                    a.AttendanceDate < endDate)
                .ToListAsync();

            var totalDays = records.Count;

            var presentDays = records.Count(a =>
                a.Status == Domain.Enums.AttendanceStatus.Present);

            var absentDays = records.Count(a =>
                a.Status == Domain.Enums.AttendanceStatus.Absent);

            var leaveDays = records.Count(a =>
                a.Status == Domain.Enums.AttendanceStatus.Leave);

            var weeklyOffDays = records.Count(a =>
                a.Status == Domain.Enums.AttendanceStatus.WeeklyOff);

            var holidayDays = records.Count(a =>
                a.Status == Domain.Enums.AttendanceStatus.Holiday);

            var lateDays = records.Count(a =>
                a.LateMinutes > 0);

            var totalWorkingMinutes = records.Sum(a => a.WorkingMinutes);

            var totalOvertimeMinutes = records.Sum(a => a.OvertimeMinutes);

            var attendancePercentage = totalDays > 0
                ? Math.Round(
                    (double)presentDays / totalDays * 100,
                    2)
                : 0;

            return new MonthlyAttendanceSummaryDto
            {
                Year = year,
                Month = month,
                TotalDays = totalDays,
                PresentDays = presentDays,
                AbsentDays = absentDays,
                LeaveDays = leaveDays,
                WeeklyOffDays = weeklyOffDays,
                HolidayDays = holidayDays,
                LateDays = lateDays,
                TotalWorkingMinutes = totalWorkingMinutes,
                TotalOvertimeMinutes = totalOvertimeMinutes,
                AttendancePercentage = attendancePercentage
            };
        }

        public async Task<int> CreateAsync(AttendanceCreateDto dto)
        {
            // Check employee
            var employeeExists = await _context.Employees
                .AnyAsync(e => e.EmployeeId == dto.EmployeeId);

            if (!employeeExists)
            {
                throw new InvalidOperationException(
                    "Employee does not exist.");
            }

            // Check shift if provided
            if (dto.ShiftId.HasValue)
            {
                var shiftExists = await _context.Shifts
                    .AnyAsync(s => s.ShiftId == dto.ShiftId.Value);

                if (!shiftExists)
                {
                    throw new InvalidOperationException(
                        "Shift does not exist.");
                }
            }

            var attendanceDate = dto.AttendanceDate.Date;

            // Check duplicate attendance
            var existingAttendance = await _context.Attendances
                .AnyAsync(a =>
                    a.EmployeeId == dto.EmployeeId &&
                    a.AttendanceDate == attendanceDate);

            if (existingAttendance)
            {
                throw new InvalidOperationException(
                    "Attendance already exists for this employee and date.");
            }

            var attendance = new Domain.Entities.Attendance
            {
                EmployeeId = dto.EmployeeId,
                AttendanceDate = attendanceDate,
                Status = dto.Status,

                FirstIn = dto.FirstIn,
                LastOut = dto.LastOut,

                WorkingMinutes = dto.WorkingMinutes,
                LateMinutes = dto.LateMinutes,
                EarlyMinutes = dto.EarlyMinutes,
                OvertimeMinutes = dto.OvertimeMinutes,

                ShiftId = dto.ShiftId,

                IsOnLeave = dto.IsOnLeave,
                LeaveTypeId = dto.LeaveTypeId,

                WeeklyOff = dto.WeeklyOff,
                Holiday = dto.Holiday,

                MissedInPunch = dto.MissedInPunch,
                MissedOutPunch = dto.MissedOutPunch,

                Remarks = dto.Remarks
            };

            _context.Attendances.Add(attendance);

            await _context.SaveChangesAsync();

            return attendance.AttendanceId;
        }
        public async Task<long> CreatePunchAsync(
    int employeeId,
    AttendancePunchCreateDto dto)
        {
            var employeeExists = await _context.Employees
                .AnyAsync(e => e.EmployeeId == employeeId);

            if (!employeeExists)
            {
                throw new InvalidOperationException(
                    "Employee does not exist.");
            }

            var punch = new Domain.Entities.AttendancePunch
            {
                EmployeeId = employeeId,
                PunchDateTime = dto.PunchDateTime,
                PunchType = dto.PunchType,
                Source = dto.Source,
                DeviceId = dto.DeviceId,
                RawRecordId = dto.RawRecordId,
                ExternalPunchId = dto.ExternalPunchId,
                UserId = dto.UserId,
                Direction = dto.Direction,
                AttDirection = dto.AttDirection,
                DownloadDate = dto.DownloadDate,
                WorkCode = dto.WorkCode,
                IsApproved = dto.IsApproved
            };

            _context.AttendancePunches.Add(punch);

            await _context.SaveChangesAsync();

            return punch.PunchId;
        }
    }


}