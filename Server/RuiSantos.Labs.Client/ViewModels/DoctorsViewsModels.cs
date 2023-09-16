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

	[ObservableProperty] int? _totalDoctors = null;
	[ObservableProperty] ObservableCollection<IGetDoctors_Doctors> _doctors = new();

	public DoctorsViewsModel(
		ILabsClient client,
		ILoadingIndicatorService loadingIndicatorService)
	{
		this.client = client;
		this.loadingIndicatorService = loadingIndicatorService;
	}

	public async Task OnReadData(DataGridReadDataEventArgs<IGetDoctors_Doctors> e)
	{
		await loadingIndicatorService.Show();
		try
		{
			if (e.CancellationToken.IsCancellationRequested)
				return;
			
				var counter = await client.CountDoctors.ExecuteAsync();
				TotalDoctors = (int?)counter?.Data?.CountDoctors;
				if (TotalDoctors == 0)
					return;

				var paginator = new PaginationInput
				{
					Take = e.VirtualizeCount,
					From = Doctors.LastOrDefault()?.License
				};

				var result = await client.GetDoctors.ExecuteAsync(paginator);
				Doctors = result?.Data?.Doctors?.Any() is true
					? new(result.Data.Doctors)
					: new();			
		}
		finally
		{
			await loadingIndicatorService.Hide();
		}
	}
}

