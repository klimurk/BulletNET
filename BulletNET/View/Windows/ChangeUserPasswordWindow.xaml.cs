using BulletNET.ViewModels.Windows;
using MaterialDesignExtensions.Controls;

namespace BulletNET.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewUserWindow.xaml
    /// </summary>
    public partial class ChangeUserPasswordWindow : MaterialWindow
    {
        public ChangeUserPasswordWindow()
        {
            InitializeComponent();
            DataContext = App.Services.GetService(typeof(ChangeUserPasswordWindowViewModel)) as ChangeUserPasswordWindowViewModel;
        }
    }
}