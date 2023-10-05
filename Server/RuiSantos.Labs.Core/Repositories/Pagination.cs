namespace RuiSantos.Labs.Core.Repositories;

public readonly struct Pagination<TModel>
{
    public IEnumerable<TModel> Models { get; init; }

	public string? PaginationToken { get; init; }

    public Pagination()
    {
        Models = Array.Empty<TModel>();
        PaginationToken = null;
    }

    public Pagination(IEnumerable<TModel> models, string? paginationToken)
    {
        Models = models;
        PaginationToken = paginationToken;
    }
}