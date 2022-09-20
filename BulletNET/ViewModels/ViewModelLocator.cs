using BulletNET.ViewModels.SubView;
using BulletNET.ViewModels.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace BulletNET.ViewModels
{
    internal class ViewModelLocator
    {
        public MainWindowViewModel MainWindowModel => App.Services.GetRequiredService<MainWindowViewModel>();
        public HistoryViewModel HistoryModel => App.Services.GetRequiredService<HistoryViewModel>();
        public DashBoardViewModel DashBoardModel => App.Services.GetRequiredService<DashBoardViewModel>();
        public AdminViewModel AdminModel => App.Services.GetRequiredService<AdminViewModel>();
        public NewUserWindowViewModel NewUserWindowModel => App.Services.GetRequiredService<NewUserWindowViewModel>();
        public ChangeUserPasswordWindowViewModel ChangeUserPasswordWindowModel => App.Services.GetRequiredService<ChangeUserPasswordWindowViewModel>();
        public LoginWindowViewModel LoginWindowModel => App.Services.GetRequiredService<LoginWindowViewModel>();
    }
}