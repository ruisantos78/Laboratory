namespace RuiSantos.ZocDoc.Core.Data
{
    public interface IDataContext
	{
        TModel? Find<TModel>(string id);
        IQueryable<TModel> Query<TModel>();       
        Task RemoveAsync<TModel>(string id);
        Task StoreAsync<TModel>(TModel model);
    }
}

