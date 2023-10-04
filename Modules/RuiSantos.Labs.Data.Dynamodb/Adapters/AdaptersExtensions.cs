namespace RuiSantos.Labs.Data.Dynamodb.Adapters;

internal static class AdaptersExtensions
{
    public static async IAsyncEnumerable<TModel> AsModelsAsyncEnumerable<TModel>(this Task<IEnumerable<Task<TModel>>> tasks)
    {
        var operations = (await tasks.ContinueWith(x => x.Result)).ToHashSet();
        while (operations.Count > 0)
        {
            var operation = await Task.WhenAny(operations);
            operations.Remove(operation);

            var result = await operation;
            if (result is not null)
                yield return result;
        }
    } 

    public static async IAsyncEnumerable<TModel> AsModelsAsyncEnumerable<TModel>(this IEnumerable<Task<TModel>> tasks)
    {   
        var operations = tasks.ToHashSet();
        while (operations.Count > 0)
        {
            var operation = await Task.WhenAny(operations);
            operations.Remove(operation);

            var result = await operation;
            if (result is not null)
                yield return result;
        }
    }     
}