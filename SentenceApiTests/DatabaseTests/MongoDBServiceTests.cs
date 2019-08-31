using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using DataAccessLayer.MongoDB.Interfaces;
using DataAccessLayer.MongoDB;

using NUnit.Framework;
using SentenceAPI.Features.Users.Models;
using System.Diagnostics;
using System.Threading.Tasks;
using SentenceApiTests.DatabaseTests.TestModels;
using DataAccessLayer.CommonInterfaces;

namespace SentenceApiTests.DatabaseTests
{
    [TestFixture]
    public class MongoDBServiceTests
    {
        #region Fields
        private IDatabaseService<TestModel> mongoDBService;
        private TestModel model;
        #endregion

        [SetUp]
        public void SetUp()
        {
            mongoDBService = new MongoDBService<TestModel>();

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


            mongoDBService.Connect().GetAwaiter().GetResult();
            if (mongoDBService.DoesCollectionExist().GetAwaiter().GetResult())
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

                await mongoDBService.Connect();
                await mongoDBService.CreateCollection();

                if (!(await mongoDBService.DoesCollectionExist()))
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

                await mongoDBService.Connect();
                if (await mongoDBService.DoesCollectionExist())
                {
                    await mongoDBService.DeleteCollection();

                    if (await mongoDBService.DoesCollectionExist())
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

            await mongoDBService.Connect();

            var obj = await mongoDBService.Get(0);
            if (obj == null || !(obj is TestModel))
            {
                Assert.Fail();
            }

            //obj = await mongoDBService.Get(-1);
            if (obj != null)
            {
                Assert.Fail();
            }

            Assert.Pass();
        }
    }
}
