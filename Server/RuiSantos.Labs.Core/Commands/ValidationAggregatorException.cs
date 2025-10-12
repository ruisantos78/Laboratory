using System.ComponentModel.DataAnnotations;

namespace RuiSantos.Labs.Core.Commands;

public class ValidationAggregatorException(IEnumerable<ValidationResult> validationResults) 
    : ValidationException("One or more validation errors occurred")
{
    public IReadOnlyList<ValidationResult> ValidationResults { get; } = [.. validationResults];
}