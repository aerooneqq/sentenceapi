using SentenceAPI.Features.UserPhoto.Services;
using SentenceAPI.KernelInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserPhoto.Interfaces
{
    interface IUserPhotoServiceFactory : IServiceFactory
    {
        IUserPhotoService GetService();
    }
}
