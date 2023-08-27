using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Data;

using static RuiSantos.ZocDoc.Data.Dynamodb.Mappings.ClassMapConstants;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable(PatientsTableName)]
internal partial class PatientDto: IDynamoEntity<Patient>
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

