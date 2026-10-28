using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.Models;

public class Doctor
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required, StringLength(100)]
    public string Specialty { get; set; } = string.Empty;

    public int ClinicId { get; set; }
    public Clinic Clinic { get; set; } = null!;

    public List<DoctorSchedule> Schedules { get; set; } = new();
    public List<Appointment> Appointments { get; set; } = new();
}
