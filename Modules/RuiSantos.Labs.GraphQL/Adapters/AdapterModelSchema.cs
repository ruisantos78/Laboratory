namespace RuiSantos.Labs.GraphQL.Adapters;

internal abstract class AdapterModelSchema<TModel, TSchema>
{
    protected abstract TSchema GetSchema(TModel model);
    protected abstract TModel GetModel(TSchema schema);
}