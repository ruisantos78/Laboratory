using System.Text;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Endpoints.Converters;

internal static class GetDoctorsPaginationResponseConverter
{
    public static GetDoctorsPaginationResponse ToGetDoctorsPaginationResponse(this Pagination<Doctor> doctor, Func<string?, string>? encoder = null)
    {
        return new GetDoctorsPaginationResponse
        {
            PaginationToken = encoder?.Invoke(doctor.PaginationToken) ?? doctor.PaginationToken,
            Items = [..doctor.Models.Select(GetDoctorResponseConverter.ToGetDoctorResponse)]
        };
    }
}