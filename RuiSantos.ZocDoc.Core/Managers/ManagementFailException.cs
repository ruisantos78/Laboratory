using System.Runtime.Serialization;

namespace RuiSantos.ZocDoc.Core.Managers;

[Serializable]
public class ManagementFailException : Exception, IFailure
{
    public static readonly ManagementFailException Empty = new();

    protected ManagementFailException() { }

    public ManagementFailException(string message) : base(message) { }

    public ManagementFailException(string? message, Exception? innerException) : base(message, innerException) { }

    protected ManagementFailException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

