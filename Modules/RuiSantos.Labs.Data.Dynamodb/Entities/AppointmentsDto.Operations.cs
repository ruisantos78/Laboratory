using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Entities.Data;
using static RuiSantos.Labs.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Entities;

partial class AppointmentsDto
{
    public static async Task<Appointment?> GetAppointmentByPatientAsync(IDynamoDBContext context, Patient patient, DateTime dateTime)
    {
        var dateTimeString = dateTime.ToUniversalTime().ToString("u");

        var query = await context.FromQueryAsync<AppointmentsDto>(new QueryOperationConfig
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
        }).GetNextSetAsync();

        return query.Select(x => new Appointment(x.AppointmentId, x.AppointmentDateTime))
            .FirstOrDefault();
    }

    public static async Task<Appointment?> GetAppointmentByDoctorAsync(IDynamoDBContext context, Doctor doctor, DateTime dateTime)
    {
        var dateTimeString = dateTime.ToUniversalTime().ToString("u");

        var query = await context.FromQueryAsync<AppointmentsDto>(new QueryOperationConfig
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
        }).GetNextSetAsync();

        return query.Select(x => new Appointment(x.AppointmentId, x.AppointmentDateTime))
            .FirstOrDefault();
    }

    public static async Task<List<Appointment>> GetAppointmentsByDoctorAsync(IDynamoDBContext context, Doctor doctor, DateOnly date)
    {
        var startOfDay = date.ToDateTime(TimeOnly.MinValue).ToString("u");
        var endOfDay = date.ToDateTime(TimeOnly.MaxValue).ToString("u");

        var query = await context.FromQueryAsync<AppointmentsDto>(new QueryOperationConfig
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
        }).GetNextSetAsync();

        return query.Select(x => new Appointment(x.AppointmentId, x.AppointmentDateTime))
            .ToList();
    }

    public static async Task RemoveAsync(IDynamoDBContext context, Appointment appointment)
    {
        await context.DeleteAsync<AppointmentsDto>(appointment.Id);
    }

    public static async Task StoreAsync(IDynamoDBContext context, Doctor doctor, Patient patient, DateTime dateTime)
    {
        var entity = new AppointmentsDto()
        {
            AppointmentDateTime = dateTime,
            DoctorId = doctor.Id,
            PatientId = patient.Id
        };

        await context.SaveAsync(entity);
    }
}
