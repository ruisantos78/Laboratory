using System;
using MongoDB.Driver;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Core;

internal static class EntityExtensions
{
	private static void CopyFrom<TEntity, TModel>(this TEntity entity, TModel model)
		where TModel : class, new()
		where TEntity: TModel, IEntity<TModel>
    {
		var properties = model.GetType().GetProperties()
			.Where(p => p.CanRead && p.CanWrite);

		foreach (var property in properties)
			property.SetValue(entity, property.GetValue(model));
	}

    public static async Task StoreAsync<TEntity, TModel>(this TEntity entity, string discriminator, TModel model, IMongoDatabase context, FilterDefinition<TEntity> filter)
        where TModel : class, new()
        where TEntity : TModel, IEntity<TModel>
    {
        entity.CopyFrom(model);

        var collection = context.GetCollection<TEntity>(discriminator);
        await collection.FindOneAndDeleteAsync(filter);
        await collection.InsertOneAsync(entity);
    }
}

