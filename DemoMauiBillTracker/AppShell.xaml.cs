using DemoMauiBillTracker.Views;

namespace DemoMauiBillTracker
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(BillPage), typeof(BillPage));
            Routing.RegisterRoute(nameof(AddBillPeriodPage), typeof(AddBillPeriodPage));
            Routing.RegisterRoute(nameof(AddBillPage), typeof(AddBillPage));
            Routing.RegisterRoute(nameof(UpdateBillPage), typeof(UpdateBillPage));
        }
    }
}
