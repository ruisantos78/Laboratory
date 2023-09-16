using Amazon.DynamoDBv2.DataModel;

using static RuiSantos.Labs.Data.Dynamodb.Mappings.ClassMapConstants;
namespace RuiSantos.Labs.Data.Dynamodb.Entities;

partial class DictionaryDto
{
    private const string Specialties = "specialties";

    public static async Task<IReadOnlySet<string>> GetSpecialtiesAsync(IDynamoDBContext context)
        => await GetAsync(context, Specialties);

    public static Task SetSpecialtyAsync(IDynamoDBContext context, IEnumerable<string> specialties)
        => SetAsync(context, Specialties, specialties.ToArray());

    public static async Task RemoveSpecialtyAsync(IDynamoDBContext context, string speciality) 
    {
        var doctorSpecialties = await context.QueryAsync<DoctorSpecialtyDto>(speciality, new DynamoDBOperationConfig
        {
            IndexName = DoctorSpecialtyIndexName
        })
        .GetRemainingAsync();

        if (doctorSpecialties?.Any() is true)
        {
            var writer = context.CreateBatchWrite<DoctorSpecialtyDto>();
            writer.AddDeleteItems(doctorSpecialties);
            await writer.ExecuteAsync();
        }

        await RemoveAsync(context, Specialties, speciality);
    }
}
