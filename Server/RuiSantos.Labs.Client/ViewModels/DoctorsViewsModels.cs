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
				From = Doctors.LastOrDefault()?.License
			});

			result?.Data?.Doctors.Select(x => new DoctorModel(x))
				.ToList()
				.ForEach(Doctors.Add);
		}
		finally
		{
			await loadingIndicatorService.Hide();
		}
	}
}

