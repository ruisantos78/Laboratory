using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Endpoints.Converters;

internal static class GetDoctorResponseConverter
{
    public static GetDoctorResponse ToGetDoctorResponse(this Doctor doctor)
    {
        return new GetDoctorResponse
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