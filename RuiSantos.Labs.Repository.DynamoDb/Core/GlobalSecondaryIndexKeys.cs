using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;

namespace RuiSantos.Labs.Repository.DynamoDb.Core;

internal class GlobalSecondaryIndexKeys : GlobalSecondaryIndex
{
    public GlobalSecondaryIndexKeys(string indexName, string hashAttributeName)
    {
        IndexName = indexName;
        KeySchema = [new KeySchemaElement(hashAttributeName, KeyType.HASH)];

        Projection = new Projection { ProjectionType = ProjectionType.ALL };
        ProvisionedThroughput = new ProvisionedThroughput(5, 5);
    }

    public GlobalSecondaryIndexKeys(string indexName, string hashAttributeName, params string[] sortAttributeNames) :
        this(indexName, hashAttributeName)
    {
        KeySchema.AddRange(sortAttributeNames.Select(x => new KeySchemaElement(x, KeyType.RANGE)));
    }
}