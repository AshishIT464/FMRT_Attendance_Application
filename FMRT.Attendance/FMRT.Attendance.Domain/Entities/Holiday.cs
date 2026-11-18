namespace FMRT.Attendance.Domain.Entities;

public class Holiday
{
    public int HolidayId { get; set; }

    public DateTime HolidayDate { get; set; }

    public string HolidayName { get; set; }
        = string.Empty;

    public bool IsRestricted { get; set; }
}
