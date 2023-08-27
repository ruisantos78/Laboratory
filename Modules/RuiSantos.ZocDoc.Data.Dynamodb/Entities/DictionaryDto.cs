using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable(DictionaryTableName)]
internal partial class DictionaryDto
{    
    [DynamoDBHashKey(AttributeName = SourceAttributeName)]
    public string Source { get; init; }

    [DynamoDBProperty(typeof(DictionaryConverter<Guid, string>))]
    public Dictionary<Guid, string> Values { get; init; }

    public DictionaryDto()
    {
        this.Source = string.Empty;
        this.Values = new();    
    }

    public DictionaryDto(string source)
    {
        this.Source = source;
        this.Values = new();    
    }

    public DictionaryDto(string source, Dictionary<Guid, string> values)
    {
        this.Source = source;
        this.Values = values;
    }
}