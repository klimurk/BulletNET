using System.Windows.Controls;

namespace BulletNET.View.SubViews
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class HistoryView : UserControl
    {
        public HistoryView()
        {
            InitializeComponent();
            //DataContext = App.Services.GetService(typeof(HistoryViewModel)) as HistoryViewModel;
        }
    }
}