using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using NUnit.Framework;

using DataAccessLayer.Aggregations.Interfaces;
using DataAccessLayer.KernelModels;
using DataAccessLayer.Aggregations;
using MongoDB.Bson.Serialization.Attributes;

namespace DataAccessLayerTests.Tests.Aggregation
{
    class TestModel : UniqueEntity
    {
        [BsonElement("field")]
        public string Field { get; set; }
    }

    [TestFixture]
    class AggregationTests
    {
        #region Initializers 
        private Type mainCollectionType = typeof(TestModel);
        private string localField = "LocalField";
        private Dictionary<string, (string foreignKey, IEnumerable<string> requestedProperties)> extraCollection =
            new Dictionary<string, (string foreignKey, IEnumerable<string> requestedProperties)>()
            {
                {  "TestCollection", ("id", new [] { "property" }) }
            };
        #endregion

        private IAggregation aggregation;

        [SetUp]
        public void SetUp()
        {
            aggregation = new DataAccessLayer.Aggregations.Aggregation(
                    mainCollectionType,
                    localField,
                    extraCollection
                );
        }

        [Test]
        public void TestAggregation()
        {
            Assert.IsTrue(aggregation.MainType == mainCollectionType);
            Assert.IsTrue(aggregation.LocalField == localField);

            Assert.IsTrue(aggregation.ExtraCollections["TestCollection"].foreignKey == "id");
            Assert.IsTrue(aggregation.ExtraCollections["TestCollection"].requestedProperties.ToList().Count == 1);
            Assert.IsTrue(aggregation.ExtraCollections["TestCollection"].requestedProperties.ToList()[0] == "property");
        }

        [Test]
        public void TestMongoAggregationFilter()
        {

        }
    }
}
