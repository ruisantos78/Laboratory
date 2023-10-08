using System.Collections.ObjectModel;
using Blazing.Mvvm.ComponentModel;
using Blazorise.DataGrid;
using Blazorise.LoadingIndicator;
using CommunityToolkit.Mvvm.ComponentModel;
using RuiSantos.Labs.Client.Core;
using RuiSantos.Labs.Client.Models;
using RuiSantos.Labs.Client.Services;

namespace RuiSantos.Labs.Client.ViewModels;

public partial class DoctorsViewsModel : ViewModelBase
{
    private readonly IDoctorService _doctorService;
    private readonly ILoadingIndicatorService _loadingIndicatorService;

    private string? _paginationToken;

    [ObservableProperty] ObservableCollection<DoctorModel> _doctors = new();

    public DoctorsViewsModel(
        IDoctorService doctorService,
        ILoadingIndicatorService loadingIndicatorService)
    {
        _doctorService = doctorService;
        _loadingIndicatorService = loadingIndicatorService;
    }

    public async Task ReadData(DataGridReadDataEventArgs<DoctorModel> e)
    {
        await _loadingIndicatorService.Show();
        try
        {
            var pagination = await _doctorService.GetDoctorsAsync(e.VirtualizeCount, _paginationToken);

            _paginationToken = pagination.PaginationToken;
            pagination.Models.ForEach(Doctors.Add);
        }
        finally
        {
            await _loadingIndicatorService.Hide();
        }
    }
}

