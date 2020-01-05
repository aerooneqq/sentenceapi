using MongoDB.Bson;
using SentenceAPI.Features.Codes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Codes.Interfaces
{
    interface ICodesService
    {
        ActivationCode CreateActivationCode(ObjectId userID);

        Task InsertCodeInDatabaseAsync(ActivationCode activationCode);
        Task ActivateCodeAsync(string code);
    }
}
