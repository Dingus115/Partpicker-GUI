using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using RJCP.IO.Ports;


namespace WpfApp2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        SerialPortStream serialPort = new SerialPortStream();
        
        public MainWindow()
        {
            InitializeComponent();
            addPort();//get available ports
        }

        private void addPort()//gets available ports
        {
            string[] ports = serialPort.GetPortNames();
            foreach (string port in ports)
            {
                comboBox_Ports.Items.Add(port);
            }
        }

        private void comboBox_Ports_SelectionChanged(object sender, SelectionChangedEventArgs e) //opens selected port of combo box
        {
            string temp = comboBox_Ports.SelectedItem.ToString();//YOU MUST PULL STRING BEFORE ATTEMPTING NAMING PORT  
            try
            {
                serialPort.PortName = temp;
                serialPort.BaudRate = 115200;
                serialPort.DataBits = 8;
                serialPort.StopBits = StopBits.One;
                serialPort.Parity = Parity.None;
                serialPort.Open();
                serialPort.DataReceived += sp_DataReceived;//subscribe to event
                commWindow.Text = "Comm port successfully opened \n";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        private void sp_DataReceived(object sender, SerialDataReceivedEventArgs e) //subscription to recieved data
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                Thread.Sleep(500);
                commWindow.Text += serialPort.ReadExisting();
            }));//forces temporary other thread while reading from arduino
        }

        private void button_ManualCommand_Click(object sender, RoutedEventArgs e) //Sends a manual command to the device
        {
            string command = textbox_ManualCommand.Text;
            
            try
            {
                serialPort.WriteLine(command);
                commWindow.Text += "\n";
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            
        }
    }
}
