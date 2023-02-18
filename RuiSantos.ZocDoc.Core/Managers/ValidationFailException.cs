namespace RuiSantos.ZocDoc.Core.Managers
{
	public class ValidationFailException: FluentValidation.ValidationException, IFailure
    {
        public static readonly ValidationFailException Empty = new ValidationFailException(Array.Empty<FluentValidation.Results.ValidationFailure>());

        public ValidationFailException(IEnumerable<FluentValidation.Results.ValidationFailure> erros): base(erros)
		{
		}
	}
}

