using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserPhoto.Interfaces
{
    interface IUserPhotoService
    {
        #region Properties
        IMemoryCache MemoryCache { set; }
        #endregion

        #region Methods
        Task<Models.UserPhoto> GetPhoto(long userID);
        Task<Models.UserPhoto> GetPhoto(string token);

        Task UpdatePhoto(Models.UserPhoto userPhoto);

        Task CreateUserPhoto(long userID);
        #endregion
    }
}
