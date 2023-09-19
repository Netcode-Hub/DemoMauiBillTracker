

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemoMauiBillTracker.Models;
using DemoMauiBillTracker.Services;
using DemoMauiBillTracker.Views;
using MvvmHelpers;

namespace DemoMauiBillTracker.ViewModels
{
    public partial class BillPageViewModel : BillBaseViewModel
    {
        private readonly IBillService billService;
        public BillPageViewModel(IBillService billService)
        {
            this.billService = billService;
            Title = "Manage Bills";
        }

        // Get bill selected and the manage it (deleting or updating)
        [ObservableProperty]
        private Bill selectedRowData;
        partial void OnSelectedRowDataChanged(Bill value)
        {
            if (value is null) return;
            ManageSelectedData(value);
        }
        private async Task ManageSelectedData(Bill selectedBill)
        {
            string action = await Shell.Current.DisplayActionSheet("Action: Choose an Option", "Cancel", null, "Edit", "Delete");
            if (string.IsNullOrEmpty(action) || string.IsNullOrWhiteSpace(action)) return;

            if (action.Equals("Cancel")) return;

            if (action.Equals("Edit"))
            {
                var navigationParameter = new Dictionary<string, object> { { "SelectedBill", selectedBill } };
                await Shell.Current.GoToAsync(nameof(UpdateBillPage), navigationParameter);
            }

            if (action.Equals("Delete"))
            {
                bool answer = await Shell.Current.DisplayAlert("Confirm Operation", "Are you sure you wanna do this?", "Yes", "No");
                if (!answer) return;

                var (code, message) = await billService.DeleteBillAsync(selectedBill);
                if (code == 200)
                    await LoadBills();
                await Shell.Current.DisplayAlert("Alert", message, "Ok");
                return;
            }
        }

        public ObservableRangeCollection<Bill> BillObjects { get; set; } = new();
        [RelayCommand]
        private async Task LoadBills()
        {
            var results = await billService.GetBillsAsync();
            BillObjects?.Clear();

            if (results is not null)
                BillObjects.AddRange(results.OrderByDescending(_ => _.Id).ToList());

        }

        [RelayCommand]
        private async Task GotoAddBill()
        {
            await Shell.Current.GoToAsync(nameof(AddBillPage));
        }
    }
}
