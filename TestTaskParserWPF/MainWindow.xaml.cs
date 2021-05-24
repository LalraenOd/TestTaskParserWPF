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
using System.IO;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Data.SqlClient;

namespace TestTaskParserWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            SQLConnectionString.Text = $"Server=localhost\\SQLEXPRESS;Database=CarDetails;Trusted_Connection=True;";
            Link.Text = "https://www.ilcats.ru/";
            Log.AppendText("Waiting...");
        }

        /// <summary>
        /// Checking DB connection availability
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void ButtonCheckBD_Click(object sender, RoutedEventArgs e)
        {
            SQLConnectionString.IsEnabled = false;
            using (SqlConnection sqlConnection = new SqlConnection(SQLConnectionString.Text))
            {
                try
                {
                    await sqlConnection.OpenAsync();
                    MsgLogger("Подключение открыто");
                    // Проверяем доступность базы данных
                    MsgLogger("Свойства подключения:");
                    MsgLogger($"Строка подключения: {sqlConnection.ConnectionString}");
                    MsgLogger($"База данных: {sqlConnection.Database}");
                    MsgLogger($"Сервер: {sqlConnection.DataSource}");
                    MsgLogger($"Версия сервера: {sqlConnection.ServerVersion}");
                    MsgLogger($"Состояние: {sqlConnection.State}");
                    MsgLogger($"Workstationld: {sqlConnection.WorkstationId}");
                    CheckBoxDBState.IsChecked = true;
                    sqlConnection.Close();
                }
                catch (Microsoft.Data.SqlClient.SqlException ex)
                {
                    MessageBox.Show($"Exception handled {ex}");
                    MsgLogger(ex.ToString());
                    MessageBox.Show("Error in connection string.\nPlease, check it and try again");
                    CheckBoxDBState.IsChecked = false;
                }
            }
        }

        private async void ButtonStart_Click(object sender, RoutedEventArgs e)
        {
            Link.IsEnabled = false;
            MsgLogger("Staring process...");
            await URLParser(SQLConnectionString.Text, Link.Text);
        }

        private async Task URLParser(string DBConnectionString, string link)
        {
            MsgLogger($"Parsing {link}");
            using (SqlConnection sqlConnection = new SqlConnection(DBConnectionString))
            {
                await sqlConnection.OpenAsync();
            }
        }

        private void MsgLogger(string LogMsg)
        {
            Log.AppendText("\n" + LogMsg);
            File.AppendAllText("log.txt", "\n" + LogMsg);
        }
    }
}
