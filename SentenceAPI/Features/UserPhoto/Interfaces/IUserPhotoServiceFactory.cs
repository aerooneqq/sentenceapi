using SentenceAPI.Features.UserPhoto.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using SharedLibrary.KernelInterfaces;

namespace SentenceAPI.Features.UserPhoto.Interfaces
{
    interface IUserPhotoServiceFactory : IServiceFactory
    {
        IUserPhotoService GetService();
    }
}
