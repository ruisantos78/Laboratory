using StrawberryShake;

namespace RuiSantos.Labs.Client;

internal static class StrawberryShakeExtensions
{
	public static AggregateException GetAggregateException(this IReadOnlyList<IClientError> errors)
	{
		var exceptions = errors.Where(x => x.Exception is not null).Select(x => x.Exception!);
		return new AggregateException(exceptions);
	}
}

