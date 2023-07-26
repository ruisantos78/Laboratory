using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable("DomainLists")]
public class DomainListsDto
{
    [DynamoDBHashKey]
    public string Source { get; set; } = string.Empty;

    [DynamoDBProperty(typeof(DictionaryConverter<Guid, String>))]
    public Dictionary<Guid, String> Values { get; set; } = new();

    public static Task<IReadOnlyDictionary<Guid, String>> GetSpecialtiesAsync(IDynamoDBContext context)
        => GetAsync(context, "specialties");

    public static Task SetSpecialtyAsync(IDynamoDBContext context, string specialty)
        => SetAsync(context, "specialties", specialty);

    public static Task RemoveSpecialtyAsync(IDynamoDBContext context, string specialty)
        => RemoveAsync(context, "specialties", specialty);

    private static async Task<IReadOnlyDictionary<Guid, String>> GetAsync(IDynamoDBContext context, string source)
    {
        var specialties = await context.LoadAsync<DomainListsDto>(source);
        return specialties.Values;
    }

    private static async Task SetAsync(IDynamoDBContext context, string source, string value)
    {
        var specialties = await context.LoadAsync<DomainListsDto>(source);

        if (!specialties.Values.ContainsValue(value))
        {
            specialties.Values.Add(Guid.NewGuid(), value);
            await context.SaveAsync(specialties);
        }
    }

    private static async Task RemoveAsync(IDynamoDBContext context, string source, string value)
    {
        var specialties = await context.LoadAsync<DomainListsDto>(source);

        if (specialties.Values.FirstOrDefault(entry => entry.Value == value) is { } entry)
        {
            specialties.Values.Remove(entry.Key);
            await context.SaveAsync(specialties);
        }
    }
}