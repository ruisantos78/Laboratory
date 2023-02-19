using MongoDB.Driver;
using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Data.Mongodb.Core.Interfaces;

internal interface IEntity<TModel>
{
    Task StoreAsync(IMongoDatabase context, TModel model);

    Task RemoveAsync(IMongoDatabase context, Guid id);

    Task<List<TModel>> QueryAsync(IMongoDatabase context, Expression<Func<TModel, bool>> expression);

    Task<List<TModel>> ListAsync(IMongoDatabase context);

    Task<TModel> FindAsync(IMongoDatabase context, Guid id);

    Task<TModel> FindAsync(IMongoDatabase context, Expression<Func<TModel, bool>> expression);

    Task<bool> AnyAsync(IMongoDatabase context, Expression<Func<TModel, bool>> expression);
}

