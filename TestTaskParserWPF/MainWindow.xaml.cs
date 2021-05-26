using Microsoft.Data.SqlClient;
using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Threading;
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
            TextBoxSQLConnectionString.Text = @"Server=localhost\SQLEXPRESS; Database=CarDetails; Trusted_Connection=True;";
            TextBoxLink.Text = "https://www.ilcats.ru/toyota/?function=getModels&market=EU";
            Logger.LogMsg("Program started.\nPlease, check DB connection and site availability to start the process");
            //ButtonStart.IsEnabled = false;
            ButtonStop.IsEnabled = false;
            CheckBoxDBState.IsEnabled = false;
            CheckBoxSiteAval.IsEnabled = false;
            Thread = new Thread(new ThreadStart(Misc.ThreadCounter));
            Thread.Start();
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
            ButtonStop.IsEnabled = true;
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
            ButtonStart.IsEnabled = true;
            Thread.Abort();
            Logger.LogMsg("Proccess aborted");
        }
    }
}