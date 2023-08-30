using RuiSantos.Labs.Core.Services.Exceptions;

namespace RuiSantos.Labs.Api.Core;

internal static class ControllerUtils
{
    public static IEnumerable<TimeSpan> StringToTimeSpanArray(string[] values)
    {
		try
		{
			return values.Select(TimeSpan.Parse);
		}
		catch 
		{			
			throw new ValidationFailException("Cannot convert the strings to a TimeSpan value.");
        }
    }

	public static bool TryParseTimeSpanArray(string[] values, out IEnumerable<TimeSpan> result)
	{
        try
        {
            result = values.Select(TimeSpan.Parse);
            return true;
        }
        catch
        {
            result = Array.Empty<TimeSpan>();
            return false;
        }
    }
}
