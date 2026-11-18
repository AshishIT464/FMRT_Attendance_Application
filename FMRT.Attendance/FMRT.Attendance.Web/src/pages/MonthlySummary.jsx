export default function MonthlySummary() {
  return (
    <div>

      <h1 className="text-3xl font-bold mb-6">
        Monthly Report
      </h1>

      <div className="grid md:grid-cols-4 gap-6">

        <ReportCard
          title="Present"
          value="22"
        />

        <ReportCard
          title="Absent"
          value="1"
        />

        <ReportCard
          title="Late"
          value="2"
        />

        <ReportCard
          title="Attendance %"
          value="95%"
        />

      </div>

      <div className="bg-white rounded-3xl shadow p-8 mt-8">

        <h2 className="text-xl font-bold mb-5">
          Monthly Statistics
        </h2>

        <div className="grid md:grid-cols-2 gap-6">

          <Stat label="Working Hours" value="182h" />
          <Stat label="Overtime Hours" value="12h" />
          <Stat label="Leave Days" value="1" />
          <Stat label="Average Login" value="09:03 AM" />

        </div>

      </div>

    </div>
  );
}

function ReportCard({ title, value }) {
  return (
    <div className="bg-white rounded-3xl shadow p-6">
      <p className="text-slate-500">
        {title}
      </p>

      <h2 className="text-4xl font-bold mt-3">
        {value}
      </h2>
    </div>
  );
}

function Stat({ label, value }) {
  return (
    <div>
      <h4 className="text-slate-500">
        {label}
      </h4>

      <p className="text-2xl font-bold mt-2">
        {value}
      </p>
    </div>
  );
}