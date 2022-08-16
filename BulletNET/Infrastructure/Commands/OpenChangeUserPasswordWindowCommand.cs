using BulletNET.Infrastructure.Commands.Base;
using BulletNET.View.Windows;

namespace BulletNET.Infrastructure.Commands;

internal class OpenChangeUserPasswordWindowCommand : Command
{
    private ChangeUserPasswordWindow _Window;

    protected override bool CanExecute(object parameter) => _Window == null;

    protected override void Execute(object parameter)
    {
        var window = new ChangeUserPasswordWindow
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
}