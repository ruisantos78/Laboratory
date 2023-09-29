using Blazing.Mvvm.ComponentModel;
using Blazorise.LoadingIndicator;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.Components;
using StrawberryShake;

namespace RuiSantos.Labs.Client.ViewModels;

public partial class DoctorViewModel : ViewModelBase
{
	private readonly ILabsClient client;
    private readonly NavigationManager navigationManager;
    private readonly ILoadingIndicatorService loadingIndicatorService;
    private readonly ILogger<DoctorViewModel> logger;

    [ObservableProperty] public string _license = string.Empty;
	[ObservableProperty] public string _firstName = string.Empty;
	[ObservableProperty] public string _lastName = string.Empty;
	[ObservableProperty] public string _email = string.Empty;
	[ObservableProperty] public string _contact = string.Empty;
	[ObservableProperty] public string _specialty = string.Empty;
	[ObservableProperty] public List<string> _contacts = new();
	[ObservableProperty] public List<string> _specialties = new();
	[ObservableProperty] public List<string> _specialtiesOptions = new();

	public DoctorViewModel(
		ILabsClient client,
		NavigationManager navigationManager,
		ILoadingIndicatorService loadingIndicatorService,
		ILogger<DoctorViewModel> logger
	)
	{
		this.client = client;
        this.navigationManager = navigationManager;
        this.loadingIndicatorService = loadingIndicatorService;
        this.logger = logger;
    }

	[RelayCommand]
	public Task AddContact()
	{
		if (string.IsNullOrWhiteSpace(Contact))
			return Task.CompletedTask;

		Contacts.Insert(0, Contact.Trim());
		Contact = string.Empty;
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
		if (string.IsNullOrWhiteSpace(Specialty))
			return Task.CompletedTask;

		Specialties.Insert(0, Specialty.Trim());
		SpecialtiesOptions.Remove(Specialty);
		Specialty = string.Empty;

		NotifyStateChanged();

		return Task.CompletedTask;
	}

	[RelayCommand]
	public Task RemoveSpecialty(string specialty)
	{
		Specialties.Remove(specialty);
		SpecialtiesOptions.Add(specialty);
		SpecialtiesOptions = SpecialtiesOptions.OrderBy(x => x).ToList();
		NotifyStateChanged();

		return Task.CompletedTask;
	}

	[RelayCommand]
	public async Task Store()
	{
		await loadingIndicatorService.Show();
		try
		{
			if (!string.IsNullOrWhiteSpace(Contact)) {
				Contacts.Insert(0, Contact.Trim());
				Contact = string.Empty;
			}

			if (!string.IsNullOrWhiteSpace(Specialty)) {
				Specialties.Insert(0, Specialty.Trim());
				Specialty = string.Empty;
			}			

			var operationResult = await client.SetDoctor.ExecuteAsync(new SetDoctorInput() {
				Doctor = new() {
					License = License,
					FirstName = FirstName,
					LastName = LastName,
					Email = Email,
					Contacts = Contacts,
					Specialties = Specialties
				}
			});

			operationResult.EnsureNoErrors();			
		}
		catch(Exception ex)
		{
			logger.LogCritical(ex, "Failure to store the doctor!");
			return;
		}
		finally
		{
			await loadingIndicatorService.Hide();
		}

        navigationManager.NavigateTo("/doctors");
    }

	public override async Task Loaded()
	{
		await loadingIndicatorService.Show();
		try
		{
			var response = await client.GetMedicalSpecialties.ExecuteAsync();
			if (response?.Data?.Specialties is {} value)
				SpecialtiesOptions = value.Select(x => x.Description).ToList();
		}
		finally
		{
			await loadingIndicatorService.Hide();
		}
	}
}