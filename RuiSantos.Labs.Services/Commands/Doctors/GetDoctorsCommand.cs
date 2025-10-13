using FluentValidation;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Models;
using RuiSantos.Labs.Domain.Repositories;
using RuiSantos.Labs.Services.Services;

namespace RuiSantos.Labs.Services.Commands.Doctors;

internal class GetDoctorsCommand(GetDoctorsCommandInput input
    , IDoctorRepository doctorRepository
    , ITokenService tokenService
    , ILogger<GetDoctorsCommand> logger
    ) : IAsyncCommand<Pagination<Doctor>>
{
    private int _take = input.Take ?? int.MaxValue;
    private string? _paginationToken;

    public async Task<Pagination<Doctor>> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting doctors with input {@Input}", input);
        try
        {
            await ValidateAndThrowAsync(cancellationToken);                    
            return await doctorRepository.FindAllAsync(_take, _paginationToken, cancellationToken);    
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
            .Must(token => tokenService.TryDecode(token, out _paginationToken))
            .When(x => !string.IsNullOrWhiteSpace(x.PaginationToken))
            .WithMessage("Invalid pagination token format.");
        
        await validator.ValidateAndThrowAsync(input, cancellationToken: cancellationToken);
    }
}

internal class GetDoctorsCommandInput
{
    public int? Take { get; init; }
    public string? PaginationToken { get; init; }
}