using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts
{
    public class DoctorContract
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

        public DoctorContract() : this(new Doctor()) { }
        public DoctorContract(Doctor model)
        {
            License = model.License;
            Specialties = model.Specialties;
            Email = model.Email;
            FirstName = model.FirstName;
            LastName = model.LastName;
            ContactNumbers = model.ContactNumbers;
        }
    }
}
