using System;
using System.Collections.Generic;
using System.Text;

namespace FMRT.Attendance.Application.Interfaces
{
    public interface IDeviceLogImportService
    {
        Task<int> ImportDeviceLogsAsync(string mdbPath);
    }
}