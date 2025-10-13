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
            new AttributeDefinition(nameof(DoctorSpecializationEntity.Specialization), ScalarAttributeType.S),
            new AttributeDefinition(nameof(DoctorSpecializationEntity.DoctorId), ScalarAttributeType.S)
        ];

        request.KeySchema = [
            new KeySchemaElement(nameof(DoctorSpecializationEntity.Specialization), KeyType.HASH),
            new KeySchemaElement(nameof(DoctorSpecializationEntity.DoctorId), KeyType.RANGE)
        ];

        request.GlobalSecondaryIndexes =
        [
            new GlobalSecondaryIndexKeys(DoctorSpecializationEntity.DoctorSpecializationByDoctorIdIndexName
                , nameof(DoctorSpecializationEntity.DoctorId)
                , nameof(DoctorSpecializationEntity.Specialization))
        ];
        
        request.ProvisionedThroughput = new ProvisionedThroughput(5, 5);
    }
}