using System;
using System.Collections.Generic;
using System.Text;

using DataAccessLayer.Filters;
using DataAccessLayer.Filters.Interfaces;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;

using NUnit.Framework;

namespace DataAccessLayer.Tests.FiltersTests
{
    [TestFixture]
    class FilterTests
    {
        private IFilter filter;
        private IFilterCollection filterCollection;

        private IMongoClient mongoClient;
        private IMongoDatabase database;
        private IMongoCollection<FilterTestModel> collection;

        [SetUp]
        public void SetUp()
        {
            mongoClient = new MongoClient(PrepareCollectionForTests.GetConnectionString());
            database = mongoClient.GetDatabase("SentenceDatabase");
            collection = database.GetCollection<FilterTestModel>("FilterTestCollection");
        }

        [Test]
        public void TestEqualityFilter()
        {
            filter = new EqualityFilter<long>("age", 2014);
            FilterDefinition<FilterTestModel> customFilter = filter.ToMongoFilter<FilterTestModel>();
            FilterDefinition<FilterTestModel> nativeFilter = Builders<FilterTestModel>.Filter.Eq("age", 2014);

            Assert.NotNull(customFilter);
            Assert.NotNull(nativeFilter);

            var customFilterResults = collection.Find(customFilter).ToList();
            var nativeFilterResults = collection.Find(nativeFilter).ToList();

            Assert.AreEqual(customFilterResults.Count, nativeFilterResults.Count);

            for (int i = 0; i < customFilterResults.Count; i++)
            {
                if (customFilterResults[i].Age != nativeFilterResults[i].Age)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TestRegexFilter()
        {
            filter = new RegexFilter("name", @"\Евген\");

            FilterDefinition<FilterTestModel> customFilter = filter.ToMongoFilter<FilterTestModel>();
            FilterDefinition<FilterTestModel> nativeFilter = filter.ToMongoFilter<FilterTestModel>();

            Assert.NotNull(customFilter);
            Assert.NotNull(nativeFilter);

            var customFilterResults = collection.Find(customFilter).ToList();
            var nativeFilterResults = collection.Find(nativeFilter).ToList();

            for (int i = 0; i < customFilterResults.Count; i++)
            {
                if (customFilterResults[i].Name != nativeFilterResults[i].Name)
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TestInFilter()
        {
            filter = new InFilter<int>("numbers", new[] { 1 });

            FilterDefinition<FilterTestModel> customFilter = filter.ToMongoFilter<FilterTestModel>();
            FilterDefinition<FilterTestModel> nativeFilter = filter.ToMongoFilter<FilterTestModel>();

            Assert.NotNull(customFilter);
            Assert.NotNull(nativeFilter);

            var customFilterResults = collection.Find(customFilter).ToList();
            var nativeFilterResults = collection.Find(nativeFilter).ToList();

            for (int i = 0; i < customFilterResults.Count; i++)
            {
                if (!customFilterResults[i].Numbers.Contains(1) ||
                    !nativeFilterResults[i].Numbers.Contains(1))
                {
                    Assert.Fail();
                }
            }
        }

        [Test]
        public void TestFilterCollection()
        {
            filterCollection = new FilterCollection(new IFilter[]
            {
                new RegexFilter("name", @"\Евген\"),
                new InFilter<int>("numbers", new[] { 1 }),
            });

            FilterDefinition<FilterTestModel> nativeFilter = Builders<FilterTestModel>.Filter.Regex("name", @"\Евген\");
            nativeFilter = Builders<FilterTestModel>.Filter.And(nativeFilter,
                Builders<FilterTestModel>.Filter.In<int>("numbers", new [] { 1 }));

            FilterDefinition<FilterTestModel> customFilter = filterCollection.ToMongoFilter<FilterTestModel>();

            Assert.NotNull(nativeFilter);
            Assert.NotNull(customFilter);

            var customFilterResults = collection.Find(customFilter).ToList();
            var nativeFilterResults = collection.Find(nativeFilter).ToList();

            for (int i = 0; i < customFilterResults.Count; i++)
            {
                if (customFilterResults[i].Name != nativeFilterResults[i].Name ||
                    !customFilterResults[i].Numbers.Contains(1) || !nativeFilterResults[i].Numbers.Contains(1))
                {
                    Assert.Fail();
                }
            }
        }
    }
}
