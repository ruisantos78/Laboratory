using RuiSantos.ZocDoc.Core.Managers.Exceptions;

namespace RuiSantos.ZocDoc.Api.Core;

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
}
