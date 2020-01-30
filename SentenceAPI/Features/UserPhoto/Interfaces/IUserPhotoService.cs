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
        Task<Domain.UserPhoto.UserPhoto> GetPhotoAsync(ObjectId userID);
        Task<Domain.UserPhoto.UserPhoto> GetPhotoAsync(string token);
        Task<byte[]> GetRawPhotoAsync(ObjectId id);
        
        Task<ObjectId> UpdatePhotoAsync(Domain.UserPhoto.UserPhoto userPhoto, byte[] newPhoto, string fileName);

        Task CreateUserPhotoAsync(ObjectId userID);
        Task InsertUserPhotoModel(Domain.UserPhoto.UserPhoto userPhoto);

        string GetUserPhotoName(ObjectId userID)
        {
            return $"UserPhoto_{userID}";
        }
        #endregion
    }
}
