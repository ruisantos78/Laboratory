using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Core.Repositories;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Repositories;

public class AppointamentsRepository: IAppointamentsRepository
{
    private readonly DynamoDBContext context;

    public AppointamentsRepository(IAmazonDynamoDB client)
	{
        this.context = new DynamoDBContext(client);
    }

    public async Task RemoveAsync(Doctor doctor, Patient patient, Appointment appointment)
    {
        var entities = await context.QueryAsync<AppointmentsDto>(patient.Id,
            new DynamoDBOperationConfig
            {
                IndexName = PatientAppointmentIndexName
            }).GetRemainingAsync();

        var entity = entities.FirstOrDefault(x => x.DoctorId == doctor.Id && x.AppointmentTime == appointment.GetDateTime());
        if (entity is null)
            return;

        await context.DeleteAsync(entity);
    }

    public async Task StoreAsync(Doctor doctor, Patient patient, Appointment appointment)
    {
        var entity = new AppointmentsDto()
        {
            AppointmentTime = appointment.GetDateTime(),
            DoctorId = doctor.Id,
            PatientId = patient.Id
        };

        await context.SaveAsync(entity);
    }
}

