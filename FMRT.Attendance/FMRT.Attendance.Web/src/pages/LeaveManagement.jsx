import { useEffect, useState } from "react";
import leaveApi from "../api/leaveApi";
import { useEmployee } from "../context/EmployeeContext";

export default function LeaveManagement() {
  const { employeeId } = useEmployee();

  const [balance, setBalance] = useState(null);
  const [leaves, setLeaves] = useState([]);

  const [form, setForm] = useState({
  leaveTypeId: 1,
  fromDate: "",
  toDate: "",
  reason: "",
  isHalfDay: false
});

  useEffect(() => {
    if (!employeeId) return;

    loadLeaves();
    loadBalance();
  }, [employeeId]);

  async function loadLeaves() {
    if (!employeeId) return;

    try {
      const response = await leaveApi.get(
        `/Leave/${employeeId}`
      );

      setLeaves(
        Array.isArray(response.data)
          ? response.data
          : response.data?.data || []
      );
    } catch (error) {
      console.error("Load Leaves Error:", error);
      setLeaves([]);
    }
  }

  async function loadBalance() {
    if (!employeeId) return;

    try {
      const response = await leaveApi.get(
        `/Leave/${employeeId}/balance`
      );

      setBalance(
        response.data?.data || response.data
      );
    } catch (error) {
      console.error("Load Balance Error:", error);
      setBalance(null);
    }
  }

  async function applyLeave() {
    try {
      if (!employeeId) {
        alert("Employee not found.");
        return;
      }

      if (
        !form.fromDate ||
        !form.toDate ||
        !form.reason.trim()
      ) {
        alert("Please fill all fields.");
        return;
      }

      const fromDate = new Date(form.fromDate);
      const toDate = new Date(form.toDate);

      if (fromDate > toDate) {
        alert(
          "From Date cannot be greater than To Date."
        );
        return;
      }

      const leaveDays =
        Math.floor(
          (toDate - fromDate) /
            (1000 * 60 * 60 * 24)
        ) + 1;

      const leaveTypeId = Number(form.leaveTypeId);

const clBalance = Number(
  balance?.casualLeaveBalance ?? 0
);

const slBalance = Number(
  balance?.sickLeaveBalance ?? 0
);

const rhBalance = Number(
  balance?.restrictedHolidayBalance ?? 0
);


if (
  leaveTypeId === 1 &&
  leaveDays > clBalance
) {
  alert(
    `Insufficient Casual Leave Balance. Available: ${clBalance}`
  );
  return;
}

if (
  leaveTypeId === 2 &&
  leaveDays > slBalance
) {
  alert(
    `Insufficient Sick Leave Balance. Available: ${slBalance}`
  );
  return;
}

if (
  leaveTypeId === 3 &&
  leaveDays > rhBalance
) {
  alert(
    `Insufficient Restricted Holiday Balance. Available: ${rhBalance}`
  );
  return;
}

      await leaveApi.post("/Leave", {
        employeeId,
        leaveTypeId,
        fromDate: form.fromDate,
        toDate: form.toDate,
        reason: form.reason.trim(),
        isHalfDay: form.isHalfDay
       });
      alert("Leave Applied Successfully");

      await loadLeaves();
      await loadBalance();

      setForm({
        leaveTypeId: 1,
        fromDate: "",
        toDate: "",
        reason: "",
        isHalfDay: false
       });
    } catch (error) {
      console.error(
        "Apply Leave Error:",
        error
      );

      alert("Unable to apply leave.");
    }
  }

  return (
    <div>
      <h1 className="text-3xl font-bold mb-6">
        Leave Management
      </h1>

      {balance && (
        <div className="grid md:grid-cols-4 gap-4 mb-8">
          <BalanceCard
            title="Casual Leave"
            value={
              balance.casualLeaveBalance ?? 0
            }
            color="bg-blue-100 text-blue-700"
          />

          <BalanceCard
            title="Sick Leave"
            value={
              balance.sickLeaveBalance ?? 0
            }
            color="bg-green-100 text-green-700"
          />

          <BalanceCard
            title="Earned Leave"
            value={
              balance.earnedLeaveBalance ?? 0
            }
            color="bg-purple-100 text-purple-700"
          />

          <BalanceCard
            title="Restricted Holiday"
            value={
              balance.restrictedHolidayBalance ??
              0
            }
            color="bg-orange-100 text-orange-700"
          />
        </div>
      )}

      <div className="bg-white rounded-3xl shadow p-6 mb-8">
        <h2 className="text-xl font-semibold mb-4">
          Apply Leave
        </h2>

        <div className="grid md:grid-cols-2 gap-4">
          <select
            value={form.leaveTypeId}
            onChange={(e) =>
              setForm({
                ...form,
                leaveTypeId: Number(
                  e.target.value
                ),
              })
            }
            className="border p-3 rounded-xl"
          >
            <option value={1}>
              Casual Leave (CL)
            </option>

            <option value={2}>
              Sick Leave (SL)
            </option>

            <option value={3}>
              Restricted Holiday (RH)
            </option>

            <option value={4}>
              Leave Without Pay (LWP)
            </option>
          </select>

          <input
            type="date"
            placeholder="From"
            value={form.fromDate}
            onChange={(e) =>
              setForm({
                ...form,
                fromDate: e.target.value,
              })
            }
            className="border p-3 rounded-xl"
          />

          <input
            type="date"
            placeholder="To"
            value={form.toDate}
            onChange={(e) =>
              setForm({
                ...form,
                toDate: e.target.value,
              })
            }
            className="border p-3 rounded-xl"
          />

          <input
            type="text"
            placeholder="Reason"
            value={form.reason}
            onChange={(e) =>
              setForm({
                ...form,
                reason: e.target.value,
              })
            }
            className="border p-3 rounded-xl"
          />

          <div className="flex items-center gap-2">
          <input
               type="checkbox"
               id="halfDay"
               checked={form.isHalfDay}
               onChange={(e) =>
             setForm({
                  ...form,
                  isHalfDay: e.target.checked,
                 })
                }
               />

              <label htmlFor="halfDay">
                 Half Day Leave
               </label>
              </div>
        </div>

        <button
          onClick={applyLeave}
          className="bg-green-600 text-white px-5 py-3 rounded-xl mt-4"
        >
          Apply Leave
        </button>
      </div>

      <div className="bg-white rounded-3xl shadow p-6">
        <h2 className="text-xl font-semibold mb-4">
          Leave History
        </h2>

        <table className="w-full">
          <thead>
            <tr className="border-b">
              <th className="p-4 text-left">
                Leave Type
              </th>
              <th className="p-4 text-left">
                From
              </th>
              <th className="p-4 text-left">
                To
              </th>
              <th className="p-4 text-left">
                Reason
              </th>
              <th className="p-4 text-left">
                Status
              </th>
            </tr>
          </thead>

          <tbody>
            {leaves.map((leave) => (
              <tr
                key={leave.employeeLeaveId}
                className="border-b"
              >
                <td className="p-4">
                  {leave.leaveType}
                </td>

                <td className="p-4">
                  {leave.fromDate
                    ? new Date(
                        leave.fromDate
                      ).toLocaleDateString()
                    : "-"}
                </td>

                <td className="p-4">
                  {leave.toDate
                    ? new Date(
                        leave.toDate
                      ).toLocaleDateString()
                    : "-"}
                </td>

                <td className="p-4">
                  {leave.reason}
                </td>

                <td className="p-4">
                  {leave.isApproved ? (
                    <span className="bg-green-100 text-green-700 px-3 py-1 rounded-full">
                      Approved
                    </span>
                  ) : (
                    <span className="bg-yellow-100 text-yellow-700 px-3 py-1 rounded-full">
                      Pending
                    </span>
                  )}
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

function BalanceCard({
  title,
  value,
  color,
}) {
  return (
    <div
      className={`rounded-3xl p-6 ${color}`}
    >
      <p className="text-sm">{title}</p>

      <h2 className="text-4xl font-bold mt-2">
        {value}
      </h2>
    </div>
  );
}