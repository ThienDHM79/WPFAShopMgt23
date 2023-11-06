using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace WPFAShopMgt23
{
    class AppConfig
    {
        public static string Username { get; set; } = "";
        public static string Password { get; set; } = "";
        public static string Server { get; set; } = "";
        public static string Database { get; set; } = "";
        public static string Entropy { get; set; } = "";
        public static void Reload()
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
        }
        public static string ConnectionString()
        {
            Reload();
            var builder = new SqlConnectionStringBuilder();
            builder.DataSource = Server;
            builder.InitialCatalog = Database;
            builder.UserID = Username;
            builder.Password = Password;
            builder.IntegratedSecurity = true; // new add for trusted connection
            builder.TrustServerCertificate = true;

            string connectionString = builder.ConnectionString;
            return connectionString;
        }
    }
}
