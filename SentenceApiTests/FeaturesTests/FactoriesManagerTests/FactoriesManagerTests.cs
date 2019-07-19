using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using SentenceAPI.Features.FactoriesManager;
using SentenceAPI.Features.FactoriesManager.Interfaces;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Authentication.Factories;
using SentenceAPI.Features.FactoriesManager.Models;

namespace SentenceApiTests.FeaturesTests.FactoriesTests
{
    [TestFixture]
    class FactoriesManagerTests
    {
        private IFactoriesManager factoriesManager;
        private ITokenServiceFactory tokenServiceFactory;

        [SetUp]
        public void SetUp()
        {
            factoriesManager = FactoriesManager.Instance;
            tokenServiceFactory = new TokenServiceFactory();
        }

        [Test]
        public void TestIndexAccess()
        {
            //Try to get factory when the manager is empty.
            var factory = factoriesManager[typeof(ITokenService)];

            if (factory != null)
            {
                Assert.Fail();
            }

            //Add a new FactoryInfoObject and try to get it.
            factoriesManager.AddFactory(new FactoryInfo(tokenServiceFactory, typeof(ITokenService)));

            factory = factoriesManager[typeof(ITokenService)];

            if (!(factory.Factory is ITokenServiceFactory))
            {
                Assert.Fail();
            }


            //Try to access "null" factory.
            try
            {
                factory = factoriesManager[null];
                Assert.Fail();
            }
            catch (ArgumentNullException) { }

            Assert.Pass();
        }

        [Test]
        public void TestAdding()
        {
            //Try to put a FactoryInfo object in the factories manager
            factoriesManager.AddFactory(new FactoryInfo(tokenServiceFactory, typeof(ITokenService)));

            if (!(factoriesManager[typeof(ITokenService)].Factory is ITokenServiceFactory))
            {
                Assert.Fail();
            }

            //Try to put a FactoryInfo with a similar properties as it was before.
            try
            {
                factoriesManager.AddFactory(new FactoryInfo(tokenServiceFactory, typeof(ITokenService)));
                Assert.Fail();
            }
            catch (ArgumentException) { }


            //Try to put a FactoryInfo object with partially same peoperties as it was before
            try
            {
                factoriesManager.AddFactory(new FactoryInfo(tokenServiceFactory, typeof(ITokenServiceFactory)));
                Assert.Fail();
            }
            catch (ArgumentException) { }

            Assert.Pass();
        }

        [Test]
        public void TestRemoval()
        {
            factoriesManager.AddFactory(new FactoryInfo(tokenServiceFactory, typeof(ITokenService)));

            //Try to remove "null" FactoryInfo.
            try
            {
                factoriesManager.RemoveFactory(null);
                Assert.Fail();
            }
            catch (ArgumentNullException) { }

            //Try to remove a factory which is not in the list of manager.
            bool result = factoriesManager.RemoveFactory(typeof(ITokenServiceFactory));
            if (result)
            {
                Assert.Fail();
            }


            //Try to remove an existing factory from a factory manager.
            factoriesManager.RemoveFactory(typeof(ITokenService));
            var factory = factoriesManager[typeof(ITokenService)];

            if (factory != null)
            {
                Assert.Fail();
            }

            Assert.Pass();
        }
    }
}
