using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using DataAccessLayer.MongoDB;
using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.KernelModels;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Filters;

using MongoDB.Bson.Serialization.Attributes;
using System.Threading.Tasks;
using System.Linq;

namespace DataAccessLayer.Tests.DatabasesTests
{
    /// <summary>
    /// The test model which is used to test the mongo database service.
    /// </summary>
    class TestModel : UniqueEntity
    {
        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("Year")]
        public int Year { get; set; }

        [BsonElement("dateTime")]
        public DateTime DateTime { get; set; }
    }


    /// <summary>
    /// This class contains methods to test the mongo database service.
    /// The tests must be run in order they appear in this class.
    /// </summary>
    [TestFixture]
    public class MongoDatabaseTests
    {
        private static readonly string databaseConfigFile = "mongo_database_config.json";

        private IDatabaseService<TestModel> databaseService;
        private IConfigurationBuilder configurationBuilder;


        [SetUp]
        public void SetUp()
        {
            DatabasesManager.DatabasesManager.Manager.MongoDBFactory.GetDatabase<TestModel>().TryGetTarget(out databaseService);
            configurationBuilder = new MongoConfigurationBuilder(databaseService.Configuration);

            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetUserName().SetPassword()
                                .SetAuthMechanism().SetDatabaseName().SetServerName().SetConnectionString();
        }

        /// <summary>
        /// Creates the collection. Make sure that there is no collection in the database 
        /// with the name "TestModelCollection"
        /// </summary>
        [Test]
        public async Task CreateCollection()
        {
            try
            {
                await databaseService.Connect();

                if ((await databaseService.DoesCollectionExist()))
                {
                    Assert.Fail("The collection already exists, delete it before test.");
                }

                await databaseService.CreateCollection();

                if (!(await databaseService.DoesCollectionExist()))
                {
                    Assert.Fail("The collection must exist.");
                }

                Assert.Pass();
            }
            catch (Exception ex) when (ex.GetType() != typeof(AssertionException) &&
                                       ex.GetType() != typeof(SuccessException))
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }

        /// <summary>
        /// If we try to insert the entity not of type of mongoDBService, then the service
        /// must fire an ArgumentException.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task TestInsertingNULLEntity()
        {
            try
            {
                await databaseService.Connect();
                await databaseService.Insert(null);
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }
            catch (Exception ex) when (ex.GetType() != typeof(AssertionException) &&
                                       ex.GetType() != typeof(SuccessException))
            {
                Assert.Fail($"Test failed with exception {ex.Message}");
            }
        }

        [Test]
        public async Task TestInsertEntity()
        {
            try
            {
                string name = "Aero";
                int year = 2019;
                long id = 0;
                DateTime date = DateTime.Now;

                TestModel testModel = new TestModel()
                {
                    DateTime = date,
                    ID = id,
                    Name = name,
                    Year = year
                };

                await databaseService.Connect();
                await databaseService.Insert(testModel);

                var testModels = (await databaseService.Get(new EqualityFilter<long>("_id", 0))).ToList();

                Assert.IsTrue(testModels.Count == 1);
                Assert.IsTrue(testModels[0].Name == "Aero" && testModels[0].DateTime == date
                              && testModels[0].Year == year && testModels[0].ID == id);
            }
            catch (Exception ex) when (ex.GetType() != typeof(AssertionException) &&
                                       ex.GetType() != typeof(SuccessException))
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }

        [Test]
        public async Task TestUpdatingNullEntity()
        {
            try
            {
                await databaseService.Connect();
                await databaseService.Update(null);
            }
            catch (ArgumentException)
            {
                Assert.Pass();
            }
            catch (Exception ex) when (ex.GetType() != typeof(AssertionException) &&
                                       ex.GetType() != typeof(SuccessException))
            {
                Assert.Fail($"Test failed with exception {ex.Message}");
            }
        }

        [Test]
        public async Task TestUpdatingEntity()
        {
            try
            {
                string newName = "Fess";
                int newYear = 2012;
                DateTime newDate = DateTime.Now;

                await databaseService.Connect();
                var testModel = (await databaseService.Get(new EqualityFilter<long>("_id", 0))).FirstOrDefault();

                Assert.IsNotNull(testModel);

                testModel.DateTime = newDate;
                testModel.Name = newName;
                testModel.Year = newYear;

                await databaseService.Update(testModel);

                testModel = (await databaseService.Get(new EqualityFilter<long>("_id", 0))).FirstOrDefault();

                Assert.NotNull(testModel);
                Assert.IsTrue(testModel.Name == newName && testModel.Year == newYear && testModel.DateTime == newDate);
            }
            catch (Exception ex) when (ex.GetType() != typeof(AssertionException) &&
                                       ex.GetType() != typeof(SuccessException))
            {
                Assert.Fail($"Test failed with exception {ex.Message}");
            }
        }
        
        /// <summary>
        /// Testing the update of the entities with only given fields.
        /// </summary>
        [Test]
        public async Task TestUpdatingEntityWithGivenFields()
        {
            try
            {
                await databaseService.Connect();

                var testModel = (await databaseService.Get(new EqualityFilter<long>("_id", 0))).FirstOrDefault();

                Assert.NotNull(testModel);

                string newName = "John";
                int newYear = 203;

                testModel.Name = newName;
                testModel.Year = newYear;

                await databaseService.Update(testModel, new[]
                {
                    "Year",
                    "Name"
                });

                testModel = (await databaseService.Get(new EqualityFilter<long>("_id", 0))).FirstOrDefault();

                Assert.NotNull(testModel);
                Assert.True(testModel.Name == newName && testModel.Year == newYear);
            }
            catch (Exception ex) when (ex.GetType() != typeof(AssertionException) &&
                                       ex.GetType() != typeof(SuccessException))
            {
                Assert.Fail($"The test failed with exception {ex.Message}");
            }
        }

        [Test]
        public async Task TestDeletingEntity()
        {
            try
            {
                await databaseService.Connect();
                await databaseService.Delete(new EqualityFilter<long>("_id", 0));

                var testModels = (await databaseService.Get(new EqualityFilter<long>("_id", 0))).ToList();

                Assert.NotNull(testModels);
                Assert.IsTrue(testModels.Count == 0);
            }
            catch (Exception ex) when (ex.GetType() != typeof(AssertionException) &&
                                       ex.GetType() != typeof(SuccessException))
            {
                Assert.Fail($"Test failed with the exception: {ex.Message}");
            }
        }

        /// <summary>
        /// Tests the deleting og the collection. 
        /// This test must always be run last, because after the completion of this test,
        /// the testable collection no longer exists.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task DeleteCollection()
        {
            try
            {
                await databaseService.Connect();

                if (!(await databaseService.DoesCollectionExist()))
                {
                    Assert.Fail("The collection does not exist.");
                }

                await databaseService.DeleteCollection();

                if (await databaseService.DoesCollectionExist())
                {
                    Assert.Fail("The collection must have been deleted");
                }

                Assert.Pass();
            }
            catch (Exception ex) when (ex.GetType() != typeof(AssertionException) &&
                                       ex.GetType() != typeof(SuccessException))
            {
                Assert.Fail($"Test failed with exception: {ex.Message}");
            }
        }
    }
}
