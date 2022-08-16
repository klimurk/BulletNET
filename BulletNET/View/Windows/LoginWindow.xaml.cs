using BulletNET.ViewModels.Windows;
using MaterialDesignExtensions.Controls;

namespace BulletNET.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : MaterialWindow
    {
        public LoginWindow()
        {
            InitializeComponent();

            DataContext = App.Services.GetService(typeof(LoginWindowViewModel)) as LoginWindowViewModel;
        }
    }
}