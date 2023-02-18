using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Api.Contracts
{
    public class PatientContract
    {
        /// <summary>
        /// Social Securiy Number
        /// </summary>
        public string SocialSecurityNumber { get; set; }

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

        public PatientContract() : this(new Patient()) { }
        public PatientContract(Patient model)
        {
            SocialSecurityNumber = model.SocialSecurityNumber;
            Email = model.Email;
            FirstName = model.FirstName;
            LastName = model.LastName;
            ContactNumbers = model.ContactNumbers;
        }
    }
}
