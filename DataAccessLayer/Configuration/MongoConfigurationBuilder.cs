using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;

using System.IO;
using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace DataAccessLayer.Configuration
{
    public class MongoConfigurationBuilder : IConfigurationBuilder
    {
        #region Static field
        private static readonly string passwordProperty = "password";
        private static readonly string databaseNameProperty = "database_name";
        private static readonly string userNameProperty = "user_name";
        private static readonly string authMechanismProperty = "auth_mechanism";
        private static readonly string serverNameProperty = "server_name";
        #endregion

        private readonly IDatabaseConfiguration configuration;

        public string ConfigurationFilePath { get; set; }

        public MongoConfigurationBuilder(IDatabaseConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IConfigurationBuilder SetConfigurationFilePath(string filePath)
        {
            ConfigurationFilePath = filePath;

            return this;
        }

        public IConfigurationBuilder SetDatabaseName()
        {
            configuration.DatabaseName = GetConfigPropertyValue(databaseNameProperty);

            return this;
        }

        /// <summary>
        /// Reads the configuration file and returns the value of the given property.
        /// </summary>
        private string GetConfigPropertyValue(string propertyName)
        {
            using (FileStream fs = new FileStream(ConfigurationFilePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    Dictionary<string, string> properties = JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());

                    return properties[propertyName];
                }
            }
        }

        public IConfigurationBuilder SetDatabaseName(string databaseName)
        {
            configuration.DatabaseName = databaseName;

            return this;
        }

        public IConfigurationBuilder SetPassword()
        {
            configuration.Password = GetConfigPropertyValue(passwordProperty);

            return this;
        }

        public IConfigurationBuilder SetPassword(string password)
        {
            configuration.Password = password;

            return this;
        }

        public IConfigurationBuilder SetUserName()
        {
            configuration.UserName = GetConfigPropertyValue(userNameProperty);

            return this;
        }

        public IConfigurationBuilder SetUserName(string userName)
        {
            configuration.UserName = userName;

            return this;
        }

        public IConfigurationBuilder SetAuthMechanism()
        {
            configuration.AuthMechanism = GetConfigPropertyValue(authMechanismProperty);

            return this;
        }

        public IConfigurationBuilder SetAuthMechanism(string authMechanism)
        {
            configuration.AuthMechanism = authMechanism;

            return this;
        }

        public IConfigurationBuilder SetConnectionString()
        {
            string serverName = configuration.ServerName;
            string password = configuration.Password;

            configuration.ConnectionString =
                $"mongodb://{serverName}";

            return this;
        }

        public IConfigurationBuilder SetServerName()
        {
            configuration.ServerName = GetConfigPropertyValue(serverNameProperty);

            return this;
        }

        public IConfigurationBuilder SetServerName(string serverName)
        {
            configuration.ServerName = serverName;

            return this;
        }

        public IDatabaseConfiguration Build()
        {
            return configuration;
        }
    }
}
