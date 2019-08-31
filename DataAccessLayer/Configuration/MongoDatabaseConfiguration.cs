using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Configuration
{
    class MongoDatabaseConfiguration : IDatabaseConfiguration
    {
        public string ConnectionString { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string DatabaseName { get; set; }
        public string AuthMechanism { get; set; }
        public string ServerName { get; set; }
    }
}
