using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Data.Dynamodb.Entities.Converters;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Entities;

[DynamoDBTable(DoctorSpecialtiesTableName)]
internal class DoctorSpecialtyEntity
{
    [DynamoDBHashKey(
        AttributeName = DoctorIdAttributeName, 
        Converter = typeof(GuidConverter))]
    [DynamoDBGlobalSecondaryIndexRangeKey(DoctorSpecialtyIndexName, 
        AttributeName = DoctorIdAttributeName)]    
    public Guid DoctorId { get; set; }
    
    [DynamoDBRangeKey(AttributeName = SpecialtyAttributeName)]
    [DynamoDBGlobalSecondaryIndexHashKey(DoctorSpecialtyIndexName, 
        AttributeName = SpecialtyAttributeName)]    
    public string Specialty { get; set; } = string.Empty; 
}