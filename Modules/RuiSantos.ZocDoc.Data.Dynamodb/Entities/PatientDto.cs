using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable("Patients")]
internal class PatientDto: DynamoDataObject<Patient>
{
    private const string PatientSocialSecurityNumberIndex = "PatientSocialSecurityNumberIndex";

    [DynamoDBHashKey(typeof(GuidConverter))]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [DynamoDBGlobalSecondaryIndexHashKey(PatientSocialSecurityNumberIndex)]
    public string SocialSecurityNumber { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string FirstName { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string LastName { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string Email { get; set; } = string.Empty;

    [DynamoDBProperty]
    public List<string> ContactNumbers { get; set; } = new();

    [DynamoDBProperty(typeof(ListConverter<Appointment>))]
    public List<Appointment> Appointments { get; init; } = new();
       
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

    protected override Task<object> FromEntityAsync(IDynamoDBContext context, Patient entity)
    {
        return Task.FromResult<object>(new PatientDto()
        {
            Id = entity.Id,
            SocialSecurityNumber = entity.SocialSecurityNumber,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            ContactNumbers = entity.ContactNumbers.ToList()            
        });
    }

    public async Task<IReadOnlyDictionary<Guid, DateTime>> GetAppointementsAsync(IDynamoDBContext context)
    {
        var appointments = await context.QueryAsync<AppointmentsDto>(Id, new DynamoDBOperationConfig
        {
            IndexName = AppointmentsDto.PatientAppointmentIndex
        }).GetRemainingAsync();

        return appointments.ToDictionary(k => k.AppointmentId, v => v.AppointmentTime);
    }

    public static async Task<Patient?> GetPatientBySocialSecurityNumberAsync(IDynamoDBContext context, string socialSecurityNumber)
    {
        var result = await SearchAsync<PatientDto>(context, PatientSocialSecurityNumberIndex, socialSecurityNumber);
        return result.FirstOrDefault();
    }

    public static async Task SetPatientAsync(IDynamoDBContext context, Patient patient) 
       => await StoreAsync<PatientDto>(context, patient);    

    public static async Task<Patient?> GetPatientAsync(IDynamoDBContext context, Guid patientId)
        => await FindAsync<PatientDto>(context, patientId);
}
