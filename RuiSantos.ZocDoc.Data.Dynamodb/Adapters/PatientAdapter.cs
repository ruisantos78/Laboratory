using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Data.Dynamodb;

public class PatientAdapter : IPatientAdapter
{
    public Task<List<Patient>> FindAllWithAppointmentsAsync(IEnumerable<Appointment> appointments)
    {
        throw new NotImplementedException();
    }

    public Task<Patient?> FindAsync(string socialSecurityNumber)
    {
        throw new NotImplementedException();
    }

    public Task StoreAsync(Patient patient)
    {
        throw new NotImplementedException();
    }
}
