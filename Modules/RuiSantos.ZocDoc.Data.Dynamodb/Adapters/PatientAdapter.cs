using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Adapters;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Adapters;

public class PatientAdapter : IPatientAdapter
{
    private readonly IDynamoDBContext context;

    public PatientAdapter(AmazonDynamoDBClient client)
    {
        this.context = new DynamoDBContext(client);
    }

    public Task<List<Patient>> FindAllWithAppointmentsAsync(IEnumerable<Appointment> appointments)
    {
        throw new NotImplementedException();
    }

    public async Task<Patient?> FindAsync(string socialSecurityNumber)
        => await PatientDto.GetPatientBySocialSecurityNumberAsync(context, socialSecurityNumber);

    public async Task StoreAsync(Patient patient)
        => await PatientDto.SetPatientAsync(context, patient);
}
