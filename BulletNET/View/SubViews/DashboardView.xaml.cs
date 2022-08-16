using System.Windows.Controls;

namespace BulletNET.View.SubViews
{
    /// <summary>
    /// Логика взаимодействия для Dashboard.xaml
    /// </summary>
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            //DataContext = App.Services.GetService(typeof(DashBoardViewModel)) as DashBoardViewModel;
        }
    }
}