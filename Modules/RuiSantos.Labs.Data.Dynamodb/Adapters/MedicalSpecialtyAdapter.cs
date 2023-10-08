using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Data.Dynamodb.Entities;

namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

internal interface IMedicalSpecialtyAdapter
{
    Task<IEnumerable<string>> LoadAsync();
    Task RemoveAsync(string value);
    Task StoreAsync(IEnumerable<string> entries);
}

internal class MedicalSpecialtyAdapter : IMedicalSpecialtyAdapter
{
    private const string Source = "specialties";

    private readonly IDynamoDBContext _context;

    public MedicalSpecialtyAdapter(IAmazonDynamoDB client)
    {
        _context = new DynamoDBContext(client);
    }

    public Task<IEnumerable<string>> LoadAsync()
    {
        return _context.QueryAsync<DictionaryEntity>(Source)
            .GetRemainingAsync()
            .ContinueWith(x => x.Result
                .Select(s => s.Value)
            );
    }

    public Task StoreAsync(IEnumerable<string> entries)
    {
        var writer = _context.CreateBatchWrite<DictionaryEntity>();
        writer.AddPutItems(entries.Select(value => new DictionaryEntity
        {
            Source = Source,
            Value = value
        }));

        return writer.ExecuteAsync();
    }

    public Task RemoveAsync(string value)
    {
        return _context.DeleteAsync<DictionaryEntity>(Source, value, CancellationToken.None);
    }
}
