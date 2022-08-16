using BulletNET.ViewModels.SubView;
using BulletNET.ViewModels.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace BulletNET.ViewModels
{
    internal class ViewModelLocator
    {
        public MainWindowViewModel MainWindowModel => App.Host.Services.GetRequiredService<MainWindowViewModel>();
        public HistoryViewModel HistoryModel => App.Host.Services.GetRequiredService<HistoryViewModel>();
        public DashBoardViewModel DashBoardModel => App.Host.Services.GetRequiredService<DashBoardViewModel>();
        public AdminViewModel AdminModel => App.Host.Services.GetRequiredService<AdminViewModel>();
        public NewUserWindowViewModel NewUserWindowModel => App.Host.Services.GetRequiredService<NewUserWindowViewModel>();
        public ChangeUserPasswordWindowViewModel ChangeUserPasswordWindowModel => App.Host.Services.GetRequiredService<ChangeUserPasswordWindowViewModel>();
        public LoginWindowViewModel LoginWindowModel => App.Host.Services.GetRequiredService<LoginWindowViewModel>();
    }
}