using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Entities;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

internal class PatientAdapter : EntityAdapter<PatientEntity, Patient>
{
    public PatientAdapter(IAmazonDynamoDB client) : base(client)
    {
    }

    protected override Task<PatientEntity> ToEntityAsync(Patient model) => Task.FromResult(new PatientEntity()
    {
        Id = model.Id,
        SocialSecurityNumber = model.SocialSecurityNumber,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Email = model.Email,
        ContactNumbers = model.ContactNumbers.ToList()
    });

    protected override Task<Patient> ToModelAsync(PatientEntity entity)=> Task.FromResult(new Patient()
    {
        Id = entity.Id,
        SocialSecurityNumber = entity.SocialSecurityNumber,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        Email = entity.Email,
        ContactNumbers = entity.ContactNumbers.ToHashSet()
    });

    private async Task<Guid?> GetIdBySocialSecurityNumbeAsync(string socialSecurityNumber)
    {
        var query = new QueryOperationConfig {        
            IndexName = PatientSocialSecurityNumberIndexName,
            Filter = new QueryFilter(SourceAttributeName, QueryOperator.Equal, socialSecurityNumber),            
            Limit = 1,
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = new() { PatientIdAttributeName } 
        };

        var entities = await Context.FromQueryAsync<PatientEntity>(query)
            .GetNextSetAsync();

        return entities.FirstOrDefault()?.Id;
    }

    public async Task<Patient?> FindAsync(Guid patientId)
    {
        var entity = await Context.LoadAsync<PatientEntity>(patientId);
        return await ToModelAsync(entity);
    }

    public async Task<Patient?> FindBySocialSecurityNumberAsync(string socialSecurityNumber)    
    {
        var query = new QueryOperationConfig {        
            IndexName = PatientSocialSecurityNumberIndexName,
            Filter = new QueryFilter(SocialSecurityNumberAttributeName, QueryOperator.Equal, socialSecurityNumber),
            Limit = 1,
        };

        var entities = await Context.FromQueryAsync<PatientEntity>(query)
            .GetNextSetAsync();

        return entities.FirstOrDefault() is {} entity 
            ? await ToModelAsync(entity)
            : default;
    }

    public async Task<PatientAppointment?> GetAppointmentAsync(Appointment appointment)
    {
        var appointmentEntity = await Context.LoadAsync<AppointmentsEntity>(appointment.Id);
        if (appointmentEntity?.PatientId is null)
            return default;

        if (await FindAsync(appointmentEntity.PatientId) is {} patient)            
            return new() 
            {
                Patient = patient,
                Date = appointmentEntity.AppointmentDateTime
            };        

        return default;
    }

    public async Task StoreAsync(Patient patient)
    {
        var entity = await ToEntityAsync(patient);

        var patientId = await GetIdBySocialSecurityNumbeAsync(patient.SocialSecurityNumber);
        if (patientId is not null)
            entity.Id = patientId.Value;

        await Context.SaveAsync<PatientEntity>(entity);
    }
}
