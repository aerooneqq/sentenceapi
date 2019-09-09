using SentenceAPI.Features.UserPhoto.Interfaces;
using SentenceAPI.Features.UserPhoto.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserPhoto.Factories
{
    class UserPhotoServiceFactory : IUserPhotoServiceFactory
    {
        public IUserPhotoService GetService()
        {
            return new UserPhotoService();
        }
    }
}
