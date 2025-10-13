using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Repository.DynamoDb.Entities;

namespace RuiSantos.Labs.Services.Tests.Fixtures.Builders;

internal class DoctorBuilder
{
    private readonly DoctorEntity _entity = new()
    {
        Id = Guid.Empty,
        License = "AAA-0001",
        FirstName = "Joe",
        LastName = "Doe",
        ContactNumbers = ["555-0001"],
        Email = "joe.doe@test.com",
        Availability = []
    };
    
    public DoctorBuilder WithLicense(string license)
    {
        _entity.License = license;
        return this;
    }
    
    public  DoctorBuilder WithFirstName(string firstName)
    {
        _entity.FirstName = firstName;
        return this;
    }
    
    public  DoctorBuilder WithLastName(string lastName)
    {
        _entity.LastName = lastName;
        return this;
    }
    
    public  DoctorBuilder WithContactNumbers(params string[] contactNumbers)
    {
        _entity.ContactNumbers = [..contactNumbers];
        return this;
    }
    
    public  DoctorBuilder WithEmail(string email)
    {
        _entity.Email = email;
        return this;
    }
    
    public DoctorBuilder WithAvailability(DayOfWeek dayOfWeek, params TimeSpan[] hours)
    {
        if (_entity.Availability.Find(x => x.DayOfWeek == dayOfWeek) is { } entry)
        {
            entry.Hours = [..hours];
            return this;
        }
        
        _entity.Availability.Add(new AvailabilityEntity()
        {
            DayOfWeek = dayOfWeek,
            Hours = [..hours]
        });
        return this;
    }
    
    public async Task<DoctorEntity> BuildAsync(DynamoDBContext context)
    {
        _entity.Id = Guid.NewGuid();
        
        await context.SaveAsync(_entity);
        return _entity;
    }
}