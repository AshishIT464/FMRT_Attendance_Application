using System;
using System.Collections.Generic;
using System.Text;

namespace FMRT.Attendance.Infrastructure.Models
{
    public class DeviceLogRecord
    {
        public long DeviceLogId { get; set; }

        public DateTime? DownloadDate { get; set; }

        public int? DeviceId { get; set; }

        public string? UserId { get; set; }

        public DateTime LogDate { get; set; }

        public string? Direction { get; set; }

        public string? AttDirection { get; set; }

        public string? WorkCode { get; set; }

        public int? IsApproved { get; set; }

        public string? SourceTable { get; set; }
    }
}
