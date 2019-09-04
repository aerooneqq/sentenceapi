using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using DataAccessLayer.Configuration;
using DataAccessLayer.Configuration.Interfaces;

using NUnit.Framework;

using Newtonsoft.Json;

namespace DataAccessLayer.Tests.ConfigurationBuilderTests
{
    [TestFixture]
    public class ConfigurationBuilderTests
    {
        private static readonly string configurationFileName = "mongo_database_config.json";

        #region Mongo config property names
        private static string mongoUserNameProperty = "user_name";
        private static string mongoPasswordProperty = "password";
        private static string mongoAuthMechProperty = "auth_mechanism";
        private static string mongoDatabaseProperty = "database_name";
        private static string mongoServerName = "server_name";
        #endregion

        private IConfigurationBuilder configurationBuilder;
        private IDatabaseConfiguration configuration;

        private string configFilePath;

        [SetUp]
        public void SetUp()
        {
            configFilePath = Path.Combine(Directory.GetCurrentDirectory(), configurationFileName);
        }

        [Test]
        public void TestAutoMongoConfiguration()
        {
            configuration = new MongoDatabaseConfiguration();
            configurationBuilder = new MongoConfigurationBuilder(configuration);

            configurationBuilder.SetConfigurationFilePath(configFilePath)
                                .SetAuthMechanism().SetDatabaseName().SetPassword()
                                .SetServerName().SetUserName().SetConnectionString();

            IDictionary<string, string> configFromFile = GetConfigFromFile();

            Assert.IsTrue(CheckIfConfigsAreSame(configFromFile, configuration));
        }

        private bool CheckIfConfigsAreSame(IDictionary<string, string> configFromFile, 
                                           IDatabaseConfiguration configuration)
        {
            return configFromFile[mongoUserNameProperty] == configuration.UserName ||
                   configFromFile[mongoPasswordProperty] == configuration.Password ||
                   configFromFile[mongoAuthMechProperty] == configuration.AuthMechanism ||
                   configFromFile[mongoDatabaseProperty] == configuration.DatabaseName ||
                   configFromFile[mongoServerName] == configuration.ServerName;
        }

        private Dictionary<string, string> GetConfigFromFile()
        {
            using (FileStream fs = new FileStream(configFilePath, FileMode.Open, FileAccess.Read))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    return JsonConvert.DeserializeObject<Dictionary<string, string>>(sr.ReadToEnd());
                }
            }
        }

        [Test]
        public void TestManualMongoConfig()
        {
            configuration = new MongoDatabaseConfiguration();
            configurationBuilder = new MongoConfigurationBuilder(configuration);

            IDictionary<string, string> config = new Dictionary<string, string>();

            config.Add(mongoAuthMechProperty, "adasdasd");
            config.Add(mongoDatabaseProperty, "Hello world");
            config.Add(mongoPasswordProperty, "Password");
            config.Add(mongoServerName, "serverName");
            config.Add(mongoUserNameProperty, "userName");

            configurationBuilder.SetAuthMechanism(config[mongoAuthMechProperty])
                                .SetDatabaseName(config[mongoDatabaseProperty])
                                .SetPassword(config[mongoPasswordProperty])
                                .SetServerName(config[mongoServerName])
                                .SetUserName(config[mongoUserNameProperty]);

            Assert.IsTrue(CheckIfConfigsAreSame(config, configuration));
        }
    }
}
