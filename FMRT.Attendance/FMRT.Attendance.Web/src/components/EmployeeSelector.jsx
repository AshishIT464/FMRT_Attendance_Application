import { useEffect, useState } from "react";

import api from "../api/attendanceApi";

import {
  useEmployee
} from "../context/EmployeeContext";

import {
  useAuth
} from "../context/AuthContext";

export default function EmployeeSelector() {

  const {
    employee,
    employeeId,
    setEmployeeId,
    loading
  } = useEmployee();

  const {
    user,
    portalMode
  } = useAuth();

  const [
    employees,
    setEmployees
  ] = useState([]);

  useEffect(() => {

    if (
      user?.role === "Admin" &&
      portalMode === "admin"
    ) {

      loadEmployees();

    }

  }, [user, portalMode]);

  async function loadEmployees() {

    try {

      const response =
        await api.get(
          "/Employees"
        );

      setEmployees(
        response.data
      );

    } catch (error) {

      console.error(error);

    }
  }

  if (loading) {

    return (
      <div className="
        border
        rounded-xl
        px-4
        py-2
        bg-white
      ">
        Loading...
      </div>
    );

  }

  const isAdmin =
    user?.role === "Admin" &&
    portalMode === "admin";

  if (isAdmin) {

    return (

      <select
        value={employeeId ?? ""}
        onChange={(e) =>
          setEmployeeId(
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
          min-w-[280px]
        "
      >

        {employees.map(emp => (

          <option
            key={emp.employeeId}
            value={emp.employeeId}
          >
            {emp.fullName}
          </option>

        ))}

      </select>

    );

  }

  if (!employee) {

    return (
      <div className="
        border
        rounded-xl
        px-4
        py-2
        bg-red-50
        text-red-600
      ">
        Employee Not Found
      </div>
    );

  }

  return (

    <div className="
      border
      rounded-xl
      px-4
      py-2
      bg-white
      min-w-[250px]
    ">

      <div className="
        font-semibold
      ">
        {employee.fullName}
      </div>

      <div className="
        text-sm
        text-slate-500
      ">
        {employee.designationName}
      </div>

    </div>

  );

}
