using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.Configuration.Interfaces
{
    public interface IDatabaseConfiguration
    {
        string ConnectionString { get; set; }
        string UserName { get; set; }
        string Password { get; set; }
        string DatabaseName { get; set; }
        string AuthMechanism { get; set; }
        string ServerName { get; set; }
    }
}
