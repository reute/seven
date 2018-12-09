using SevenPrism.ViewModels;
using System.Windows;

namespace SevenPrism.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Closing += (DataContext as MainWindowViewModel).OnClosing;
        }      
    }
}
