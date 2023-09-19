using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemoMauiBillTracker.Models;
using DemoMauiBillTracker.Services;
using DemoMauiBillTracker.Views;
using System.Collections.ObjectModel;

namespace DemoMauiBillTracker.ViewModels
{
    [QueryProperty(nameof(BillObject), "SelectedBill")]
    public partial class UpdateBillPageViewModel : BillBaseViewModel
    {
        private readonly IBillService billService;
        public UpdateBillPageViewModel(IBillService billService)
        {
            this.billService = billService;
            //BillObject = new Bill();
            Title = "Add Bill Data";
        }

        [ObservableProperty]
        private Bill billObject;

        //Get selected date from the incoming bill object as Query Parameter
        partial void OnBillObjectChanged(Bill value)
        {
            SelectedDate = value.StartingDate;
        }

        //Calendar date selection changed
        [ObservableProperty]
        private DateTime? selectedDate;
        partial void OnSelectedDateChanged(DateTime? value)
        {
            if (value is null) return;
            BillObject.StartingDate = (DateTime)value;
        }

        //Get selected Bill Period by querying the name to retrieve its ID from the DB.
        [ObservableProperty]
        private BillPeriod selectedPeriod;
        partial void OnSelectedPeriodChanged(BillPeriod value)
        {
            if (value is null) return;
            BillObject.BillPeriodId = value.Id;
        }

        public ObservableCollection<BillPeriod> BillPeriods { get; set; } = new();
        [RelayCommand]
        private async Task LoadPeriods()
        {
            var result = await billService.GetPeriodsAsync();
            BillPeriods?.Clear();

            if (result is not null)
            {
                foreach (var item in result.OrderByDescending(_ => _.Id))
                    BillPeriods.Add(item);
            }

        }

        [RelayCommand]
        private async Task GotoBillsPage()
        {
            await Shell.Current.GoToAsync(nameof(BillPage));
        }

        [RelayCommand]
        private async Task SaveObject()
        {
            if (BillObject.StartingDate is null || BillObject.BillPeriodId <= 0) return;
            var (code, message) = await billService.UpdateBillAsync(BillObject);
            if (code == 200)
                await Shell.Current.GoToAsync($"//{nameof(BillPage)}");
            else
                await Shell.Current.DisplayAlert("Alert", message, "Ok"); return;

        }
    }
}
