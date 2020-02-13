using System.Threading.Tasks;

using Application.Codes.Interfaces;
using Application.Email.Interfaces;
using Application.Links.Interfaces;
using Application.UserFriends.Interfaces;
using Application.Workplace.DocumentStorage.UserMainFoldersService.Interfaces;
using Domain.Codes;
using Domain.Users;

using SharedLibrary.Events.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;


namespace SentenceAPI.Features.Users.Events
{
    /// <summary>
    /// Does all support job for the user creation (sending an email, inserting code in the database,
    /// creates the main folder objects and user friends record in the database)
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

            await userMainFoldersService.CreateNewUserMainFolders(user.ID).ConfigureAwait(false);
            
            //await emailService.SendConfirmationEmailAsync(activationCode.Code, user.Email).ConfigureAwait(false);
        }
    }
}