namespace RuiSantos.ZocDoc.Core.Managers
{
    [Serializable]
    public class ValidationFailException : FluentValidation.ValidationException, IFailure
    {
        public static readonly ValidationFailException Empty = new();

        protected ValidationFailException() : this(string.Empty) { }

        public ValidationFailException(string message) : base(message) { }

        public ValidationFailException(IEnumerable<FluentValidation.Results.ValidationFailure> erros) : base(erros) { }

        protected ValidationFailException(System.Runtime.Serialization.SerializationInfo serializationInfo, System.Runtime.Serialization.StreamingContext streamingContext) : base(serializationInfo, streamingContext) { }
    }
}

