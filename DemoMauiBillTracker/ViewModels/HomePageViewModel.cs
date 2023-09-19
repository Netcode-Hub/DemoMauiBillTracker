using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DemoMauiBillTracker.Models;
using DemoMauiBillTracker.Services;
using MvvmHelpers;
using System.Collections.ObjectModel;
using System.Globalization;

namespace DemoMauiBillTracker.ViewModels
{
    public partial class HomePageViewModel : BillBaseViewModel
    {
        private readonly IBillService billService;
        public HomePageViewModel(IBillService billService)
        {
            this.billService = billService;
            Title = "Due Bills & History";

        }

        [ObservableProperty]
        private decimal totalBill;

        //Get all Due Bills, Histories and Default Bills
        public ObservableCollection<DueBill> DueBills { get; set; } = new();
        public ObservableRangeCollection<Bill> Bills { get; set; } = new();
        public ObservableCollection<BillHistory> BillsHistory { get; set; } = new();

        [RelayCommand]
        private async Task LoadData()
        {
            try
            {
                // due bills
                var dueBills = await billService.GetDueBillsAsync();
                DueBills?.Clear();

                if (dueBills is not null)
                {
                    foreach (var due in dueBills.OrderBy(_ => _.BillType).ToList())
                    {
                        int monthNumber = due.DueDate.Month;
                        DueBills.Add(new DueBill
                        {
                            Id = due.Id,
                            Name = due.Name,
                            Amount = due.Amount,
                            BillId = due.BillId,
                            DueDateName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber - 1),
                            DueDate = due.DueDate,
                            Paid = due.Paid,
                            BillType = due.BillType
                        });
                    }
                    TotalBill = DueBills.Sum(_ => _.Amount);
                }


                // bills
                var bills = await billService.GetBillsAsync();
                Bills?.Clear();
                if (bills is not null)
                    Bills.AddRange(bills.ToList());

                // bills History
                var billsHistory = await billService.GetBillsHistoryAsync();
                BillsHistory?.Clear();

                //loop through so we can get the month name and due month.
                if (billsHistory is not null)
                    foreach (var bill in billsHistory.OrderBy(_ => _.BillType).ToList())
                    {
                        int monthNumber = bill.DueDate.Month;
                        BillsHistory.Add(new BillHistory
                        {
                            Id = bill.Id,
                            Name = bill.Name,
                            Amount = bill.Amount,
                            BillId = bill.BillId,
                            DueDateName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(monthNumber - 1),
                            DueDate = bill.DueDate,
                            Paid = bill.Paid,
                            BillType = bill.BillType
                        });
                    }
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Alert", ex.Message, "Ok");
            }

        }

        [RelayCommand]
        private async Task PayBill(int id)
        {
            bool result = await billService.PayBillAsync(id);
            if (result)
                await LoadData();
        }
    }
}
