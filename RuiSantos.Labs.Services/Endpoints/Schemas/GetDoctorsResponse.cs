namespace RuiSantos.Labs.Services.Endpoints.Schemas;

public class GetDoctorsResponse()
{
    public string? PaginationToken { get; init; }

    public GetDoctorResponse[] Items { get; init; } = [];
}