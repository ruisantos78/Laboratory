using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts
{
    public class DoctorAvailabilityContract
    {
        /// <summary>
        /// Doctor license number
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// Array of medical spcialties
        /// </summary>
        public List<string> Specialties { get; set; }

        /// <summary>
        /// E-mail
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// First name
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Array of contact numbers
        /// </summary>
        public List<string> ContactNumbers { get; set; }

        /// <summary>
        /// Availability
        /// </summary>
        public List<DateTime> Availability { get; set; }

        public DoctorAvailabilityContract() : this(new Doctor(), DateTime.MinValue) { }
        public DoctorAvailabilityContract(Doctor model, DateTime dateTime)
        {
            var date = DateOnly.FromDateTime(dateTime);

            License = model.License;
            Specialties = model.Specialties;
            Email = model.Email;
            FirstName = model.FirstName;
            LastName = model.LastName;
            ContactNumbers = model.ContactNumbers;
            Availability = model.OfficeHours.Where(oh => oh.Week == date.DayOfWeek)
                .SelectMany(oh => oh.Hours)
                .Except(model.Appointments.Where(app => app.Date == date).Select(s => s.Time))
                .Select(time => date.ToDateTime(TimeOnly.FromTimeSpan(time)))
                .ToList();
        }
    }
}
