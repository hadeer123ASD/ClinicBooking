using ClinicBooking.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Data;

public static class DbSeeder
{
    public static async Task SeedAsync(ClinicDbContext context)
    {
        if (await context.Clinics.AnyAsync())
        {
            return;
        }

        var clinic = new Clinic
        {
            Name = "City Care Clinic",
            Address = "12 Nile Street, Cairo",
            Phone = "0222 345 678"
        };

        var ahmed = new Doctor { FullName = "Dr. Ahmed", Specialty = "General Practice", Clinic = clinic };
        var sara = new Doctor { FullName = "Dr. Sara Mostafa", Specialty = "Pediatrics", Clinic = clinic };
        var omar = new Doctor { FullName = "Dr. Omar Khaled", Specialty = "Dermatology", Clinic = clinic };

        // Dr. Ahmed: 4 PM to 8 PM every day except Friday
        foreach (DayOfWeek day in Enum.GetValues<DayOfWeek>())
        {
            if (day == DayOfWeek.Friday)
            {
                continue;
            }

            ahmed.Schedules.Add(new DoctorSchedule
            {
                DayOfWeek = day,
                StartTime = TimeSpan.FromHours(16),
                EndTime = TimeSpan.FromHours(20)
            });
        }

        // Dr. Sara: 9 AM to 2 PM, Sunday to Thursday
        foreach (var day in new[] { DayOfWeek.Sunday, DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday })
        {
            sara.Schedules.Add(new DoctorSchedule
            {
                DayOfWeek = day,
                StartTime = TimeSpan.FromHours(9),
                EndTime = TimeSpan.FromHours(14)
            });
        }

        // Dr. Omar: 10 AM to 3 PM, Saturday, Monday and Wednesday
        foreach (var day in new[] { DayOfWeek.Saturday, DayOfWeek.Monday, DayOfWeek.Wednesday })
        {
            omar.Schedules.Add(new DoctorSchedule
            {
                DayOfWeek = day,
                StartTime = TimeSpan.FromHours(10),
                EndTime = TimeSpan.FromHours(15)
            });
        }

        var mona = new Patient { FullName = "Mona Adel", BirthDate = new DateTime(1990, 3, 14), PhoneNumber = "01012345678" };
        var youssef = new Patient { FullName = "Youssef Hany", BirthDate = new DateTime(2016, 9, 2), PhoneNumber = "01123456789" };

        // Two sample bookings on the next day Dr. Ahmed works
        var date = DateTime.Today.AddDays(1);
        while (date.DayOfWeek == DayOfWeek.Friday)
        {
            date = date.AddDays(1);
        }

        var appointments = new[]
        {
            new Appointment { Doctor = ahmed, Patient = mona, AppointmentDate = date, StartTime = new TimeSpan(17, 0, 0), DurationMinutes = 30, CreatedAt = DateTime.Now },
            new Appointment { Doctor = ahmed, Patient = youssef, AppointmentDate = date, StartTime = new TimeSpan(18, 30, 0), DurationMinutes = 30, CreatedAt = DateTime.Now }
        };

        context.Doctors.AddRange(ahmed, sara, omar);
        context.Appointments.AddRange(appointments);
        await context.SaveChangesAsync();
    }
}
