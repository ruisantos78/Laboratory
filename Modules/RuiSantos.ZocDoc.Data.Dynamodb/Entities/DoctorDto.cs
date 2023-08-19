using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable("Doctors")]
internal class DoctorDto: DynamoDataObject<Doctor> {
    const string DoctorLicenseIndex = "DoctorLicenseIndex";

    [DynamoDBHashKey(typeof(GuidConverter))]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [DynamoDBGlobalSecondaryIndexHashKey(DoctorLicenseIndex)]
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

    protected override Task<object> FromEntityAsync(IDynamoDBContext context, Doctor entity)
    {
        return Task.FromResult<object>(new DoctorDto()
        {
            Id = entity.Id,
            License = entity.License,
            FirstName = entity.FirstName,
            LastName = entity.LastName,
            Email = entity.Email,
            ContactNumbers = entity.ContactNumbers.ToList(),
            Availability = entity.OfficeHours.ToList()
        });
    }

    public async Task<HashSet<string>> GetSpecialtiesAsync(IDynamoDBContext context)
    {
        var specialties = await context.QueryAsync<DoctorSpecialtyDto>(Id).GetRemainingAsync();
        return specialties.Select(x => x.Specialty).Distinct().ToHashSet();
    } 

    public async Task<IReadOnlyDictionary<Guid, DateTime>> GetAppointementsAsync(IDynamoDBContext context)
    {
        var appointments = await context.QueryAsync<AppointmentsDto>(Id, new DynamoDBOperationConfig
        {
            IndexName = AppointmentsDto.DoctorAppointmentIndex
        }).GetRemainingAsync();

        return appointments.ToDictionary(k => k.AppointmentId, v => v.AppointmentTime);
    }

    public static async Task SetDoctorAsync(IDynamoDBContext context, Doctor doctor) 
    {
        await StoreAsync<DoctorDto>(context, doctor);
    }

    public static async Task SetDoctorAsync(IDynamoDBContext context, Doctor doctor, HashSet<string> specialties) 
    {
        var doctorSpecialties = await context.QueryAsync<DoctorSpecialtyDto>(doctor.Id).GetRemainingAsync();
        
        var batch = context.CreateBatchWrite<DoctorSpecialtyDto>();
        batch.AddDeleteItems(doctorSpecialties.Where(x => !specialties.Contains(x.Specialty)));
        batch.AddPutItems(specialties.Where(x => doctorSpecialties.All(a => a.Specialty != x))
            .Select(s => new DoctorSpecialtyDto {
                DoctorId = doctor.Id,
                Specialty = s
            }
        ));       

        await StoreAsync<DoctorDto>(context, doctor, batch);
    }
        
    public static async Task<Doctor?> GetDoctorByIdAsync(IDynamoDBContext context, Guid id)
        => await FindAsync<DoctorDto>(context, id);

    public static async Task<Doctor?> GetDoctorByLicenseAsync(IDynamoDBContext context, string license)
    {
        var result =  await SearchAsync<DoctorDto>(context, DoctorLicenseIndex, license);
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
            IndexName = DoctorSpecialtyDto.DoctorSpecialtyIndex
        }).GetRemainingAsync();

        var ids = specialties.Select(x => (object)x.DoctorId).ToList();
        return await FindListAsync<DoctorDto>(context, ids);
    }
}