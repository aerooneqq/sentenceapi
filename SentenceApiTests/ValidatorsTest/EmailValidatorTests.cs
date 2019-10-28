using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using SharedLibrary.KernelInterfaces;
using SentenceAPI.Validators;

namespace SentenceApiTests.ValidatorsTest
{
    [TestFixture]
    class EmailValidatorTests
    {
        private IValidator emailValidator;

        
        [Test]
        [TestCase("sadlkjasldasd")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("@asdasd")]
        [TestCase("asdasdasd@")]
        public void TestWrongEmails(string email)
        {
            emailValidator = new EmailValidator(email);

            Assert.IsFalse(emailValidator.Validate().result);
        }

        [Test]
        [TestCase("aerroneq@yandex.ru")]
        [TestCase("asd-23asd-asd@sdadasd.sasdsad")]
        public void TestRightEmails(string email)
        {
            emailValidator = new EmailValidator(email);

            Assert.IsTrue(emailValidator.Validate().result);
        }
    }
}
