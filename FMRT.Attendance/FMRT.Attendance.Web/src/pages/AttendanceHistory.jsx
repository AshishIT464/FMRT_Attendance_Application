import { useEffect, useState } from "react";
import api from "../api/attendanceApi";
import { useEmployee } from "../context/EmployeeContext";

export default function AttendanceHistory() {

  const { employeeId } =
    useEmployee();

  const [records, setRecords] =
    useState([]);

  const [summary, setSummary] =
    useState(null);

  const [month, setMonth] =
    useState(new Date().getMonth() + 1);

  const [year, setYear] =
    useState(new Date().getFullYear());

  useEffect(() => {

    loadAttendance();
    loadSummary();

  }, [employeeId, month, year]);

  async function loadAttendance() {

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

      setRecords(response.data);

    } catch (error) {

      console.error(error);

    }
  }

  async function loadSummary() {

    try {

      const response =
        await api.get(
          `/Attendance/${employeeId}/summary`,
          {
            params: {
              year,
              month
            }
          }
        );

      setSummary(response.data);

    } catch (error) {

      console.error(error);

    }
  }

  function exportExcel() {

    const url =
      `/api/Attendance/${employeeId}/export-excel?year=${year}&month=${month}`;

    window.open(url, "_blank");
  }

  function formatDate(date) {

    return new Date(date)
      .toLocaleDateString("en-IN");

  }

  function formatTime(date) {

    if (!date)
      return "--";

    return new Date(date)
      .toLocaleTimeString(
        "en-IN",
        {
          hour: "2-digit",
          minute: "2-digit"
        }
      );
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

  function getStatusClass(
  status,
  remarks
) {

  if (
    status === "HalfDay"
  ) {

    if (
      remarks ===
      "Approved Half Day Leave"
    ) {
      return "bg-blue-100 text-blue-700";
    }

    if (
      remarks === "Left Early"
    ) {
      return "bg-amber-100 text-amber-700";
    }
  }

  switch (status) {

    case "Present":
      return "bg-green-100 text-green-700";

    case "Absent":
      return "bg-red-100 text-red-700";

    case "Leave":
      return "bg-purple-100 text-purple-700";

    case "Holiday":
      return "bg-cyan-100 text-cyan-700";

    case "WeeklyOff":
      return "bg-orange-100 text-orange-700";

    default:
      return "bg-slate-100 text-slate-700";
  }
}

  return (

    <div className="space-y-6">

      <div className="
        flex
        flex-col
        lg:flex-row
        lg:items-center
        lg:justify-between
        gap-4
      ">

        <div>

          <h1 className="
            text-3xl
            font-bold
          ">
            Attendance History
          </h1>

          <p className="
            text-slate-500
            mt-1
          ">
            Monthly Attendance Records
          </p>

        </div>

        <button
          onClick={exportExcel} className="bg-green-600 text-white px-5 py-3 rounded-xl hover:bg-green-700">
          Export Excel
        </button>

      </div>

      <div className="bg-white rounded-3xl shadow p-6">
        <div className="grid md:grid-cols-2 gap-4">
          <select value={month} onChange={(e) => 
            setMonth( Number( e.target.value )
              )
            }
            className="border p-3 rounded-xl">
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
                Number(
                  e.target.value
                )
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

      {summary && (

        <div className="
          grid
          grid-cols-2
          lg:grid-cols-4
          gap-4
        ">

          <SummaryCard
            title="Present"
            value={summary.presentDays}
            color="bg-green-50"
          />

          <SummaryCard
            title="Absent"
            value={summary.absentDays}
            color="bg-red-50"
          />

          <SummaryCard
            title="Leave"
            value={summary.leaveDays}
            color="bg-yellow-50"
          />

          <SummaryCard
            title="Holiday"
            value={summary.holidayDays}
            color="bg-blue-50"
          />

        </div>

      )}

      {/* Mobile View */}

      <div className="
        lg:hidden
        space-y-4
      ">

        {records.map(item => (

          <div
            key={item.attendanceId}
            className="
              bg-white
              rounded-3xl
              shadow
              p-5
            "
          >

            <div className="
              flex
              justify-between
              items-center
              mb-4
            ">

              <h3 className="
                font-semibold
              ">
                {formatDate(
                  item.attendanceDate
                )}
              </h3>

              <span
                className={`
                  px-3
                  py-1
                  rounded-full
                  text-sm
                  ${getStatusClass(
                     item.status,
                     item.remarks
                    )}
                `}
              >
                {
                    item.status === "HalfDay"
                    ? (
                   item.remarks ===
                   "Approved Half Day Leave"
                    )
                   ? "Half Day Leave"
                   : "Left Early"
                   : item.status
                 }
              </span>

            </div>

            <div className="
              grid
              grid-cols-2
              gap-3
              text-sm
            ">

              <Info
                label="First In"
                value={formatTime(
                  item.firstIn
                )}
              />

              <Info
                label="Last Out"
                value={formatTime(
                  item.lastOut
                )}
              />

              <Info
                label="Working"
                value={formatMinutes(
                  item.workingMinutes
                )}
              />

              <Info
                label="Late"
                value={formatMinutes(
                  item.lateMinutes
                )}
              />

              <Info
                label="OT"
                value={formatMinutes(
                  item.overtimeMinutes
                )}
              />
              <Info
                label="Remarks"
                value={
                item.remarks || "-"
              }
             />

            </div>

          </div>

        ))}

      </div>

      {/* Desktop View */}

      <div className="hidden lg:block bg-white rounded-3xl shadow p-6 overflow-x-auto">
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

            {records.map(item => (

              <tr
                key={item.attendanceId}
                className="
                  border-b
                  hover:bg-slate-50
                "
              >

                <td className="p-4">
                  {formatDate(
                    item.attendanceDate
                  )}
                </td>

                <td className="p-4">

                  <span
                    className={`
                      px-3 py-1
                      rounded-full
                      ${getStatusClass(
                       item.status,
                       item.remarks
                      )}
                    `}
                  >
                    {
                     item.status === "HalfDay"
                      ? (
                     item.remarks ===
                     "Approved Half Day Leave"
                      )
                     ? "Half Day Leave"
                     : "Left Early"
                    : item.status
                    }
                  </span>

                </td>

                <td className="p-4">
                  {formatTime(
                    item.firstIn
                  )}
                </td>

                <td className="p-4">
                  {formatTime(
                    item.lastOut
                  )}
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

            ))}

          </tbody>

        </table>

      </div>

    </div>

  );
}

function SummaryCard({
  title,
  value,
  color
}) {
  return (
    <div className={`
      ${color}
      rounded-3xl
      p-5
      shadow
    `}>
      <p className="text-slate-600">
        {title}
      </p>

      <h2 className="
        text-3xl
        font-bold
        mt-2
      ">
        {value}
      </h2>
    </div>
  );
}

function Info({
  label,
  value
}) {
  return (
    <div>
      <p className="
        text-slate-500
      ">
        {label}
      </p>

      <p className="
        font-semibold
      ">
        {value}
      </p>
    </div>
  );
} 