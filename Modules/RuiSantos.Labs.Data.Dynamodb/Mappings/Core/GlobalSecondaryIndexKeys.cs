using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace RuiSantos.Labs.Data.Dynamodb.Mappings.Core;

internal class GlobalSecondaryIndexKeys : GlobalSecondaryIndex
{
    public GlobalSecondaryIndexKeys(string indexName, string hashAttributeName)
    {
        IndexName = indexName;
        KeySchema = new List<KeySchemaElement>
        {
            new(hashAttributeName, KeyType.HASH)
        };

        Projection = new() { ProjectionType = ProjectionType.ALL };
        ProvisionedThroughput = new ProvisionedThroughput(5, 5);
    }

    public GlobalSecondaryIndexKeys(string indexName, string hashAttributeName, params string[] sortAttributeNames) :
        this(indexName, hashAttributeName)
    {
        KeySchema.AddRange(sortAttributeNames.Select(x => new KeySchemaElement(x, KeyType.RANGE)));
    }
}