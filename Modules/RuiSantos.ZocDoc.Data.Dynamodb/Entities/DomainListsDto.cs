using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable(DomainListsTableName)]
internal class DomainListsDto
{
    private const string Specialties = "specialties";

    [DynamoDBHashKey(AttributeName = SourceAttributeName)]
    public string Source { get; set; } = string.Empty;

    [DynamoDBProperty(typeof(DictionaryConverter<Guid, string>))]
    public Dictionary<Guid, string> Values { get; set; } = new();

    public static Task<IReadOnlyDictionary<Guid, string>> GetSpecialtiesAsync(IDynamoDBContext context)
        => GetAsync(context, Specialties);

    public static Task SetSpecialtyAsync(IDynamoDBContext context, string specialty)
        => SetAsync(context, Specialties, specialty);

    public static Task RemoveSpecialtyAsync(IDynamoDBContext context, string specialty)
        => RemoveAsync(context, Specialties, specialty);

    private static async Task<IReadOnlyDictionary<Guid, String>> GetAsync(IDynamoDBContext context, string source)
    {
        var specialties = await context.LoadAsync<DomainListsDto>(source);
        return specialties?.Values ?? new Dictionary<Guid, String>();
    }

    private static async Task SetAsync(IDynamoDBContext context, string source, string value)
    {
        var specialties = await context.LoadAsync<DomainListsDto>(source) 
            ?? new DomainListsDto() {
                Source = source,
                Values = new Dictionary<Guid, String>()            
            };

        if (!specialties.Values.ContainsValue(value))
        {
            specialties.Values.Add(Guid.NewGuid(), value);
            await context.SaveAsync(specialties);
        }
    }

    private static async Task RemoveAsync(IDynamoDBContext context, string source, string value)
    {
        var specialties = await context.LoadAsync<DomainListsDto>(source);
        if (specialties is null)
            return;

        if (specialties.Values.FirstOrDefault(entry => entry.Value == value) is { } entry)
        {
            specialties.Values.Remove(entry.Key);
            await context.SaveAsync(specialties);
        }
    }
}