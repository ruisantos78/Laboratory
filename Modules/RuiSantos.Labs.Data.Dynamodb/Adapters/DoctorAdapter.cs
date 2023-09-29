using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Entities;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

internal class DoctorAdapter : EntityAdapter<DoctorEntity, Doctor>
{
    const int DefaultPageSize = 25;

    public DoctorAdapter(IAmazonDynamoDB client) : base(client) { }

    protected override Task<DoctorEntity> ToEntityAsync(Doctor model) => Task.FromResult(new DoctorEntity() {
        Id = model.Id,
        License = model.License,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Email = model.Email,
        ContactNumbers = model.ContactNumbers.ToList(),
        Availability = model.OfficeHours.ToList()   
    });

    protected override async Task<Doctor> ToModelAsync(DoctorEntity entity) => new()
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

    private async Task<HashSet<string>> GetSpecialtiesAsync(DoctorEntity entity)
    {
        var query = new QueryOperationConfig {        
            Filter = new QueryFilter(DoctorIdAttributeName, QueryOperator.Equal, entity.Id),
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = new() { SpecialtyAttributeName }
        };

        return await Context.FromQueryAsync<DoctorSpecialtyEntity>(query)
            .GetRemainingAsync()
            .ContinueWith(task => task.Result
                .Select(x => x.Specialty)
                .ToHashSet());
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
        return await ToModelAsync(entity);
    }
     
    public async Task<Doctor?> FindByAppointmentAsync(Appointment appointment)
    {
        var appointmentEntity = await Context.LoadAsync<AppointmentsEntity>(appointment.Id);
        return await FindAsync(appointmentEntity.DoctorId);
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
            ? await ToModelAsync(entity)
            : default;
    }

    public IAsyncEnumerable<Doctor> LoadBySpecialtyAsync(string specialty)
    {
        var query = new QueryOperationConfig {        
            IndexName = DoctorSpecialtyIndexName,
            Filter = new QueryFilter(SpecialtyAttributeName, QueryOperator.Equal, specialty),            
        };

        return Context.FromQueryAsync<DoctorSpecialtyEntity>(query)
            .GetRemainingAsync()
            .ContinueWith(task => task.Result.Select(x => FindAsync(x.DoctorId)).OfType<Task<Doctor>>())                        
            .ToModelsAsync();
    }

    public IAsyncEnumerable<Doctor> LoadByLicenseAsync(string? previousLicense = null, int pageSize = DefaultPageSize)
    {
        var query = new QueryOperationConfig {        
            IndexName = DoctorLicenseIndexName,
            Limit = pageSize,
        };

        if (previousLicense is not null)
            query.Filter = new QueryFilter(LicenseAttributeName, QueryOperator.GreaterThan, previousLicense);

        return Context.FromQueryAsync<DoctorEntity>(query)
            .GetRemainingAsync()
            .ContinueWith(task => task.Result.Select(x => ToModelAsync(x)))
            .ToModelsAsync();
    }

    public async Task StoreAsync(Doctor doctor)
    {
        var entity = await ToEntityAsync(doctor);

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
