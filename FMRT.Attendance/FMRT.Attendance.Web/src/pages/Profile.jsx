import { useEffect, useState } from "react";
import api from "../api/attendanceApi";
import { useEmployee } from "../context/EmployeeContext";

export default function Profile() {

  const [employee, setEmployee] = useState(null);

  const { employeeId } = useEmployee();

  useEffect(() => {
    loadEmployee();
  }, [employeeId]);

  async function loadEmployee() {
    try {

      const response =
        await api.get(
          `/Employees/${employeeId}`
        );

      setEmployee(response.data);

    } catch (error) {
      console.error(error);
    }
  }

  if (!employee) {
    return (
      <div className="text-center p-10">
        Loading...
      </div>
    );
  }

  return (
    <div className="max-w-4xl">

      <div className="bg-white rounded-3xl shadow p-8">

        <div className="flex items-center gap-6 mb-8">

          <div className="w-24 h-24 bg-blue-600 rounded-full text-white flex items-center justify-center text-4xl font-bold">
            {employee.fullName?.charAt(0)}
          </div>

          <div>
            <h1 className="text-3xl font-bold">
              {employee.fullName}
            </h1>

            <p className="text-slate-500">
              {employee.designationName}
            </p>
          </div>

        </div>

        <div className="grid md:grid-cols-2 gap-6">

          <div>
            <label className="font-semibold">
              Employee Code
            </label>

            <p>{employee.employeeCode}</p>
          </div>

          <div>
            <label className="font-semibold">
              Department
            </label>

            <p>{employee.departmentName}</p>
          </div>

          <div>
            <label className="font-semibold">
              Email
            </label>

            <p>{employee.email}</p>
          </div>

          <div>
            <label className="font-semibold">
              Mobile
            </label>

            <p>{employee.mobileNumber}</p>
          </div>

          <div>
            <label className="font-semibold">
              Joining Date
            </label>

            <p>
              {employee.joiningDate}
            </p>
          </div>

        </div>

      </div>

    </div>
  );
}