using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RobotInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        Boolean couleur = false;
        private void buttonEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            textBoxReception.Text += "Recu : "+TextBoxEmission.Text;
            TextBoxEmission.Clear();
            if (couleur == false)
            {
                buttonEnvoyer.Background = Brushes.RoyalBlue;
                
                couleur = true;
            }
            else
            {
                buttonEnvoyer.Background = Brushes.Beige;
                couleur = false;
            }
        }
    }
}