namespace RuiSantos.Labs.Services.Endpoints.Schemas;

public class GetDoctorsPaginationResponse()
{
    public string? PaginationToken { get; init; }

    public GetDoctorResponse[] Items { get; init; } = [];
}