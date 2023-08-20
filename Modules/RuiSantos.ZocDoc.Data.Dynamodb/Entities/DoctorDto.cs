using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Data;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable(DoctorsTableName)]
internal class DoctorDto: DynamoDataObject<Doctor> {

    [DynamoDBHashKey(typeof(GuidConverter))]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [DynamoDBGlobalSecondaryIndexHashKey(DoctorLicenseIndexName)]
    public string License { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string FirstName { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string LastName { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string Email { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public List<string> ContactNumbers { get; set; } = new();
    
    [DynamoDBProperty(typeof(ListConverter<OfficeHour>))]
    public List<OfficeHour> Availability { get; set; } = new();

    protected override async Task<Doctor> ToEntityAsync(IDynamoDBContext context)
    {
        var specialties = await this.GetSpecialtiesAsync(context);
        var appointments = await this.GetAppointementsAsync(context);

        return new Doctor()
        {
            Id = this.Id,
            License = this.License,
            FirstName = this.FirstName,
            LastName = this.LastName,
            Email = this.Email,
            ContactNumbers = this.ContactNumbers.ToHashSet(),
            OfficeHours = this.Availability.ToHashSet(),
            Specialties = specialties,
            Appointments = appointments.Select(a => new Appointment(a.Key, a.Value)).ToHashSet()
        };
    }

    protected override Task FromEntityAsync(IDynamoDBContext context, Doctor entity)
    {
        this.Id = entity.Id;
        this.License = entity.License;
        this.FirstName = entity.FirstName;
        this.LastName = entity.LastName;
        this.Email = entity.Email;
        this.ContactNumbers = entity.ContactNumbers.ToList();
        this.Availability = entity.OfficeHours.ToList();

        return Task.CompletedTask;
    }

    private async Task<HashSet<string>> GetSpecialtiesAsync(IDynamoDBContext context)
    {
        var specialties = await context.QueryAsync<DoctorSpecialtyDto>(Id).GetRemainingAsync();
        return specialties.Select(x => x.Specialty).Distinct().ToHashSet();
    }

    private async Task SetSpecialtiesAsync(IDynamoDBContext context, HashSet<string> specialties)
    {
        var current = await context.QueryAsync<DoctorSpecialtyDto>(Id).GetRemainingAsync();

        var writer = context.CreateBatchWrite<DoctorSpecialtyDto>();

        writer.AddPutItems(specialties.Where(s => current.All(dto => dto.Specialty != s))
            .Select(s => new DoctorSpecialtyDto
            {
                DoctorId = this.Id,
                Specialty = s
            }));

        writer.AddDeleteItems(current.Where(dto => !specialties.Contains(dto.Specialty)));

        await writer.ExecuteAsync();
    }

    private async Task<IReadOnlyDictionary<Guid, DateTime>> GetAppointementsAsync(IDynamoDBContext context)
    {
        var appointments = await context.QueryAsync<AppointmentsDto>(Id, new DynamoDBOperationConfig
        {
            IndexName = DoctorAppointmentIndexName
        }).GetRemainingAsync();

        return appointments.ToDictionary(k => k.AppointmentId, v => v.AppointmentTime);
    }
    
    private static async Task<Doctor?> GetDoctorByIdAsync(IDynamoDBContext context, Guid id)
        => await FindAsync<DoctorDto>(context, id);

    public static async Task<Doctor?> GetDoctorByLicenseAsync(IDynamoDBContext context, string license)
    {
        var result =  await SearchAsync<DoctorDto>(context, DoctorLicenseIndexName, license);
        return result.FirstOrDefault();   
    }

    public static async Task<Doctor?> GetDoctorByAppointmentIdAsync(IDynamoDBContext context, Guid appointmentId)
    {
        var appointment = await context.LoadAsync<AppointmentsDto>(appointmentId);
        return await GetDoctorByIdAsync(context, appointment.DoctorId);    
    }

    public static async Task<List<Doctor>> GetDoctorsBySpecialtyAsync(IDynamoDBContext context, string specialty)
    {
        var specialties = await context.QueryAsync<DoctorSpecialtyDto>(specialty, new DynamoDBOperationConfig {
            IndexName = DoctorSpecialtyIndexName
        }).GetRemainingAsync();

        var ids = specialties.Select(x => x.DoctorId as object).ToList();
        return await FindListAsync<DoctorDto>(context, ids);
    }

    public static async Task SetDoctorAsync(IDynamoDBContext context, Doctor doctor) 
    {        
        var dto = await StoreAsync<DoctorDto>(context, doctor);
        await dto.SetSpecialtiesAsync(context, doctor.Specialties);
    }
}