using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Tests.Fixtures;

public sealed partial class DatabaseFixture: IDisposable
{    
    public AmazonDynamoDBClient Client { get; }
    public List<Doctor> Doctors { get; }

    private readonly DynamoDBContext context;
    
    public DatabaseFixture()
	{
        this.Client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8002"
        });
        
        RegisterClassMaps.InitializeDatabase(Client);

        this.Doctors = Task.WhenAll(CreeateDoctorsAsync()).Result.ToList();
        this.context = new DynamoDBContext(Client);
    }

    public void Dispose()
    {
        FinalizeDatabaseAsync();

        this.context.Dispose();
        this.Client.Dispose();        
    }

    private void FinalizeDatabaseAsync()
    {
        var tableNames = Client.ListTablesAsync().Result.TableNames;
        var tasks = tableNames.Select(tn => Client.DeleteTableAsync(tn)).ToArray();
        Task.WaitAll(tasks);
    }
}
