using RuiSantos.Labs.Repository.DynamoDb.Core.Converters;
using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.Labs.Repository.DynamoDb.Entities;

[DynamoDBTable(DoctorSpecializationTableName)]
internal record DoctorSpecializationEntity
{
    public const string DoctorSpecializationTableName = "doctors_specializations";
    public const string DoctorSpecializationByDoctorIdIndexName = "doctors_specializations_doctor_id_index";
    
    [DynamoDBHashKey( AttributeName = nameof(Specialization))]
    [DynamoDBGlobalSecondaryIndexRangeKey(DoctorSpecializationByDoctorIdIndexName, AttributeName = nameof(Specialization))]
    public required string Specialization { get; set; }
    
    [DynamoDBRangeKey(AttributeName = nameof(DoctorId), Converter = typeof(GuidConverter))]
    [DynamoDBGlobalSecondaryIndexHashKey(DoctorSpecializationByDoctorIdIndexName, AttributeName = nameof(DoctorId))]
    public Guid DoctorId { get; set; }
}