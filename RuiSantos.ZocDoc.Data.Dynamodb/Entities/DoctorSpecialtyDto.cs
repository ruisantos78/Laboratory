using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Data.Dynamodb.Entities.Converters;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

[DynamoDBTable("DoctorSpecialties")]
internal class DoctorSpecialtyDto
{
    const string DoctorSpecialtyIndex = "DoctorSpecialtyIndex";

    [DynamoDBHashKey(typeof(GuidConverter))]
    public Guid DoctorId { get; set; }
    
    [DynamoDBRangeKey]
    [DynamoDBGlobalSecondaryIndexHashKey(DoctorSpecialtyIndex)]
    public string Specialty { get; set; } = string.Empty;

    public static async Task<IReadOnlyList<Guid>> GetDoctorsBySpecialtyAsync(IDynamoDBContext context, string specialty)
    {
        var specialties = await context.QueryAsync<DoctorSpecialtyDto>(specialty, new DynamoDBOperationConfig {
            IndexName = DoctorSpecialtyIndex
        }).GetRemainingAsync();

        return specialties.Select(x => x.DoctorId).ToList();
    }

    public static async Task<IReadOnlyList<String>> GetSpecialtiesByDoctorIdAsync(IDynamoDBContext context, Guid doctorId)
    {
        var specialties = await context.QueryAsync<DoctorSpecialtyDto>(doctorId).GetRemainingAsync();
        return specialties.Select(x => x.Specialty).ToList();
    }

    internal static async Task SetSpecialtiesByDoctorIdAsync(IDynamoDBContext context, Guid id, HashSet<string> specialties)
    {
        var currentSpecialties = await GetSpecialtiesByDoctorIdAsync(context, id);
        
        var tasks = new List<Task>();
        tasks.AddRange(currentSpecialties.Where(x => !specialties.Contains(x)).Select(s => context.DeleteAsync<DoctorSpecialtyDto>(id, s)));
        tasks.AddRange(specialties.Where(x => !currentSpecialties.Contains(x)).Select(s => context.SaveAsync(new DoctorSpecialtyDto {
            DoctorId = id,
            Specialty = s
        })));       

        await Task.WhenAll(tasks.ToArray());
    }    
}