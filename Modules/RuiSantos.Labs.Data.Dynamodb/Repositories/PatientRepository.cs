using Amazon.DynamoDBv2;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Adapters;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly PatientAdapter _patientAdapter;

    public PatientRepository(IAmazonDynamoDB client)
    {
        _patientAdapter = new PatientAdapter(client);
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
