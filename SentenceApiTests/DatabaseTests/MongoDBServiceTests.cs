using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using SentenceAPI.Databases.MongoDB.Interfaces;
using SentenceAPI.Databases.MongoDB;

using NUnit.Framework;
using SentenceAPI.Features.Users.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using SentenceApiTests.DatabaseTests.TestModels;

namespace SentenceApiTests.DatabaseTests
{
    [TestFixture]
    public class MongoDBServiceTests
    {
        #region Fields
        private IMongoDBServiceBuilder<TestModel> mongoDBServiceBuilder;
        private IMongoDBService<TestModel> mongoDBService;
        private TestModel model;
        #endregion

        [SetUp]
        public void SetUp()
        {
            mongoDBService = new MongoDBService<TestModel>();
            mongoDBServiceBuilder = new MongoDBServiceBuilder<TestModel>(mongoDBService);

            CareerStage firstStage = new CareerStage()
            {
                Company = "Amazon",
                FinishYear = new DateTime(2014, 2, 3),
                StartYear = new DateTime(2017, 3, 4),
                Job = "Software engineer"
            };

            CareerStage secondStage = new CareerStage()
            {
                Company = "Google",
                FinishYear = new DateTime(2017, 4, 5),
                StartYear = DateTime.Now,
                Job = "Scrum manager"
            };

            model = new TestModel()
            {
                City = "Moscow",
                Email = "aerooneQ@yandex.ru",
                Name = "John",
                Year = 2019,
            };

            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json").
                SetConnectionString().
                SetDatabaseName("SentenceDatabase").
                SetCollectionName().Build();

            mongoDBService.Connect().GetAwaiter().GetResult();
            if (mongoDBService.IsCollectionExist().GetAwaiter().GetResult())
            {
                mongoDBService.DeleteCollection().GetAwaiter().GetResult();
                mongoDBService.CreateCollection().GetAwaiter().GetResult();
            }
        }

        [Test]
        public async Task TestCreatingCollection()
        {
            try
            {
                mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json").
                    SetConnectionString().
                    SetDatabaseName("SentenceDatabase").
                    SetCollectionName().Build();

                await mongoDBService.Connect();
                await mongoDBService.CreateCollection();

                if (!(await mongoDBService.IsCollectionExist()))
                {
                    Assert.Fail();
                }
            }
            catch
            {
                Assert.Fail();
            }

            Assert.Pass();
        }

        [Test]
        public async Task TestDeletingCollection()
        {
            try
            {
                mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json").
                    SetConnectionString().
                    SetDatabaseName("SentenceDatabase").
                    SetCollectionName().Build();

                await mongoDBService.Connect();
                if (await mongoDBService.IsCollectionExist())
                {
                    await mongoDBService.DeleteCollection();

                    if (await mongoDBService.IsCollectionExist())
                    {
                        Assert.Fail();
                    }
                }
                else
                {
                    Assert.Fail("The collection doesn't exist before deleting, so the test failed");
                }
            }
            catch
            {
                Assert.Fail();
            }

            Assert.Pass();
        }

        [Test]
        public async Task TestAddingAnObject()
        {
            try
            {
                mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json").
                    SetConnectionString().
                    SetDatabaseName("SentenceDatabase").
                    SetCollectionName().Build();

                await mongoDBService.Connect();
                await mongoDBService.Insert(model);
            }
            catch
            {
                Assert.Fail();
            }

            Assert.Pass();
        }

        [Test]
        public async Task TestDeletingTheRecord()
        {
            try
            {
                mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json").
                    SetConnectionString().
                    SetDatabaseName("SentenceDatabase").
                    SetCollectionName().Build();

                await mongoDBService.Connect();
                await mongoDBService.Delete(0);
            }
            catch
            {
                Assert.Fail();
            }

            Assert.Pass();
        }

        [Test]
        public async Task TestGettingEntititiesByID()
        {
            //warning make sure that entity with id = 0 is in the collection
            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json").
                SetConnectionString().
                SetDatabaseName("SentenceDatabase").
                SetCollectionName().Build();

            await mongoDBService.Connect();

            var obj = await mongoDBService.Get(0);
            if (obj == null || !(obj is TestModel))
            {
                Assert.Fail();
            }

            obj = await mongoDBService.Get(-1);
            if (obj != null)
            {
                Assert.Fail();
            }

            Assert.Pass();
        }

        [Test]
        public async Task TestGettingElementByProperties()
        {
            //make sure that there are 4 similar records in the database (except of the ID property)
            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json").
                SetConnectionString().
                SetDatabaseName("SentenceDatabase").
                SetCollectionName().Build();
            await mongoDBService.Connect();

            Dictionary<string, object> properties = new Dictionary<string, object>()
            {
                {"year", 2019 },
                {"email", "aerooneQ@yandex.ru" }
            };

            var objs = mongoDBService.Get(properties).GetAwaiter().GetResult().ToList();
            if (objs.Count != 1)
            {
                Assert.Fail();
            }

            properties.Add("_id", 1);

            objs = mongoDBService.Get(properties).GetAwaiter().GetResult().ToList();
            if (objs.Count != 0)
            {
                Assert.Fail();
            }

            properties.Add("asdasd", "asdaasd");

            objs = (await mongoDBService.Get(properties)).ToList();
            if (objs.Count != 0)
            {
                Assert.Fail();
            }

            Assert.Pass();
        }
    }
}
