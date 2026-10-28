using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.Models;

public class Clinic
{
    public int Id { get; set; }

    [Required, StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Address { get; set; } = string.Empty;

    [StringLength(30)]
    public string? Phone { get; set; }

    public List<Doctor> Doctors { get; set; } = new();
}
