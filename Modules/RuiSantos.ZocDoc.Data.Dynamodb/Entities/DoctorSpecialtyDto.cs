using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable(DoctorSpecialtiesTableName)]
internal class DoctorSpecialtyDto
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