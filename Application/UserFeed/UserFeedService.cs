﻿using SentenceAPI.Features.Authentication.Interfaces;
using SentenceAPI.Features.UserFeed.Interfaces;
using SentenceAPI.Features.UserFeed.Models;
using SentenceAPI.Features.UserFriends.Interfaces;
using SentenceAPI.Features.Users.Interfaces;
using SentenceAPI.Features.UserPhoto.Interfaces;

using SharedLibrary.Loggers.Interfaces;
using SharedLibrary.FactoriesManager.Interfaces;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using DataAccessLayer.CommonInterfaces;
using DataAccessLayer.Configuration.Interfaces;
using DataAccessLayer.Configuration;
using DataAccessLayer.Filters;
using DataAccessLayer.Exceptions;

using MongoDB.Bson;

using DataAccessLayer.DatabasesManager.Interfaces;

using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Users;


namespace SentenceAPI.Features.UserFeed.Services
{
    public class UserFeedService : IUserFeedService
    {
        #region Static fields
        private static readonly string databaseConfigFile = "./configs/mongo_database_config.json";
        #endregion


        #region Databases
        private readonly IDatabaseService<Domain.UserFeed.UserFeed> database;
        private IConfigurationBuilder configurationBuilder;
        #endregion


        #region Services
        private readonly IUserFriendsService userFriendsService;
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly ITokenService tokenService;
        private readonly IUserService<UserInfo> userService;
        private readonly IUserPhotoService userPhotoService;
        #endregion

        private readonly LogConfiguration logConfiguration;


        public UserFeedService(IFactoriesManager factoriesManager, IDatabaseManager databasesManager)
        {
            databasesManager.MongoDBFactory.GetDatabase<Domain.UserFeed.UserFeed>().TryGetTarget(out database);

            configurationBuilder = new MongoConfigurationBuilder(database.Configuration);
            configurationBuilder.SetConfigurationFilePath(databaseConfigFile).SetAuthMechanism()
                                .SetUserName().SetPassword().SetDatabaseName().SetServerName().SetConnectionString();

            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IUserFriendsService>().TryGetTarget(out userFriendsService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<IUserService<UserInfo>>().TryGetTarget(out userService);
            factoriesManager.GetService<IUserPhotoService>().TryGetTarget(out userPhotoService);

            logConfiguration = new LogConfiguration(GetType());
        }


        public async Task<UsersFeedDto> GetUserFeedAsync(ObjectId userID)
        {
            try
            {
                List<ObjectId> subscriptionsID = (await userFriendsService.GetSubscriptionsAsync(userID))
                    .Select(uf => uf.UserID).ToList();
                subscriptionsID.Add(userID);

                Dictionary<ObjectId, ObjectId> usersPhotoes = await GetUsersPhotoesAsync(subscriptionsID).ConfigureAwait(false);
                Dictionary<ObjectId, string> usersRawPhotoes = await GetUsersRawPhotoesAsync(usersPhotoes).ConfigureAwait(false);

                await database.Connect().ConfigureAwait(false);
                var usersFeed = await database.GetCombined(new InFilter<ObjectId>("userID", subscriptionsID), "userID",
                    (typeof(UserInfo), "_id", new[] { "name", "surname" })).ConfigureAwait(false);

                return new UsersFeedDto(usersFeed.ToList(), usersRawPhotoes);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while getting user feed");
            }
        }

        private async Task<Dictionary<ObjectId, string>> GetUsersRawPhotoesAsync(Dictionary<ObjectId, ObjectId> userPhotoes)
        {
            Dictionary<ObjectId, string> userRawPhotoes = new Dictionary<ObjectId, string>();

            foreach (var (userID, photoID) in userPhotoes)
            {
                byte[] currentPhoto = await userPhotoService.GetRawPhotoAsync(photoID).ConfigureAwait(false);
                
                userRawPhotoes.Add(userID, Convert.ToBase64String(currentPhoto));
            }

            return userRawPhotoes;
        }

        private async Task<Dictionary<ObjectId, ObjectId>> GetUsersPhotoesAsync(IEnumerable<ObjectId> userIDs)
        {
            Dictionary<ObjectId, ObjectId> userPhotoes = new Dictionary<ObjectId, ObjectId>();

            foreach (ObjectId userID in userIDs)
            {
                ObjectId id = (await userPhotoService.GetPhotoAsync(userID).ConfigureAwait(false)).CurrentPhotoID;
                userPhotoes.Add(userID, id);
            }

            return userPhotoes;
        }

        public async Task<UsersFeedDto> GetUserFeedAsync(string token)
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(token, "ID"));

                return await GetUserFeedAsync(userID);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while getting user feed");
            }
        }

        public async Task InsertUserPostAsync(Domain.UserFeed.UserFeed userFeed)
        {
            try
            {
                await database.Connect();
                await database.Insert(userFeed);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while inserting new post");
            }
        }

        public async Task InsertUserPostAsync(string token, string message)
        {
            try
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(token, "ID"));
                await InsertUserPostAsync(new Domain.UserFeed.UserFeed()
                {
                    UserID = userID,
                    Message = message,
                    PublicationDate = DateTime.Now,
                }).ConfigureAwait(false);
            }
            catch (Exception ex) when (ex.GetType() != typeof(DatabaseException))
            {
                 exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                throw new DatabaseException("Error occured while inserting new post");
            }
        }
    }
}
