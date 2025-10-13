namespace RuiSantos.Labs.Services.Endpoints.Schemas;

public class GetDoctorResponse() {
    public required string LicenseNumber { get; init; }
    public required string Email { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public string[] ContactNumbers { get; init; } = [];
    public string[] Specializations { get; init; } = [];
}