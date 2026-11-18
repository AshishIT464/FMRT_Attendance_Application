import { useEffect, useState } from "react";
import api from "../api/attendanceApi";
import { useEmployee } from "../context/EmployeeContext";

export default function Dashboard() {

  const { employeeId } =
    useEmployee();

  const today =
    new Date();

  const [selectedMonth, setSelectedMonth] =
    useState(
      today.getMonth() + 1
    );

  const [selectedYear, setSelectedYear] =
    useState(
      today.getFullYear()
    );

  const [summary, setSummary] =
    useState(null);

  useEffect(() => {

    loadSummary();

  }, [
    employeeId,
    selectedMonth,
    selectedYear
  ]);

  async function loadSummary() {

    try {

      const response =
        await api.get(
          `/Attendance/${employeeId}/summary`,
          {
            params: {
              year: selectedYear,
              month: selectedMonth
            }
          }
        );

      setSummary(
        response.data
      );

    } catch (error) {

      console.error(error);

    }
  }

  function formatMinutes(minutes) {

    if (!minutes)
      return "0 hr";

    const hrs =
      Math.floor(minutes / 60);

    const mins =
      minutes % 60;

    return `${hrs} hr ${mins} min`;
  }

  function attendanceStatus() {

    const percentage =
      summary?.attendancePercentage ?? 0;

    if (percentage >= 95)
      return "Excellent";

    if (percentage >= 85)
      return "Good";

    if (percentage >= 75)
      return "Average";

    return "Needs Improvement";
  }

  return (

    <div className="space-y-8">

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
            md:text-4xl
            font-bold
          ">
            Employee Dashboard
          </h1>

          <p className="
            text-slate-500
            mt-2
          ">
            Attendance Overview
          </p>

        </div>

        <div className="
          flex
          flex-col
          sm:flex-row
          gap-3
        ">

          <select
            value={selectedMonth}
            onChange={(e) =>
              setSelectedMonth(
                Number(
                  e.target.value
                )
              )
            }
            className="
              border
              rounded-xl
              px-4
              py-2
              bg-white
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
            value={selectedYear}
            onChange={(e) =>
              setSelectedYear(
                Number(
                  e.target.value
                )
              )
            }
            className="
              border
              rounded-xl
              px-4
              py-2
              bg-white
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
        xl:grid-cols-4
        gap-4
      ">

        <StatCard
          title="Attendance %"
          value={`${summary?.attendancePercentage ?? 0}%`}
          color="bg-blue-50"
        />

        <StatCard
          title="Present Days"
          value={
            summary?.presentDays ?? 0
          }
          color="bg-green-50"
        />

        <StatCard
          title="Working Hours"
          value={
            formatMinutes(
              summary?.totalWorkingMinutes
            )
          }
          color="bg-purple-50"
        />

        <StatCard
          title="OT Hours"
          value={
            formatMinutes(
              summary?.totalOvertimeMinutes
            )
          }
          color="bg-emerald-50"
        />

      </div>

      <div className="
        grid
        grid-cols-2
        xl:grid-cols-4
        gap-4
      ">

        <StatCard
          title="Absent Days"
          value={
            summary?.absentDays ?? 0
          }
          color="bg-red-50"
        />

        <StatCard
          title="Leave Days"
          value={
            summary?.leaveDays ?? 0
          }
          color="bg-yellow-50"
        />

        <StatCard
          title="Weekly Off"
          value={
            summary?.weeklyOffDays ?? 0
          }
          color="bg-orange-50"
        />

        <StatCard
          title="Holidays"
          value={
            summary?.holidayDays ?? 0
          }
          color="bg-cyan-50"
        />

      </div>

      <div className="
        grid
        lg:grid-cols-2
        gap-6
      ">

        <div className="
          bg-white
          rounded-3xl
          shadow
          p-6
        ">

          <h2 className="
            text-xl
            font-bold
            mb-6
          ">
            Attendance Insights
          </h2>

          <div className="
            grid
            grid-cols-2
            gap-4
          ">

            <InsightBox
              title="Total Days"
              value={
                summary?.totalDays ?? 0
              }
            />

            <InsightBox
              title="Late Days"
              value={
                summary?.lateDays ?? 0
              }
            />

            <InsightBox
              title="Working Hours"
              value={
                formatMinutes(
                  summary?.totalWorkingMinutes
                )
              }
            />

            <InsightBox
              title="OT Hours"
              value={
                formatMinutes(
                  summary?.totalOvertimeMinutes
                )
              }
            />

          </div>

        </div>

        <div className="
          bg-white
          rounded-3xl
          shadow
          p-6
        ">

          <h2 className="
            text-xl
            font-bold
            mb-6
          ">
            Performance
          </h2>

          <div className="space-y-5">

            <PerformanceRow
              title="Attendance Percentage"
              value={`${summary?.attendancePercentage ?? 0}%`}
            />

            <PerformanceRow
              title="Present Days"
              value={
                summary?.presentDays ?? 0
              }
            />

            <PerformanceRow
              title="Status"
              value={
                attendanceStatus()
              }
            />

          </div>

        </div>

      </div>

    </div>

  );
}

function StatCard({
  title,
  value,
  color
}) {
  return (

    <div className={`
      ${color}
      rounded-3xl
      shadow
      p-5
    `}>

      <p className="
        text-slate-600
        text-sm
      ">
        {title}
      </p>

      <h2 className="
        text-xl
        md:text-3xl
        font-bold
        mt-3
      ">
        {value}
      </h2>

    </div>

  );
}

function InsightBox({
  title,
  value
}) {
  return (

    <div className="
      bg-slate-50
      rounded-2xl
      p-4
    ">

      <p className="
        text-slate-500
        text-sm
      ">
        {title}
      </p>

      <h3 className="
        text-xl
        font-bold
        mt-2
      ">
        {value}
      </h3>

    </div>

  );
}

function PerformanceRow({
  title,
  value
}) {
  return (

    <div className="
      flex
      justify-between
      border-b
      pb-3
    ">

      <span className="text-slate-500">
        {title}
      </span>

      <span className="font-semibold">
        {value}
      </span>

    </div>

  );
}