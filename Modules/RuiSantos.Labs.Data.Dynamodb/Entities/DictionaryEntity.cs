using Amazon.DynamoDBv2.DataModel;
using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Entities;

[DynamoDBTable(DictionaryTableName)]
internal class DictionaryEntity
{
    [DynamoDBHashKey(AttributeName = SourceAttributeName)]
    public string Source { get; set; } = string.Empty;

    [DynamoDBRangeKey(AttributeName = ValueAttributeName)]
    public string Value { get; set; } = string.Empty;
}