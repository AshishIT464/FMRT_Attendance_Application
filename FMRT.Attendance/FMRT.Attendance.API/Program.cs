using FMRT.Attendance.Application.Interfaces;
using FMRT.Attendance.Infrastructure.Data;
using FMRT.Attendance.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IDeviceLogImportService, DeviceLogImportService>();
builder.Services.AddScoped<IAttendanceProcessingService, AttendanceProcessingService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("React",
        policy =>
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Static Files (React Build)
app.UseDefaultFiles();
app.UseStaticFiles();


app.UseHttpsRedirection();

// CORS
app.UseCors("React");

app.UseAuthorization();

// API Routes
app.MapControllers();

// React Router Support
app.MapFallbackToFile("index.html");

app.Run();