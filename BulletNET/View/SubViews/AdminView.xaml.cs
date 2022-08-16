using System.Windows.Controls;

namespace BulletNET.View.SubViews
{
    /// <summary>
    /// Логика взаимодействия для AdminView.xaml
    /// </summary>
    public partial class AdminView : UserControl
    {
        public AdminView()
        {
            InitializeComponent();
            //DataContext = App.Services.GetService(typeof(AdminViewModel)) as AdminViewModel;
        }
    }
}