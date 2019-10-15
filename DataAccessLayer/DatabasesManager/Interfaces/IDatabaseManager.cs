using DataAccessLayer.CommonInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.DatabasesManager.Interfaces
{
    public interface IDatabaseManager
    {
        IDatabaseFactory MongoDBFactory { get; }
    }
}
