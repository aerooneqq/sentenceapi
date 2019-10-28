using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using NUnit.Framework;

using DataAccessLayer.DatabasesManager;
using DataAccessLayer.MongoDB.Interfaces;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.KernelModels;
using DataAccessLayer.MongoDB.Factories;
using DataAccessLayer.MongoDB;

namespace DataAccessLayer.Tests.DatabaseFactory
{
    /// <summary>
    /// Test model class which is used to get the database service for this class.
    /// </summary>
    class TestModel : UniqueEntity { }

    [TestFixture]
    public class DatabaseManagerTests
    {
        private DatabasesManager.DatabasesManager databasesManager = DatabasesManager.DatabasesManager.Manager;

        [Test]
        public void TestDatabaseManager()
        {
            var factory = databasesManager.MongoDBFactory;

            Assert.True(factory.GetType() == typeof(MongoDBServiceFactory));
            Assert.True(factory.GetType().GetInterface("IDatabaseFactory") == typeof(IDatabaseFactory));

            var serviceWeakRef = databasesManager.MongoDBFactory.GetDatabase<TestModel>();

            Assert.True(serviceWeakRef.GetType() == typeof(WeakReference<IDatabaseService<TestModel>>));

            IDatabaseService<TestModel> databaseService;
            serviceWeakRef.TryGetTarget(out databaseService);

            Assert.NotNull(databaseService);
            Assert.True(databaseService.GetType().GetInterfaces().Contains(typeof(IDatabaseService<TestModel>)));
        }
    }
}
