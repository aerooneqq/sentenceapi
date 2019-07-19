using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using SentenceAPI.Features.Authentication.Factories;
using SentenceAPI.Features.Authentication.Interfaces;

namespace SentenceApiTests.FeaturesTests.AuthenticationTests
{
    [TestFixture]
    class TokenServiceFactoryTest
    {
        private ITokenServiceFactory tokenServiceFactory;

        [SetUp]
        public void SetUp()
        {
            tokenServiceFactory = new TokenServiceFactory();
        }

        [Test]
        public void TestTokenServiceFactory()
        {
            ITokenService tokenSerivce = tokenServiceFactory.GetService();

            if (!(tokenSerivce is ITokenService))
            {
                Assert.Fail();
            }

            Assert.Pass();
        }
    }
}
