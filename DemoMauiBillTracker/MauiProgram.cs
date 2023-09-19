using CommunityToolkit.Maui;
using DemoMauiBillTracker.Services;
using DemoMauiBillTracker.ViewModels;
using DemoMauiBillTracker.Views;
using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;

namespace DemoMauiBillTracker
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<IBillService, BillService>();
            builder.Services.AddSingleton<AddBillPeriodPageViewModel>();
            builder.Services.AddSingleton<AddBillPeriodPage>();

            builder.Services.AddSingleton<AddBillPageViewModel>();
            builder.Services.AddSingleton<AddBillPage>();

            builder.Services.AddSingleton<BillPageViewModel>();
            builder.Services.AddSingleton<BillPage>();

            builder.Services.AddSingleton<UpdateBillPageViewModel>();
            builder.Services.AddSingleton<UpdateBillPage>();

            builder.Services.AddSingleton<HomePageViewModel>();
            builder.Services.AddSingleton<HomePage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
