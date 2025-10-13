using System.Text;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Endpoints.Converters;

internal static class GetDoctorsResponseConverter
{
    public static GetDoctorsResponse ToGetDoctorsResponse(this Pagination<Doctor> doctor, Func<string?, string>? encoder = null)
    {
        return new GetDoctorsResponse
        {
            PaginationToken = encoder?.Invoke(doctor.PaginationToken) ?? doctor.PaginationToken,
            Items = [..doctor.Models.Select(GetDoctorResponseConverter.ToGetDoctorResponse)]
        };
    }
}