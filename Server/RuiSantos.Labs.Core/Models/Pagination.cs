﻿namespace RuiSantos.Labs.Core.Models;

public readonly struct Pagination<TModel>
{
    public IReadOnlyList<TModel> Models { get; init; }

    public string? PaginationToken { get; init; }

    public Pagination()
    {
        Models = Array.Empty<TModel>();
        PaginationToken = null;
    }

    public Pagination(IEnumerable<TModel> models, string? paginationToken)
    {
        Models = models.ToList();
        PaginationToken = paginationToken;
    }
}