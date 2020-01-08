using System.Threading.Tasks;
using MongoDB.Bson;
using SentenceAPI.Events.Interfaces;
using SentenceAPI.Features.Codes.Interfaces;
using SentenceAPI.Features.Codes.Models;
using SentenceAPI.Features.Email.Interfaces;
using SentenceAPI.Features.Links.Interfaces;
using SentenceAPI.Features.UserFriends.Interfaces;
using SentenceAPI.Features.Users.Models;
using SentenceAPI.Features.Workplace.DocumentsStorage.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

namespace SentenceAPI.Features.Users.Events
{
    /// <summary>
    /// Does all support job for the user creation (sending an email, inserting code in the database,
    /// creates the main folder objects and user friends recorcd in the database)
    /// </summary>
    public class UserCreatedEvent : IDomainEvent
    {
        #region Services
        private readonly IUserFriendsService userFriendsService;
        private readonly ICodesService codesService;
        private readonly ILinkService linkService;
        private readonly IEmailService emailService;
        private readonly IUserMainFoldersService userMainFoldersService;
        #endregion

        private readonly UserInfo user;


        public UserCreatedEvent(IFactoriesManager factoriesManager, UserInfo user)
        {
            factoriesManager.GetService<ILinkService>().TryGetTarget(out linkService);
            factoriesManager.GetService<IEmailService>().TryGetTarget(out emailService);
            factoriesManager.GetService<IUserFriendsService>().TryGetTarget(out userFriendsService);
            factoriesManager.GetService<ICodesService>().TryGetTarget(out codesService);
            factoriesManager.GetService<IUserMainFoldersService>().TryGetTarget(out userMainFoldersService);

            this.user = user;
        }


        public async Task Handle()
        {
            await userFriendsService.CreateUserFriendsRecord(user.ID).ConfigureAwait(false);

            ActivationCode activationCode = codesService.CreateActivationCode(user.ID);
            await codesService.InsertCodeInDatabaseAsync(activationCode).ConfigureAwait(false);

            await emailService.SendConfirmationEmailAsync(activationCode.Code, user.Email).ConfigureAwait(false);

            await userMainFoldersService.CreateNewUserMainFolders(user.ID).ConfigureAwait(false);
        }
    }
}