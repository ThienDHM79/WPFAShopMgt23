using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Common;
using System.Dynamic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Permissions;
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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Server { get; set; }
        public string Database { get; set; }
        public string Entropy { get; set; }
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void serverSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            var screen = new Config();
            if (screen.ShowDialog() == true)
            {
                ReloadSettings();
            }
        }

        private async void loginButton_Click(object sender, RoutedEventArgs e)
        {
            //design - turn off sql
            AppConfig.Username = usernameTextBox.Text;
            AppConfig.Password = passwordBox.Password;
            var connectionString = AppConfig.ConnectionString();

            progressBar.IsIndeterminate = true;
            var (success, message, connection) = await Task.Run(() =>
            {
                var __connection = new SqlConnection(connectionString);
                bool success = true;
                string message = "";
                try
                {
                    __connection.Open();
                }
                catch (Exception ex)
                {
                    success = false;
                    message = ex.Message;
                }
                return new Tuple<bool, string, SqlConnection>(success, message, __connection);
            });
            progressBar.IsIndeterminate = false;

            if (success)
            {
                MessageBox.Show("Login successfully");
                if (rememberPassCheckBox.IsChecked == true)
                {
                    var passwordInBytes = Encoding.UTF8.GetBytes(AppConfig.Password);
                    var entropy = new byte[20];

                    using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
                    {
                        rng.GetBytes(entropy);
                    }
                    var cypherText = ProtectedData.Protect(passwordInBytes, entropy, DataProtectionScope.CurrentUser);
                    Configuration config = System.Configuration.ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["Username"].Value = Username;
                    config.AppSettings.Settings["Password"].Value = Convert.ToBase64String(cypherText);
                    config.AppSettings.Settings["Entropy"].Value = Convert.ToBase64String(entropy);
                    config.Save(ConfigurationSaveMode.Full);
                    System.Configuration.ConfigurationManager.RefreshSection("appSettings");


                }
                var mainscreen = new MainWindow();
                mainscreen.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show($"Cannot login.Reason {message}");
            }
            //
            //UI design flow 1
            /*
            var screen = new MainWindow();
            screen.Show();
            this.Close();
            */
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            AppConfig.Reload();

            DataContext = new AppConfig();
            usernameTextBox.Text = AppConfig.Username;
            passwordBox.Password = AppConfig.Password;
        }

        private void ReloadSettings()
        {
            var config = System.Configuration.ConfigurationManager.AppSettings;
            Username = config["Username"] ?? "";
            //get password in encrypted
            var passwordIn64 = config["Password"] ?? "";
            var entropyIn64 = config["Entropy"] ?? "";
            Server = config["Server"] ?? "";
            Database = config["Database"] ?? "";

            //decode step
            if (passwordIn64.Length != 0)
            {
                var passwordInBytes = Convert.FromBase64String(passwordIn64);
                var entropyInBytes = Convert.FromBase64String(entropyIn64);
                var unencryptedPassword = ProtectedData.Unprotect(passwordInBytes, entropyInBytes, DataProtectionScope.CurrentUser);
                Password = Encoding.UTF8.GetString(unencryptedPassword);
            }
            /* old :dung user secret
            var config = new ConfigurationBuilder().AddUserSecrets<MainWindow>().Build();
            Username = config["DB:Username"] ?? "";
            Password = config["DB:Password"] ?? "";
            Server = config["DB:Server"] ?? "";
            Database = config["DB:Database"] ?? "";
            */
        }

    }
}
