using ClosedXML.Excel;
using System.IO;
using FMRT.Attendance.Application.DTOs.Attendance;
using FMRT.Attendance.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using DocumentFormat.OpenXml.Wordprocessing;

namespace FMRT.Attendance.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet("{employeeId:int}")]
        public async Task<IActionResult> GetAttendance(
            int employeeId,
            [FromQuery] DateTime fromDate,
            [FromQuery] DateTime toDate)
        {
            var attendance = await _attendanceService
                .GetEmployeeAttendanceAsync(
                    employeeId,
                    fromDate,
                    toDate);

            return Ok(attendance);
        }

        [HttpGet("{employeeId:int}/date")]
        public async Task<IActionResult> GetAttendanceByDate(
            int employeeId,
            [FromQuery] DateTime date)
        {
            var attendance = await _attendanceService
                .GetEmployeeAttendanceByDateAsync(
                    employeeId,
                    date);

            if (attendance == null)
            {
                return NotFound(new
                {
                    message = "Attendance record not found."
                });
            }

            return Ok(attendance);
        }

        [HttpGet("{employeeId:int}/punches")]
        public async Task<IActionResult> GetPunches(
            int employeeId,
            [FromQuery] DateTime date)
        {
            var punches = await _attendanceService
                .GetEmployeePunchesAsync(
                    employeeId,
                    date);

            return Ok(punches);
        }

        [HttpGet("{employeeId:int}/summary")]
        public async Task<IActionResult> GetMonthlySummary(
            int employeeId,
            [FromQuery] int year,
            [FromQuery] int month)
        {
            if (month < 1 || month > 12)
            {
                return BadRequest(new
                {
                    message = "Month must be between 1 and 12."
                });
            }

            var summary = await _attendanceService
                .GetMonthlySummaryAsync(
                    employeeId,
                    year,
                    month);

            return Ok(summary);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
        [FromBody] AttendanceCreateDto dto)
        {
            try
            {
                var attendanceId = await _attendanceService.CreateAsync(dto);

                return CreatedAtAction(
                    nameof(GetAttendanceByDate),
                    new
                    {
                        employeeId = dto.EmployeeId,
                        date = dto.AttendanceDate
                    },
                    new
                    {
                        attendanceId = attendanceId,
                        message = "Attendance created successfully."
                    });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    message = ex.Message
                });
            }
        }

        [HttpGet("{employeeId:int}/export-excel")]
        public async Task<IActionResult> ExportExcel(
    int employeeId,
    [FromQuery] int year,
    [FromQuery] int month)
        {
            var fromDate =
                new DateTime(year, month, 1);

            var toDate =
                fromDate
                    .AddMonths(1)
                    .AddDays(-1);

            var data =
                await _attendanceService
                    .GetEmployeeAttendanceAsync(
                        employeeId,
                        fromDate,
                        toDate);

            using var workbook =
                new XLWorkbook();

            var worksheet =
                workbook.Worksheets.Add(
                    "Attendance Report");

            // =================================================
            // Report Header
            // =================================================

            worksheet.Range("A1:E2").Merge();

            worksheet.Cell("A1").Value =
                "FMRT ATTENDANCE REPORT";

            worksheet.Cell("A1")
                .Style.Font.SetBold();

            worksheet.Cell("A1")
                .Style.Font.FontSize = 18;

            worksheet.Cell("A1")
                .Style.Font.FontColor =
                    XLColor.White;

            worksheet.Cell("A1")
                .Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

            worksheet.Cell("A1")
                .Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;

            worksheet.Range("A1:E2")
                .Style.Fill.BackgroundColor =
                    XLColor.DarkBlue;

            // =================================================
            // Employee Info
            // =================================================

            worksheet.Cell("A4").Value =
                "Employee Name";

            worksheet.Cell("B5").Value =
                "Ashish Gupta";

            worksheet.Cell("A5").Value =
                "Employee Id";

            worksheet.Cell("B5").Value =
                employeeId;

            worksheet.Cell("D5").Value =
                "Month";

            worksheet.Cell("E5").Value =
                $"{fromDate:MMMM yyyy}";

            worksheet.Cell("D3").Value =
                "Generated On";

            worksheet.Cell("E3").Value =
                DateTime.Now.ToString(
                    "dd-MMM-yyyy HH:mm");

            worksheet.Range("A4:E5")
                .Style.Font.Bold = true;

            // =================================================
            // Summary
            // =================================================

            var presentDays =
                data.Count(x =>
                    x.Status == "Present");

            var absentDays =
                data.Count(x =>
                    x.Status == "Absent");

            var leaveDays =
                data.Count(x =>
                    x.Status == "Leave");

            var holidayDays =
                data.Count(x =>
                    x.Status == "Holiday");

            var weeklyOffDays =
                data.Count(x =>
                    x.Status == "WeeklyOff");

            worksheet.Range("A7:E7").Merge();

            worksheet.Cell("A7").Value =
                "SUMMARY";

            worksheet.Cell("A7")
                .Style.Font.SetBold();

            worksheet.Cell("A7")
                .Style.Font.FontSize = 16;

            worksheet.Cell("A7")
                .Style.Alignment.Horizontal =
                    XLAlignmentHorizontalValues.Center;

            worksheet.Cell("A7")
                .Style.Alignment.Vertical =
                    XLAlignmentVerticalValues.Center;

            worksheet.Cell("A8").Value =
                "Present";

            worksheet.Cell("B8").Value =
                presentDays;

            worksheet.Cell("A9").Value =
                "Absent";

            worksheet.Cell("B9").Value =
                absentDays;

            worksheet.Cell("A10").Value =
                "Leave";

            worksheet.Cell("B10").Value =
                leaveDays;

            worksheet.Cell("A11").Value =
                "Holiday";

            worksheet.Cell("B11").Value =
                holidayDays;

            worksheet.Cell("A12").Value =
                "Weekly Off";

            worksheet.Cell("B12").Value =
                weeklyOffDays;

            // =================================================
            // Table Header
            // =================================================

            var headerRow = 15;

            worksheet.Cell(headerRow, 1).Value =
                "Date";

            worksheet.Cell(headerRow, 2).Value =
                "Status";

            worksheet.Cell(headerRow, 3).Value =
                "First In";

            worksheet.Cell(headerRow, 4).Value =
                "Last Out";

            worksheet.Cell(headerRow, 5).Value =
                "Working Hours";

            worksheet.Range(
                $"A{headerRow}:E{headerRow}")
                .Style.Font.Bold = true;

            worksheet.Range(
                $"A{headerRow}:E{headerRow}")
                .Style.Fill.BackgroundColor =
                    XLColor.SteelBlue;

            worksheet.Range(
                $"A{headerRow}:E{headerRow}")
                .Style.Font.FontColor =
                    XLColor.White;

            // =================================================
            // Data Rows
            // =================================================

            int row = headerRow + 1;

            foreach (var item in data)
            {
                worksheet.Cell(row, 1).Value =
                    item.AttendanceDate
                        .ToString("dd-MMM-yyyy");

                worksheet.Cell(row, 2).Value =
                    item.Status;

                worksheet.Cell(row, 3).Value =
                    item.FirstIn?
                        .ToString("hh:mm tt")
                    ?? "-";

                worksheet.Cell(row, 4).Value =
                    item.LastOut?
                        .ToString("hh:mm tt")
                    ?? "-";

                worksheet.Cell(row, 5).Value =
                    FormatMinutes(
                        item.WorkingMinutes);

                // Status Color

                if (string.Equals(
        item.Status,
        "Present",
        StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Cell(row, 2)
                        .Style.Fill.BackgroundColor =
                            XLColor.LightGreen;
                }
                else if (string.Equals(
                            item.Status,
                            "Absent",
                            StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Cell(row, 2)
                        .Style.Fill.BackgroundColor =
                            XLColor.LightPink;
                }
                else if (string.Equals(
                            item.Status,
                            "Leave",
                            StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Cell(row, 2)
                        .Style.Fill.BackgroundColor =
                            XLColor.Plum;
                }
                else if (string.Equals(
                            item.Status,
                            "Holiday",
                            StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Cell(row, 2)
                        .Style.Fill.BackgroundColor =
                            XLColor.LightBlue;
                }
                else if (string.Equals(
                            item.Status,
                            "WeeklyOff",
                            StringComparison.OrdinalIgnoreCase))
                {
                    worksheet.Cell(row, 2)
                        .Style.Fill.BackgroundColor =
                            XLColor.LightSalmon;
                }


                row++;
            }

            // =================================================
            // Formatting
            // =================================================

            worksheet.Columns()
                .AdjustToContents();

            var usedRange =
    worksheet.RangeUsed();

            if (usedRange != null)
            {
                usedRange.Style.Border.OutsideBorder =
                    XLBorderStyleValues.Thin;

                usedRange.Style.Border.InsideBorder =
                    XLBorderStyleValues.Thin;
            }

            worksheet.SheetView
                .FreezeRows(headerRow);

            using var stream =
                new MemoryStream();

            workbook.SaveAs(stream);

            return File(
                stream.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                $"Attendance_Report_{fromDate:MMM_yyyy}.xlsx");
        }

        private static string FormatMinutes(int minutes)
        {
            if (minutes <= 0)
                return "-";

            var hrs = minutes / 60;
            var mins = minutes % 60;

            if (hrs == 0)
                return $"{mins} min";

            return $"{hrs} hr {mins} min";
        }
    }
    
 }