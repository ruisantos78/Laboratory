using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.Model;
using RuiSantos.Labs.Repository.DynamoDb.Core;

namespace RuiSantos.Labs.Repository.DynamoDb.Entities.Mappers;

internal class DoctorSpecializationClassMap : ClassMap
{
    protected override void Map(CreateTableRequest request)
    {
        request.TableName = DoctorSpecializationEntity.DoctorSpecializationTableName;

        request.AttributeDefinitions =
        [
            new AttributeDefinition(nameof(DoctorSpecializationEntity.DoctorId), ScalarAttributeType.S),
            new AttributeDefinition(nameof(DoctorSpecializationEntity.Specialization), ScalarAttributeType.S)
        ];

        request.KeySchema = [
            new KeySchemaElement(nameof(DoctorSpecializationEntity.DoctorId), KeyType.HASH),
            new KeySchemaElement(nameof(DoctorSpecializationEntity.Specialization), KeyType.RANGE)
        ];

        request.GlobalSecondaryIndexes =
        [
            new GlobalSecondaryIndexKeys(DoctorSpecializationEntity.DoctorSpecializationIndexName
                , nameof(DoctorSpecializationEntity.Specialization)
                , nameof(DoctorSpecializationEntity.DoctorId))
        ];
        
        request.ProvisionedThroughput = new ProvisionedThroughput(5, 5);
    }
}