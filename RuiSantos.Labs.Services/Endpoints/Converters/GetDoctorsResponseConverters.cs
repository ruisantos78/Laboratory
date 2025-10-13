using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Services.Endpoints.Schemas;
using RuiSantos.Labs.Services.Services;

namespace RuiSantos.Labs.Services.Endpoints.Converters;

internal static class GetDoctorsResponseConverters
{
    public static GetDoctorsResponse ToGetDoctorsResponse(this Pagination<Doctor> doctor, ITokenService tokenService)
    {
        return new GetDoctorsResponse
        {
            PaginationToken = tokenService.Encode(doctor.PaginationToken),
            Items = [..doctor.Models.Select(Convert)]
        };
    }

    private static GetDoctorsResponseItem Convert(Doctor doctor)
    {
        return new GetDoctorsResponseItem
        {
            LicenseNumber = doctor.License,
            FirstName = doctor.FirstName,
            LastName = doctor.LastName,
            Email = doctor.Email,
            ContactNumbers = [..doctor.ContactNumbers],
            Specializations = [..doctor.Specializations]
        };
    }
}