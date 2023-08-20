using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Data;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable(PatientsTableName)]
internal class PatientDto: DynamoDataObject<Patient>
{
    [DynamoDBHashKey(AttributeName = IdAttributeName, Converter = typeof(GuidConverter))]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [DynamoDBGlobalSecondaryIndexHashKey(PatientSocialSecurityNumberIndexName, AttributeName = SocialSecurityNumberAttributeName)]
    public string SocialSecurityNumber { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string FirstName { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string LastName { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string Email { get; set; } = string.Empty;

    [DynamoDBProperty]
    public List<string> ContactNumbers { get; set; } = new();
       
    protected override async Task<Patient> ToEntityAsync(IDynamoDBContext context)
    {
        var appointments = await this.GetAppointementsAsync(context);
     
        return new Patient()
        {
            Id = this.Id,
            SocialSecurityNumber = this.SocialSecurityNumber,
            FirstName = this.FirstName,
            LastName = this.LastName,
            Email = this.Email,
            ContactNumbers = this.ContactNumbers.ToHashSet(),
            Appointments = appointments.Select(a => new Appointment(a.Key, a.Value)).ToHashSet()
        };
    }

    protected override Task FromEntityAsync(IDynamoDBContext context, Patient entity)
    {
        Id = entity.Id;
        SocialSecurityNumber = entity.SocialSecurityNumber;
        FirstName = entity.FirstName;
        LastName = entity.LastName;
        Email = entity.Email;
        ContactNumbers = entity.ContactNumbers.ToList();

        return Task.CompletedTask;
    }

    public async Task<IReadOnlyDictionary<Guid, DateTime>> GetAppointementsAsync(IDynamoDBContext context)
    {
        var appointments = await context.QueryAsync<AppointmentsDto>(Id, new DynamoDBOperationConfig
        {
            IndexName = PatientAppointmentIndexName
        }).GetRemainingAsync();

        return appointments.ToDictionary(k => k.AppointmentId, v => v.AppointmentTime);
    }

    private static async Task<Patient?> GetPatientByIdAsync(IDynamoDBContext context, Guid id)
        => await FindAsync<PatientDto>(context, id);

    public static async Task<Patient?> GetPatientBySocialSecurityNumberAsync(IDynamoDBContext context, string socialSecurityNumber)
    {
        var result = await SearchAsync<PatientDto>(context, PatientSocialSecurityNumberIndexName, socialSecurityNumber);
        return result.FirstOrDefault();
    }

    public static async Task<Patient?> GetPatientByAppointmentIdAsync(IDynamoDBContext context, Guid appointmentId)
    {
        var appointment = await context.LoadAsync<AppointmentsDto>(appointmentId);
        return await GetPatientByIdAsync(context, appointment.PatientId);    
    }

    public static async Task SetPatientAsync(IDynamoDBContext context, Patient patient) 
       => await StoreAsync<PatientDto>(context, patient);  
}

