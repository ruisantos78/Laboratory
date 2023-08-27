using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Data;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable(DoctorsTableName)]
internal partial class DoctorDto: IDynamoEntity<Doctor> {

    [DynamoDBHashKey(
        AttributeName = DoctorIdAttributeName,
        Converter = typeof(GuidConverter))]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [DynamoDBGlobalSecondaryIndexHashKey(DoctorLicenseIndexName,
        AttributeName = LicenseAttributeName)]
    public string License { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string FirstName { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string LastName { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string Email { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public List<string> ContactNumbers { get; set; } = new();
    
    [DynamoDBProperty(typeof(ListConverter<OfficeHour>))]
    public List<OfficeHour> Availability { get; set; } = new();
}