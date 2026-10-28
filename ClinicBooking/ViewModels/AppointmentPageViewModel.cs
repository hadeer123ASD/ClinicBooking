using Microsoft.AspNetCore.Mvc.Rendering;

namespace ClinicBooking.ViewModels;

public class AppointmentPageViewModel
{
    public AppointmentFormViewModel Form { get; set; } = new();
    public List<SelectListItem> Doctors { get; set; } = new();
    public List<AppointmentRowViewModel> Appointments { get; set; } = new();
}
