using System.Windows;
using System.Windows.Controls;

namespace GestaoEventosCorporativos.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            MainContent.Content = new Views.LoginView(this);

            WindowState = WindowState.Maximized;
            WindowStyle = WindowStyle.SingleBorderWindow;
            ResizeMode = ResizeMode.CanResize; 
        }

        public void Navigate(UserControl nextPage)
        {
            MainContent.Content = nextPage;
        }
    }
}
