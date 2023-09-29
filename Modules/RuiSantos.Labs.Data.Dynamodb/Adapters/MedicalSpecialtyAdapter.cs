using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Data.Dynamodb.Entities;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

public class MedicalSpecialtyAdapter
{
    private const string Source = "specialties";
    private readonly IDynamoDBContext context;

    public MedicalSpecialtyAdapter(IAmazonDynamoDB client)
    {
        this.context = new DynamoDBContext(client);
    }

    public async Task<IReadOnlySet<string>> LoadAsync() 
    {
        return await context.QueryAsync<DictionaryEntity>(Source)
            .GetRemainingAsync()
            .ContinueWith(x => x.Result.Select(s => s.Value).ToHashSet());                              
    }

    public async Task StoreAsync(IEnumerable<string> values)
    {
        if (!values.Any())
            return;

        var writer = context.CreateBatchWrite<DictionaryEntity>();
        writer.AddPutItems(values.Select(value => new DictionaryEntity 
        { 
            Source = Source, 
            Value = value 
        }));

        await writer.ExecuteAsync();  
    }

    public async Task RemoveAsync(string value)
    {
        await context.DeleteAsync<DictionaryEntity>(Source, value, CancellationToken.None);
    }
}
