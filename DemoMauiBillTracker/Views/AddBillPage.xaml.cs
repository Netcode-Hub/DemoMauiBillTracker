using DemoMauiBillTracker.ViewModels;
namespace DemoMauiBillTracker.Views;
public partial class AddBillPage : ContentPage
{
    private readonly AddBillPageViewModel addBillPageViewModel;

    public AddBillPage(AddBillPageViewModel addBillPageViewModel)
    {
        InitializeComponent();
        BindingContext = addBillPageViewModel;
        this.addBillPageViewModel = addBillPageViewModel;
    }

    protected override void OnAppearing()
    {
        addBillPageViewModel.LoadPeriodsCommand.Execute(this);
    }
}