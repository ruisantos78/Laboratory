using System.Text;
using FluentValidation;
using RuiSantos.Labs.Domain.Commands;
using RuiSantos.Labs.Domain.Repositories;
using RuiSantos.Labs.Services.Endpoints.Converters;
using RuiSantos.Labs.Services.Endpoints.Schemas;

namespace RuiSantos.Labs.Services.Commands.Doctors;

internal class GetDoctorsCommand(GetDoctorsCommandInput input
    , IDoctorRepository doctorRepository
    , ILogger<GetDoctorsCommand> logger
    ) : IAsyncCommand<GetDoctorsResponse>
{
    private int _take = input.Take ?? int.MaxValue;
    private string? _paginationToken;

    public async Task<GetDoctorsResponse> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Getting doctors with input {@Input}", input);
        try
        {
            await ValidateAndThrowAsync(cancellationToken);
            
            var pagination = await doctorRepository.FindAllAsync(_take, _paginationToken, cancellationToken);            
            return pagination.ToGetDoctorsResponse(EncodePaginationToken);
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
            .Must(token => TryDecodePaginationToken(token, out _paginationToken))
            .When(x => !string.IsNullOrWhiteSpace(x.PaginationToken))
            .WithMessage("Invalid pagination token format.");

        await validator.ValidateAndThrowAsync(input, cancellationToken: cancellationToken);
    }

    static string EncodePaginationToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return string.Empty;

        return Convert.ToBase64String(Encoding.UTF8.GetBytes(token));
    }        
        
    static bool TryDecodePaginationToken(string? token, out string decodedToken)
    {
        try
        {
            decodedToken = Encoding.UTF8.GetString(Convert.FromBase64String(token ?? ""));
            return true;
        }   
        catch (Exception)
        {
            decodedToken = "";
            return false;
        }   
    }
}

internal class GetDoctorsCommandInput
{
    public int? Take { get; init; }
    public string? PaginationToken { get; init; }
}