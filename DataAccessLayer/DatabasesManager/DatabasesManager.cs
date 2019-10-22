using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.KernelModels;
using DataAccessLayer.MongoDB.Factories;
using DataAccessLayer.MongoDB.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace DataAccessLayer.DatabasesManager
{
    public class DatabasesManager : IDatabaseManager
    {
        #region Factories
        public IDatabaseFactory MongoDBFactory { get; }
        #endregion

        private DatabasesManager()
        {
            MongoDBFactory = new MongoDBServiceFactory();
        }

        #region Singleton
        private static DatabasesManager databasesManager;

        public static DatabasesManager Manager
        {
            get
            {
                if (databasesManager == null)
                {
                    databasesManager = new DatabasesManager();
                }

                return databasesManager;
            }
        }
        #endregion
    }
}
