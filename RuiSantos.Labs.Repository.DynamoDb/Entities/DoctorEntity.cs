using Amazon.DynamoDBv2.DataModel;
using RuiSantos.Labs.Repository.DynamoDb.Core.Converters;

namespace RuiSantos.Labs.Repository.DynamoDb.Entities;

[DynamoDBTable(DoctorTableName)]
internal record DoctorEntity
{
    public const string DoctorTableName = "doctors";
    public const string DoctorLicenceIndexName = "doctor_license_index";
    
    [DynamoDBHashKey(AttributeName = nameof(Id), Converter = typeof(GuidConverter))]
    public Guid Id { get; set; } = Guid.NewGuid();
    
    [DynamoDBGlobalSecondaryIndexHashKey(DoctorLicenceIndexName, AttributeName = nameof(License))]
    public string License { get; set; } = string.Empty;
    
    [DynamoDBProperty] public string FirstName { get; set; } = string.Empty;
    
    [DynamoDBProperty] public string LastName { get; set; } = string.Empty;
    
    [DynamoDBProperty] public string Email { get; set; } = string.Empty;
    
    [DynamoDBProperty] public List<string> ContactNumbers { get; set; } = [];
    
    [DynamoDBProperty] public List<AvailabilityEntity> Availability { get; set; } = [];
}

internal record AvailabilityEntity
{
    [DynamoDBProperty(typeof(DayOfWeekConverter))] 
    public DayOfWeek DayOfWeek { get; set; }
    
    [DynamoDBProperty(typeof(TimeSpanConverter))] 
    public List<TimeSpan> Hours { get; set; } = [];
}