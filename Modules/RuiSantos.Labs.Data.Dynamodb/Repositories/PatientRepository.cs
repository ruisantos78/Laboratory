using Amazon.DynamoDBv2;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Adapters;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly PatientAdapter patientAdapter;

    public PatientRepository(IAmazonDynamoDB client)
    {
        this.patientAdapter = new PatientAdapter(client);
    }

    public async Task StoreAsync(Patient patient)
    {
        await patientAdapter.StoreAsync(patient);
    }

    public async Task<Patient?> FindAsync(string socialSecurityNumber)
    {
        return await patientAdapter.FindBySocialSecurityNumberAsync(socialSecurityNumber);
    }
}
