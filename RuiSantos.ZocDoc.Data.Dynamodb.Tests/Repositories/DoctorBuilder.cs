using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Tests;

public class DoctorBuilder
{
    private readonly DynamoDBContext context;

    private readonly DoctorDto doctor;
    private readonly List<DoctorSpecialtyDto> doctorSpecialties;

    public DoctorBuilder(AmazonDynamoDBClient client)
    {
        context = new DynamoDBContext(client);

        doctor = new DoctorDto() {
            Id = Guid.NewGuid(),
            License = "ABC123",
            FirstName = "Joe",
            LastName = "Doe",
            Email = "joe.doe@fakemail.com"
        };

        doctorSpecialties = new List<DoctorSpecialtyDto>();
    }

    public DoctorBuilder WithDoctor(
        string? license = default,
        string? firstName = default,
        string? lastName = default,
        string? email = default)
    {        
        if (license != default) doctor.License = license;
        if (firstName != default) doctor.FirstName = firstName;
        if (lastName != default) doctor.LastName = lastName;
        if (email != default) doctor.Email = email;

        return this;
    }

    public DoctorBuilder WithContactNumber(params string[] contactNumbers)
    {
        doctor.ContactNumbers.AddRange(contactNumbers);

        return this;
    }

    public DoctorBuilder WithAvailability(DayOfWeek dayOfWeek, params TimeSpan[] times)
    {
        doctor.Availability.Add(new OfficeHour() {
            Week = dayOfWeek,
            Hours = times
        });

        return this;
    }

    public DoctorBuilder WithSpecialty(params string[] specialties)
    {
        doctorSpecialties.AddRange(specialties.Select(s => new DoctorSpecialtyDto() {
            Specialty = s,
            DoctorId = doctor.Id
        }));

        return this;    
    }

    public async Task<Doctor> BuildAsync() 
    {        
        if (!doctorSpecialties.Any()) {
            await context.SaveAsync(doctor);
            return GetDoctor();
        }

        var doctorWriter = context.CreateBatchWrite<DoctorDto>();
        doctorWriter.AddPutItem(doctor);
        
        var specialtyWriter = context.CreateBatchWrite<DoctorSpecialtyDto>();
        specialtyWriter.AddPutItems(doctorSpecialties);

        await context.ExecuteBatchWriteAsync(new BatchWrite[] { doctorWriter, specialtyWriter });
        return GetDoctor();
    }

    private Doctor GetDoctor()
    {
        return new Doctor()
        {
            Id = doctor.Id,
            License = doctor.License,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Email = doctor.Email,
            ContactNumbers = doctor.ContactNumbers.ToHashSet(),
            OfficeHours = doctor.Availability.ToHashSet(),
            Specialties = doctorSpecialties.Select(s => s.Specialty).ToHashSet(),
            Appointments = new()
        };
    }
}