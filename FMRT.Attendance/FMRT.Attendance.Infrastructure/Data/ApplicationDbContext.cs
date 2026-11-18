using FMRT.Attendance.Domain.Entities;
using AttendanceEntity = FMRT.Attendance.Domain.Entities.Attendance;
using Microsoft.EntityFrameworkCore;

namespace FMRT.Attendance.Infrastructure.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Employee> Employees => Set<Employee>();

        public DbSet<Department> Departments => Set<Department>();

        public DbSet<Designation> Designations => Set<Designation>();

        public DbSet<Shift> Shifts => Set<Shift>();

        public DbSet<AttendanceEntity> Attendances => Set<AttendanceEntity>();

        public DbSet<AttendancePunch> AttendancePunches => Set<AttendancePunch>();

        public DbSet<BiometricDevice> BiometricDevices => Set<BiometricDevice>();

        public DbSet<Holiday> Holidays => Set<Holiday>();

        public DbSet<LeaveType> LeaveTypes => Set<LeaveType>();

        public DbSet<EmployeeLeave> EmployeeLeaves => Set<EmployeeLeave>();

        public DbSet<EmployeeLeaveBalance> EmployeeLeaveBalances => Set<EmployeeLeaveBalance>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.EmployeeId);

                entity.Property(e => e.EmployeeCode)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.BiometricId)
                    .HasMaxLength(50);

                entity.Property(e => e.FullName)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.Property(e => e.Email)
                    .HasMaxLength(150);

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(20);

                entity.HasOne(e => e.Department)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(e => e.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Designation)
                    .WithMany(d => d.Employees)
                    .HasForeignKey(e => e.DesignationId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Department>(entity =>
            {
                entity.HasKey(d => d.DepartmentId);

                entity.Property(d => d.DepartmentName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(d => d.Code)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasIndex(d => d.Code)
                    .IsUnique();
            });

            modelBuilder.Entity<Designation>(entity =>
            {
                entity.HasKey(d => d.DesignationId);

                entity.Property(d => d.DesignationName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<Shift>(entity =>
            {
                entity.HasKey(s => s.ShiftId);

                entity.Property(s => s.ShiftCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(s => s.ShiftName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<AttendanceEntity>(entity =>
            {
                entity.HasKey(a => a.AttendanceId);

                entity.HasOne(a => a.Employee)
                    .WithMany(e => e.Attendances)
                    .HasForeignKey(a => a.EmployeeId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(a => a.Shift)
                    .WithMany(s => s.Attendances)
                    .HasForeignKey(a => a.ShiftId)
                    .OnDelete(DeleteBehavior.Restrict);

                entity.HasIndex(a => new
                {
                    a.EmployeeId,
                    a.AttendanceDate
                })
                .IsUnique();
            });

            modelBuilder.Entity<AttendancePunch>(entity =>
            {
                entity.HasKey(p => p.PunchId);

                entity.Property(p => p.Source)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasOne(p => p.Employee)
               .WithMany(e => e.AttendancePunches)
               .HasForeignKey(p => p.EmployeeId)
               .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(p => p.Attendance)
                    .WithMany(a => a.Punches)
                    .HasForeignKey(p => p.AttendanceId)
                    .OnDelete(DeleteBehavior.SetNull);

                entity.HasOne(p => p.Device)
                    .WithMany(d => d.AttendancePunches)
                    .HasForeignKey(p => p.DeviceId)
                    .OnDelete(DeleteBehavior.SetNull);
            });

            modelBuilder.Entity<BiometricDevice>(entity =>
            {
                entity.HasKey(d => d.DeviceId);

                entity.Property(d => d.DeviceName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(d => d.SerialNumber)
                    .HasMaxLength(100);

                entity.Property(d => d.IPAddress)
                    .HasMaxLength(50);

                entity.Property(d => d.Location)
                    .HasMaxLength(150);
            });

            modelBuilder.Entity<Holiday>(entity =>
            {
                entity.HasKey(h => h.HolidayId);

                entity.Property(h => h.HolidayName)
                    .IsRequired()
                    .HasMaxLength(200);
            });

            modelBuilder.Entity<LeaveType>(entity =>
            {
                entity.HasKey(l => l.LeaveTypeId);

                entity.Property(l => l.LeaveCode)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(l => l.LeaveName)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<EmployeeLeave>(entity =>
            {
                entity.HasKey(l => l.EmployeeLeaveId);

                entity.HasOne(l => l.Employee)
                    .WithMany()
                    .HasForeignKey(l => l.EmployeeId);

                entity.HasOne(l => l.LeaveType)
                    .WithMany()
                    .HasForeignKey(l => l.LeaveTypeId);
            });

            modelBuilder.Entity<EmployeeLeaveBalance>(entity =>
            {
                entity.HasKey(x =>
                x.EmployeeLeaveBalanceId);

                entity.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId);

                entity.Property(x => x.CasualLeaveBalance)
                .HasPrecision(18, 2);

                entity.Property(x => x.SickLeaveBalance)
                .HasPrecision(18, 2);

                entity.Property(x => x.EarnedLeaveBalance)
                .HasPrecision(18, 2);

                entity.Property(x => x.RestrictedHolidayBalance)
                .HasPrecision(18, 2);
            });

        }
    }
}