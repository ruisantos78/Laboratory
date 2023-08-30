using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Core.Models;

namespace RuiSantos.Labs.Data.Dynamodb.Entities;

partial class PatientDto
{
    public Task<Patient> GetModelAsync(IDynamoDBContext context)
    {
        var result = new Patient()
        {
            Id = this.Id,
            SocialSecurityNumber = this.SocialSecurityNumber,
            FirstName = this.FirstName,
            LastName = this.LastName,
            Email = this.Email,
            ContactNumbers = this.ContactNumbers.ToHashSet()
        };

        return Task.FromResult(result);
    }

    public Task LoadFromAsync(IDynamoDBContext context, Patient entity)
    {
        Id = entity.Id;
        SocialSecurityNumber = entity.SocialSecurityNumber;
        FirstName = entity.FirstName;
        LastName = entity.LastName;
        Email = entity.Email;
        ContactNumbers = entity.ContactNumbers.ToList();

        return Task.CompletedTask;
    }
}
