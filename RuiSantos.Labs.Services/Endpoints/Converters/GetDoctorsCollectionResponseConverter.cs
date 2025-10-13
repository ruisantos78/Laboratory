using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Endpoints.Converters;

internal static class GetDoctorsCollectionResponseConverter
{
    public static GetDoctorsCollectionResponse ToGetDoctorsCollectionResponse(this IEnumerable<Doctor> doctors)
    {
        return new GetDoctorsCollectionResponse
        {
            Items = [..doctors.Select(GetDoctorResponseConverter.ToGetDoctorResponse)]
        };
    }
}