using Amazon.DynamoDBv2.DataModel;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Entities;

[DynamoDBTable(DictionaryTableName)]
internal partial class DictionaryEntity
{    
    [DynamoDBHashKey(AttributeName = SourceAttributeName)]
    public string Source { get; init; }

    [DynamoDBRangeKey(AttributeName = ValueAttributeName)]
    public string Value { get; init; }

    public DictionaryEntity()
    {
        this.Source = string.Empty;
        this.Value = string.Empty;
    }
}