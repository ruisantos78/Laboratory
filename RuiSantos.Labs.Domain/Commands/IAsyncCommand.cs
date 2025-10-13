namespace RuiSantos.Labs.Domain.Commands;

public interface IAsyncCommand
{
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}

public interface IAsyncCommand<TResult>
{
    Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default);
}