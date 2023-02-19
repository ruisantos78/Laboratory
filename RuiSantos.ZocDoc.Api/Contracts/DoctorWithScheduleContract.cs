using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts
{
    public class DoctorWithScheduleContract
    {
        /// <summary>
        /// Doctor license number
        /// </summary>
        public string License { get; set; }

        /// <summary>
        /// Array of medical spcialties
        /// </summary>
        public IEnumerable<string> Specialties { get; set; }

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
        public IEnumerable<string> ContactNumbers { get; set; }

        /// <summary>
        /// Availability
        /// </summary>
        public IEnumerable<DateTime> Schedule { get; set; }

        public DoctorWithScheduleContract() : this(new Doctor(), Enumerable.Empty<DateTime>()) { }
        public DoctorWithScheduleContract(Doctor model, IEnumerable<DateTime> schedule)
        {
            License = model.License;
            Specialties = model.Specialties;
            Email = model.Email;
            FirstName = model.FirstName;
            LastName = model.LastName;
            ContactNumbers = model.ContactNumbers;
            Schedule = schedule;
        }
    }
}
