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
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Xml;




namespace RobotInterface
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

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


            serialPort1 = new ExtendedSerialPort("COM8", 115200, Parity.None, 8, StopBits.One);
            serialPort1.DataReceived += SerialPort1_DataReceived;
            serialPort1.Open();
            InitializeComponent();


        }

        public byte CalculateChecksum(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {

            byte checksum = 0;
            checksum ^= 0xFE;
            checksum ^= (byte)(msgFunction >> 8);
            checksum ^= (byte)(msgFunction >> 0);
            checksum ^= (byte)(msgPayloadLength >> 8);
            checksum ^= (byte)(msgPayloadLength >> 0);

            foreach (byte c in msgPayload)
            {
                checksum ^= c;
            }

            return checksum;
        }

        void UartEncodeAndSendMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            int pos = 0;
            byte checksum = CalculateChecksum(msgFunction, msgPayloadLength, msgPayload);
            byte[] trame = new byte[6 + msgPayload.Length];
            trame[pos++] = 0xFE;
            trame[pos++] = (byte)(msgFunction >> 8);
            trame[pos++] = (byte)(msgFunction >> 0);
            trame[pos++] = (byte)(msgPayloadLength >> 8);
            trame[pos++] = (byte)(msgPayloadLength >> 0);
            for (int i = 0; i < msgPayload.Length; i++)
            {
                trame[pos++] = msgPayload[i];
            }
            trame[pos++] = checksum;

            serialPort1.Write(trame, 0, trame.Length);
        }
        private void TimerAffichage_Tick(object? sender, EventArgs e)
        {
            StringBuilder hexBuuilder = new StringBuilder();
            //textBoxReception.Text += robot.receivedText;
            //robot.receivedText = string.Empty;
            while (robot.byteListReceived.Count > 0)
            {
                byte b = robot.byteListReceived.Dequeue();
                DecodeMessage(b);
                //textBoxReception.Text += "0x" + b.ToString("X2") + " ";
                //hexBuuilder.AppendLine($"ToString(): {b.ToString()}");
                //hexBuuilder.AppendLine($"ToString(\"X\"): {b.ToString("X")}");
                //hexBuuilder.AppendLine($"ToString(\"X2\"): {b.ToString("X2")}");
                //hexBuuilder.AppendLine($"ToString(\"X4\"): {b.ToString("X4")}");
                //hexBuuilder.AppendLine();
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
        //private void buttonEnvoyer_Click(object sender, RoutedEventArgs e)
        //{
        //    SendMessage();
        //    if (couleur == false)
        //    {
        //        buttonEnvoyer.Background = Brushes.RoyalBlue;

        //        couleur = true;
        //    }
        //    else
        //    {
        //        buttonEnvoyer.Background = Brushes.Beige;
        //        couleur = false;

        //    }
        //}
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            textBoxReception.Clear();
            if (couleur2 == false)
            {
                // CLear.Background = Brushes.RoyalBlue;

                couleur2 = true;
            }
            else
            {
                // CLear.Background = Brushes.Beige;
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

        //private void CLear_Click(object sender, RoutedEventArgs e)
        //{
        //    textBoxReception.Clear();
        //    if (couleur2 == false)
        //    {
        //        CLear.Background = Brushes.RoyalBlue;

        //        couleur2 = true;
        //    }
        //    else
        //    {
        //        CLear.Background = Brushes.Beige;
        //        couleur = false;

        //    }
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //byte[] byteList = new byte[20];
            //for (int i = 0; i < 20; i++)
            //{
            //    byteList[i] = (byte)(2 * i);
            //}
            //Console.WriteLine("Données envoyées (hexa)");
            //foreach (byte b in byteList)
            //{
            //    Console.WriteLine($"{b:X2}");

            //}
            //Console.WriteLine();
            //serialPort1.Write(byteList, 0, byteList.Length);
            //Console.WriteLine("Données envoyées");
            byte[] array = Encoding.ASCII.GetBytes("Bonjour");
            UartEncodeAndSendMessage(0x0080, array.Length, array);
        }

        public enum StateReception
        {
            Waiting,
            FunctionMSB, FunctionLSB,
            PayloadLengthMSB,
            PayloadLengthLSB,
            Payload,
            CheckSum
        }
        StateReception rcvState = StateReception.Waiting;
        int msgDecodedFunction = 0;
        int msgDecodedPayloadLength = 0;
        byte[] msgDecodedPayload;
        int msgDecodedPayloadIndex = 0;

        int calculatedChecksum = 0; // Variable pour stocker le checksum calculé
        int receivedChecksum = 0;  // Variable pour stocker le checksum reçu
        private void DecodeMessage(byte c)
        {
            switch (rcvState)
            {
                case StateReception.Waiting:
                    //a faire
                    if (c == 0xFE)
                        rcvState = StateReception.FunctionMSB;
                    break;
                case StateReception.FunctionMSB:
                    // a faire
                    msgDecodedFunction = c << 8;
                    rcvState = StateReception.FunctionLSB;
                    break;
                case StateReception.FunctionLSB:
                    // a faire
                    msgDecodedFunction |= c << 0;
                    rcvState = StateReception.PayloadLengthMSB;

                    break;
                case StateReception.PayloadLengthMSB:
                    // a faire
                    msgDecodedPayloadLength = c << 8;
                    rcvState = StateReception.PayloadLengthLSB;

                    break;
                case StateReception.PayloadLengthLSB:
                    // a faire
                    msgDecodedPayloadLength |= c << 0;
                    if (msgDecodedPayloadLength == 0)
                        rcvState = StateReception.CheckSum;
                    else
                    {
                        msgDecodedPayload = new byte[msgDecodedPayloadLength];
                        msgDecodedPayloadIndex = 0;
                        rcvState = StateReception.Payload;
                    }
                    break;
                case StateReception.Payload:
                    msgDecodedPayload[msgDecodedPayloadIndex++] = c;
                    if (msgDecodedPayloadIndex >= msgDecodedPayloadLength)
                        rcvState = StateReception.CheckSum;
                    //a faire
                    break;
                case StateReception.CheckSum:
                    // a faire
                    calculatedChecksum = CalculateChecksum(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                    receivedChecksum = c;
                    if (calculatedChecksum == receivedChecksum)
                    {
                        // Succès, on a un message valide.
                        textBoxReception.Text += "Cool";
                        ProcessDecodedMessage(msgDecodedFunction, msgDecodedPayloadLength, msgDecodedPayload);
                        rcvState = StateReception.Waiting; // Revenir à l'état initial pour un nouveau message.
                    }
                    else
                    {
                        // Le checksum ne correspond pas, traiter l'erreur.
                        textBoxReception.Text += "ca Marche pas ";
                        rcvState = StateReception.Waiting; // Revenir à l'état initial pour recommencer.
                    }
                    break;
                default:
                    rcvState = StateReception.Waiting;
                    break;
            }
        }
        void ProcessDecodedMessage(int msgFunction, int msgPayloadLength, byte[] msgPayload)
        {
            if (msgFunction == 0x0020 && msgPayload.Length == 2)
            {
                byte numled = msgDecodedPayload[0];
                bool ledState = msgDecodedPayload[1] == 1;
                switch (numled)
                {
                    case 0:
                        Led1.IsChecked = ledState;
                        break;
                    case 1:
                        Led2.IsChecked = ledState;
                        break;
                    case 2:
                        Led3.IsChecked = ledState;
                        break;
                    default:
                        textBoxReception.Text += "Led inconnue";
                        break;
                }
            }
            if (msgFunction == 0x0030 && msgPayload.Length == 2)
            {
                byte telemetredroit = msgDecodedPayload[2];
                byte telemetregauche = msgDecodedPayload[0];
                byte telemetrecentre = msgDecodedPayload[1];

                IRGauche.Text = msgDecodedPayload[0].ToString;

            }


        }


    }
}