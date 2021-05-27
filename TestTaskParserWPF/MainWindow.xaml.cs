using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TestTaskParserWPF.Core;

namespace TestTaskParserWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow AppWindow;
        private Thread Thread;

        public MainWindow()
        {
            InitializeComponent();
            AppWindow = this;
            OnStartUp();
        }

        /// <summary>
        /// Some startup things
        /// </summary>
        private void OnStartUp()
        {
            TextBoxSQLConnectionString.Text = @"Server=localhost\SQLEXPRESS; Database=CarParcing; Trusted_Connection=True;";
            TextBoxLink.Text = "https://www.ilcats.ru/toyota/?function=getModels&market=EU";
            Logger.LogMsg("Program started.\nPlease, check DB connection and site availability to start the process");
            DbWriter.dBConnectionString = TextBoxSQLConnectionString.Text;
            ButtonStart.IsEnabled = false;
            ButtonStop.IsEnabled = false;
            CheckBoxDBState.IsEnabled = false;
            CheckBoxSiteAval.IsEnabled = false;
            CheckBoxProxyAval.IsEnabled = false;
            Logger.LogMsg("Checking proxy list. It will take some time.");
            //Start proxy info updater
            Thread ThreadProxyInfo = new Thread(new ThreadStart(ProxyWorker.ProxyInfoUpdate));
            ThreadProxyInfo.IsBackground = true;
            ThreadProxyInfo.Start();
            //Start proxy getter
            Thread ThreadProxyFounder = new Thread(new ThreadStart(ProxyWorker.ProxyGetter));
            ThreadProxyFounder.IsBackground = true;
            ThreadProxyFounder.Start();
        }

        /// <summary>
        /// Checking DB connection availability
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCheckBD_Click(object sender, RoutedEventArgs e)
        {
            TextBoxSQLConnectionString.IsEnabled = false;
            if (Misc.CheckDbConnection(TextBoxSQLConnectionString.Text))
            {
                CheckBoxDBState.IsChecked = true;
            }
            else
            {
                MessageBox.Show("Error in connection string.\nPlease, check it and try again. Exception logged.");
                CheckBoxDBState.IsChecked = false;
            }
        }

        /// <summary>
        /// Checking webpage availablility
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCheckWebPage_Click(object sender, RoutedEventArgs e)
        {
            TextBoxLink.IsEnabled = false;
            if (Misc.CheckWebPageAvailability(TextBoxLink.Text))
            {
                CheckBoxSiteAval.IsChecked = true;
            }
            else
            {
                CheckBoxSiteAval.IsChecked = false;
                MessageBox.Show("Site is not avilable. Exception logged.");
            }
        }

        /// <summary>
        /// Checks if both checkboxes are checked and ready to start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxSiteAval_Checked(object sender, RoutedEventArgs e)
        {
            ButtonStart.IsEnabled = CheckBoxDBState.IsChecked == true && CheckBoxSiteAval.IsChecked == true && CheckBoxProxyAval.IsChecked == true;
            if (ButtonStart.IsEnabled)
                ButtonStart_Click(sender, e);
        }

        /// <summary>
        /// Checks if both checkboxes are checked and ready to start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxDBState_Checked(object sender, RoutedEventArgs e)
        {
            ButtonStart.IsEnabled = CheckBoxDBState.IsChecked == true && CheckBoxSiteAval.IsChecked == true && CheckBoxProxyAval.IsChecked == true;
            if (ButtonStart.IsEnabled)
                ButtonStart_Click(sender, e);
        }

        /// <summary>
        /// Starts parsing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            TextBoxSQLConnectionString.IsEnabled = false;
            TextBoxLink.IsEnabled = false;
            ButtonStop.IsEnabled = true;
            Logger.LogMsg("Staring process...");
            Thread = new Thread(new ThreadStart(WebPageWork.WebPageWorker));
            Thread.Start();
        }
        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            ButtonStart.IsEnabled = true;
            Thread.Abort();
            Logger.LogMsg("Proccess aborted");
        }

        private void CheckBoxProxyAval_Checked(object sender, RoutedEventArgs e)
        {
            ButtonStart.IsEnabled = CheckBoxDBState.IsChecked == true && CheckBoxSiteAval.IsChecked == true && CheckBoxProxyAval.IsChecked == true;
            if (ButtonStart.IsEnabled)
                ButtonStart_Click(sender, e);
        }

        private void CheckboxAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            ButtonCheckBD_Click(sender, e);
            ButtonCheckWebPage_Click(sender, e);
            if (ButtonStart.IsEnabled)
                ButtonStart_Click(sender, e);
        }
    }
}