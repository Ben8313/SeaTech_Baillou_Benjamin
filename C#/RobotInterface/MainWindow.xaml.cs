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
        //string receivedText; 
        DispatcherTimer timerAffichage;
        Robot robot = new Robot();

        public MainWindow()
        {
            timerAffichage = new DispatcherTimer();
            timerAffichage.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timerAffichage.Tick += TimerAffichage_Tick;
            timerAffichage.Start();

            //Queue<byte> byteListReceived = new Queue<byte>();
            serialPort1 = new ExtendedSerialPort("COM3", 115200, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Open();
            InitializeComponent();


        }

        private void TimerAffichage_Tick(object? sender, EventArgs e)
        {
            StringBuilder hexBuuilder = new StringBuilder();
            //textBoxReception.Text += robot.receivedText;
            //robot.receivedText = string.Empty;
            while (robot.byteListReceived.Count>0)
            {
                byte b=robot.byteListReceived.Dequeue();
                hexBuuilder.AppendLine($"ToString(): {b.ToString()}");
                hexBuuilder.AppendLine($"ToString(\"X\"): {b.ToString("X")}");
                hexBuuilder.AppendLine($"ToString(\"X2\"): {b.ToString("X2")}");
                hexBuuilder.AppendLine($"ToString(\"X4\"): {b.ToString("X4")}");
                hexBuuilder.AppendLine();
            }
            textBoxReception.Text += hexBuuilder.ToString();
        }

        public void SerialPort1_DataReceived(object sender, DataReceivedArgs e)
        {
            foreach (byte b in e.Data)
            {


                //robot.receivedText += Encoding.UTF8.GetString(e.Data, 0, e.Data.Length);
                robot.byteListReceived.Enqueue(b);
            }
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            byte[] byteList = new byte[20];
            for (int i = 0; i < 20; i++)
            {
                byteList[i] = (byte)(2 * i);
            }
            Console.WriteLine("Données envoyées (hexa)");
            foreach (byte b in byteList)
            {
                Console.WriteLine($"{b:X2}");

            }
            Console.WriteLine();
            serialPort1.Write(byteList, 0, byteList.Length);
            Console.WriteLine("Données envoyées");
        }
    }
}