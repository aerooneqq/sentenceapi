using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using SentenceAPI.Features.Users.Models;
using SentenceAPI.Features.Extensions;

using NUnit.Framework;
using System.Reflection;

namespace SentenceApiTests.ExtensionsTests
{
    [TestFixture]
    class GenericExtensionsTests
    {
        private UserInfo user;
        private IEnumerable<string> propertiesNames;

        [SetUp]
        public void SetUp()
        {
            user = new UserInfo()
            {
                City = "Moscow",
                Email = "stepanov-ev@yandex.ru",
                Country = "Russia",
                Password = "asd"
            };

            propertiesNames = new[] { "country", "city", "email", "password" };
        }

        [Test]
        public void TestConfiguringNewObject()
        {
            var newObject = user.ConfigureNewObject(propertiesNames);

            foreach (var property in newObject)
            {
                if (!propertiesNames.Select(p => p.ToLower()).Contains(property.Key.ToLower()))
                {
                    Assert.Fail($"The property {property.Key} must not be in the new object");
                }

                var propertyValue = typeof(UserInfo).GetTypeInfo().GetProperties()
                    .Where(p => p.Name.ToLower() == property.Key.ToLower()).FirstOrDefault().GetValue(user);

                if (property.Value != propertyValue)
                {
                    Assert.Fail($"The value of the property {property.Key} is incorrect. " +
                        $"Expected {propertyValue}, found {property.Value}");
                }
            }

            Assert.Pass();
        }
    }
}
