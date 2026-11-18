import { useEffect, useState } from "react";
import api from "../api/attendanceApi";

export default function HolidayManagement() {
  const [holidays, setHolidays] = useState([]);

  const [form, setForm] = useState({
    holidayDate: "",
    holidayName: "",
    isRestricted: false,
  });

  useEffect(() => {
    loadHolidays();
  }, []);

  async function loadHolidays() {
    try {
      const response = await api.get("/Holiday");

      const data = Array.isArray(response.data)
        ? response.data
        : response.data?.data || [];

      setHolidays(data);
    } catch (error) {
      console.error("Load Holidays Error:", error);
      setHolidays([]);
    }
  }

  async function addHoliday() {
    try {
      if (!form.holidayDate) {
        alert("Please select a date.");
        return;
      }

      if (!form.holidayName.trim()) {
        alert("Please enter holiday name.");
        return;
      }

      await api.post("/Holiday", {
        holidayDate: form.holidayDate,
        holidayName: form.holidayName.trim(),
        isRestricted: form.isRestricted,
      });

      alert("Holiday Added Successfully");

      setForm({
        holidayDate: "",
        holidayName: "",
        isRestricted: false,
      });

      await loadHolidays();
    } catch (error) {
      console.error("Add Holiday Error:", error);
      alert("Unable to add holiday");
    }
  }

  async function deleteHoliday(id) {
    try {
      await api.delete(`/Holiday/${id}`);

      await loadHolidays();
    } catch (error) {
      console.error("Delete Holiday Error:", error);
      alert("Unable to delete holiday");
    }
  }

  return (
    <div>
      <h1 className="text-3xl font-bold mb-6">
        Holiday Management
      </h1>

      <div className="bg-white rounded-3xl shadow p-6 mb-8">
        <h2 className="text-xl font-semibold mb-4">
          Add Holiday
        </h2>

        <div className="grid md:grid-cols-3 gap-4">
          <input
            type="date"
            value={form.holidayDate}
            onChange={(e) =>
              setForm({
                ...form,
                holidayDate: e.target.value,
              })
            }
            className="border p-3 rounded-xl"
          />

          <input
            type="text"
            placeholder="Holiday Name"
            value={form.holidayName}
            onChange={(e) =>
              setForm({
                ...form,
                holidayName: e.target.value,
              })
            }
            className="border p-3 rounded-xl"
          />

          <select
            value={String(form.isRestricted)}
            onChange={(e) =>
              setForm({
                ...form,
                isRestricted:
                  e.target.value === "true",
              })
            }
            className="border p-3 rounded-xl"
          >
            <option value="false">
              Gazetted Holiday
            </option>

            <option value="true">
              Restricted Holiday
            </option>
          </select>
        </div>

        <button
          onClick={addHoliday}
          className="mt-4 bg-blue-600 text-white px-5 py-3 rounded-xl hover:bg-blue-700"
        >
          Add Holiday
        </button>
      </div>

      <div className="bg-white rounded-3xl shadow p-6">
        <h2 className="text-xl font-semibold mb-4">
          Holiday List
        </h2>

        <table className="w-full">
          <thead>
            <tr className="border-b">
              <th className="p-4 text-left">
                Date
              </th>

              <th className="p-4 text-left">
                Holiday Name
              </th>

              <th className="p-4 text-left">
                Type
              </th>

              <th className="p-4 text-left">
                Action
              </th>
            </tr>
          </thead>

          <tbody>
            {holidays.map((holiday) => (
              <tr
                key={holiday.holidayId}
                className="border-b hover:bg-slate-50"
              >
                <td className="p-4">
                  {holiday.holidayDate
                    ? new Date(
                        holiday.holidayDate
                      ).toLocaleDateString("en-IN")
                    : "-"}
                </td>

                <td className="p-4">
                  {holiday.holidayName}
                </td>

                <td className="p-4">
                  {holiday.isRestricted ? (
                    <span className="bg-yellow-100 text-yellow-700 px-3 py-1 rounded-full">
                      Restricted
                    </span>
                  ) : (
                    <span className="bg-blue-100 text-blue-700 px-3 py-1 rounded-full">
                      Gazetted
                    </span>
                  )}
                </td>

                <td className="p-4">
                  <button
                    onClick={() =>
                      deleteHoliday(
                        holiday.holidayId
                      )
                    }
                    className="bg-red-600 text-white px-4 py-2 rounded-xl hover:bg-red-700"
                  >
                    Delete
                  </button>
                </td>
              </tr>
            ))}

            {holidays.length === 0 && (
              <tr>
                <td
                  colSpan="4"
                  className="text-center p-6 text-slate-500"
                >
                  No holidays found
                </td>
              </tr>
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}