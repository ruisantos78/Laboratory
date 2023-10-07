using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using Blazing.Mvvm.ComponentModel;
using Blazorise.LoadingIndicator;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using RuiSantos.Labs.Client.Core;
using RuiSantos.Labs.Client.Services;

namespace RuiSantos.Labs.Client.ViewModels;

public partial class DoctorViewModel : ValidatorViewModelBase
{
    private readonly IDoctorService _doctorService;
    private readonly IMedicalSpecialtiesService _medicalSpecialtiesService;
    private readonly NavigationManager _navigationManager;
    private readonly ILoadingIndicatorService _loadingIndicatorService;
    private readonly ILogger<DoctorViewModel> _logger;

	[Required]
	[ObservableProperty] string _license = string.Empty;

	[Required]
	[ObservableProperty] string _firstName = string.Empty;

	[Required]
	[ObservableProperty] string _lastName = string.Empty;

	[Required, EmailAddress]
	[ObservableProperty] string _email = string.Empty;

	[ObservableProperty] List<string> _specialties = new();
	[ObservableProperty] List<string> _contacts = new();
	
	[ObservableProperty] string _contactSelected = string.Empty;
	[ObservableProperty] string _specialtySelected = string.Empty;

	[ObservableProperty] bool _visible;
	[ObservableProperty] bool _editing;

	public ObservableCollection<string> SpecialtiesOptions { get; } = new();

    public DoctorViewModel(
		IDoctorService doctorService,
		IMedicalSpecialtiesService medicalSpecialtiesService,
		NavigationManager navigationManager,
		ILoadingIndicatorService loadingIndicatorService,
		ILogger<DoctorViewModel> logger
	)
	{
        _doctorService = doctorService;
        _medicalSpecialtiesService = medicalSpecialtiesService;
        _navigationManager = navigationManager;
        _loadingIndicatorService = loadingIndicatorService;
        _logger = logger;
    }

	[RelayCommand]
	public Task AddContact()
	{
		if (string.IsNullOrWhiteSpace(ContactSelected))
			return Task.CompletedTask;

		Contacts.Insert(0, ContactSelected.Trim());
		ContactSelected = string.Empty;

		NotifyStateChanged();

		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task RemoveContact(string contact)
	{
		Contacts.Remove(contact);
		NotifyStateChanged();

		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task AddSpecialty()
	{
		if (string.IsNullOrWhiteSpace(SpecialtySelected))
			return Task.CompletedTask;

		Specialties.Insert(0, SpecialtySelected.Trim());
		SpecialtiesOptions.Remove(SpecialtySelected);
		SpecialtySelected = string.Empty;

		NotifyStateChanged();

		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task RemoveSpecialty(string specialty)
	{
		Specialties.Remove(specialty);
		SpecialtiesOptions.Add(specialty);
		SpecialtiesOptions.RemoveAt(SpecialtiesOptions.IndexOf(specialty));

		NotifyStateChanged();

		return Task.CompletedTask;
	}

	[RelayCommand]
	public async Task Store(Func<Task<bool>> validations)
	{
		await _loadingIndicatorService.Show();
		try
		{
			FillRemainListItems();

			if (await validations() is false)
				return;

			await _doctorService.SetDoctorAsync(
				license: License,
				firstName: FirstName,
				lastName: LastName,
				email: Email,
				contacts: Contacts,
				specialties: Specialties
			);
		}
		catch(Exception ex)
		{
			_logger.LogCritical(ex, "Failure to store the doctor!");
			return;
		}
		finally
		{
			await _loadingIndicatorService.Hide();
		}

        _navigationManager.NavigateTo("/doctors");
    }

	public async Task InitializeAsync(string? license)
	{
		await _loadingIndicatorService.Show();
		try
		{
			await _medicalSpecialtiesService.GetMedicalSpecialtiesAsync()
				.ContinueWith(task => task.Result.ForEach(SpecialtiesOptions.Add));

			if (!string.IsNullOrWhiteSpace(license) && await TryGetDoctorAsync(license) is false)
				return;

			Visible = true;
		}
		finally
		{
			await _loadingIndicatorService.Hide();
		}
	}

	private async Task<bool> TryGetDoctorAsync(string license)
	{
		var response = await _doctorService.GetDoctorByLicenseAsync(license);
		if (response is not {} doctor)
		{
            _navigationManager.NavigateTo("/doctors");
            return false;
        }
			
		Editing = true;
		License = doctor.License;
		FirstName = doctor.FirstName;
		LastName = doctor.LastName;
		Email = doctor.Email;
		Contacts = doctor.Contacts.ToList();
		Specialties = doctor.Specialties.ToList();
		Specialties.ForEach(x => SpecialtiesOptions.Remove(x));

		return true;
	}

	private void FillRemainListItems()
	{
		if (!string.IsNullOrWhiteSpace(ContactSelected))
		{
			Contacts.Add(ContactSelected.Trim());
			ContactSelected = string.Empty;
		}

		if (!string.IsNullOrWhiteSpace(SpecialtySelected))
		{
			Specialties.Add(SpecialtySelected.Trim());
			SpecialtiesOptions.Remove(SpecialtySelected);
			SpecialtySelected = string.Empty;
		}
	}
}