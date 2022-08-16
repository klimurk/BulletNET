using BulletNET.ViewModels.SubView;
using BulletNET.ViewModels.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace BulletNET.ViewModels
{
    internal static class ViewModelRegistrator
    {
        public static IServiceCollection RegisterViewModels(this IServiceCollection services) => services
            .AddSingleton<MainWindowViewModel>()
            .AddTransient<AdminViewModel>()
            .AddTransient<HistoryViewModel>()
            .AddSingleton<DashBoardViewModel>()
            .AddTransient<NewUserWindowViewModel>()
            .AddTransient<ChangeUserPasswordWindowViewModel>()
            .AddTransient<LoginWindowViewModel>()
        ;
    }
}