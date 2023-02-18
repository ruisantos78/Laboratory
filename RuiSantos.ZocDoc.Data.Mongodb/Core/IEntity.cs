using MongoDB.Driver;
using RuiSantos.ZocDoc.Core.Models;
using RuiSantos.ZocDoc.Data.Mongodb.Entities;

namespace RuiSantos.ZocDoc.Data.Mongodb.Core;

internal interface IEntity<TModel>
{
    Task RemoveAsync(IMongoDatabase context, string id);
    Task StoreAsync(IMongoDatabase context, TModel model);
    IQueryable<TModel> Query(IMongoDatabase context);
    TModel Find(IMongoDatabase context, string id);
}

