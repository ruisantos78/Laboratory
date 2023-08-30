using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

partial class DoctorDto
{
    public async Task<Doctor> GetModelAsync(IDynamoDBContext context)
    {
        return new Doctor()
        {
            Id = this.Id,
            License = this.License,
            FirstName = this.FirstName,
            LastName = this.LastName,
            Email = this.Email,
            ContactNumbers = this.ContactNumbers.ToHashSet(),
            OfficeHours = this.Availability.ToHashSet(),
            Specialties = await this.GetSpecialtiesAsync(context)
        };
    }

    public Task LoadFromAsync(IDynamoDBContext context, Doctor entity)
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
        var specialties = await context.QueryAsync<DoctorSpecialtyDto>(Id)
            .GetRemainingAsync();

        return specialties.Select(x => x.Specialty)
            .Distinct()
            .ToHashSet();
    }
}
