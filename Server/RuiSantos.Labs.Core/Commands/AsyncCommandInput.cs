using System.ComponentModel.DataAnnotations;

namespace RuiSantos.Labs.Core.Commands;

public abstract class AsyncCommandInput: IValidatableObject
{
    public void ThrowIfNotValid()
    {
        var validationContext = new ValidationContext(this);
        var validationResults = new List<ValidationResult>();
        
        var isValid = Validator.TryValidateObject(
            this, 
            validationContext, 
            validationResults, 
            validateAllProperties: true
        );

        if (isValid) 
            return;
        
        throw new ValidationAggregatorException(validationResults);
    }

    IEnumerable<ValidationResult> IValidatableObject.Validate(ValidationContext validationContext)
    {
        return OnValidate(validationContext);
    }

    protected virtual IEnumerable<ValidationResult> OnValidate(ValidationContext validationContext)
    {
        yield break;
    }
}