using System.Data.OleDb;
using FMRT.Attendance.Application.Interfaces;
using FMRT.Attendance.Domain.Entities;
using FMRT.Attendance.Infrastructure.Data;
using FMRT.Attendance.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace FMRT.Attendance.Infrastructure.Services;

public class DeviceLogImportService : IDeviceLogImportService
{
    private readonly ApplicationDbContext _context;

    public DeviceLogImportService(
        ApplicationDbContext context)
    {
        _context = context;
    }
    private async Task ImportEmployeesAsync(
    OleDbConnection connection)
    {
        var sql = @"
        SELECT
            EmployeeId,
            EmployeeName,
            EmployeeCode,
            EmployeeCodeInDevice
        FROM Employees";

        using var command =
            new OleDbCommand(sql, connection);

        using var reader =
            await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var biometricId =
                reader["EmployeeCodeInDevice"]
                    ?.ToString();

            var employeeName = reader["EmployeeName"]
                    ?.ToString() ?? "";
            if (
                employeeName.StartsWith(
                    "del_",
                    StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (string.IsNullOrWhiteSpace(
                biometricId))
            {
                continue;
            }

            var exists =
                await _context.Employees
                    .AnyAsync(x =>
                        x.BiometricId ==
                        biometricId);

            if (exists)
                continue;

            var employee =
                new Employee
                {
                    EmployeeCode =
                        reader["EmployeeCode"]
                            ?.ToString() ?? "",

                    BiometricId =
                        biometricId,

                    FullName = employeeName,

                    Email =
                        $"emp{biometricId}@fmrt.local",

                    MobileNumber = "",

                    DepartmentId = 1,

                    DesignationId = 1,

                    JoiningDate =
                        DateTime.Today,

                    IsActive = true
                };

            _context.Employees.Add(employee);
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> ImportDeviceLogsAsync(
        string mdbPath)
    {
        var importedCount = 0;

        var logs = new List<DeviceLogRecord>();

        var connectionString =
            $@"Provider=Microsoft.ACE.OLEDB.12.0;
            Data Source={mdbPath};
            Persist Security Info=False;";

        using var connection =
            new OleDbConnection(connectionString);

        await connection.OpenAsync();
        await ImportEmployeesAsync(connection);

        var tables =
            connection.GetSchema("Tables");

        var deviceLogTables =
            new List<string>();

        foreach (System.Data.DataRow row
                 in tables.Rows)
        {
            var tableName =
                row["TABLE_NAME"]
                    .ToString();

            if (!string.IsNullOrEmpty(tableName)
                &&
                tableName.StartsWith(
                    "DeviceLogs_"))
            {
                deviceLogTables.Add(tableName);
            }
        }

        foreach (var tableName
         in deviceLogTables)
        {
            Console.WriteLine(
                $"Reading table: {tableName}");

            var sql = $@"
    SELECT
        DeviceLogId,
        DownloadDate,
        DeviceId,
        UserId,
        LogDate,
        Direction,
        AttDirection,
        WorkCode,
        IsApproved
    FROM [{tableName}]";

            using var command =
                new OleDbCommand(
                    sql,
                    connection);

            using var reader =
                await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                var deviceLogId =
                    Convert.ToInt64(
                        reader["DeviceLogId"]);

                logs.Add(
                    new DeviceLogRecord
                    {
                        DeviceLogId =
                            deviceLogId,

                        DownloadDate =
                            reader["DownloadDate"] == DBNull.Value
                                ? null
                                : Convert.ToDateTime(
                                    reader["DownloadDate"]),

                        DeviceId = null,

                        UserId =
                            reader["UserId"]
                                ?.ToString(),

                        LogDate =
                            Convert.ToDateTime(
                                reader["LogDate"]),

                        Direction =
                            reader["Direction"]
                                ?.ToString(),

                        AttDirection =
                            reader["AttDirection"]
                                ?.ToString(),

                        WorkCode =
                            reader["WorkCode"]
                                ?.ToString(),

                        IsApproved =
                            reader["IsApproved"] == DBNull.Value
                                ? null
                                : Convert.ToInt32(
                                    reader["IsApproved"]),

                        SourceTable =
                            tableName
                    });
            }
        }

        Console.WriteLine(
            $"TOTAL LOGS READ FROM MDB = {logs.Count}");

        foreach (var log in logs)
        {
            Console.WriteLine(
                $"Processing DeviceLogId={log.DeviceLogId}, UserId=[{log.UserId}]");

            var rawRecordId =
                $"{log.SourceTable}_{log.DeviceLogId}";

            var alreadyImported =
                await _context.AttendancePunches
                    .AnyAsync(x =>
                        x.RawRecordId ==
                        rawRecordId);

            if (alreadyImported)
            {
                Console.WriteLine(
                    $"Already Imported: {rawRecordId}");

                continue;
            }

            if (string.IsNullOrWhiteSpace(log.UserId))
            {
                Console.WriteLine(
                    $"EMPTY USERID FOR LOG {log.DeviceLogId}");

                continue;
            }

            Console.WriteLine(
                $"Searching Employee using BiometricId [{log.UserId}]");

            var employee =
                await _context.Employees
                    .FirstOrDefaultAsync(x =>
                        x.BiometricId != null &&
                        x.BiometricId.Trim() ==
                        log.UserId.Trim());

            if (employee == null)
            {
                Console.WriteLine(
                    $"NO EMPLOYEE FOUND FOR USERID [{log.UserId}]");

                continue;
            }

            Console.WriteLine(
                $"MATCH FOUND => {employee.FullName}");

            var punch =
                new AttendancePunch
                {
                    EmployeeId =
                        employee.EmployeeId,

                    PunchDateTime =
                        log.LogDate,

                    DeviceId =
                        log.DeviceId,

                    UserId =
                        log.UserId,

                    Direction =
                        log.Direction,

                    AttDirection =
                        log.AttDirection,

                    WorkCode =
                        log.WorkCode,

                    IsApproved =
                        log.IsApproved == 1,

                    DownloadDate =
                        log.DownloadDate,

                    RawRecordId =
                        rawRecordId,

                    Source =
                        "eTimeTrackLite"
                };

            _context.AttendancePunches
                .Add(punch);

            Console.WriteLine(
                $"IMPORTING LOG {rawRecordId}");

            importedCount++;
        }

        await _context.SaveChangesAsync();

        Console.WriteLine(
            $"TOTAL IMPORTED = {importedCount}");

        return importedCount;
    }
}