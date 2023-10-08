using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RuiSantos.Labs.Data.Dynamodb.Mediators;

namespace RuiSantos.Labs.Data.Dynamodb;

internal class DatabaseServices : IHostedService
{
    private readonly IAmazonDynamoDB _amazonDynamo;
    private readonly ILogger _logger;

    public DatabaseServices(
        IAmazonDynamoDB amazonDynamo,
        ILogger<DatabaseServices> logger)
    {
        _amazonDynamo = amazonDynamo;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Initializing Amazon DynamoDB...");

        var tables = await _amazonDynamo.ListTablesAsync(cancellationToken)
            .ConfigureAwait(false);

        var tasks = RegisterClassMaps.CreateTableRequests()
            .Where(req => !tables.TableNames.Contains(req.TableName))
            .Select(req => _amazonDynamo.CreateTableAsync(req, cancellationToken));

        await Task.WhenAll(tasks)
            .ContinueWith(CreateTableResponseLogger, cancellationToken);

        _logger.LogInformation("Amazon DynamoDB Update Completed!");
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private void CreateTableResponseLogger(Task<CreateTableResponse[]> task)
    {
        foreach (var response in task.Result)
        {
            _logger.LogInformation("Created Table: {TableName}.", response.TableDescription.TableName);
        }
    }
}