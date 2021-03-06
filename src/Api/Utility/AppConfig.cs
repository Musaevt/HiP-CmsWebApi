﻿using System.Text;
using Microsoft.Extensions.Configuration;

namespace PaderbornUniversity.SILab.Hip.CmsApi.Utility
{
    public class AppConfig
    {
        public DatabaseConfig DatabaseConfig { get; }

        public AuthConfig AuthConfig { get; }

        public string EmailService { get; }

        public bool RequireHttpsMetadata { get; }

        public string AdminEmail { get; }

        public AppConfig(IConfiguration configuration)
        {
            DatabaseConfig = new DatabaseConfig
            {
                Host = configuration.GetValue<string>("DB_HOST"),
                Username = configuration.GetValue<string>("DB_USERNAME"),
                Password = configuration.GetValue<string>("DB_PASSWORD"),
                Name = configuration.GetValue<string>("DB_NAME")
            };

            AuthConfig = new AuthConfig
            {
                ClientId = configuration.GetValue<string>("CLIENT_ID"),
                Domain = configuration.GetValue<string>("DOMAIN")
            };

            EmailService = configuration.GetValue<string>("EMAIL_SERVICE");

            RequireHttpsMetadata = !configuration.GetValue<bool>("ALLOW_HTTP");
            AdminEmail = configuration.GetValue<string>("ADMIN_EMAIL");
        }
    }


    public class DatabaseConfig
    {
        public string Name { get; set; }

        public string Host { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string ConnectionString
        {
            get
            {
                var connectionString = new StringBuilder();

                connectionString.Append($"Host={Host};");
                connectionString.Append($"Username={Username};");
                connectionString.Append($"Password={Password};");
                connectionString.Append($"Database={Name};");
                connectionString.Append($"Pooling=true;");

                return connectionString.ToString();
            }
        }
    }

    public class AuthConfig
    {
        public string ClientId { get; set; }

        public string Domain { get; set; }
    }
}
