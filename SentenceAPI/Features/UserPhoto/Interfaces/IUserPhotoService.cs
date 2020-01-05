using Microsoft.Extensions.Caching.Memory;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.UserPhoto.Interfaces
{
    interface IUserPhotoService
    {
        #region Methods
        Task<Models.UserPhoto> GetPhotoAsync(ObjectId userID);
        Task<Models.UserPhoto> GetPhotoAsync(string token);
        Task<byte[]> GetRawPhotoAsync(ObjectId id);
        
        Task<ObjectId> UpdatePhotoAsync(Models.UserPhoto userPhoto, byte[] newPhoto, string fileName);

        Task CreateUserPhotoAsync(ObjectId userID);
        Task InsertUserPhotoModel(Models.UserPhoto userPhoto);

        string GetUserPhotoName(ObjectId userID)
        {
            return $"UserPhoto_{userID}";
        }
        #endregion
    }
}
