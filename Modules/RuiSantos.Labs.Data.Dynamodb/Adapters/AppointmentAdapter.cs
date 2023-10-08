using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Entities;
using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

public interface IAppointmentAdapter
{
    Task<Appointment?> FindByPatientAsync(Patient patient, DateTime dateTime);
    Task<Appointment?> FindByDoctorAsync(Doctor doctor, DateTime dateTime);
    Task<IEnumerable<Appointment>> LoadByDoctorAsync(Doctor doctor, DateOnly date);
    Task RemoveAsync(Appointment appointment);
    Task StoreAsync(Doctor doctor, Patient patient, DateTime dateTime);
}

internal class AppointmentAdapter : IAppointmentAdapter
{
    private readonly IDynamoDBContext _context;

    public AppointmentAdapter(IAmazonDynamoDB client)
    {
        _context = new DynamoDBContext(client);
    }

    private static Appointment GetModel(AppointmentsEntity entity) => new()
    {
        Id = entity.AppointmentId,
        Week = entity.AppointmentDateTime.DayOfWeek,
        Date = DateOnly.FromDateTime(entity.AppointmentDateTime),
        Time = entity.AppointmentDateTime.TimeOfDay
    };

    public Task<Appointment?> FindByPatientAsync(Patient patient, DateTime dateTime)
    {
        var filter = new QueryFilter(PatientIdAttributeName, QueryOperator.Equal, patient.Id);
        filter.AddCondition(AppointmentDateTimeAttributeName, QueryOperator.Equal, dateTime);

        var query = new QueryOperationConfig
        {
            IndexName = PatientAppointmentsIndexName,
            Limit = 1,
            Filter = filter
        };

        return _context.FromQueryAsync<AppointmentsEntity>(query)
            .GetNextSetAsync()
            .ContinueWith(task => task.Result
                .Select(GetModel)
                .FirstOrDefault()
            );
    }

    public Task<Appointment?> FindByDoctorAsync(Doctor doctor, DateTime dateTime)
    {
        var filter = new QueryFilter(DoctorIdAttributeName, QueryOperator.Equal, doctor.Id);
        filter.AddCondition(AppointmentDateTimeAttributeName, QueryOperator.Equal, dateTime);

        var query = new QueryOperationConfig
        {
            IndexName = DoctorAppointmentsIndexName,
            Limit = 1,
            Filter = filter
        };

        return _context.FromQueryAsync<AppointmentsEntity>(query)
            .GetNextSetAsync()
            .ContinueWith(task => task.Result
                .Select(GetModel)
                .FirstOrDefault()
            );
    }

    public Task<IEnumerable<Appointment>> LoadByDoctorAsync(Doctor doctor, DateOnly date)
    {
        var startOfDay = date.ToDateTime(TimeOnly.MinValue);
        var endOfDay = date.ToDateTime(TimeOnly.MaxValue);

        var filter = new QueryFilter(DoctorIdAttributeName, QueryOperator.Equal, doctor.Id);
        filter.AddCondition(AppointmentDateTimeAttributeName, QueryOperator.Between, startOfDay, endOfDay);

        var query = new QueryOperationConfig
        {
            IndexName = DoctorAppointmentsIndexName,
            Filter = filter
        };

        return _context.FromQueryAsync<AppointmentsEntity>(query)
            .GetNextSetAsync()
            .ContinueWith(task => task.Result.Select(GetModel));
    }

    public Task RemoveAsync(Appointment appointment)
    {
        return _context.DeleteAsync<AppointmentsEntity>(appointment.Id);
    }

    public Task StoreAsync(Doctor doctor, Patient patient, DateTime dateTime)
    {
        var entity = new AppointmentsEntity
        {
            AppointmentDateTime = dateTime,
            DoctorId = doctor.Id,
            PatientId = patient.Id
        };

        return _context.SaveAsync(entity);
    }
}