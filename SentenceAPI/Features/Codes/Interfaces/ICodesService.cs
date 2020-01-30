using MongoDB.Bson;

using System.Threading.Tasks;

using Domain.Codes;


namespace SentenceAPI.Features.Codes.Interfaces
{
    interface ICodesService
    {
        ActivationCode CreateActivationCode(ObjectId userID);

        Task InsertCodeInDatabaseAsync(ActivationCode activationCode);
        Task ActivateCodeAsync(string code);
    }
}
