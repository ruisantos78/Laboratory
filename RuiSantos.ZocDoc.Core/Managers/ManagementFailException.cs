namespace RuiSantos.ZocDoc.Core.Managers;

public class ManagementFailException : Exception, IFailure
{
    public static readonly ManagementFailException Empty = new ManagementFailException(string.Empty);

    public ManagementFailException(string message) : base(message) { }

    public static Task FromException(string message) => Task.FromException(new ManagementFailException(message));
}

