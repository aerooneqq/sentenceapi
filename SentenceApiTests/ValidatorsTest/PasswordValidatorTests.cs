using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using SentenceAPI.Validators;
using SharedLibrary.KernelInterfaces;

namespace SentenceApiTests.ValidatorsTest
{
    [TestFixture]
    class PasswordValidatorTests
    {
        private IValidator passwordValidator;

        static string[] WrongTestCases => new[] 
        {
            "a",
            "asd2",
            "qwerty",
            "AS2sdas",
            "asdas",
            "",
            null,
            "null"
        };

        static string[] RightTestCases => new[]
        {
            "AeroOne123",
            "AeroAero123",
            "aASDSDASDASDASDASD1",
            "qwertY123"
        };

        [Test]
        [TestCaseSource("WrongTestCases")]
        public void TestWrongCases(string password)
        {
            passwordValidator = new PasswordValidator(password);

            Assert.IsFalse(passwordValidator.Validate().result);
        }

        [Test]
        [TestCaseSource("RightTestCases")]
        public void TestRightCases(string password)
        {
            passwordValidator = new PasswordValidator(password);

            Assert.IsTrue(passwordValidator.Validate().result);
        }
    }
}
