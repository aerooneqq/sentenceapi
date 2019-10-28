using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;

using SentenceAPI.Validators;
using SharedLibrary.KernelInterfaces;

namespace SentenceApiTests.ValidatorsTest
{
    [TestFixture]
    public class LoginValidatorsTests
    {
        private IValidator loginValidator;

        static string[] WrongTestCases => new[]
        {
            "asd",
            "asdasd",
            "asdasdasdasdasdasdasd",
            "ASLD",
            "asdklasdlasdalsd",
            "ASLDASJDKHJASKDHAJSKDHJAKSDSKD",
            "sakdkasdklsAKLSDJALKSDJKALSDJKALSDJAKLDj",
            "ASLDASDdklsdsladasdLDSLKSLDKASLDKLDKsldkasldLKD",
            "123123123123123123124124132",
            "123123123"
        };

        static string[] RightTestCases => new[]
        {
            "AeroOne123",
            "asdASdasdasDASDD1232",
            "asdA1sdadsdasd",
            "ASDASDASDASDd2",
            "AeroOnOne123"
        };

        [Test]
        [TestCaseSource("WrongTestCases")]
        public void TestWrongCases(string login)
        {
            loginValidator = new LoginValidator(login);

            Assert.IsFalse(loginValidator.Validate().result);
        }

        [Test]
        [TestCaseSource("RightTestCases")]
        public void TestRightCases(string login)
        {
            loginValidator = new LoginValidator(login);

            Assert.IsTrue(loginValidator.Validate().result);
        }
    }
}
