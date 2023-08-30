using Amazon.DynamoDBv2.DataModel;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable(DictionaryTableName)]
internal partial class DictionaryDto
{    
    [DynamoDBHashKey(AttributeName = SourceAttributeName)]
    public string Source { get; init; }

    [DynamoDBProperty]
    public HashSet<string> Values { get; init; }

    public DictionaryDto(): this(string.Empty)
    { 
    }

    public DictionaryDto(string source)
    {
        this.Source = source;
        this.Values = new();
    }
}