using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Data.Dynamodb.Entities.Converters;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.MappingConstants;

namespace RuiSantos.Labs.Data.Dynamodb.Entities;

[DynamoDBTable(PatientsTableName)]
internal class PatientEntity
{
    [DynamoDBHashKey(
        AttributeName = PatientIdAttributeName, 
        Converter = typeof(GuidConverter))]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [DynamoDBGlobalSecondaryIndexHashKey(PatientSocialSecurityNumberIndexName, 
        AttributeName = SocialSecurityNumberAttributeName)]
    public string SocialSecurityNumber { get; set; } = string.Empty;

    [DynamoDBProperty]
    public string FirstName { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string LastName { get; set; } = string.Empty;
    
    [DynamoDBProperty]
    public string Email { get; set; } = string.Empty;

    [DynamoDBProperty]
    public List<string> ContactNumbers { get; set; } = new();
}

