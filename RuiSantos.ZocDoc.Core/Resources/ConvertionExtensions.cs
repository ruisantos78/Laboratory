namespace RuiSantos.ZocDoc.Core;

internal static class ConvertionExtensions
{
    public static DateTime ToDateTime(this DateOnly date, TimeSpan timeSpan)
    {
        return date.ToDateTime(TimeOnly.FromTimeSpan(timeSpan));
    }
}
