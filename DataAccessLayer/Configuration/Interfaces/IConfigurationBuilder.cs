using DataAccessLayer.CommonInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Configuration.Interfaces
{
    public interface IConfigurationBuilder
    {
        string ConfigurationFilePath { get; set; }

        IConfigurationBuilder SetConfigurationFilePath(string filePath);

        IConfigurationBuilder SetConnectionString();

        IConfigurationBuilder SetUserName();
        IConfigurationBuilder SetUserName(string userName);

        IConfigurationBuilder SetPassword();
        IConfigurationBuilder SetPassword(string password);

        IConfigurationBuilder SetDatabaseName();
        IConfigurationBuilder SetDatabaseName(string databaseName);

        IConfigurationBuilder SetAuthMechanism();
        IConfigurationBuilder SetAuthMechanism(string authMechanism);

        IConfigurationBuilder SetServerName();
        IConfigurationBuilder SetServerName(string serverName);

        IDatabaseConfiguration Build();
    }
}
