using Microsoft.Extensions.Configuration;
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
using System.Windows.Shapes;

namespace WPFAShopMgt23
{
    /// <summary>
    /// Interaction logic for Config.xaml
    /// </summary>
    public partial class Config : Window
    {
        public string Server { get; set; }
        public string Database { get; set; }
        public Config()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var config = new ConfigurationBuilder().AddUserSecrets<MainWindow>().Build();
            Server = config["DB:Server"];
            Database = config["DB:Database"];

            DataContext = this; //data binding secret to text display
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new LoginWindow();
            screen.Show();
            this.Close();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            var config = new ConfigurationBuilder().AddUserSecrets<MainWindow>().Build();
            config["DB:Server"] = serverTextBox.Text;
            config["DB:Database"] = databaseTextBox.Text;
            MessageBox.Show("Config save successful!");
        }
    }
}
