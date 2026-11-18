import { useEffect, useState } from "react";
import api from "../api/attendanceApi";

export default function EmployeeManagement() {

  const [employees, setEmployees] =
    useState([]);

  const [departments, setDepartments] =
  useState([]);

const [designations, setDesignations] =
  useState([]);

  const [search, setSearch] =
    useState("");

  const [editingEmployee, setEditingEmployee] =
    useState(null);

  const [form, setForm] =
    useState({
      employeeCode: "",
      biometricId: "",
      fullName: "",
      email: "",
      mobileNumber: "",
      departmentId: 1,
      designationId: 1,
      joiningDate: "",
      isActive: true,
      role: "Employee",
      reportingManagerId: null
    });

  useEffect(() => {
  loadEmployees();
  loadDepartments();
  loadDesignations();
}, []);

  async function loadEmployees() {

    try {

      const response =
        await api.get("/Employees");

      setEmployees(response.data);

    } catch (error) {

      console.error(error);

    }
  }

  async function loadDepartments() {

  try {

    const response =
      await api.get(
        "/Departments"
      );

    setDepartments(
      response.data
    );

  } catch (error) {

    console.error(error);

  }
}

async function loadDesignations() {

  try {

    const response =
      await api.get(
        "/Designations"
      );

    setDesignations(
      response.data
    );

  } catch (error) {

    console.error(error);

  }
}

  function openEdit(employee) {

    setEditingEmployee(employee);

    setForm({
      employeeCode:
        employee.employeeCode ?? "",

      biometricId:
        employee.biometricId ?? "",

      fullName:
        employee.fullName ?? "",

      email:
        employee.email ?? "",

      mobileNumber:
        employee.mobileNumber ?? "",

      departmentId:
        employee.departmentId ?? 1,

      designationId:
        employee.designationId ?? 1,

      joiningDate:
        employee.joiningDate?.substring(0, 10) ??
        "",

      isActive:
        employee.isActive,

      role:
        employee.role || "Employee",

      reportingManagerId:
        employee.reportingManagerId
    });
  }

  async function updateEmployee() {

    try {

      await api.put(
        `/Employees/${editingEmployee.employeeId}`,
        form
      );

      alert(
        "Employee updated successfully."
      );

      setEditingEmployee(null);

      loadEmployees();

    } catch (error) {

      console.error(error);

      alert(
        "Unable to update employee."
      );
    }
  }

  const filteredEmployees =
    employees.filter(x =>
      x.fullName
        ?.toLowerCase()
        .includes(
          search.toLowerCase()
        )
    );

  return (

    <div>

      <div className="
        flex
        flex-col
        md:flex-row
        md:items-center
        md:justify-between
        gap-4
        mb-6
      ">

        <h1 className="
          text-3xl
          font-bold
        ">
          Employee Master
        </h1>

        <input
          type="text"
          placeholder="Search Employee..."
          value={search}
          onChange={(e) =>
            setSearch(
              e.target.value
            )
          }
          className="
            border
            rounded-xl
            px-4
            py-3
            w-full
            md:w-80
          "
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
                Name
              </th>

              <th className="p-4 text-left">
                Email
              </th>

              <th className="p-4 text-left">
                Department
              </th>

              <th className="p-4 text-left">
                Designation
              </th>

              <th className="p-4 text-left">
                Role
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

            {filteredEmployees.map(emp => (

              <tr
                key={emp.employeeId}
                className="border-b"
              >

                <td className="p-4">
                  {emp.fullName}
                </td>

                <td className="p-4">
                  {emp.email}
                </td>

                <td className="p-4">
                  {emp.departmentName}
                </td>

                <td className="p-4">
                  {emp.designationName}
                </td>

                <td className="p-4">
                  {emp.role}
                </td>

                <td className="p-4">

                  <span
                    className={`
                      px-3 py-1 rounded-full text-white
                      ${
                        emp.isActive
                          ? "bg-green-500"
                          : "bg-red-500"
                      }
                    `}
                  >
                    {emp.isActive
                      ? "Active"
                      : "Inactive"}
                  </span>

                </td>

                <td className="p-4">

                  <button
                    onClick={() =>
                      openEdit(emp)
                    }
                    className="
                      bg-blue-600
                      text-white
                      px-4
                      py-2
                      rounded-lg
                    "
                  >
                    Edit
                  </button>

                </td>

              </tr>

            ))}

          </tbody>

        </table>

      </div>

      {editingEmployee && (

        <div className="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-3xl p-6 w-full max-w-4xl">
            <h2 className="text-2xl font-bold mb-6">
              Edit Employee
            </h2>

            <div className="grid md:grid-cols-2 gap-4">

              <input value={form.fullName} disabled className="border p-3 rounded-xl bg-slate-100"/>
              <input value={form.email} 
              onChange={(e) => 
                setForm({ ...form, email: e.target.value })
              }
              placeholder="Email" className="border p-3 rounded-xl"/>
              <select value={form.departmentId}
               onChange={(e) =>
                setForm({...form, departmentId: Number(e.target.value) 
                })
              }
              className="border p-3 rounded-xl">
  {departments.map(dep => (
    <option
      key={dep.departmentId}
      value={dep.departmentId}
    >
      {dep.departmentName}
    </option>

  ))}
</select>

<select
  value={form.designationId}
  onChange={(e) =>
    setForm({
      ...form,
      designationId:
        Number(
          e.target.value
        )
    })
  }
  className="
    border
    p-3
    rounded-xl
  "
>
  {designations.map(des => (

    <option
      key={des.designationId}
      value={des.designationId}
    >
      {des.designationName}
    </option>

  ))}
</select>

<select
  value={form.role}
  onChange={(e) =>
    setForm({
      ...form,
      role:
        e.target.value
    })
  }
  className="
    border
    p-3
    rounded-xl
  "
>
  <option value="Employee">
    Employee
  </option>

  <option value="Manager">
    Manager
  </option>

  <option value="HR">
    HR
  </option>

  <option value="Admin">
    Admin
  </option>
</select>

<div
  className="
    flex
    items-center
    gap-3
    border
    rounded-xl
    p-3
  "
>

  <input
    type="checkbox"
    checked={form.isActive}
    onChange={(e) =>
      setForm({
        ...form,
        isActive:
          e.target.checked
      })
    }
  />

  <label>
    Active Employee
  </label>

</div>

            </div>

            <div className="
              flex
              justify-end
              gap-3
              mt-6
            ">

              <button
                onClick={() =>
                  setEditingEmployee(
                    null
                  )
                }
                className="
                  px-4
                  py-2
                  border
                  rounded-xl
                "
              >
                Cancel
              </button>

              <button
                onClick={
                  updateEmployee
                }
                className="
                  bg-blue-600
                  text-white
                  px-5
                  py-2
                  rounded-xl
                "
              >
                Save
              </button>

            </div>

          </div>

        </div>

      )}

    </div>
  );
}