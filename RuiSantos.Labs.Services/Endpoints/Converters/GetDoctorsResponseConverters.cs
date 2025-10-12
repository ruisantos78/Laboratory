using RuiSantos.Labs.Domain.Extensions.Models;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Endpoints.Converters;

internal static class GetDoctorsResponseConverters
{
    public static GetDoctorsResponse ToGetDoctorsResponse(this Pagination<Doctor> doctor)
    {
        return new GetDoctorsResponse
        {
            PaginationToken = doctor.PaginationToken,
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