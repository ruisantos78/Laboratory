using System.Text;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Newtonsoft.Json;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Core.Repositories;
using RuiSantos.Labs.Data.Dynamodb.Entities;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

internal class DoctorAdapter : EntityModelAdapter<DoctorEntity, Doctor>
{
    const int DefaultPageSize = 25;

    public DoctorAdapter(IAmazonDynamoDB client) : base(client) { }

    protected override Task<DoctorEntity> AsEntityAsync(Doctor model) => Task.FromResult(new DoctorEntity {
        Id = model.Id,
        License = model.License,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Email = model.Email,
        ContactNumbers = model.ContactNumbers.ToList(),
        Availability = model.OfficeHours.ToList()   
    });

    protected override async Task<Doctor> AsModelAsync(DoctorEntity entity) => new()
    {
        Id = entity.Id,
        License = entity.License,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        Email = entity.Email,
        ContactNumbers = entity.ContactNumbers.ToHashSet(),
        OfficeHours = entity.Availability.ToHashSet(),
        Specialties = await GetSpecialtiesAsync(entity)        
    };    

    private Task<HashSet<string>> GetSpecialtiesAsync(DoctorEntity entity)
    {
        var query = new QueryOperationConfig {        
            Filter = new QueryFilter(DoctorIdAttributeName, QueryOperator.Equal, entity.Id),
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = new() { SpecialtyAttributeName }
        };

        return Context.FromQueryAsync<DoctorSpecialtyEntity>(query)
            .GetRemainingAsync()
            .ContinueWith(task => task.Result
                .Select(x => x.Specialty)
                .ToHashSet()
            );
    }

    private async Task<Guid?> GetIdByLicenseAsync(string license)
    {
        var query = new QueryOperationConfig {        
            IndexName = DoctorLicenseIndexName,
            Filter = new QueryFilter(LicenseAttributeName, QueryOperator.Equal, license),            
            Limit = 1,
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = new() { DoctorIdAttributeName } 
        };

        var entities = await Context.FromQueryAsync<DoctorEntity>(query)
            .GetNextSetAsync();

        return entities.FirstOrDefault()?.Id;
    }

    public async Task<Doctor?> FindAsync(Guid doctorId)
    {
        var entity = await Context.LoadAsync<DoctorEntity>(doctorId);
        return await AsModelAsync(entity);
    }
     
    public async Task<Doctor?> FindByAppointmentAsync(Appointment appointment)
    {
        var entity = await Context.LoadAsync<AppointmentsEntity>(appointment.Id);
        return await FindAsync(entity.DoctorId);
    }

    public async Task<Doctor?> FindByLicenseAsync(string license)
    {
        var query = new QueryOperationConfig {        
            IndexName = DoctorLicenseIndexName,
            Filter = new QueryFilter(LicenseAttributeName, QueryOperator.Equal, license),            
            Limit = 1,
        };

        var entities = await Context.FromQueryAsync<DoctorEntity>(query)
            .GetNextSetAsync();

        return entities.FirstOrDefault() is {} entity 
            ? await AsModelAsync(entity)
            : default;
    }

    public async IAsyncEnumerable<Doctor> LoadBySpecialtyAsync(string specialty)
    {
        var query = new QueryOperationConfig {        
            IndexName = DoctorSpecialtyIndexName,
            Filter = new QueryFilter(SpecialtyAttributeName, QueryOperator.Equal, specialty),            
        };

        var result = await Context.FromQueryAsync<DoctorSpecialtyEntity>(query).GetRemainingAsync();

        if (result is null)
            yield break;

        foreach (var entity in result)
        {
            if (await FindAsync(entity.DoctorId) is { } doctor)
                yield return doctor;
        }            
    }

    public async Task<Pagination<Doctor>> LoadByLicenseAsync(int pageSize = DefaultPageSize, string? paginationToken = null)
    {
        var table = Context.GetTargetTable<DoctorEntity>();

        var search = table.Scan(new ScanOperationConfig
        {
            IndexName = DoctorLicenseIndexName,
            Limit = pageSize,
            PaginationToken = paginationToken is not null
                ? Encoding.UTF8.GetString(Convert.FromBase64String(paginationToken))
                : null
        });

        var documents = await search.GetNextSetAsync();
        if (documents is null)
            return new();

        var entities = documents.Select(x => JsonConvert.DeserializeObject<DoctorEntity>(x.ToJson()))
            .OfType<DoctorEntity>();

        var token = search.PaginationToken is not null
            ? Convert.ToBase64String(Encoding.UTF8.GetBytes(search.PaginationToken))
            : default;

        var doctors = await Task.WhenAll(entities.Select(AsModelAsync).ToArray());
        return new(doctors, token);
    }

    public async Task StoreAsync(Doctor doctor)
    {
        var entity = await AsEntityAsync(doctor);

        var doctorId = await GetIdByLicenseAsync(doctor.License);
        if (doctorId is not null)
            entity.Id = doctorId.Value;

        var specialtiesWriter = await CreateSpecialtiesWriterAsync(doctor);
        
        var writer = Context.CreateBatchWrite<DoctorEntity>();
        writer.AddPutItem(entity);        

        await writer.Combine(specialtiesWriter)
            .ExecuteAsync();        
    }

    private async Task<BatchWrite> CreateSpecialtiesWriterAsync(Doctor doctor)
    {
        var writer = Context.CreateBatchWrite<DoctorSpecialtyEntity>();
        
        var currentSpecialties = await Context.QueryAsync<DoctorSpecialtyEntity>(doctor.Id)
            .GetRemainingAsync();

        writer.AddDeleteItems(
            currentSpecialties.FindAll(x => !doctor.Specialties.Contains(x.Specialty))
        );

        writer.AddPutItems(doctor.Specialties
            .Except(currentSpecialties.Select(x => x.Specialty))
            .Select(specialty => new DoctorSpecialtyEntity
            {
                DoctorId = doctor.Id,
                Specialty = specialty
            })
        );

        return writer;
    }
}