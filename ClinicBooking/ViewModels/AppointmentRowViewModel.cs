namespace ClinicBooking.ViewModels;

public class AppointmentRowViewModel
{
    public string PatientName { get; set; } = string.Empty;
    public DateTime BirthDate { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public int DurationMinutes { get; set; }
}
