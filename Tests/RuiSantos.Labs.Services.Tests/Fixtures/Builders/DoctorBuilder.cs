using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Entities;
using RuiSantos.Labs.Domain.Extensions.Models;

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
        Availability =
        [
            new OfficeHour()
            {
                Week = DayOfWeek.Monday,
                Hours = [TimeSpan.FromHours(9), TimeSpan.FromHours(10), TimeSpan.FromHours(11)]
            }
        ]
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
    
    public  DoctorBuilder WithAvailability(params OfficeHour[] availability)
    {
        _entity.Availability = [..availability];
        return this;
    }
    
    public async Task<DoctorEntity> BuildAsync(DynamoDBContext context)
    {
        _entity.Id = Guid.NewGuid();
        
        await context.SaveAsync(_entity);
        return _entity;
    }
}