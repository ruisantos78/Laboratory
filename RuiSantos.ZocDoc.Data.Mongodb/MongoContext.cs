using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Data;
using RuiSantos.ZocDoc.Data.Mongodb.Core;
using RuiSantos.ZocDoc.Data.Mongodb.Mappings;

namespace RuiSantos.ZocDoc.Data.Mongodb;

public class MongoContext: IDataContext
{
    private readonly MongoClient client;
    private readonly IMongoDatabase database;

    public MongoContext(MongoSettings settings)
    {
        this.client = new MongoClient(settings.ToMongoClientSettings());
        this.database = client.GetDatabase(settings.Schema);

        this.RegisterClassMaps();
    }

    private void RegisterClassMaps()
    {
        var mappings = this.GetType().Assembly.GetTypes()
            .Where(t => !t.IsInterface && t.IsAssignableFrom(typeof(IRegisterClassMap)));

        foreach (var mapping in mappings)
        {
            if (Activator.CreateInstance(mapping) is IRegisterClassMap instance)
                instance.Register();
        }
    }

    public async Task RemoveAsync<TModel>(string id)
    {
        if (Mediator.GetEntity<TModel>() is IEntity<TModel> entity)
            await entity.RemoveAsync(database, id);
    }

    public async Task StoreAsync<TModel>(TModel model)
    {
        if (Mediator.GetEntity<TModel>() is IEntity<TModel> entity)
            await entity.StoreAsync(database, model);
    }

    public IQueryable<TModel> Query<TModel>()
    {
        if (Mediator.GetEntity<TModel>() is IEntity<TModel> entity)
            return entity.Query(database);

        return Array.Empty<TModel>().AsQueryable();
    }

    public TModel? Find<TModel>(string id)
    {
        if (Mediator.GetEntity<TModel>() is IEntity<TModel> entity)
            return entity.Find(database, id);

        return default;
    }
}