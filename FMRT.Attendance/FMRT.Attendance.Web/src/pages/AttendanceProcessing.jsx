import axios from "axios";

export default function AttendanceProcessing() {
  async function importData() {
    try {
      await axios.post(
        "/api/Import/devicelogs"
      );

      alert("MDB Data Imported");
    } catch (error) {
      console.error(error);

      alert("Unable to import MDB data");
    }
  }

  async function generateAttendance() {
    try {
      await axios.post(
        "/api/AttendanceProcessing/generate"
      );

      alert("Attendance Generated");
    } catch (error) {
      console.error(error);

      alert(
        "Unable to generate attendance"
      );
    }
  }

  return (
    <div>
      <h1 className="text-3xl font-bold mb-6">
        Attendance Processing
      </h1>

      <div className="bg-white rounded-3xl p-8 shadow">
        <div className="flex gap-4 flex-wrap">
          <button
            onClick={importData}
            className="
              bg-blue-600
              text-white
              px-5
              py-3
              rounded-xl
              hover:bg-blue-700
            "
          >
            Import MDB Data
          </button>

          <button
            onClick={generateAttendance}
            className="
              bg-green-600
              text-white
              px-5
              py-3
              rounded-xl
              hover:bg-green-700
            "
          >
            Generate Attendance
          </button>
        </div>
      </div>
    </div>
  );
}