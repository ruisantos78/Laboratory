using System.Linq.Expressions;

namespace RuiSantos.ZocDoc.Core.Data;

/// <summary>
/// Defines the contract for a data context.
/// </summary>
public interface IDataContext
{
    /// <summary>
    /// Checks if a model exists.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns>True if the model exists, false otherwise.</returns>
    Task<bool> ExistsAsync<TModel>(Expression<Func<TModel, bool>> expression);

    /// <summary>
    /// Finds a model.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="id">The model id.</param>
    /// <returns>The model.</returns>
    Task<TModel?> FindAsync<TModel>(Guid id);

    /// <summary>
    /// Finds a model.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns>The model.</returns>
    Task<TModel?> FindAsync<TModel>(Expression<Func<TModel, bool>> expression);

    /// <summary>
    /// Gets all the models.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <returns>The models.</returns>
    Task<List<TModel>> ToListAsync<TModel>();

    /// <summary>
    /// Queries the data.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="expression">The expression.</param>
    /// <returns>The models.</returns>
    Task<List<TModel>> QueryAsync<TModel>(Expression<Func<TModel, bool>> expression);

    /// <summary>
    /// Removes a model.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="id">The model id.</param>
    Task RemoveAsync<TModel>(Guid id);

    /// <summary>
    /// Stores a model.
    /// </summary>
    /// <typeparam name="TModel">The model type.</typeparam>
    /// <param name="model">The model.</param>
    Task StoreAsync<TModel>(TModel model);
}

