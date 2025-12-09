using Microsoft.Extensions.Logging;
using Target.Services;
using Target.ViewModels;
using Target.Views;

namespace Target
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    //Fonts:
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("NarkisBold.ttf", "NarkisBold");
                    fonts.AddFont("NarkisThin.ttf", "NarkisThin");
                });

#if DEBUG
            // --------------------- DEBUG & BUILD SERVICES ---------------------
            builder.Logging.AddDebug();

            // Services:
            builder.Services.AddSingleton<FirebaseService>();
            builder.Services.AddSingleton<UserService>();

            // ViewModels:
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<RegisterViewModel>();
            builder.Services.AddSingleton<HomeViewModel>();
            builder.Services.AddSingleton<InfoViewModel>();
            builder.Services.AddSingleton<CalendarViewModel>();

            // Views:
            builder.Services.AddSingleton<AppShell>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<LogIn>();
            builder.Services.AddTransient<Register>();
            builder.Services.AddTransient<HomePage>();
            builder.Services.AddTransient<Info>();
            builder.Services.AddTransient<InfoDetailPage>();
            builder.Services.AddTransient<Calendar>();

#endif

            return builder.Build();
        }
    }
}
