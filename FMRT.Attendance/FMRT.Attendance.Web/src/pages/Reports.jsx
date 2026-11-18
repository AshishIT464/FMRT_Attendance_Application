import { useEffect, useState } from "react";
import api from "../api/attendanceApi";
import { useEmployee } from "../context/EmployeeContext";

export default function Reports() {

  const { employeeId } = useEmployee();

  const [month, setMonth] = useState(9);
  const [year, setYear] = useState(2026);

  const [reportData, setReportData] = useState([]);

  useEffect(() => {
    generateReport();
  }, [employeeId, month, year]);

  async function generateReport() {

    try {

      const lastDay =
        new Date(year, month, 0).getDate();

      const response =
        await api.get(
          `/Attendance/${employeeId}`,
          {
            params: {
              fromDate:
                `${year}-${String(month).padStart(2, "0")}-01`,
              toDate:
                `${year}-${String(month).padStart(2, "0")}-${String(lastDay).padStart(2, "0")}`
            }
          }
        );

      setReportData(response.data);

    } catch (error) {

      console.error(error);

    }
  }

  function exportExcel() {

    const url =
      `/api/Attendance/${employeeId}/export-excel?year=${year}&month=${month}`;

    window.open(url, "_blank");
  }

  function formatMinutes(minutes) {

    if (!minutes)
      return "0 min";

    const hrs =
      Math.floor(minutes / 60);

    const mins =
      minutes % 60;

    if (hrs === 0)
      return `${mins} min`;

    return `${hrs} hr ${mins} min`;
  }

  const presentCount =
    reportData.filter(
      x => x.status === "Present"
    ).length;

  const absentCount =
    reportData.filter(
      x => x.status === "Absent"
    ).length;

  const leaveCount =
    reportData.filter(
      x => x.status === "Leave"
    ).length;

  const holidayCount =
    reportData.filter(
      x => x.status === "Holiday"
    ).length;

  const weeklyOffCount =
    reportData.filter(
      x => x.status === "WeeklyOff"
    ).length;

  const halfDayLeaveCount =
    reportData.filter(
      x =>
        x.status === "HalfDay" &&
        x.remarks ===
          "Approved Half Day Leave"
    ).length;

  const leftEarlyCount =
    reportData.filter(
      x =>
        x.status === "HalfDay" &&
        x.remarks === "Left Early"
    ).length;

  const totalWorkingMinutes =
    reportData.reduce(
      (sum, x) =>
        sum + (x.workingMinutes || 0),
      0
    );

  const totalOvertimeMinutes =
    reportData.reduce(
      (sum, x) =>
        sum + (x.overtimeMinutes || 0),
      0
    );

  return (

    <div className="space-y-6">

      <div className="
        flex
        flex-col
        lg:flex-row
        justify-between
        gap-4
      ">

        <div>

          <h1 className="text-3xl font-bold">
            Attendance Reports
          </h1>

          <p className="text-slate-500 mt-1">
            Monthly Attendance Analysis
          </p>

        </div>

        <button
          onClick={exportExcel}
          className="
            bg-green-600
            text-white
            px-5
            py-3
            rounded-xl
            hover:bg-green-700
          "
        >
          Export Excel
        </button>

      </div>

      <div className="
        bg-white
        rounded-3xl
        shadow
        p-6
      ">

        <div className="
          grid
          md:grid-cols-2
          gap-4
        ">

          <select
            value={month}
            onChange={(e) =>
              setMonth(
                Number(e.target.value)
              )
            }
            className="
              border
              p-3
              rounded-xl
            "
          >
            <option value={1}>January</option>
            <option value={2}>February</option>
            <option value={3}>March</option>
            <option value={4}>April</option>
            <option value={5}>May</option>
            <option value={6}>June</option>
            <option value={7}>July</option>
            <option value={8}>August</option>
            <option value={9}>September</option>
            <option value={10}>October</option>
            <option value={11}>November</option>
            <option value={12}>December</option>
          </select>

          <select
            value={year}
            onChange={(e) =>
              setYear(
                Number(e.target.value)
              )
            }
            className="
              border
              p-3
              rounded-xl
            "
          >
            <option value={2025}>2025</option>
            <option value={2026}>2026</option>
            <option value={2027}>2027</option>
          </select>

        </div>

      </div>

      <div className="
        grid
        grid-cols-2
        md:grid-cols-4
        xl:grid-cols-8
        gap-4
      ">

        <ReportCard
          title="Present"
          value={presentCount}
        />

        <ReportCard
          title="Absent"
          value={absentCount}
        />

        <ReportCard
          title="Leave"
          value={leaveCount}
        />

        <ReportCard
          title="Holiday"
          value={holidayCount}
        />

        <ReportCard
          title="Weekly Off"
          value={weeklyOffCount}
        />

        <ReportCard
          title="Half Day Leave"
          value={halfDayLeaveCount}
        />

        <ReportCard
          title="Left Early"
          value={leftEarlyCount}
        />

        <ReportCard
          title="Records"
          value={reportData.length}
        />

      </div>

      <div className="
        grid
        md:grid-cols-2
        gap-4
      ">

        <ReportCard
          title="Working Hours"
          value={formatMinutes(
            totalWorkingMinutes
          )}
        />

        <ReportCard
          title="OT Hours"
          value={formatMinutes(
            totalOvertimeMinutes
          )}
        />

      </div>

      <div className="
        bg-white
        rounded-3xl
        shadow
        p-6
        overflow-x-auto
      ">

        <table className="w-full">

          <thead>

            <tr className="border-b">

              <th className="p-4 text-left">
                Date
              </th>

              <th className="p-4 text-left">
                Status
              </th>

              <th className="p-4 text-left">
                First In
              </th>

              <th className="p-4 text-left">
                Last Out
              </th>

              <th className="p-4 text-left">
                Working Hours
              </th>

              <th className="p-4 text-left">
                Late
              </th>

              <th className="p-4 text-left">
                Overtime
              </th>

              <th className="p-4 text-left">
                Remarks
              </th>

            </tr>

          </thead>

          <tbody>

            {reportData.map(item => {

              let badgeColor =
                "bg-slate-500";

              let statusText =
                item.status;

              if (
                item.status ===
                "Present"
              ) {
                badgeColor =
                  "bg-green-500";
              }

              if (
                item.status ===
                "Absent"
              ) {
                badgeColor =
                  "bg-red-500";
              }

              if (
                item.status ===
                "Leave"
              ) {
                badgeColor =
                  "bg-purple-500";
              }

              if (
                item.status ===
                "Holiday"
              ) {
                badgeColor =
                  "bg-blue-500";
              }

              if (
                item.status ===
                "WeeklyOff"
              ) {
                badgeColor =
                  "bg-orange-500";
              }

              if (
                item.status ===
                "HalfDay"
              ) {

                if (
                  item.remarks ===
                  "Approved Half Day Leave"
                ) {

                  badgeColor =
                    "bg-blue-500";

                  statusText =
                    "Half Day Leave";
                }
                else {

                  badgeColor =
                    "bg-amber-500";

                  statusText =
                    "Left Early";
                }
              }

              return (

                <tr
                  key={
                    item.attendanceId
                  }
                  className="
                    border-b
                    hover:bg-slate-50
                  "
                >

                  <td className="p-4">
                    {new Date(
                      item.attendanceDate
                    ).toLocaleDateString()}
                  </td>

                  <td className="p-4">

                    <span
                      className={`
                        px-3 py-1
                        rounded-full
                        text-white
                        ${badgeColor}
                      `}
                    >
                      {statusText}
                    </span>

                  </td>

                  <td className="p-4">
                    {item.firstIn
                      ? new Date(
                          item.firstIn
                        ).toLocaleTimeString()
                      : "-"}
                  </td>

                  <td className="p-4">
                    {item.lastOut
                      ? new Date(
                          item.lastOut
                        ).toLocaleTimeString()
                      : "-"}
                  </td>

                  <td className="p-4">
                    {formatMinutes(
                      item.workingMinutes
                    )}
                  </td>

                  <td className="p-4">
                    {formatMinutes(
                      item.lateMinutes
                    )}
                  </td>

                  <td className="p-4">
                    {formatMinutes(
                      item.overtimeMinutes
                    )}
                  </td>

                  <td className="p-4">
                    {item.remarks}
                  </td>

                </tr>

              );

            })}

          </tbody>

        </table>

      </div>

    </div>

  );
}

function ReportCard({
  title,
  value
}) {
  return (
    <div className="
      bg-white
      rounded-3xl
      shadow
      p-5
      border
    ">
      <p className="text-slate-500">
        {title}
      </p>

      <h2 className="
        text-2xl
        font-bold
        mt-2
      ">
        {value}
      </h2>
    </div>
  );
}