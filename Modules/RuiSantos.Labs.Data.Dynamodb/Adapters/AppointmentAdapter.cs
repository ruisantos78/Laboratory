﻿using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DocumentModel;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Entities;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

internal class AppointmentAdapter : EntityModelAdapter<AppointmentsEntity, Appointment>
{
    public AppointmentAdapter(IAmazonDynamoDB client) : base(client) { }

    protected override async Task<AppointmentsEntity> AsEntityAsync(Appointment model)
    {
        var entity = await Context.LoadAsync<AppointmentsEntity>(model.Id);
        return new() {
            AppointmentId = model.Id,
            AppointmentDateTime = model.GetDateTime(),
            DoctorId = entity?.DoctorId ?? Guid.Empty,
            PatientId = entity?.PatientId ?? Guid.Empty
        };      
    }

    protected override Task<Appointment> AsModelAsync(AppointmentsEntity entity)
        => Task.FromResult(AsModel(entity));

    private static Appointment AsModel(AppointmentsEntity entity) => new()
    {
        Id = entity.AppointmentId,
        Week = entity.AppointmentDateTime.DayOfWeek,
        Date = DateOnly.FromDateTime(entity.AppointmentDateTime),
        Time = entity.AppointmentDateTime.TimeOfDay
    };

    public Task<Appointment?> FindByPatientAsync(Patient patient, DateTime dateTime)
    {
        var dateTimeString = dateTime.ToUniversalTime().ToString("u");

        var query = new QueryOperationConfig
        {
            IndexName = PatientAppointmentsIndexName,
            Limit = 1,
            KeyExpression = new Expression()
            {
                ExpressionStatement = "#patientId = :patientId AND #dateTime = :dateTime",
                ExpressionAttributeNames = {
                    {"#patientId", PatientIdAttributeName},
                    {"#dateTime", AppointmentDateTimeAttributeName}
                },
                ExpressionAttributeValues = {
                    {":patientId", patient.Id},
                    {":dateTime", dateTimeString}                    
                }
            }
        };

        return Context.FromQueryAsync<AppointmentsEntity>(query)
            .GetNextSetAsync()
            .ContinueWith(task => task.Result
                .Select(AsModel)
                .FirstOrDefault()
            );
    }

    public Task<Appointment?> FindByDoctorAsync(Doctor doctor, DateTime dateTime)
    {
        var dateTimeString = dateTime.ToUniversalTime().ToString("u");

        var query = new QueryOperationConfig
        {
            IndexName = DoctorAppointmentsIndexName,
            Limit = 1,
            KeyExpression = new Expression()
            {
                ExpressionStatement = "#doctorId = :doctorId AND #dateTime = :dateTime",
                ExpressionAttributeNames = {
                    {"#doctorId", DoctorIdAttributeName},
                    {"#dateTime", AppointmentDateTimeAttributeName}
                },
                ExpressionAttributeValues = {
                    {":doctorId", doctor.Id},
                    {":dateTime", dateTimeString}
                }
            }
        };

        return Context.FromQueryAsync<AppointmentsEntity>(query)
            .GetNextSetAsync()
            .ContinueWith(task => task.Result
                .Select(AsModel)
                .FirstOrDefault()
            );
    }

    public IAsyncEnumerable<Appointment> LoadByDoctorAsync(Doctor doctor, DateOnly date)
    {
        var startOfDay = date.ToDateTime(TimeOnly.MinValue).ToString("u");
        var endOfDay = date.ToDateTime(TimeOnly.MaxValue).ToString("u");

        var query = new QueryOperationConfig
        {
            IndexName = DoctorAppointmentsIndexName,
            KeyExpression = new Expression
            {
                ExpressionStatement = "#doctorId = :doctorId AND #dateTime BETWEEN :start AND :end",
                ExpressionAttributeNames = {
                    {"#doctorId", DoctorIdAttributeName},
                    {"#dateTime", AppointmentDateTimeAttributeName}
                },
                ExpressionAttributeValues = {
                    {":doctorId", doctor.Id},
                    {":start", startOfDay},
                    {":end", endOfDay}
                }
            }
        };

        return Context.FromQueryAsync<AppointmentsEntity>(query)
            .GetNextSetAsync()
            .ContinueWith(task => task.Result
                .Select(AsModelAsync)
            )
            .AsModelsAsyncEnumerable();
    }

    public Task RemoveAsync(Appointment appointment)
    {        
        return Context.DeleteAsync<AppointmentsEntity>(appointment.Id);
    }

    public Task StoreAsync(Doctor doctor, Patient patient, DateTime dateTime)
    {
        var entity = new AppointmentsEntity
        {
            AppointmentDateTime = dateTime,
            DoctorId = doctor.Id,
            PatientId = patient.Id
        };

        return Context.SaveAsync(entity);
    }
}