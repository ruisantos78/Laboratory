﻿using Amazon.DynamoDBv2.DataModel;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Entities;

[DynamoDBTable(DictionaryTableName)]
internal partial class DictionaryDto
{    
    [DynamoDBHashKey(AttributeName = SourceAttributeName)]
    public string Source { get; init; }

    [DynamoDBRangeKey(AttributeName = ValueAttributeName)]
    public string Value { get; init; }

    public DictionaryDto()
    {
        this.Source = string.Empty;
        this.Value = string.Empty;
    }
}