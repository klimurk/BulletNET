using BulletNET.ViewModels.Windows;
using MaterialDesignExtensions.Controls;

namespace BulletNET.View.Windows;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : MaterialWindow
{
    //public const string DialogHostName = "dialogHost";
    //public DialogHost DialogHost => m_dialogHost;

    public MainWindow()
    {
        InitializeComponent();
        //DataContext = App.Services.GetService(typeof(MainWindowViewModel)) as MainWindowViewModel;
    }
}