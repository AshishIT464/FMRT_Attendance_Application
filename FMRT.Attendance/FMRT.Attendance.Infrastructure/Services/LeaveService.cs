using FMRT.Attendance.Application.DTOs.Leave;
using FMRT.Attendance.Application.Interfaces;
using FMRT.Attendance.Domain.Entities;
using FMRT.Attendance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FMRT.Attendance.Infrastructure.Services;

public class LeaveService
    : ILeaveService
{
    private readonly ApplicationDbContext
        _context;

    public LeaveService(
        ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<int> ApplyLeaveAsync(
        EmployeeLeaveCreateDto dto)
    {
        var leave =
    new EmployeeLeave
    {
        EmployeeId = dto.EmployeeId,
        LeaveTypeId = dto.LeaveTypeId,
        FromDate = dto.FromDate,
        ToDate = dto.ToDate,
        Reason = dto.Reason,

        IsHalfDay = dto.IsHalfDay,

        IsApproved = false
    };

        _context.EmployeeLeaves
            .Add(leave);

        await _context.SaveChangesAsync();

        return leave.EmployeeLeaveId;
    }

    public async Task<IEnumerable<EmployeeLeaveDto>>
        GetEmployeeLeavesAsync(
            int employeeId)
    {
        return await _context.EmployeeLeaves
            .Where(x =>
                x.EmployeeId == employeeId)
            .Include(x => x.LeaveType)
            .Select(x =>
                new EmployeeLeaveDto
                {
                    EmployeeLeaveId =
                        x.EmployeeLeaveId,

                    LeaveType =
                        x.LeaveType.LeaveName,

                    FromDate =
                        x.FromDate,

                    ToDate =
                        x.ToDate,

                    Reason =
                        x.Reason,

                    IsApproved =
                        x.IsApproved
                })
            .ToListAsync();
    }

    public async Task ApproveLeaveAsync(
     int employeeLeaveId)
    {
        var leave =
            await _context.EmployeeLeaves
                .Include(x => x.LeaveType)
                .FirstAsync(x =>
                    x.EmployeeLeaveId ==
                    employeeLeaveId);

        if (leave.IsApproved)
            return;

        var balance =
            await _context.EmployeeLeaveBalances
                .FirstOrDefaultAsync(x =>
                    x.EmployeeId ==
                    leave.EmployeeId);

        if (balance == null)
        {
            balance =
                new EmployeeLeaveBalance
                {
                    EmployeeId =
                        leave.EmployeeId,

                    CasualLeaveBalance = 0,

                    SickLeaveBalance = 0,

                    EarnedLeaveBalance = 0,

                    RestrictedHolidayBalance = 4
                };

            _context.EmployeeLeaveBalances
                .Add(balance);

            await _context.SaveChangesAsync();
        }

        decimal leaveDays =
    leave.IsHalfDay
        ? 0.5m
        : (leave.ToDate.Date -
           leave.FromDate.Date)
          .Days + 1;

        switch (leave.LeaveType.LeaveCode)
        {
            case "CL":

                if (balance.CasualLeaveBalance
                    < leaveDays)
                {
                    throw new Exception(
                        "Insufficient Casual Leave balance.");
                }

                balance.CasualLeaveBalance
                    -= leaveDays;

                break;

            case "SL":

                if (balance.SickLeaveBalance
                    < leaveDays)
                {
                    throw new Exception(
                        "Insufficient Sick Leave balance.");
                }

                balance.SickLeaveBalance
                    -= leaveDays;

                break;

            case "EL":

                if (balance.EarnedLeaveBalance
                    < leaveDays)
                {
                    throw new Exception(
                        "Insufficient Earned Leave balance.");
                }

                balance.EarnedLeaveBalance
                    -= leaveDays;

                break;

            case "RH":

                if (balance.RestrictedHolidayBalance
                    < leaveDays)
                {
                    throw new Exception(
                        "Insufficient Restricted Holiday balance.");
                }

                balance.RestrictedHolidayBalance
                    -= leaveDays;

                break;
        }

        leave.IsApproved = true;

        await _context.SaveChangesAsync();
    }

    public async Task<LeaveBalanceDto>
 GetLeaveBalanceAsync(
     int employeeId)
    {
        var balance =
            await _context.EmployeeLeaveBalances
                .FirstOrDefaultAsync(x =>
                    x.EmployeeId ==
                    employeeId);

        if (balance == null)
        {
            balance =
                  new EmployeeLeaveBalance
            {
               EmployeeId = employeeId,

               CasualLeaveBalance = 0,

               SickLeaveBalance = 0,

               EarnedLeaveBalance = 0,

               RestrictedHolidayBalance = 4
             };

            _context.EmployeeLeaveBalances
                .Add(balance);

            await _context
                .SaveChangesAsync();
        }

        return new LeaveBalanceDto
        {
            CasualLeaveBalance =
                balance.CasualLeaveBalance,

            SickLeaveBalance =
                balance.SickLeaveBalance,

            EarnedLeaveBalance =
                balance.EarnedLeaveBalance,

            RestrictedHolidayBalance =
                balance.RestrictedHolidayBalance
        };
    }
    public async Task RejectLeaveAsync(
    int leaveId)
    {
        var leave =
            await _context.EmployeeLeaves
                .FirstOrDefaultAsync(x =>
                    x.EmployeeLeaveId == leaveId);

        if (leave == null)
            throw new Exception(
                "Leave not found.");

        _context.EmployeeLeaves.Remove(
            leave);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteLeaveAsync(
    int leaveId)
    {
        var leave =
            await _context.EmployeeLeaves
                .Include(x => x.LeaveType)
                .FirstOrDefaultAsync(x =>
                    x.EmployeeLeaveId == leaveId);

        if (leave == null)
            throw new Exception(
                "Leave not found.");

        if (leave.IsApproved)
        {
            var balance =
                await _context.EmployeeLeaveBalances
                    .FirstOrDefaultAsync(x =>
                        x.EmployeeId ==
                        leave.EmployeeId);

            if (balance != null)
            {
                decimal leaveDays =
                    leave.IsHalfDay
                        ? 0.5m
                        : (leave.ToDate.Date -
                           leave.FromDate.Date)
                          .Days + 1;

                switch (leave.LeaveType.LeaveCode)
                {
                    case "CL":
                        balance.CasualLeaveBalance += leaveDays;
                        break;

                    case "SL":
                        balance.SickLeaveBalance += leaveDays;
                        break;

                    case "EL":
                        balance.EarnedLeaveBalance += leaveDays;
                        break;

                    case "RH":
                        balance.RestrictedHolidayBalance += leaveDays;
                        break;
                }
            }
        }

        _context.EmployeeLeaves.Remove(
            leave);

        await _context.SaveChangesAsync();
    }
}