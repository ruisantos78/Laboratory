using System.Text;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Endpoints.Converters;

internal static class GetDoctorsResponseConverters
{
    public static GetDoctorsResponse ToGetDoctorsResponse(this Pagination<Doctor> doctor, Func<string?, string>? encoder = null)
    {
        return new GetDoctorsResponse
        {
            PaginationToken = encoder?.Invoke(doctor.PaginationToken) ?? doctor.PaginationToken,
            Items = [..doctor.Models.Select(ToGetDoctorsResponseItem)]
        };
    }

    private static GetDoctorsResponseItem ToGetDoctorsResponseItem(Doctor doctor)
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