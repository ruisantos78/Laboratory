using System.Collections.ObjectModel;
using Blazing.Mvvm.ComponentModel;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RuiSantos.Labs.Client.ViewModels;

public partial class DoctorsViewsModel : ViewModelBase
{
	private readonly ILabsClient client;
	private readonly ILoadingIndicatorService loadingIndicatorService;

	private string? paginationToken = null;

	[ObservableProperty] ObservableCollection<DoctorModel> _doctors = new();

	public DoctorsViewsModel(
		ILabsClient client,
		ILoadingIndicatorService loadingIndicatorService)
	{
		this.client = client;
		this.loadingIndicatorService = loadingIndicatorService;
	}

	public async Task ReadData(DataGridReadDataEventArgs<DoctorModel> e)
	{
		await loadingIndicatorService.Show();
		try
		{
			var result = await client.GetDoctors.ExecuteAsync(new()
			{
				Take = e.VirtualizeCount,
				Token = paginationToken
			});

			if (result?.Data?.Doctors is null)
				return;

			result.Data.Doctors.Doctors.Select(x => new DoctorModel(x))
				.ToList()
				.ForEach(Doctors.Add);

			paginationToken = result.Data.Doctors.PaginationToken;
		}
		finally
		{
			await loadingIndicatorService.Hide();
		}
	}
}

