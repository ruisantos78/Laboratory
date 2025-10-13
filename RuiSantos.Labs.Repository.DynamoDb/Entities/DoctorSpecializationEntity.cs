using RuiSantos.Labs.Repository.DynamoDb.Core.Converters;
using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.Labs.Repository.DynamoDb.Entities;

[DynamoDBTable(DoctorSpecializationTableName)]
internal record DoctorSpecializationEntity
{
    public const string DoctorSpecializationTableName = "doctors_specializations";
    public const string DoctorSpecializationIndexName = "doctors_specializations_index";
    
    [DynamoDBHashKey(AttributeName = nameof(DoctorId), Converter = typeof(GuidConverter))]
    [DynamoDBGlobalSecondaryIndexRangeKey(DoctorSpecializationIndexName, AttributeName = nameof(DoctorId))]
    public Guid DoctorId { get; set; }
    
    [DynamoDBRangeKey( AttributeName = nameof(Specialization))]
    [DynamoDBGlobalSecondaryIndexHashKey(DoctorSpecializationIndexName, AttributeName = nameof(Specialization))]
    public required string Specialization { get; set; }
    
    
}