using SentenceAPI.Features.Codes.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.Features.Codes.Interfaces
{
    interface ICodesService
    {
        ActivationCode CreateActivationCode(long userID);

        Task InsertCodeInDatabase(ActivationCode activationCode);
        Task ActivateCode(string code);
    }
}
