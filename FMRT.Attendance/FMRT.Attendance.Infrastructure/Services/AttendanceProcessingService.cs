using FMRT.Attendance.Domain.Enums;
using FMRT.Attendance.Application.Interfaces;
using FMRT.Attendance.Domain.Entities;
using FMRT.Attendance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AttendanceEntity = FMRT.Attendance.Domain.Entities.Attendance;

namespace FMRT.Attendance.Infrastructure.Services;

public class AttendanceProcessingService
    : IAttendanceProcessingService
{
    private readonly ApplicationDbContext _context;

    public AttendanceProcessingService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> GenerateAttendanceAsync()
    {
        var generated = 0;

        var employeeIds =
            await _context.Employees
                .Select(x => x.EmployeeId)
                .ToListAsync();

        foreach (var employeeId in employeeIds)
        {
            var firstPunch =
                await _context.AttendancePunches
                    .Where(x => x.EmployeeId == employeeId)
                    .OrderBy(x => x.PunchDateTime)
                    .FirstOrDefaultAsync();

            if (firstPunch == null)
                continue;

            var lastPunch =
                await _context.AttendancePunches
                    .Where(x => x.EmployeeId == employeeId)
                    .OrderByDescending(x => x.PunchDateTime)
                    .FirstAsync();

            var currentDate =
                firstPunch.PunchDateTime.Date;

            var endDate =
                DateTime.Today.Date;

            while (currentDate <= endDate)
            {
                var existingAttendance =
                  await _context.Attendances.FirstOrDefaultAsync(x =>
                     x.EmployeeId == employeeId &&
                     x.AttendanceDate.Date == currentDate);

                if (existingAttendance != null)
                {
                    _context.Attendances
                    .Remove(existingAttendance);

                    await _context.SaveChangesAsync();
                }

                var dayPunches =
                    await _context.AttendancePunches
                        .Where(x =>
                            x.EmployeeId == employeeId &&
                            x.PunchDateTime.Date == currentDate)
                        .OrderBy(x => x.PunchDateTime)
                        .ToListAsync();

                AttendanceEntity attendance;

                if (dayPunches.Any())
                {
                    var firstIn =
                        dayPunches.First().PunchDateTime;

                    var lastOut =
                        dayPunches.Last().PunchDateTime;

                    var workingMinutes =
                       (int)(lastOut - firstIn).TotalMinutes;

                    /*
                     FMRT Policy

                     Office Start Time = 09:30 AM
                     Grace Time        = 10:00 AM
                     Full Working Day  = 8.5 Hours
                                      = 510 Minutes

                     Late calculation starts after 10:00 AM
                     Overtime calculation starts after 510 Minutes
                    */

                    var graceTime =
                        currentDate
                            .AddHours(10)
                            .AddMinutes(0);

                    var lateMinutes =
                        firstIn > graceTime
                            ? (int)(firstIn - graceTime)
                                .TotalMinutes
                            : 0;

                    var overtimeMinutes =
                        workingMinutes > 510
                            ? workingMinutes - 510
                            : 0;
                    var approvedHalfDayLeave =
                         await _context.EmployeeLeaves
                        .AnyAsync(x =>
                         x.EmployeeId == employeeId &&
                         x.IsApproved &&
                         x.IsHalfDay &&
                         currentDate >= x.FromDate.Date &&
                          currentDate <= x.ToDate.Date);

                    AttendanceStatus status;
                    string remarks;

                    if (approvedHalfDayLeave)
                    {
                        status =
                            AttendanceStatus.HalfDay;

                        remarks =
                            "Approved Half Day Leave";
                    }
                    else if (workingMinutes >= 510)
                    {
                        status =
                            AttendanceStatus.Present;

                        remarks =
                            "Generated from punches";
                    }
                    else if (workingMinutes >= 240)
                    {
                        status =
                            AttendanceStatus.HalfDay;

                        remarks =
                            "Left Early";
                    }
                    else
                    {
                        status =
                            AttendanceStatus.Absent;

                        remarks =
                            "Less than 4 hours worked";
                    }

                    attendance =
                        new AttendanceEntity
                        {
                            EmployeeId = employeeId,
                            AttendanceDate = currentDate,

                            Status = status,

                            FirstIn = firstIn,
                            LastOut = lastOut,

                            WorkingMinutes = workingMinutes,

                            LateMinutes = lateMinutes,

                            OvertimeMinutes = overtimeMinutes,

                            MissedInPunch =
                                dayPunches.Count == 1,

                            MissedOutPunch =
                                dayPunches.Count == 1,

                            Remarks = remarks
                        };

                }
                else
                {
                    var approvedLeave =
                        await _context.EmployeeLeaves
                        .FirstOrDefaultAsync(x =>
                         x.EmployeeId == employeeId &&
                         x.IsApproved &&
                         currentDate >= x.FromDate.Date &&
                         currentDate <= x.ToDate.Date);

                    var holiday =
                        await _context.Holidays
                            .FirstOrDefaultAsync(x =>
                                x.HolidayDate.Date ==
                                currentDate);

                    if (approvedLeave != null)
                    {
                        attendance =
                            new AttendanceEntity
                            {
                                EmployeeId = employeeId,
                                AttendanceDate = currentDate,

                                Status =
                                    approvedLeave.IsHalfDay
                                        ? AttendanceStatus.HalfDay
                                        : AttendanceStatus.Leave,

                                IsOnLeave = true,

                                Remarks =
                                    approvedLeave.IsHalfDay
                                        ? "Approved Half Day Leave"
                                        : "Approved Leave"
                            };
                    }
                    else if (holiday != null)
                    {
                        attendance =
                            new AttendanceEntity
                            {
                                EmployeeId = employeeId,
                                AttendanceDate = currentDate,

                                Status =
                                    AttendanceStatus.Holiday,

                                Holiday = true,

                                Remarks =
                                    holiday.HolidayName
                            };
                    }
                    else if (
                        currentDate.DayOfWeek ==
                        DayOfWeek.Sunday)
                    {
                        attendance =
                            new AttendanceEntity
                            {
                                EmployeeId = employeeId,
                                AttendanceDate = currentDate,

                                Status =
                                    AttendanceStatus.WeeklyOff,

                                WeeklyOff = true,

                                Remarks =
                                    "Weekly Off"
                            };
                    }
                    else
                    {
                        attendance =
                            new AttendanceEntity
                            {
                                EmployeeId = employeeId,
                                AttendanceDate = currentDate,

                                Status =
                                    AttendanceStatus.Absent,

                                Remarks =
                                    "Absent"
                            };
                    }
                }

                _context.Attendances.Add(attendance);

                generated++;

                currentDate =
                    currentDate.AddDays(1);
            }
        }

        await _context.SaveChangesAsync();

        return generated;
    }
}