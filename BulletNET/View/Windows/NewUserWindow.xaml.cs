using BulletNET.ViewModels.Windows;
using MaterialDesignExtensions.Controls;

namespace BulletNET.View.Windows
{
    /// <summary>
    /// Логика взаимодействия для NewUserWindow.xaml
    /// </summary>
    public partial class NewUserWindow : MaterialWindow
    {
        public NewUserWindow()
        {
            InitializeComponent();
            DataContext = App.Services.GetService(typeof(NewUserWindowViewModel)) as NewUserWindowViewModel;
        }
    }
}