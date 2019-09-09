using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Linq;

using NUnit.Framework;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Factories;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Interfaces;
using SentenceAPI.Features.Workplace.DocumentsDeskState.Services;

namespace SentenceApiTests.FeaturesTests
{
    [TestFixture]
    class DocumentDeskTests
    {
        private IDocumentDeskStateServiceFactory documentDeskStateServiceFactory;

        [SetUp]
        public void SetUp()
        {
            documentDeskStateServiceFactory = new DocumentDeskStateServiceFactory();
        }

        [Test]
        public void TestFactory()
        {
            var service = documentDeskStateServiceFactory.GetService();

            Assert.IsTrue(service is IDocumentDeskStateService);
        }

        [Test]
        public void TestServiceDependencies()
        {
            var service = new DocumentDeskStateService();

            Assert.IsTrue(service.GetType().GetTypeInfo().GetInterfaces().Contains(typeof(IDocumentDeskStateService)));
        }
    }
}
