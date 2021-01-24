using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;
using Bejko_SocketAsyncLib;

namespace Bejko_AsyncTimeServer
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        AsyncSocketServer mServer;
        bool flag = false;

        public MainWindow()
        {
            InitializeComponent();
            mServer = new AsyncSocketServer();
            Lbl_msg.Foreground = Brushes.Red;
            btn_invia.Background = Brushes.Red;

            //  DispatcherTimer setup

            DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
                dispatcherTimer.Tick += new EventHandler(Logo);
                dispatcherTimer.Interval = new TimeSpan(0, 0, 0);
                dispatcherTimer.Start();
        }
        private void Logo(object sender, EventArgs e)
        {
            DateTime thisDay = DateTime.Today;
            lbl_real_time.Content = thisDay.ToString("g");
        }

        private void btn_ascolta_Click(object sender, RoutedEventArgs e)
        {
            if (flag == false)
            {
                Lbl_msg.Content = "Server Avviato ";
                Lbl_msg.Foreground = Brushes.Green;
                lbl_title.Foreground = Brushes.Green;
                btn_ascolta.Background = Brushes.Green;
                btn_kick.Background = Brushes.Red;
                btn_invia.Background = Brushes.Green;
                mServer.In_Ascolto();
                flag = true;

            }



        }

        private void btn_kick_Click(object sender, RoutedEventArgs e)
        {
            if (flag == true)
            {
                mServer.Dissconetti();
                flag = false;
            }
            else
            {
                MessageBox.Show("Server non avviato");
            }
           
           
        }

        private void btn_invia_Click(object sender, RoutedEventArgs e)
        {
            if (flag == true)
            {
                mServer.InviaATuttiClient(txt_messaggio.Text);
            }
            else
            {
                MessageBox.Show("Server non avviato");
            }
        }
    }
}
