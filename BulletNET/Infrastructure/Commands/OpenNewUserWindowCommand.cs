using BulletNET.Infrastructure.Commands.Base;
using BulletNET.Services.Managers;
using BulletNET.View.Windows;

namespace BulletNET.Infrastructure.Commands;

internal class OpenNewUserWindowCommand : Command
{
    private NewUserWindow _Window;
    private readonly IManagerUser _ManagerUser;

    protected override bool CanExecute(object parameter) => _Window == null && _ManagerUser.IsAdmin;

    protected override void Execute(object parameter)
    {
        var window = new NewUserWindow
        {
            Owner = Application.Current.MainWindow,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };
        _Window = window;
        window.Closed += OnWindowClosed;
        window.ShowDialog();
    }

    private void OnWindowClosed(object sender, EventArgs e)
    {
        ((Window)sender).Closed -= OnWindowClosed;
        _Window = null;
    }

    public OpenNewUserWindowCommand()
    {
        _ManagerUser = (IManagerUser)App.Host.Services.GetService(typeof(IManagerUser));
    }
}