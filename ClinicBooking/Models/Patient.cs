using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicBooking.Models;

public class Patient
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Column(TypeName = "date")]
    public DateTime BirthDate { get; set; }

    [StringLength(30)]
    public string? PhoneNumber { get; set; }

    public List<Appointment> Appointments { get; set; } = new();
}
