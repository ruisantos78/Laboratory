using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Entities;
using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

public interface IPatientAdapter
{
    Task<Patient?> FindAsync(Guid patientId);
    Task<Patient?> FindBySocialSecurityNumberAsync(string socialSecurityNumber);
    Task<PatientAppointment?> GetAppointmentAsync(Appointment appointment);
    Task StoreAsync(Patient patient);
}

internal class PatientAdapter : IPatientAdapter
{
    private readonly IDynamoDBContext _context;

    public PatientAdapter(IAmazonDynamoDB client)
    {
        _context = new DynamoDBContext(client);
    }

    private static PatientEntity GetEntity(Patient model) => new()
    {
        Id = model.Id,
        SocialSecurityNumber = model.SocialSecurityNumber,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Email = model.Email,
        ContactNumbers = model.ContactNumbers.ToList()
    };

    private static Patient GetModel(PatientEntity entity) => new()
    {
        Id = entity.Id,
        SocialSecurityNumber = entity.SocialSecurityNumber,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        Email = entity.Email,
        ContactNumbers = entity.ContactNumbers.ToHashSet()
    };

    private Task<Guid?> GetIdBySocialSecurityNumbeAsync(string socialSecurityNumber)
    {
        var query = new QueryOperationConfig
        {
            IndexName = PatientSocialSecurityNumberIndexName,
            Filter = new QueryFilter(SourceAttributeName, QueryOperator.Equal, socialSecurityNumber),
            Limit = 1,
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = new() { PatientIdAttributeName }
        };

        return _context.FromQueryAsync<PatientEntity>(query)
            .GetNextSetAsync()
            .ContinueWith(task => task.Result.FirstOrDefault()?.Id);
    }

    public async Task<Patient?> FindAsync(Guid patientId)
    {
        return await _context.LoadAsync<PatientEntity>(patientId) is { } entity
            ? GetModel(entity)
            : default;
    }

    public async Task<Patient?> FindBySocialSecurityNumberAsync(string socialSecurityNumber)
    {
        var query = new QueryOperationConfig
        {
            IndexName = PatientSocialSecurityNumberIndexName,
            Filter = new QueryFilter(SocialSecurityNumberAttributeName, QueryOperator.Equal, socialSecurityNumber),
            Limit = 1
        };

        var entities = await _context.FromQueryAsync<PatientEntity>(query)
            .GetNextSetAsync();

        return entities.FirstOrDefault() is { } entity
            ? GetModel(entity)
            : default;
    }

    public async Task<PatientAppointment?> GetAppointmentAsync(Appointment appointment)
    {
        if (await _context.LoadAsync<AppointmentsEntity>(appointment.Id) is not { } appointmentEntity)
            return default;

        return await FindAsync(appointmentEntity.PatientId) is { } patient
            ? new PatientAppointment(patient, appointmentEntity.AppointmentDateTime)
            : default;
    }

    public async Task StoreAsync(Patient patient)
    {
        var entity = GetEntity(patient);
        if (await GetIdBySocialSecurityNumbeAsync(patient.SocialSecurityNumber) is { } patientId)
            entity.Id = patientId;

        await _context.SaveAsync(entity);
    }
}
