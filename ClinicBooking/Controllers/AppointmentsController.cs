using System.Diagnostics;
using ClinicBooking.Data;
using ClinicBooking.Services;
using ClinicBooking.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ClinicBooking.Controllers;

public class AppointmentsController : Controller
{
    private readonly ClinicDbContext _context;
    private readonly AppointmentService _appointmentService;

    public AppointmentsController(ClinicDbContext context, AppointmentService appointmentService)
    {
        _context = context;
        _appointmentService = appointmentService;
    }

    public async Task<IActionResult> Index()
    {
        return View(await BuildPageAsync(new AppointmentFormViewModel()));
    }

    [HttpGet]
    public async Task<IActionResult> Slots(int doctorId, DateTime date)
    {
        var result = await _appointmentService.GetAvailableSlotsAsync(doctorId, date);

        return Json(new
        {
            message = result.Message,
            slots = result.Slots.Select(s => new
            {
                value = s.ToString(@"hh\:mm"),
                text = DateTime.Today.Add(s).ToString("h:mm tt")
            })
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind(Prefix = "Form")] AppointmentFormViewModel form)
    {
        if (ModelState.IsValid)
        {
            var (success, error) = await _appointmentService.BookAsync(form);
            if (success)
            {
                TempData["Success"] = "Appointment saved successfully.";
                return RedirectToAction(nameof(Index));
            }

            ModelState.AddModelError("Form.StartTime", error!);
        }

        return View("Index", await BuildPageAsync(form));
    }

    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    private async Task<AppointmentPageViewModel> BuildPageAsync(AppointmentFormViewModel form)
    {
        var doctors = await _context.Doctors.AsNoTracking()
            .OrderBy(d => d.FullName)
            .Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.FullName + " - " + d.Specialty })
            .ToListAsync();

        var rows = await _context.Appointments.AsNoTracking()
            .OrderBy(a => a.AppointmentDate).ThenBy(a => a.StartTime)
            .Select(a => new AppointmentRowViewModel
            {
                PatientName = a.Patient.FullName,
                BirthDate = a.Patient.BirthDate,
                DoctorName = a.Doctor.FullName,
                AppointmentDate = a.AppointmentDate,
                StartTime = a.StartTime,
                DurationMinutes = a.DurationMinutes
            })
            .ToListAsync();

        return new AppointmentPageViewModel
        {
            Form = form,
            Doctors = doctors,
            Appointments = rows
        };
    }
}
