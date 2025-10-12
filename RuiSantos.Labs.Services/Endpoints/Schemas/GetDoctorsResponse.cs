namespace RuiSantos.Labs.Services.Endpoints.Schemas;

public class GetDoctorsResponse()
{
    public string? PaginationToken { get; init; }

    public GetDoctorsResponseItem[] Items { get; init; } = [];
}

public class GetDoctorsResponseItem()
{
    public string? LicenseNumber { get; init; }
    public string? Email { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string[]? ContactNumbers { get; init; }
    public string[]? Specializations { get; init; }
}