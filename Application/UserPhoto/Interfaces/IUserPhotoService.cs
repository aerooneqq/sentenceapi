using MongoDB.Bson;

using System.Threading.Tasks;


namespace Application.UserPhoto.Interfaces
{
    public interface IUserPhotoService
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
