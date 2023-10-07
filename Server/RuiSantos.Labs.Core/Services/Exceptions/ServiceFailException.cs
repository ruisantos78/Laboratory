using System.Runtime.Serialization;

namespace RuiSantos.Labs.Core.Services.Exceptions;

/// <summary>
/// Exception thrown when a management operation fails.
/// </summary>
[Serializable]
public class ServiceFailException : Exception
{
    /// <summary>
    /// Empty exception.
    /// </summary>
    public static readonly ServiceFailException Empty = new();

    /// <summary>
    /// Creates a new ManagementFailException.
    /// </summary>
    protected ServiceFailException() { }

    /// <summary>
    /// Creates a new ManagementFailException.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public ServiceFailException(string message) : base(message) { }

    /// <summary>
    /// Creates a new ManagementFailException.
    /// </summary>
    /// <param name="message">Exception message.</param>
    /// <param name="innerException">Inner exception.</param>
    public ServiceFailException(string? message, Exception? innerException) : base(message, innerException) { }

    /// <summary>
    /// Creates a new ManagementFailException.
    /// </summary>
    /// <param name="info">Serialization info.</param>
    /// <param name="context">Streaming context.</param>
    protected ServiceFailException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}

