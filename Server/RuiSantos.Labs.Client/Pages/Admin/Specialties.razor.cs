using Blazing.Mvvm.Components;
using Blazorise.DataGrid;
using RuiSantos.Labs.Client.ViewModels;

namespace RuiSantos.Labs.Client.Pages.Admin
{
    public partial class Specialties : MvvmComponentBase<MedicalSpecialtiesViewModel>
    {
        public bool NoSelectable(RowSelectableEventArgs<String> e) => false;
    }
}