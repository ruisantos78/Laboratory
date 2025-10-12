using FluentValidation;

namespace RuiSantos.Labs.Domain.Extensions.Commands;

public interface IAsyncCommand
{
    Task Execute(CancellationToken cancellationToken = default);
}

public interface IAsyncCommand<TResult>
{
    Task<TResult> Execute(CancellationToken cancellationToken = default);
}