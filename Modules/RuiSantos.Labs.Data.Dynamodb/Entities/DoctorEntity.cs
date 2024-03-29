using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Core.Models;
using RuiSantos.Labs.Data.Dynamodb.Entities.Converters;
using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Entities;

[DynamoDBTable(DoctorsTableName)]
internal class DoctorEntity
{
    [DynamoDBHashKey(
        AttributeName = DoctorIdAttributeName,
        Converter = typeof(GuidConverter))]
    public Guid Id { get; set; } = Guid.NewGuid();

    [DynamoDBGlobalSecondaryIndexHashKey(DoctorLicenseIndexName,
        AttributeName = LicenseAttributeName)]
    public string License { get; set; } = string.Empty;

    [DynamoDBProperty] public string FirstName { get; set; } = string.Empty;

    [DynamoDBProperty] public string LastName { get; set; } = string.Empty;

    [DynamoDBProperty] public string Email { get; set; } = string.Empty;

    [DynamoDBProperty] public List<string> ContactNumbers { get; set; } = new();

    [DynamoDBProperty(typeof(ListConverter<OfficeHour>))]
    public List<OfficeHour> Availability { get; set; } = new();
}