using Amazon.DynamoDBv2.DataModel;
using RuiSantos.ZocDoc.Core.Models;

namespace RuiSantos.ZocDoc.Data.Dynamodb.Entities;

partial class DictionaryDto
{
    private const string Specialties = "specialties";

    public static async Task<List<MedicalSpecialty>> GetSpecialtiesAsync(IDynamoDBContext context)
    {
        var result = await GetAsync(context, Specialties);
        return result.Select(s => new MedicalSpecialty
        {
            Id = s.Key,
            Description = s.Value
        })
        .ToList();
    }

    public static Task SetSpecialtyAsync(IDynamoDBContext context, string specialty)
        => SetAsync(context, Specialties, specialty);

    public static Task RemoveSpecialtyAsync(IDynamoDBContext context, string specialty)
        => RemoveAsync(context, Specialties, specialty);
}
