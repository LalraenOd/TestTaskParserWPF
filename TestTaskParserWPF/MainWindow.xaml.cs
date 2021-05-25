using Microsoft.Data.SqlClient;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Threading;

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
            TextBoxSQLConnectionString.Text = @"Server=localhost\SQLEXPRESS; Database=CarDetails; Trusted_Connection=True;";
            TextBoxLink.Text = "https://www.ilcats.ru/toyota/?function=getModels&market=EU";
            Logger.LogMsg("Program started.\nPlease, check DB connection and site availability to start the process");
            //ButtonStart.IsEnabled = false;
            CheckBoxDBState.IsEnabled = false;
            CheckBoxSiteAval.IsEnabled = false;
        }

        /// <summary>
        /// Checking DB connection availability
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonCheckBD_Click(object sender, RoutedEventArgs e)
        {
            TextBoxSQLConnectionString.IsEnabled = false;
            using (SqlConnection sqlConnection = new SqlConnection(TextBoxSQLConnectionString.Text))
            {
                try
                {
                    sqlConnection.Open();
                    Logger.LogMsg($"DB connection opened.\n" +
                        $"Connection properties:\n" +
                        $"Connection string: {sqlConnection.ConnectionString}\n" +
                        $"DB: {sqlConnection.Database}\n" +
                        $"Server: { sqlConnection.DataSource}\n" +
                        $"State: {sqlConnection.State}");
                    CheckBoxDBState.IsChecked = true;
                    sqlConnection.Close();
                }
                catch (SqlException ex)
                {
                    MessageBox.Show($"Exception handled {ex}");
                    Logger.LogMsg(ex.ToString());
                    MessageBox.Show("Error in connection string.\nPlease, check it and try again. Exception logged.");
                    CheckBoxDBState.IsChecked = false;
                }
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
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(TextBoxLink.Text);
                request.Timeout = 3000;
                request.AllowAutoRedirect = false; // find out if this site is up and don't follow a redirector
                request.Method = "HEAD";
                using (var response = request.GetResponse())
                {
                    CheckBoxSiteAval.IsChecked = true;
                    Logger.LogMsg("Site is available.");
                }
            }
            catch (Exception ex)
            {
                CheckBoxSiteAval.IsChecked = false;
                MessageBox.Show("Site is not avilable. Exception logged.");
                Logger.LogMsg(ex.ToString());
            }
        }

        /// <summary>
        /// Checks if both checkboxes are checked and ready to start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxSiteAval_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBoxDBState.IsChecked == true && CheckBoxSiteAval.IsChecked == true)
                ButtonStart.IsEnabled = true;
            else
                ButtonStart.IsEnabled = false;
        }

        /// <summary>
        /// Checks if both checkboxes are checked and ready to start
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckBoxDBState_Checked(object sender, RoutedEventArgs e)
        {
            if (CheckBoxDBState.IsChecked == true && CheckBoxSiteAval.IsChecked == true)
                ButtonStart.IsEnabled = true;
            else
                ButtonStart.IsEnabled = false;
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
            Logger.LogMsg("Staring process...");
            Thread = new Thread(new ThreadStart(WebPageWork.WebPageWorker));
            Thread.Start();
            //WebPageWork.WebPageWorker(TextBoxSQLConnectionString.Text, TextBoxLink.Text);
        }

        private void RichTextBoxLog_TextChanged(object sender, TextChangedEventArgs e)
        {
            RichTextBoxLog.ScrollToEnd();
        }

        private void ButtonStop_Click(object sender, RoutedEventArgs e)
        {
            Thread.Abort();
            Logger.LogMsg("Proccess aborted");
        }
    }
}