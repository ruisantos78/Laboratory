namespace RuiSantos.Labs.Client.Core;

internal static class Extensions
{
    public static void ForEach<TItem>(this IReadOnlyList<TItem>? items, Action<TItem> action)
    {
        if (items is null) return;
        foreach (var item in items) action(item);
    }
}