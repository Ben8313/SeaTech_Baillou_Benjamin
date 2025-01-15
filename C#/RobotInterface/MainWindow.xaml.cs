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
using System.IO.Ports;
using System.Windows.Threading;



namespace RobotInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        Boolean couleur = false;
        Boolean couleur2 = false;
        ExtendedSerialPort serialPort1;
        string receivedText; 
        DispatcherTimer timerAffichage;





        public MainWindow()
        {
            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();
            serialPort1 = new ExtendedSerialPort("COM3", 115200, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Open();
            InitializeComponent();
            

        }

        private void TimerAffichage_Tick(object? sender, EventArgs e)
        {

            textBoxReception.Text += receivedText;
            receivedText=string.Empty;
        }

        public void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
           
        }

        private void SendMessage()
        {
            serialPort1.WriteLine("Recu : " + TextBoxEmission.Text);
            //textBoxReception.Text += "Recu : " + TextBoxEmission.Text;
            //TextBoxEmission.Clear();



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

        private void CLear_Click(object sender, RoutedEventArgs e)
        {
            textBoxReception.Clear();
            if (couleur2 == false)
            {
                CLear.Background = Brushes.RoyalBlue;

                couleur2 = true;
            }
            else
            {
                CLear.Background = Brushes.Beige;
                couleur = false;

            }
        }
    }
}