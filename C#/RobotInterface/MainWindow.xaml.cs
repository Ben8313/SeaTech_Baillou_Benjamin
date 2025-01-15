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
using ExtendedSerialPort_NS;

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

        private void SendMessage()
        {
            textBoxReception.Text += "Recu : " + TextBoxEmission.Text;
            TextBoxEmission.Clear();



        }
        private void buttonEnvoyer_Click(object sender, RoutedEventArgs e)
        {
            SendMessage();
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

        private void TextBoxEmission_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SendMessage();
            }
        }
    }
}