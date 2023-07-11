using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Mappings;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Tests.Fixtures;

public sealed class DatabaseFixture: IDisposable
{    
    public AmazonDynamoDBClient Client { get; }
    public List<Doctor> Doctors { get; }

    private readonly DynamoDBContext context;
    
    public DatabaseFixture()
	{
        this.Client = new AmazonDynamoDBClient(new AmazonDynamoDBConfig
        {
            ServiceURL = "http://localhost:8000"
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
        var tasks = tableNames.Select(tn => Client.DeleteTableAsync(tn));
        Task.WaitAll(tasks.ToArray());
    }

    private Task<Doctor>[] CreeateDoctorsAsync() => new[]
    {
        new DoctorBuilder(Client)
            .WithDoctor("ABC001", "Orli", "Waller", "iam.dictum@protonmail.edu")
            .WithSpecialty("Cardiology")
            .WithContactNumber("1-516-872-1630")
            .BuildAsync(),

        new DoctorBuilder(Client)
            .WithDoctor("ABC002", "Xyla", "Miranda", "etiam.ligula@hotmail.com")
            .WithSpecialty("Cardiology", "Dermatology")
            .WithContactNumber("1-356-676-7417")
            .BuildAsync()
    };
}

