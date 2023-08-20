using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Repositories;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Repositories;

public class PatientRepository : IPatientRepository
{
    private readonly IDynamoDBContext context;

    public PatientRepository(IAmazonDynamoDB client)
    {
        this.context = new DynamoDBContext(client);
    }

    public async Task StoreAsync(Patient patient)
        => await PatientDto.SetPatientAsync(context, patient);

    public async Task<Patient?> FindAsync(string socialSecurityNumber)
        => await PatientDto.GetPatientBySocialSecurityNumberAsync(context, socialSecurityNumber);

    public async Task<List<Patient>> FindAllWithAppointmentsAsync(IEnumerable<Appointment> appointments)
    {
        var query = appointments.Select(async appoint => await PatientDto.GetPatientByAppointmentIdAsync(context, appoint.Id));
        var queryResult = await Task.WhenAll(query);
        
        var patients = queryResult?.Where(patient => patient is not null).Select(patient => patient!);
        return patients?.ToList() ?? new List<Patient>();    
    }
}
