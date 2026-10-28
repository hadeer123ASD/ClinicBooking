using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicBooking.Models;

public class Appointment
{
    public int Id { get; set; }

    public int DoctorId { get; set; }
    public Doctor Doctor { get; set; } = null!;

    public int PatientId { get; set; }
    public Patient Patient { get; set; } = null!;

    [Column(TypeName = "date")]
    public DateTime AppointmentDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public int DurationMinutes { get; set; } = 30;

    public DateTime CreatedAt { get; set; }
}
