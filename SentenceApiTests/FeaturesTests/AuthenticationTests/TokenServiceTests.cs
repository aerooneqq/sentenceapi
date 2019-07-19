using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.Authentication.Services;
using SentenceAPI.Features.Users.Models;

namespace SentenceApiTests.FeaturesTests.AuthenticationTests
{
    [TestFixture]
    class TokenServiceTests
    {
        private ITokenService tokenService;

        [SetUp]
        public void SetUp()
        {
            tokenService = new TokenService();
        }

        [Test]
        public void TestService()
        {
            UserInfo user = new UserInfo()
            {
                Email = "aerooneQ@yandex.ru",
                Password = "AeroOne123",
                Login = "Aero"
            };

            var tokenStr = tokenService.CreateEncodedToken(user);
            var handler = new JwtSecurityTokenHandler();
            var token = handler.ReadToken(tokenStr) as JwtSecurityToken;

            List<Claim> claims = token.Claims.ToList();

            if (claims[0].Value != user.Email)
            {
                Assert.Fail();
            }
            if (claims[1].Value != user.Login)
            {
                Assert.Fail();
            }

            Assert.Pass();
        }

        [Test]
        public void TestLifeTimeDel()
        {
            var lifeTimeDel = tokenService.GetLifeTimeValidationDel();
            if (lifeTimeDel.GetType() != typeof(LifetimeValidator))
            {
                Assert.Fail();
            }

            Assert.Pass();
        }
    }
}
