using CommunityToolkit.Mvvm.ComponentModel;
namespace DemoMauiBillTracker.ViewModels
{
    public partial class BillBaseViewModel : ObservableObject
    {
        [ObservableProperty]
        private string title;
    }
}
