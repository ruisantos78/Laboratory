using FluentValidation;
using FluentValidation.Results;

namespace RuiSantos.Labs.Core.Services.Exceptions;

/// <summary>
/// Exception thrown when a validation fails.
/// </summary>
[Serializable]
public class ValidationFailException : ValidationException, IFailure
{
    /// <summary>
    /// Empty exception used when no validation fails.
    /// </summary>
    public static readonly ValidationFailException Empty = new();

    /// <summary>
    /// Creates a new instance of the exception.
    /// </summary>
    protected ValidationFailException() : this(string.Empty) { }

    /// <summary>
    /// Creates a new instance of the exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    public ValidationFailException(string message) : base(message) { }

    /// <summary>
    /// Creates a new instance of the exception.
    /// </summary>
    /// <param name="errors">The validation errors.</param>
    public ValidationFailException(IEnumerable<ValidationFailure> errors) : base(errors) { }

    /// <summary>
    /// Creates a new instance of the exception.
    /// </summary>
    /// <param name="serializationInfo">The serialization info.</param>
    /// <param name="streamingContext">The streaming context.</param>
    protected ValidationFailException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
}


