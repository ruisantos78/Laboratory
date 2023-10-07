namespace RuiSantos.Labs.Client.Models;

public readonly struct PaginationModel<TModel>
{
    public IReadOnlyList<TModel> Models { get; init; }

    public string? PaginationToken { get; init; }

    public PaginationModel()
    {
        Models = Array.Empty<TModel>();
        PaginationToken = null;
    }

    public PaginationModel(IEnumerable<TModel> models, string? paginationToken)
    {
        Models = models.ToList();
        PaginationToken = paginationToken;
    }
}