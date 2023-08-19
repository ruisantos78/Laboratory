using RuiSantos.ZocDoc.Core.Managers;
using RuiSantos.ZocDoc.GraphQL.Schemas;

namespace RuiSantos.ZocDoc.GraphQL;

public class Mutations
{
    [GraphQLDescription("Create a medical specialty.")]
    public async Task<List<SpecialtySchema>> CreateSpecialty(
      [GraphQLDescription("The description of  the specialty.")] string description,
      [Service] IMedicalSpecialtiesManagement management)
    {
        var descriptions = new List<string>() { description };
        await management.CreateMedicalSpecialtiesAsync(descriptions);
        return descriptions.Select(x => new SpecialtySchema { Specialty = x }).ToList();
    }

    [GraphQLDescription("Create a list of medical specialties.")]
    public async Task<List<SpecialtySchema>> CreateSpecialties(
      [GraphQLDescription("A list of medical specialties.")] List<string> descriptions,
      [Service] IMedicalSpecialtiesManagement management)
    {
        await management.CreateMedicalSpecialtiesAsync(descriptions);
        return descriptions.Select(x => new SpecialtySchema { Specialty = x }).ToList();
    }
}