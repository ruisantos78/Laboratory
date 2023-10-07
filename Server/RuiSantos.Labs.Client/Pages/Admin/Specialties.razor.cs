using Blazorise.DataGrid;
using Microsoft.AspNetCore.Components.Web;

namespace RuiSantos.Labs.Client.Pages.Admin;

partial class Specialties
{
    public bool NoSelectable(RowSelectableEventArgs<String> e) => false;

    public Task OnInputSpecialtyKeyPress(KeyboardEventArgs e)
    {
        if (e.Key is "Enter" && !string.IsNullOrWhiteSpace(ViewModel?.InputSpecialty))
            ViewModel.SetInputSpecialty(ViewModel.InputSpecialty);

        return Task.CompletedTask;
    }
}
