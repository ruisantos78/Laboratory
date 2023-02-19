using FluentValidation;
using FluentValidation.Results;

namespace RuiSantos.ZocDoc.Core.Managers.Exceptions
{
    [Serializable]
    public class ValidationFailException : ValidationException, IFailure
    {
        public static readonly ValidationFailException Empty = new();

        protected ValidationFailException() : this(string.Empty) { }

        public ValidationFailException(string message) : base(message) { }

        public ValidationFailException(IEnumerable<ValidationFailure> erros) : base(erros) { }

        protected ValidationFailException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}

