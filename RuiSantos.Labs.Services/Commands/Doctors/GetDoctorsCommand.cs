using FluentValidation;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Data;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Domain.Repositories;

namespace RuiSantos.Labs.Services.Commands.Doctors;

internal class GetDoctorsCommand(GetDoctorsCommandInput input
    , IDoctorRepository doctorRepository
    , ILogger<GetDoctorsCommand> logger
    ) : IAsyncCommand<Pagination<Doctor>>
{
    public async Task<Pagination<Doctor>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting doctors with input {@Input}", input);
        try
        {
            await ValidateAndThrowAsync(cancellationToken);
            
            var take = input.Take ?? int.MaxValue;
            return await doctorRepository.FindAllAsync(take, input.PaginationToken, cancellationToken);    
        } 
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting doctors with input {@Input}", input);
            throw;
        }
    }
    
    private async Task ValidateAndThrowAsync(CancellationToken cancellationToken)
    {
        var validator = new InlineValidator<GetDoctorsCommandInput>();
        
        validator.RuleFor(x => x.Take)
            .GreaterThan(0)
            .When(x => x.Take.HasValue);
        
        validator.RuleFor(x => x.PaginationToken)
            .Custom((token, context) => 
            {
                if (!Tokens.TryDecode(token, out _))
                    context.AddFailure("Invalid pagination token format");
            })
            .When(x => !string.IsNullOrWhiteSpace(x.PaginationToken));
        
        await validator.ValidateAndThrowAsync(input, cancellationToken: cancellationToken);
    }
}

internal class GetDoctorsCommandInput
{
    public int? Take { get; init; }
    public string? PaginationToken { get; init; }
}