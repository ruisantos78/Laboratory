using Blazing.Mvvm.Components;
using Blazorise.DataGrid;
using RuiSantos.Labs.Client.ViewModels;

namespace RuiSantos.Labs.Client.Pages.Doctors {
    public partial class Index: MvvmComponentBase<DoctorsViewsModel>
    {
        public bool NoSelectable(RowSelectableEventArgs<DoctorModel> e) => false;

        public readonly VirtualizeOptions VirtualizeOptions = new()
        {
            DataGridHeight = "auto",
            DataGridMaxHeight = $"75vh"
        };
    }
}