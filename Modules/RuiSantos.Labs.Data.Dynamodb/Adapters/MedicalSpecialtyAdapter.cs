using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Data.Dynamodb.Entities;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

public class MedicalSpecialtyAdapter
{
    private const string Source = "specialties";
    
    private IDynamoDBContext Context { get; }

    public MedicalSpecialtyAdapter(IAmazonDynamoDB client)
    {
        Context = new DynamoDBContext(client);
    }

    public async Task<IReadOnlySet<string>> LoadAsync() 
    {
        return await Context.QueryAsync<DictionaryEntity>(Source)
            .GetRemainingAsync()
            .ContinueWith(x => x.Result
                .Select(s => s.Value)
                .ToHashSet()
            );                              
    }

    public Task StoreAsync(IEnumerable<string> entries)
    {
        var values = entries.ToArray();
        if (!values.Any())
            return Task.CompletedTask;

        var writer = Context.CreateBatchWrite<DictionaryEntity>();
        writer.AddPutItems(values.Select(value => new DictionaryEntity 
        { 
            Source = Source, 
            Value = value 
        }));

        return writer.ExecuteAsync();  
    }

    public Task RemoveAsync(string value)
    {
        return Context.DeleteAsync<DictionaryEntity>(Source, value, CancellationToken.None);
    }
}
