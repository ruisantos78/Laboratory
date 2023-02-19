namespace RuiSantos.ZocDoc.Core.Tests;

internal static class TaskExtensions
{
    public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> enumerable)
    {
        var result = new List<T>();
        await foreach (var item in enumerable) result.Add(item);
        return result;
    }
}
