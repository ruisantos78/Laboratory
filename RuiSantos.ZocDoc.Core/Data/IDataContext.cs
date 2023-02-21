using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Core.Data;

public interface IDataContext
{
    Task<bool> ExistsAsync<TModel>(Expression<Func<TModel, bool>> expression);
    Task<TModel?> FindAsync<TModel>(Guid id);
    Task<TModel?> FindAsync<TModel>(Expression<Func<TModel, bool>> expression);
    Task<List<TModel>> ToListAsync<TModel>();
    Task<List<TModel>> QueryAsync<TModel>(Expression<Func<TModel, bool>> expression);
    Task RemoveAsync<TModel>(Guid id);
    Task StoreAsync<TModel>(TModel model);
}

