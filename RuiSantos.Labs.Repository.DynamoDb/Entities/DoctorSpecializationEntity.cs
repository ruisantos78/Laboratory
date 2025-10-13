using RuiSantos.Labs.Repository.DynamoDb.Core.Converters;
using Amazon.DynamoDBv2.DataModel;

namespace RuiSantos.Labs.Repository.DynamoDb.Entities;

[DynamoDBTable(DoctorSpecializationTableName)]
internal record DoctorSpecializationEntity
{
    public const string DoctorSpecializationTableName = "doctors_specializations";
    public const string SpecializationDoctorIndexName = "specializations_doctor_index";
    
    [DynamoDBHashKey( AttributeName = nameof(Specialization))]
    public required string Specialization { get; set; }
    
    [DynamoDBRangeKey(AttributeName = nameof(DoctorId), Converter = typeof(GuidConverter))]
    [DynamoDBGlobalSecondaryIndexHashKey(SpecializationDoctorIndexName, AttributeName = nameof(DoctorId))]
    public Guid DoctorId { get; set; }
}