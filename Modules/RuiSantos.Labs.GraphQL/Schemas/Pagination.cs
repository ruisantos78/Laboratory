namespace RuiSantos.Labs.GraphQL.Schemas;

public class Pagination
{
	public int Take { get; init; }

	public string? Token { get; init; }

	public Pagination(): this(-1, null)
	{
	}

	public Pagination(int take, string? token)
	{
		this.Take = take;
		this.Token = token;
	}
}