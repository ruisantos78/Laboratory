using System.Runtime.Serialization;

namespace RuiSantos.ZocDoc.Core.Managers.Exceptions;

/// <summary>
/// Exception thrown when a management operation fails.
/// </summary>
[Serializable]
public class ManagementFailException : Exception, IFailure
{
    /// <summary>
    /// Empty exception.
    /// </summary>
    public static readonly ManagementFailException Empty = new();

    /// <summary>
    /// Creates a new ManagementFailException.
    /// </summary>
    protected ManagementFailException() { }

    /// <summary>
    /// Creates a new ManagementFailException.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public ManagementFailException(string message) : base(message) { }

    /// <summary>
    /// Creates a new ManagementFailException.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Inner exception.</param>
    public ManagementFailException(string? message, Exception? innerException) : base(message, innerException) { }

    /// <summary>
    /// Creates a new ManagementFailException.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="context">Streaming context.</param>
    protected ManagementFailException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

