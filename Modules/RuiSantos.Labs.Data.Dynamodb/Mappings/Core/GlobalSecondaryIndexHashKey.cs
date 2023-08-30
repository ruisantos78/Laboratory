using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace RuiSantos.Labs.Data.Dynamodb.Mappings.Core;

internal class GlobalSecondaryIndexHashKey : GlobalSecondaryIndex
{
    public GlobalSecondaryIndexHashKey(string indexName, string hashAttributeName)
    {
        this.IndexName = indexName;

        this.KeySchema = new List<KeySchemaElement>
        {
            new(hashAttributeName, KeyType.HASH)
        };

        this.Projection = new() { ProjectionType = ProjectionType.ALL };

        this.ProvisionedThroughput = new ProvisionedThroughput(5, 5);
    }

    public GlobalSecondaryIndexHashKey(string indexName, string hashAttributeName, string sortAttributeName)
    {
        this.IndexName = indexName;

        this.KeySchema = new List<KeySchemaElement>
        {
            new(hashAttributeName, KeyType.HASH),
            new(sortAttributeName, KeyType.RANGE)
        };

        this.Projection = new() { ProjectionType = ProjectionType.ALL };

        this.ProvisionedThroughput = new ProvisionedThroughput(5, 5);
    }
}