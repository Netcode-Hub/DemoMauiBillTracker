using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemoMauiBillTracker.Models;
using DemoMauiBillTracker.Services;
using MvvmHelpers;

namespace DemoMauiBillTracker.ViewModels
{
    public partial class AddBillPeriodPageViewModel : BillBaseViewModel
    {
        private readonly IBillService billService;
        public AddBillPeriodPageViewModel(IBillService billService)
        {
            this.billService = billService;
            Title = "Add Bill Period";
        }

        [ObservableProperty]
        private string headerTitle;

        [ObservableProperty]
        private bool showPopup;

        [ObservableProperty]
        private BillPeriod billPeriodObject;

        public ObservableRangeCollection<BillPeriod> BillPeriods { get; set; } = new();

        [RelayCommand]
        private void ShowDialog()
        {
            HeaderTitle = "Add Bill Period";
            BillPeriodObject = new BillPeriod();
            ShowPopup = true;
        }

        [RelayCommand]
        private async Task SaveObject()
        {
            if (BillPeriodObject is null) return;

            await billService.AddPeriodAsync(BillPeriodObject);
            await LoadPeriods();
            ShowPopup = false;
        }

        [RelayCommand]
        private async Task LoadPeriods()
        {
            var result = await billService.GetPeriodsAsync();
            BillPeriods?.Clear();

            if (result is not null)
                BillPeriods.AddRange(result);
        }
    }
}
