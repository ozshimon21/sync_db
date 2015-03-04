using System.Windows;
using Client.ViewModels;

namespace Client.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel MainViewModel { get; set; }

        public MainWindow()
        {
            InitializeComponent();

            MainViewModel = new MainViewModel();
            DataContext = MainViewModel;
        }

        private void MenuItem_OnClick(object sender, RoutedEventArgs e)
        {


        }
    }
}
