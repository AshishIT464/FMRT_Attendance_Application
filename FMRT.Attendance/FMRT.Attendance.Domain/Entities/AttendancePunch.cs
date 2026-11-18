namespace FMRT.Attendance.Domain.Entities;

public class AttendancePunch
{
    public long PunchId { get; set; }

    // Employee associated with this punch
    public int EmployeeId { get; set; }

    // Attendance summary associated with this punch
    public int? AttendanceId { get; set; }

    // Actual biometric punch time
    public DateTime PunchDateTime { get; set; }

    // IN / OUT if available from the biometric system
    public string? PunchType { get; set; }

    // Biometric device
    public int? DeviceId { get; set; }

    // Where the record came from
    // Example: eTimeTrackLite
    public string Source { get; set; } = string.Empty;

    // Original record ID from the biometric system
    public string? RawRecordId { get; set; }

    // External punch ID used to prevent duplicate imports
    public string? ExternalPunchId { get; set; }

    // =========================================================
    // eTimeTrackLite fields
    // =========================================================

    public string? UserId { get; set; }

    public string? Direction { get; set; }

    public string? AttDirection { get; set; }

    public DateTime? DownloadDate { get; set; }

    public string? WorkCode { get; set; }

    public bool? IsApproved { get; set; }

    // =========================================================
    // Audit
    // =========================================================

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;


    // =========================================================
    // Navigation Properties
    // =========================================================

    public Employee Employee { get; set; } = null!;

    public Attendance? Attendance { get; set; }

    public BiometricDevice? Device { get; set; }
}