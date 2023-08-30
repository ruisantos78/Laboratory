using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Entities;

namespace RuiSantos.Labs.Data.Dynamodb.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly IDynamoDBContext context;

    public PatientRepository(IAmazonDynamoDB client)
    {
        this.context = new DynamoDBContext(client);
    }

    public async Task StoreAsync(Patient patient)
    {
        await PatientDto.SetPatientAsync(context, patient);
    }

    public async Task<Patient?> FindAsync(string socialSecurityNumber)
    {
        return await PatientDto.GetPatientBySocialSecurityNumberAsync(context, socialSecurityNumber);
    }
}
