using ClinicBooking.Data;
using ClinicBooking.Models;
using ClinicBooking.ViewModels;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Services;

public class AvailableSlotsResult
{
    public List<TimeSpan> Slots { get; set; } = new();
    public string? Message { get; set; }
}

public class AppointmentService
{
    public const int SlotMinutes = 30;

    private readonly ClinicDbContext _context;

    public AppointmentService(ClinicDbContext context)
    {
        _context = context;
    }

    public async Task<AvailableSlotsResult> GetAvailableSlotsAsync(int doctorId, DateTime date)
    {
        date = date.Date;
        var result = new AvailableSlotsResult();

        var doctor = await _context.Doctors.AsNoTracking().FirstOrDefaultAsync(d => d.Id == doctorId);
        if (doctor == null)
        {
            result.Message = "Doctor not found.";
            return result;
        }

        if (date < DateTime.Today)
        {
            result.Message = "Appointments cannot be booked in the past.";
            return result;
        }

        var dayOfWeek = date.DayOfWeek;
        var schedule = await _context.DoctorSchedules.AsNoTracking()
            .FirstOrDefaultAsync(s => s.DoctorId == doctorId && s.DayOfWeek == dayOfWeek);
        if (schedule == null)
        {
            result.Message = $"{doctor.FullName} does not work on {dayOfWeek}.";
            return result;
        }

        var booked = await _context.Appointments.AsNoTracking()
            .Where(a => a.DoctorId == doctorId && a.AppointmentDate == date)
            .Select(a => new { a.StartTime, a.DurationMinutes })
            .ToListAsync();

        var now = DateTime.Now;
        var slotLength = TimeSpan.FromMinutes(SlotMinutes);

        for (var start = schedule.StartTime; start + slotLength <= schedule.EndTime; start += slotLength)
        {
            var end = start + slotLength;

            if (date + start <= now)
            {
                continue;
            }

            var isTaken = booked.Any(b => start < b.StartTime + TimeSpan.FromMinutes(b.DurationMinutes) && end > b.StartTime);
            if (!isTaken)
            {
                result.Slots.Add(start);
            }
        }

        if (result.Slots.Count == 0)
        {
            result.Message = "No free time slots on this date.";
        }

        return result;
    }

    public async Task<(bool Success, string? Error)> BookAsync(AppointmentFormViewModel form)
    {
        var doctorId = form.DoctorId!.Value;
        var date = form.AppointmentDate!.Value.Date;
        var startTime = form.StartTime!.Value;

        // Check again on the server, the slot list in the browser may be stale
        var available = await GetAvailableSlotsAsync(doctorId, date);
        if (!available.Slots.Contains(startTime))
        {
            return (false, "The selected time is no longer available. Please choose another slot.");
        }

        var patientName = form.PatientName.Trim();
        var birthDate = form.BirthDate!.Value.Date;

        var patient = await _context.Patients
            .FirstOrDefaultAsync(p => p.FullName == patientName && p.BirthDate == birthDate);

        if (patient == null)
        {
            patient = new Patient { FullName = patientName, BirthDate = birthDate };
        }

        if (!string.IsNullOrWhiteSpace(form.PhoneNumber))
        {
            patient.PhoneNumber = form.PhoneNumber.Trim();
        }

        var appointment = new Appointment
        {
            DoctorId = doctorId,
            Patient = patient,
            AppointmentDate = date,
            StartTime = startTime,
            DurationMinutes = SlotMinutes,
            CreatedAt = DateTime.Now
        };

        _context.Appointments.Add(appointment);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException { Number: 2601 or 2627 })
        {
            // Another request took the same doctor/date/time between the check and the save
            return (false, "This time slot was just booked by someone else. Please choose another slot.");
        }

        return (true, null);
    }
}
