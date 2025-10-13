using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Repository.DynamoDb.Entities;
using RuiSantos.Labs.Repository.DynamoDb.Entities.Converters;
using RuiSantos.Labs.Repository.DynamoDb.Entities.Queries;

namespace RuiSantos.Labs.Repository.DynamoDb.Commands.Doctors;

internal class GetDoctorByIdCommand(Guid doctorId
    , IAmazonDynamoDB client
    , ILogger<GetDoctorByIdCommand> logger
    ): IAsyncCommand<Doctor?>
{
    private readonly DynamoDBContext _context = new DynamoDBContextBuilder()
        .WithDynamoDBClient(() => client)
        .Build();
    
    public async Task<Doctor?> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("GetDoctorByIdCommand: {DoctorId}", doctorId);
        try
        {
            var doctorEntity = await _context.LoadAsync<DoctorEntity>(doctorId, cancellationToken);
            if (doctorEntity is null)
                return null;
            
            var specializations = await _context.GetDoctorSpecializationsAsync(doctorEntity, cancellationToken);
            return doctorEntity.ToDoctor(specializations);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetDoctorByIdCommand: {DoctorId}", doctorId);
            throw;
        }
    }
}