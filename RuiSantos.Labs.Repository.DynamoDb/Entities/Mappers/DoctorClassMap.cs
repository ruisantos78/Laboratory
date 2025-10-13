using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using RuiSantos.Labs.Repository.DynamoDb.Core;

namespace RuiSantos.Labs.Repository.DynamoDb.Entities.Mappers;

internal class DoctorClassMap : ClassMap
{
    protected override void Map(CreateTableRequest request)
    {
        request.TableName = DoctorEntity.DoctorTableName;

        request.AttributeDefinitions =
        [
            new AttributeDefinition(nameof(DoctorEntity.Id), ScalarAttributeType.S),
            new AttributeDefinition(nameof(DoctorEntity.License), ScalarAttributeType.S)
        ];

        request.KeySchema = [
            new KeySchemaElement(nameof(DoctorEntity.Id), KeyType.HASH)
        ];

        request.GlobalSecondaryIndexes =
        [
            new GlobalSecondaryIndexKeys(DoctorEntity.DoctorLicenceIndexName, nameof(DoctorEntity.License))
        ];

        request.ProvisionedThroughput = new ProvisionedThroughput(5, 5);
    }
}