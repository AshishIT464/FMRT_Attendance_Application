import { useEffect, useState } from "react";
import api from "../api/attendanceApi";
import { useEmployee } from "../context/EmployeeContext";

export default function Calendar() {
  const { employeeId } = useEmployee();

  const today = new Date();

  const [records, setRecords] = useState([]);
  const [selectedRecord, setSelectedRecord] = useState(null);

  const [month, setMonth] = useState(
    today.getMonth() + 1
  );

  const [year, setYear] = useState(
    today.getFullYear()
  );

  useEffect(() => {
    if (!employeeId) return;
    loadAttendance();
  }, [employeeId, month, year]);

  async function loadAttendance() {
    if (!employeeId) return;

    try {
      const fromDate = `${year}-${String(month).padStart(
        2,
        "0"
      )}-01`;

      const lastDay = new Date(
        year,
        month,
        0
      ).getDate();

      const toDate = `${year}-${String(month).padStart(
        2,
        "0"
      )}-${String(lastDay).padStart(2, "0")}`;

      const response = await api.get(
        `/Attendance/${employeeId}`,
        {
          params: {
            fromDate,
            toDate,
          },
        }
      );

      const data = Array.isArray(response.data)
        ? response.data
        : response.data?.data || [];

      setRecords(data);
      setSelectedRecord(null);
    } catch (error) {
      console.error(
        "Attendance load failed:",
        error
      );

      setRecords([]);
      setSelectedRecord(null);
    }
  }

  function getColor(status, remarks) {

  if (
    status === "HalfDay" ||
    status === "Half Day"
  ) {

    if (
      remarks ===
      "Approved Half Day Leave"
    ) {
      return "bg-blue-500";
    }

    if (
      remarks === "Left Early"
    ) {
      return "bg-amber-500";
    }

    return "bg-yellow-500";
  }

  switch (status) {

    case "Present":
      return "bg-green-500";

    case "Absent":
      return "bg-red-500";

    case "Leave":
      return "bg-purple-500";

    case "Holiday":
      return "bg-blue-500";

    case "WeeklyOff":
    case "Weekly Off":
      return "bg-orange-500";

    default:
      return "bg-slate-300";
  }
}

  function getStatusShort(
  status,
  remarks
) {

  if (
    status === "HalfDay" ||
    status === "Half Day"
  ) {

    if (
      remarks ===
      "Approved Half Day Leave"
    ) {
      return "HD";
    }

    if (
      remarks === "Left Early"
    ) {
      return "LE";
    }

    return "HD";
  }

  switch (status) {

    case "Present":
      return "P";

    case "Absent":
      return "A";

    case "Leave":
      return "L";

    case "Holiday":
      return "H";

    case "WeeklyOff":
    case "Weekly Off":
      return "W";

    default:
      return "-";
  }
}

  const daysInMonth = new Date(
    year,
    month,
    0
  ).getDate();

  const cells = [];

  for (let day = 1; day <= daysInMonth; day++) {
    const currentDate = `${year}-${String(month).padStart(
      2,
      "0"
    )}-${String(day).padStart(2, "0")}`;

    const attendance = records.find(
      (x) =>
        x?.attendanceDate?.substring(0, 10) ===
        currentDate
    );

    cells.push({
  day,
  status: attendance?.status,
  remarks: attendance?.remarks,
  record: attendance,
});
  }

  return (
    <div>
      <div className="mb-8">
        <h1 className="text-3xl font-bold">
          Attendance Calendar
        </h1>

        <div className="flex gap-4 mt-4">
          <select
            value={month}
            onChange={(e) =>
              setMonth(Number(e.target.value))
            }
            className="border px-4 py-2 rounded-xl"
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
              setYear(Number(e.target.value))
            }
            className="border px-4 py-2 rounded-xl"
          >
            <option value={2025}>2025</option>
            <option value={2026}>2026</option>
            <option value={2027}>2027</option>
          </select>
        </div>
      </div>

      <div className="bg-white rounded-3xl shadow p-8">
        <div className="flex gap-4 mb-8 flex-wrap">
          <Legend color="bg-green-500" text="Present" />
          <Legend color="bg-red-500" text="Absent" />
          <Legend color="bg-purple-500" text="Leave" />
          <Legend color="bg-blue-500" text="Holiday" />
          <Legend color="bg-orange-500" text="Weekly Off" />
          <Legend color="bg-yellow-500" text="Half Day" />
        </div>

        <div className="grid grid-cols-2 sm:grid-cols-4 md:grid-cols-5 lg:grid-cols-7 gap-4">
          {cells.map((item) => (
            <div
              key={item.day}
              onClick={() =>
                item.record &&
                setSelectedRecord(item.record)
              }
              className={`
                h-20
                rounded-2xl
                flex
                flex-col
                items-center
                justify-center
                text-white
                shadow
                transition
                ${
                  item.record
                    ? "cursor-pointer hover:scale-105"
                    : ""
                }
                ${getColor( item.status,item.remarks)}
              `}
            >
              <div className="text-lg font-bold">
                {item.day}
              </div>

              <div className="text-xs">
                {getStatusShort(item.status, item.remarks)}
              </div>
            </div>
          ))}
        </div>

        {selectedRecord && (
          <div className="mt-10 border-t pt-8">
            <h2 className="text-xl font-bold mb-6">
              Attendance Details
            </h2>

            <div className="grid md:grid-cols-2 gap-4">
              <DetailCard
                label="Date"
                value={new Date(
                  selectedRecord.attendanceDate
                ).toLocaleDateString()}
              />

              <DetailCard
                label="Status"
             value={
               selectedRecord.status === "HalfDay"
                 ? selectedRecord.remarks ===
                  "Approved Half Day Leave"
                    ? "Half Day Leave"
                   : "Left Early"
                  : selectedRecord.status
                  }
               />

              <DetailCard
                label="First In"
                value={
                  selectedRecord.firstIn
                    ? new Date(
                        selectedRecord.firstIn
                      ).toLocaleTimeString()
                    : "-"
                }
              />

              <DetailCard
                label="Last Out"
                value={
                  selectedRecord.lastOut
                    ? new Date(
                        selectedRecord.lastOut
                      ).toLocaleTimeString()
                    : "-"
                }
              />

              <DetailCard
                label="Working Minutes"
                value={
                  selectedRecord.workingMinutes ?? 0
                }
              />

              <DetailCard
                label="Late Minutes"
                value={
                  selectedRecord.lateMinutes ?? 0
                }
              />

              <DetailCard
                label="Overtime Minutes"
                value={
                  selectedRecord.overtimeMinutes ??
                  0
                }
              />
              <DetailCard
                label="Remarks"
                value={
                selectedRecord.remarks ??
                "-"
               }
              />
            </div>
          </div>
        )}
      </div>
    </div>
  );
}

function Legend({ color, text }) {
  return (
    <div className="flex items-center gap-2">
      <div
        className={`w-4 h-4 rounded-full ${color}`}
      />
      <span>{text}</span>
    </div>
  );
}

function DetailCard({ label, value }) {
  return (
    <div className="bg-slate-50 rounded-xl p-4">
      <p className="text-sm text-slate-500">
        {label}
      </p>

      <h3 className="text-lg font-semibold mt-1">
        {value}
      </h3>
    </div>
  );
}