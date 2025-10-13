using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Repository.DynamoDb.Entities;
using RuiSantos.Labs.Repository.DynamoDb.Entities.Converters;

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

            string[] specializations = [..
                await _context.QueryAsync<DoctorSpecializationEntity>(doctorEntity.Id)
                .GetRemainingAsync(cancellationToken)
                .ContinueWith(t => t.Result.Select(x => x.Specialization), cancellationToken)
            ];

            return doctorEntity.ToDoctor(specializations);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "GetDoctorByIdCommand: {DoctorId}", doctorId);
            throw;
        }
    }
}