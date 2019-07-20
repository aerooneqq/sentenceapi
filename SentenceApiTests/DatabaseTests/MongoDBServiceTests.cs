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

namespace SentenceApiTests.DatabaseTests
{
    [TestFixture]
    public class MongoDBServiceTests
    {
        #region Fields
        private IMongoDBServiceBuilder<UserInfo> mongoDBServiceBuilder;
        private IMongoDBService<UserInfo> mongoDBService;
        private UserInfo user;
        #endregion

        [SetUp]
        public void SetUp()
        {
            mongoDBService = new MongoDBService<UserInfo>();
            mongoDBServiceBuilder = new MongoDBServiceBuilder<UserInfo>(mongoDBService);

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

            user = new UserInfo()
            {
                ID = 1,
                City = "Washington",
                CareerStages = new List<CareerStage>() { firstStage, secondStage },
                Country = "USA",
                Email = "aerooneQ@yandex.ru",
                IsAccountVerified = false,
                Login = "Aero",
                MiddleName = "Вадимович",
                Name = "Евгений",
                Password = "AeroOne1234",
                Surname = "Степанов"
            };
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
                await mongoDBService.Insert(user);
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
                await mongoDBService.Delete(2);
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
            long id = 0;

            mongoDBService = mongoDBServiceBuilder.AddConfigurationFile("database_config.json").
                SetConnectionString().
                SetDatabaseName("SentenceDatabase").
                SetCollectionName().Build();

            await mongoDBService.Connect();

            var obj = await mongoDBService.Get(0);
            if (obj == null || !(obj is UserInfo))
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
                {"login", "Aero" },
                {"email", "aerooneQ@yandex.ru" }
            };

            var objs = mongoDBService.Get(properties).GetAwaiter().GetResult().ToList();
            if (objs.Count != 4)
            {
                Assert.Fail();
            }

            properties.Add("_id", 1);

            objs = mongoDBService.Get(properties).GetAwaiter().GetResult().ToList();
            if (objs.Count != 1 || objs[0].ID != 1)
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
