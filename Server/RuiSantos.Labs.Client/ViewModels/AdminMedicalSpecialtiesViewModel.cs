using System.Collections.ObjectModel;
using Blazing.Mvvm.ComponentModel;
using Blazorise;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StrawberryShake;

namespace RuiSantos.Labs.Client.ViewModels;

[RegisterService]
public partial class AdminMedicalSpecialtiesViewModel : ViewModelBase
{
    private readonly ILabsClient client;

    [ObservableProperty] private LinkedList<string> _specialties = new();
    [ObservableProperty] private bool _modalVisible = false;
    [ObservableProperty] private string _specialty = string.Empty;

    public AdminMedicalSpecialtiesViewModel(ILabsClient client)
    {
        this.client = client;
    }    

    [RelayCommand] public void AddNew()
    {
        ModalVisible = true;
    }

    [RelayCommand] public void CloseModal()
    {
        ModalVisible = false;
    }

    [RelayCommand] public async Task Save()
    {        
        var response = await client.AddSpecialties.ExecuteAsync(new AddSpecialtiesInput()
        {
            Descriptions = new[] { Specialty }
        });

        if (response.IsSuccessResult() && !Specialties.Contains(Specialty))
        {                                        
            var index = Specialties.LastOrDefault(x => x.CompareTo(Specialty) < 0, string.Empty);
            if (Specialties.Find(index) is {} node)                
                Specialties.AddAfter(node, Specialty);
            else
                Specialties.AddFirst(Specialty);
            
            this.NotifyStateChanged();       
        }

        CloseModal();
    }

    [RelayCommand] public async Task Remove(string specialty)
    {
        var response = await client.RemoveSpecialties.ExecuteAsync(new RemoveSpecialtiesInput() 
        {
            Description = specialty
        });

        if (response.IsSuccessResult() && response.Data?.RemoveSpecialties?.Specialties?.Any() is true)
        {    
            response.Data.RemoveSpecialties.Specialties
                .ToList()
                .ForEach(x => Specialties.Remove(x.Description));     

            this.NotifyStateChanged();       
        }        
    }

    public Task OnModalClosing(ModalClosingEventArgs e)
    {
        Specialty = string.Empty;
        return Task.CompletedTask;
    }

    public override async Task Loaded()
    {
        Specialties.Clear();

        var response = await client.GetMedicalSpecialties.ExecuteAsync();
        if (response.IsSuccessResult() && response.Data?.Specialties?.Any() is true)
        {
            Specialties = new(response.Data.Specialties.Select(x => x.Description));
            
            this.NotifyStateChanged();       
        }
    }
}