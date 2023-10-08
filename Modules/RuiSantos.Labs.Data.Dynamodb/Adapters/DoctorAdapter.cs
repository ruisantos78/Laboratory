using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Core;
using RuiSantos.Labs.Data.Dynamodb.Entities;
using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

internal interface IDoctorAdapter
{
    Task<Doctor?> FindAsync(Guid doctorId);

    Task<Doctor?> FindByAppointmentAsync(Appointment appointment);

    Task<Doctor?> FindByLicenseAsync(string license);

    Task<IEnumerable<Doctor>> LoadBySpecialtyAsync(string specialty);

    Task<Pagination<Doctor>> LoadByLicenseAsync(int pageSize = DoctorAdapter.DefaultPageSize,
        string? paginationToken = null);

    Task StoreAsync(Doctor doctor);
}

internal class DoctorAdapter : IDoctorAdapter
{
    internal const int DefaultPageSize = 25;

    private readonly IDynamoDBContext _context;

    public DoctorAdapter(IAmazonDynamoDB client)
    {
        _context = new DynamoDBContext(client);
    }

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
        Specialties = await GetSpecialtiesAsync(entity)
    };

    private Task<HashSet<string>> GetSpecialtiesAsync(DoctorEntity entity)
    {
        var query = new QueryOperationConfig
        {
            Filter = new QueryFilter(DoctorIdAttributeName, QueryOperator.Equal, entity.Id),
            Select = SelectValues.SpecificAttributes,
            AttributesToGet = new() { SpecialtyAttributeName }
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
            AttributesToGet = new() { DoctorIdAttributeName }
        };

        return _context.FromQueryAsync<DoctorEntity>(query)
            .GetNextSetAsync()
            .ContinueWith(task => task.Result.FirstOrDefault()?.Id);
    }

    public async Task<Doctor?> FindAsync(Guid doctorId)
    {
        return await _context.LoadAsync<DoctorEntity>(doctorId) is { } entity
            ? await GetModelAsync(entity)
            : default;
    }

    public async Task<Doctor?> FindByAppointmentAsync(Appointment appointment)
    {
        return await _context.LoadAsync<AppointmentsEntity>(appointment.Id) is { } entity
            ? await FindAsync(entity.DoctorId)
            : default;
    }

    public async Task<Doctor?> FindByLicenseAsync(string license)
    {
        var query = new QueryOperationConfig
        {
            IndexName = DoctorLicenseIndexName,
            Filter = new QueryFilter(LicenseAttributeName, QueryOperator.Equal, license),
            Limit = 1
        };

        var entities = await _context.FromQueryAsync<DoctorEntity>(query)
            .GetNextSetAsync();

        return entities.FirstOrDefault() is { } entity
            ? await GetModelAsync(entity)
            : default;
    }

    public async Task<IEnumerable<Doctor>> LoadBySpecialtyAsync(string specialty)
    {
        var query = new QueryOperationConfig
        {
            IndexName = DoctorSpecialtyIndexName,
            Filter = new QueryFilter(SpecialtyAttributeName, QueryOperator.Equal, specialty)
        };

        var tasks = await _context.FromQueryAsync<DoctorSpecialtyEntity>(query)
            .GetRemainingAsync()
            .ContinueWith(task => task.Result
                .Select(x => FindAsync(x.DoctorId))
                .OfType<Task<Doctor>>());

        return await Task.WhenAll(tasks);
    }

    public async Task<Pagination<Doctor>> LoadByLicenseAsync(int pageSize = DefaultPageSize,
        string? paginationToken = null)
    {
        var search = _context.GetTargetTable<DoctorEntity>()
            .Scan(new ScanOperationConfig
            {
                IndexName = DoctorLicenseIndexName,
                Limit = pageSize,
                PaginationToken = Tokens.Decode(paginationToken)
            });

        var entities = await search
            .GetNextSetAsync()
            .ContinueWith(task => task.Result
                .Select(x => GetModelAsync(_context.FromDocument<DoctorEntity>(x)))
                .ToArray());

        var doctors = await Task.WhenAll(entities);
        return new(doctors, Tokens.Encode(search.PaginationToken));
    }

    public async Task StoreAsync(Doctor doctor)
    {
        var entity = GetEntity(doctor);

        if (await GetIdByLicenseAsync(doctor.License) is { } doctorId)
            entity.Id = doctorId;

        var specialtiesWriter = await CreateSpecialtiesWriterAsync(doctor);

        var writer = _context.CreateBatchWrite<DoctorEntity>();
        writer.AddPutItem(entity);

        await writer.Combine(specialtiesWriter)
            .ExecuteAsync();
    }

    private async Task<BatchWrite> CreateSpecialtiesWriterAsync(Doctor doctor)
    {
        var writer = _context.CreateBatchWrite<DoctorSpecialtyEntity>();

        var currentSpecialties = await _context.QueryAsync<DoctorSpecialtyEntity>(doctor.Id)
            .GetRemainingAsync();

        writer.AddDeleteItems(currentSpecialties
            .Where(x => !doctor.Specialties.Contains(x.Specialty))
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