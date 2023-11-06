using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
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

namespace WPFAShopMgt23
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
        SqlConnection _connection;
        private void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            ProductWindow productWindow = new ProductWindow();
            productWindow.Show();
            Close();
        }

        private async void loadButton_Click(object sender, RoutedEventArgs e)
        {
            /*
            var config = new ConfigurationBuilder().AddUserSecrets<MainWindow>().Build();
            var connectionString = config.GetSection("DB")["ConnectionString"];
            */
            var connectionString = AppConfig.ConnectionString();
            Debug.WriteLine(connectionString);

            loadingProgress.IsIndeterminate = true;
            var (success, message, connection) = await Task.Run(() =>
            {
                var __connection = new SqlConnection(connectionString);
                bool success = true;
                string message = "";
                try
                {
                    //test khi chay  qua nhanh
                    System.Threading.Thread.Sleep(3000);
                    __connection.Open();

                }
                catch (Exception ex)
                {
                    success = false;
                    message = ex.Message;
                }
                return new Tuple<bool, string, SqlConnection>(success, message, __connection);
            });
            loadingProgress.IsIndeterminate = false;

            if (success)
            {
                _connection = connection;
                MessageBox.Show("Connect successfully");
            }
            else
            {
                MessageBox.Show($"Cannot connect.Reason {message}");
            }

        }
        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            string sql = "select * from Product";
            var command = new SqlCommand(sql, _connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                string ProductName = (string)reader[2];
                int Quantity = (int)reader[4];
                int year = (int)reader[4];
                Debug.WriteLine($"{ProductName} - qty: {Quantity}");
            }
        }

        private void OrderButton_Click(object sender, RoutedEventArgs e)
        {
            var orderScreen = new OrderAddWindow();
            orderScreen.Show();
            this.Close();
        }
    }
}
