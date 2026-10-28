# Clinic Appointment Booking

ASP.NET Core MVC (.NET 10), Entity Framework Core (Code First), SQL Server, Razor, Bootstrap 5, Fetch API.

A secretary registers a patient, picks a doctor and a date, chooses one of the free 30-minute slots and saves the appointment. Saved appointments are listed in a table below the form.

## Requirements

- .NET 10 SDK
- SQL Server (LocalDB, Express or full)

## Run

1. Check the connection string in `ClinicBooking/appsettings.json`.
   - LocalDB (default): `Server=(localdb)\\MSSQLLocalDB;Database=ClinicBookingDb;Trusted_Connection=True;TrustServerCertificate=True`
   - SQL login: `Server=localhost;Database=ClinicBookingDb;User Id=sa;Password=YourPassword;TrustServerCertificate=True`
2. From the solution folder:

   ```
   dotnet restore
   dotnet run --project ClinicBooking
   ```
3. Open `http://localhost:5080`.

On startup the app applies the migrations (creating the database if needed) and seeds sample data once. To apply migrations manually instead, run `dotnet ef database update --project ClinicBooking` (needs the `dotnet-ef` tool).

## Database

ERD (Chen notation): `docs/ERD.png` (also `docs/ERD.svg`).

- `Clinic` 1 - * `Doctor`
- `Doctor` 1 - * `DoctorSchedule` (one row per working weekday, with start and end time)
- `Doctor` 1 - * `Appointment`, `Patient` 1 - * `Appointment`

A weekday without a `DoctorSchedule` row is a day off. Dr. Ahmed has rows for every day except Friday, 4:00 PM to 8:00 PM.

Seed data: one clinic, three doctors (Dr. Ahmed, Dr. Sara Mostafa, Dr. Omar Khaled) with their weekly schedules, two patients and two sample appointments for Dr. Ahmed on the next working day (5:00 PM and 6:30 PM).

## How availability works

`Services/AppointmentService.cs`

1. Find the doctor's schedule row for the selected weekday. No row means a day off.
2. Generate 30-minute slots from the start time while the slot still ends by the end time.
3. Drop slots in the past (for today) and slots that overlap an existing appointment of that doctor on that date.
4. The browser loads the slots with `fetch` from `GET /Appointments/Slots` whenever the doctor or date changes.
5. On save, the server runs the same check again before inserting.
6. A unique index on `Appointments (DoctorId, AppointmentDate, StartTime)` stops two simultaneous requests from taking the same slot. The loser gets a friendly "just booked" message.

## Assumptions

- The task document does not specify patient fields beyond name and birth date ("..."), so an optional phone number is included.
- An existing patient is reused when name and birth date match.
- Times use the server's local clock. There is no login, since the task only describes the secretary page.
- Slot length is a constant (`AppointmentService.SlotMinutes = 30`), stored per appointment as `DurationMinutes`.
- The migration in `Migrations/` was written by hand. If `dotnet ef` complains, delete the `Migrations` folder and run `dotnet ef migrations add InitialCreate --project ClinicBooking`.
"# ClinicBooking" 
