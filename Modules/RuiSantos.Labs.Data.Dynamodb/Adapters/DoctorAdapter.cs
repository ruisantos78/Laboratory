using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using RuiSantos.Labs.Core;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Entities;
using RuiSantos.Labs.Domain.Extensions.Data;
using RuiSantos.Labs.Domain.Extensions.Models;
using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

public interface IDoctorAdapter
{
    Task<Doctor?> FindAsync(Guid doctorId, 
        CancellationToken cancellationToken = default);

    Task<Doctor?> FindByAppointmentAsync(Appointment appointment, 
        CancellationToken cancellationToken = default);

    Task<Doctor?> FindByLicenseAsync(string license, 
        CancellationToken cancellationToken = default);

    Task<IEnumerable<Doctor>> LoadBySpecialtyAsync(string specialty, 
        CancellationToken cancellationToken = default);

    Task<Pagination<Doctor>> LoadByLicenseAsync(int pageSize = DoctorAdapter.DefaultPageSize,
        string? paginationToken = null, 
        CancellationToken cancellationToken = default);

    Task StoreAsync(Doctor doctor, 
        CancellationToken cancellationToken = default);
}

internal class DoctorAdapter(IAmazonDynamoDB client) : IDoctorAdapter
{
    internal const int DefaultPageSize = 25;

    private readonly DynamoDBContext _context = new DynamoDBContextBuilder()
        .WithDynamoDBClient(() => client)
        .Build(); 

    private static DoctorEntity GetEntity(Doctor model) => new()
    {
        Id = model.Id,
        License = model.License,
        FirstName = model.FirstName,
        LastName = model.LastName,
        Email = model.Email,
        ContactNumbers = model.ContactNumbers.ToList(),
        Availability = model.OfficeHours.ToList()
    };

    private async Task<Doctor> GetModelAsync(DoctorEntity entity) => new()
    {
        Id = entity.Id,
        License = entity.License,
        FirstName = entity.FirstName,
        LastName = entity.LastName,
        Email = entity.Email,
        ContactNumbers = entity.ContactNumbers.ToHashSet(),
        OfficeHours = entity.Availability.ToHashSet(),
        Specializations = await GetSpecialtiesAsync(entity)
    };

    private Task<HashSet<string>> GetSpecialtiesAsync(DoctorEntity entity)
    {
        var query = new QueryOperationConfig
        {
            Filter = new QueryFilter(DoctorIdAttributeName, QueryOperator.Equal, entity.Id),
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = [SpecialtyAttributeName]
        };

        return _context.FromQueryAsync<DoctorSpecialtyEntity>(query)
            .GetRemainingAsync()
            .ContinueWith(task => task.Result
                .Select(x => x.Specialty)
                .ToHashSet()
            );
    }

    private Task<Guid?> GetIdByLicenseAsync(string license)
    {
        var query = new QueryOperationConfig
        {
            IndexName = DoctorLicenseIndexName,
            Filter = new QueryFilter(LicenseAttributeName, QueryOperator.Equal, license),
            Limit = 1,
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = [DoctorIdAttributeName]
        };

        return _context.FromQueryAsync<DoctorEntity>(query)
            .GetNextSetAsync()
            .ContinueWith(task => task.Result.FirstOrDefault()?.Id);
    }

    public async Task<Doctor?> FindAsync(Guid doctorId, CancellationToken cancellationToken)
    {
        return await _context.LoadAsync<DoctorEntity>(doctorId, cancellationToken) is { } entity
            ? await GetModelAsync(entity)
            : null;
    }

    public async Task<Doctor?> FindByAppointmentAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        return await _context.LoadAsync<AppointmentsEntity>(appointment.Id, cancellationToken) is { } entity
            ? await FindAsync(entity.DoctorId, cancellationToken)
            : null;
    }

    public async Task<Doctor?> FindByLicenseAsync(string license, CancellationToken cancellationToken)
    {
        var query = new QueryOperationConfig
        {
            IndexName = DoctorLicenseIndexName,
            Filter = new QueryFilter(LicenseAttributeName, QueryOperator.Equal, license),
            Limit = 1
        };

        var entities = await _context.FromQueryAsync<DoctorEntity>(query)
            .GetNextSetAsync(cancellationToken);

        return entities.FirstOrDefault() is { } entity
            ? await GetModelAsync(entity)
            : null;
    }

    public async Task<IEnumerable<Doctor>> LoadBySpecialtyAsync(string specialty, CancellationToken cancellationToken)
    {
        var query = new QueryOperationConfig
        {
            IndexName = DoctorSpecialtyIndexName,
            Filter = new QueryFilter(SpecialtyAttributeName, QueryOperator.Equal, specialty)
        };

        var tasks = await _context.FromQueryAsync<DoctorSpecialtyEntity>(query)
            .GetRemainingAsync(cancellationToken)
            .ContinueWith(task => task.Result
                .Select(x => FindAsync(x.DoctorId, cancellationToken))
                .OfType<Task<Doctor>>(), cancellationToken);

        return await Task.WhenAll(tasks);
    }

    public async Task<Pagination<Doctor>> LoadByLicenseAsync(int pageSize = DoctorAdapter.DefaultPageSize,
        string? paginationToken = null, CancellationToken cancellationToken = default)
    {
        var search = _context.GetTargetTable<DoctorEntity>()
            .Scan(new ScanOperationConfig
            {
                IndexName = DoctorLicenseIndexName,
                Limit = pageSize,
                PaginationToken = Tokens.Decode(paginationToken)
            });

        var entities = await search
            .GetNextSetAsync(cancellationToken)
            .ContinueWith(task => task.Result
                .Select(x => GetModelAsync(_context.FromDocument<DoctorEntity>(x)))
                .ToArray(), cancellationToken);

        var doctors = await Task.WhenAll(entities);
        return new(doctors, Tokens.Encode(search.PaginationToken));
    }

    public async Task StoreAsync(Doctor doctor, CancellationToken cancellationToken)
    {
        var entity = GetEntity(doctor);

        if (await GetIdByLicenseAsync(doctor.License) is { } doctorId)
            entity.Id = doctorId;

        var specialtiesWriter = await CreateSpecialtiesWriterAsync(doctor, cancellationToken);

        var writer = _context.CreateBatchWrite<DoctorEntity>();
        writer.AddPutItem(entity);

        await writer.Combine(specialtiesWriter)
            .ExecuteAsync(cancellationToken);
    }

    private async Task<IBatchWrite<DoctorSpecialtyEntity>> CreateSpecialtiesWriterAsync(Doctor doctor,
        CancellationToken cancellationToken)
    {
        var writer = _context.CreateBatchWrite<DoctorSpecialtyEntity>();

        var currentSpecialties = await _context.QueryAsync<DoctorSpecialtyEntity>(doctor.Id)
            .GetRemainingAsync(cancellationToken);

        writer.AddDeleteItems(currentSpecialties
            .Where(x => !doctor.Specializations.Contains(x.Specialty))
        );

        writer.AddPutItems(doctor.Specializations
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