using MongoDB.Bson;

using System.Threading.Tasks;

using Domain.Codes;


namespace Application.Codes.Interfaces
{
    public interface ICodesService
    {
        ActivationCode CreateActivationCode(ObjectId userID);

        Task InsertCodeInDatabaseAsync(ActivationCode activationCode);
        Task ActivateCodeAsync(string code);
    }
}
