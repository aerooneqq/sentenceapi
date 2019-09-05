using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

using NUnit.Framework;

using DataAccessLayer.Hashes;

namespace DataAccessLayerTests.Tests.HashesTests
{
    [TestFixture]
    public class HashesTests
    {
        static string[] TestCases = new[]
        {
            "asdadasasd",
            "AeroOne123",
            "213123123323232",
            "q",
            ""
        };

        [Test]
        [TestCaseSource("TestCases")]
        public void TestMD5Hash(string str)
        {
            string hash = GetMD5Hash(str);
            Assert.IsTrue(hash == str.GetMD5Hash());
        }

        private string GetMD5Hash(string str)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(str);

            MD5 md5Hash = MD5.Create();
            byte[] hash = md5Hash.ComputeHash(strBytes);

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hash)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
