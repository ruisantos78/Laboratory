using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable("DoctorSpecialties")]
internal class DoctorSpecialtyDto
{
    internal const string DoctorSpecialtyIndex = "DoctorSpecialtyIndex";

    [DynamoDBHashKey(typeof(GuidConverter))]
    public Guid DoctorId { get; set; }
    
    [DynamoDBRangeKey]
    [DynamoDBGlobalSecondaryIndexHashKey(DoctorSpecialtyIndex)]
    public string Specialty { get; set; } = string.Empty; 
}