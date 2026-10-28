using System.ComponentModel.DataAnnotations;

namespace ClinicBooking.ViewModels;

public class AppointmentFormViewModel : IValidatableObject
{
    [Required(ErrorMessage = "Patient name is required.")]
    [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
    [Display(Name = "Patient name")]
    public string PatientName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Birth date is required.")]
    [DataType(DataType.Date)]
    [Display(Name = "Birth date")]
    public DateTime? BirthDate { get; set; }

    [Phone(ErrorMessage = "Enter a valid phone number.")]
    [StringLength(30)]
    [Display(Name = "Phone number")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Please select a doctor.")]
    [Display(Name = "Doctor")]
    public int? DoctorId { get; set; }

    [Required(ErrorMessage = "Please select an appointment date.")]
    [DataType(DataType.Date)]
    [Display(Name = "Appointment date")]
    public DateTime? AppointmentDate { get; set; }

    [Required(ErrorMessage = "Please select an available time slot.")]
    [Display(Name = "Time slot")]
    public TimeSpan? StartTime { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (BirthDate.HasValue && (BirthDate.Value.Date > DateTime.Today || BirthDate.Value.Year < 1900))
        {
            yield return new ValidationResult("Enter a valid birth date.", new[] { nameof(BirthDate) });
        }

        if (AppointmentDate.HasValue && AppointmentDate.Value.Date < DateTime.Today)
        {
            yield return new ValidationResult("Appointment date cannot be in the past.", new[] { nameof(AppointmentDate) });
        }
    }
}
