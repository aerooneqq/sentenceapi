using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.DatabasesManager.Interfaces;
using DataAccessLayer.MongoDB.Factories;


namespace DataAccessLayer.DatabasesManager
{
    public class DatabasesManager : IDatabaseManager
    {
        #region Factories
        public IDatabaseFactory MongoDBFactory { get; }
        #endregion

        public DatabasesManager()
        {
            MongoDBFactory = new MongoDBServiceFactory();
        }
    }
}
