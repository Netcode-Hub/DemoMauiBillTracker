using DemoMauiBillTracker.ViewModels;
namespace DemoMauiBillTracker.Views;
public partial class AddBillPeriodPage : ContentPage
{
    private readonly AddBillPeriodPageViewModel addBillPeriodPageViewModel;

    public AddBillPeriodPage(AddBillPeriodPageViewModel addBillPeriodPageViewModel)
    {
        InitializeComponent();
        BindingContext = addBillPeriodPageViewModel;
        this.addBillPeriodPageViewModel = addBillPeriodPageViewModel;
    }

    protected override void OnAppearing()
    {
        addBillPeriodPageViewModel.LoadPeriodsCommand.Execute(this);
    }
}