import { useEffect, useState } from "react";
import api from "../api/attendanceApi";

export default function AdminLeaves() {
  const [leaves, setLeaves] = useState([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadLeaves();
  }, []);

  async function loadLeaves() {
    try {
      setLoading(true);

      const employeesResponse =
        await api.get("/Employees");

      const employees =
        Array.isArray(
          employeesResponse.data
        )
          ? employeesResponse.data
          : employeesResponse.data?.data || [];

      const allLeaves = [];

      for (const employee of employees) {
        try {
          const leaveResponse =
            await api.get(
              `/Leave/${employee.employeeId}`
            );

          const employeeLeaves =
            Array.isArray(
              leaveResponse.data
            )
              ? leaveResponse.data
              : leaveResponse.data?.data ||
                [];

          employeeLeaves.forEach(
            (leave) => {
              allLeaves.push({
                employeeName:
                  employee.fullName,
                ...leave,
              });
            }
          );
        } catch (error) {
          console.error(
            `Failed loading leaves for employee ${employee.employeeId}`,
            error
          );
        }
      }

      setLeaves(allLeaves);
    } catch (error) {
      console.error(
        "Load Leaves Error:",
        error
      );
      setLeaves([]);
    } finally {
      setLoading(false);
    }
  }

  async function approveLeave(
    leaveId
  ) {
    try {
      await api.post(
        `/Leave/${leaveId}/approve`
      );

      await loadLeaves();

      alert(
        "Leave approved successfully"
      );
    } catch (error) {
      console.error(
        "Approve Leave Error:",
        error
      );

      alert(
        "Unable to approve leave"
      );
    }
  }

  async function rejectLeave(
  leaveId
) {
  try {
    await api.post(
      `/Leave/${leaveId}/reject`
    );

    await loadLeaves();

    alert("Leave rejected");
  } catch (error) {
    console.error(error);

    alert("Unable to reject leave");
  }
}

async function deleteLeave(
  leaveId
) {
  try {
    if (
      !window.confirm(
        "Delete this leave request?"
      )
    )
      return;

    await api.delete(
      `/Leave/${leaveId}`
    );

    await loadLeaves();

    alert("Leave deleted");
  } catch (error) {
    console.error(error);

    alert("Unable to delete leave");
  }
}

  return (
    <div>
      <h1 className="text-3xl font-bold mb-6">
        Leave Approval
      </h1>

      <div className="bg-white rounded-3xl shadow p-6">
        {loading ? (
          <div className="text-center py-8">
            Loading leaves...
          </div>
        ) : (
          <table className="w-full">
            <thead>
              <tr className="border-b">
                <th className="p-4 text-left">
                  Employee
                </th>

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
                  Status
                </th>

                <th className="p-4 text-left">
                  Action
                </th>
              </tr>
            </thead>

            <tbody>
              {leaves.map((leave) => (
                <tr
                  key={
                    leave.employeeLeaveId
                  }
                  className="border-b"
                >
                  <td className="p-4">
                    {leave.employeeName}
                  </td>

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

                  <td className="p-4 flex gap-2">
                      {!leave.isApproved && ( <>
                        <button
                           onClick={() =>
                           approveLeave(
                           leave.employeeLeaveId
                               )
                            }
                            className="bg-green-600 text-white px-4 py-2 rounded-xl"
                                >
                                 Approve
                            </button>

                      <button
                       onClick={() =>
                       rejectLeave(
                         leave.employeeLeaveId
                         )
                       }
                      className="bg-yellow-600 text-white px-4 py-2 rounded-xl"
                       >
                      Reject
                      </button>
                           </>
                          )}

                        <button
                          onClick={() =>
                            deleteLeave(
                              leave.employeeLeaveId
                                )
                              }
                            className="bg-red-600 text-white px-4 py-2 rounded-xl"
                          >
                         Delete
                        </button>
                   </td>
                </tr>
              ))}

              {leaves.length === 0 && (
                <tr>
                  <td
                    colSpan="6"
                    className="text-center p-6 text-slate-500"
                  >
                    No leave requests found
                  </td>
                </tr>
              )}
            </tbody>
          </table>
        )}
      </div>
    </div>
  );
}