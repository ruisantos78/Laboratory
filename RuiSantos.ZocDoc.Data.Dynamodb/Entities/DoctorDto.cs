using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable("Doctors")]
internal class DoctorDto {
    private const string DoctorLicenseIndex = "DoctorLicenseIndex";

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

    public static async Task SetDoctorAsync(IDynamoDBContext context, Doctor doctor) 
    {
        if (doctor is null)
            return;

        var doctorWriter = context.CreateBatchWrite<DoctorDto>();
        doctorWriter.AddPutItem(new DoctorDto()
        {
            Id = doctor.Id,
            License = doctor.License,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Email = doctor.Email,
            ContactNumbers = doctor.ContactNumbers.ToList(),
            Availability = doctor.OfficeHours.ToList()
        });

        var specialtiesWriter = await DoctorSpecialtyDto.CreateDoctorSpecialtiesBatchWriteAsync(context, doctor.Id, doctor.Specialties);

        await context.ExecuteBatchWriteAsync(new BatchWrite[] { doctorWriter, specialtiesWriter });
    }        

    public static async Task<Doctor?> GetDoctorByIdAsync(IDynamoDBContext context, Guid id) {
        var doctor = await context.LoadAsync<DoctorDto>(id);
        return await GetDoctorAsync(context, doctor);
    }

    public static async Task<Doctor?> GetDoctorByLicenseAsync(IDynamoDBContext context, string license)
    {
        var config = new DynamoDBOperationConfig
        {
            IndexName = DoctorLicenseIndex
        };

        var doctors = await context.QueryAsync<DoctorDto>(license, config).GetRemainingAsync();
        return await GetDoctorAsync(context, doctors.FirstOrDefault());
    }

    public static async IAsyncEnumerable<Doctor> GetDoctorsAsync(IDynamoDBContext context, IEnumerable<DoctorDto> doctors) {
        foreach (var doctor in doctors.DistinctBy(d => d.Id))
        {
            var result = await GetDoctorAsync(context, doctor);
            if (result is not null)
                yield return result;
        }
    }

    public static async Task<Doctor?> GetDoctorAsync(IDynamoDBContext context, DoctorDto? doctor)
    {
        if (doctor is null)
            return default;

        var specialties = await DoctorSpecialtyDto.GetSpecialtiesByDoctorIdAsync(context, doctor.Id);
        var appointments = await AppointmentsDto.GetAppointmentsByDoctorIdAsync(context, doctor.Id);

        return new Doctor()
        {
            Id = doctor.Id,
            License = doctor.License,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Email = doctor.Email,
            ContactNumbers = doctor.ContactNumbers.ToHashSet(),
            OfficeHours = doctor.Availability.ToHashSet(),
            Specialties = specialties.ToHashSet(),
            Appointments = appointments.Select(a => new Appointment(a.Id, a.Date)).ToHashSet()
        };
    }
}