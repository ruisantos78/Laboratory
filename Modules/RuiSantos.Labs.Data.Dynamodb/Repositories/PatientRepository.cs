using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Adapters;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly IPatientAdapter _patientAdapter;

    internal PatientRepository(IPatientAdapter patientAdapter)
    {
        _patientAdapter = patientAdapter;
    }

    public Task StoreAsync(Patient patient)
    {
        return _patientAdapter.StoreAsync(patient);
    }

    public Task<Patient?> FindAsync(string socialSecurityNumber)
    {
        return _patientAdapter.FindBySocialSecurityNumberAsync(socialSecurityNumber);
    }
}
