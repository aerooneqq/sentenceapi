using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Application.Documents.DocumentStructure.Exceptions;
using Application.Documents.DocumentStructure.Interfaces;
using Application.Documents.DocumentStructure.Models;
using Application.Tokens.Interfaces;

using DataAccessLayer.Exceptions;

using DocumentsAPI.Features.DocumentStructure.Validators;

using Domain.DocumentStructureModels;
using Domain.Logs;
using Domain.Logs.Configuration;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;

using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;
using Application.Requests.Interfaces;

namespace DocumentsAPI.Features.DocumentStructure 
{
    [ApiController, Route("documentsapi/[controller]"), Authorize]
    public class DocumentStructureController : Controller 
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IDocumentStructureService documentStructureService;
        private readonly IRequestService requestService;
        private readonly ITokenService tokenService;
        #endregion

        private readonly LogConfiguration logConfiguration;

        
        public DocumentStructureController(IFactoriesManager factoriesManager)
        { 
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);

            logConfiguration = new LogConfiguration(GetType());

            factoriesManager.GetService<IDocumentStructureService>().TryGetTarget(out documentStructureService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
        }


        [HttpGet]
        public async Task<IActionResult> GetDocumentStructure([FromQuery]string documentID)
        { 
            try 
            { 
                if (documentID is null) 
                    return new BadSentRequest<string>("Document id can not be null");

                ObjectId documentObjectId = ObjectId.Parse(documentID);

                var documentStructure = await documentStructureService.GetDocumentStructureAsync(documentObjectId)
                    .ConfigureAwait(false);

                if (documentStructure is null)
                    return new BadSentRequest<string>("There is no structure for this document");

                return new OkJson<DocumentStructureModel>(documentStructure);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }


        [HttpDelete("item")]
        public async Task<IActionResult> DeleteItemFromStructure([FromQuery]string documentStructureID,
                                                                 [FromQuery]string itemToDeleteID) 
        {
            try
            {
                ObjectId documentObjectID = ObjectId.Parse(documentStructureID);
                ObjectId itemToDeleteObjectID = ObjectId.Parse(itemToDeleteID);

                var documentStructure = await documentStructureService.GetStructureByID(documentObjectID)
                    .ConfigureAwait(false);

                if (documentStructure is null)
                    return new BadSentRequest<string>("Incorrect document ID");

                FindParentItemRecursive(itemToDeleteObjectID, documentStructure.Items[0], out Item parentItem);

                if (parentItem is null)
                    return new BadSentRequest<string>("Incorrect item ID");

                DeleteItemFromParentItem(parentItem, itemToDeleteObjectID);

                await documentStructureService.UpdateDocumentStructureAsync(documentStructure).ConfigureAwait(false);

                return new Ok();
            }
            catch (DatabaseException ex) 
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }

        private void DeleteItemFromParentItem(Item parentItem, ObjectId itemToDeleteID) =>
            parentItem.Items.Remove(parentItem.Items.Find(item => item.ID == itemToDeleteID));

        private void FindParentItemRecursive(ObjectId itemID, Item parentItem, out Item searchResult) 
        {
            searchResult = null;

            if (parentItem is null)
                return;

            foreach (Item item in parentItem.Items)
            {
                if (item.ID == itemID)
                {
                    searchResult = parentItem;
                    return;
                }

                if (searchResult is null)
                    FindParentItemRecursive(itemID, item, out searchResult);
            }
        }


        [HttpPut("item/replacement")]
        public async Task<IActionResult> MoveOneItemToAnother([FromQuery]string itemToMoveID,
                                                              [FromQuery]string destinationItemID,
                                                              [FromQuery]string documentStructureID) 
        {
            try
            {
                ObjectId itemToMoveObjectID = ObjectId.Parse(itemToMoveID);
                ObjectId destinationItemObjectID = ObjectId.Parse(destinationItemID);
                ObjectId documentStructureObjectID = ObjectId.Parse(documentStructureID);

                var documentStructure = await documentStructureService.GetStructureByID(documentStructureObjectID)
                    .ConfigureAwait(false);

                if (documentStructure is null)
                    return new BadSentRequest<string>("The structure with such an id does not exist");

                FindParentItemRecursive(itemToMoveObjectID, documentStructure.Items[0], out Item parentItem);
                FindItemRecursive(destinationItemObjectID, documentStructure.Items, out Item destinationItem);

                if (parentItem is null || destinationItem is null)
                    return new BadSentRequest<string>("Incorrect item selection");

                documentStructureService.MoveItemToDestination(documentStructure, parentItem, itemToMoveObjectID, destinationItem);

                return new Ok();
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("Bad id format");
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }

        private void FindItemRecursive(ObjectId itemID, IEnumerable<Item> items, out Item seatchResult)
        {
            seatchResult = null;

            if (items is null)
                return;

            foreach (Item item in items)
            {
                if (item.ID == itemID)
                {
                    seatchResult = item;
                    return;
                }

                if (seatchResult is null)
                    FindItemRecursive(itemID, item.Items, out seatchResult);
            }
        }


        [HttpPut]
        public async Task<IActionResult> PutItemInDocumentStructure()
        {
            try 
            {
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));
                var itemUpdateDto = await requestService.GetRequestBody<ItemUpdateDto>(Request)
                    .ConfigureAwait(false);

                if (itemUpdateDto is null)
                    return new BadSentRequest<string>("Update info was sent in a bad format");

                var documentStructure = await documentStructureService.GetStructureByID(
                    itemUpdateDto.ParentDocumentStructureID).ConfigureAwait(false);

                if (documentStructure is null)
                    return new BadSentRequest<string>("Document structure with given ID does not exist");

                var validationResult = new ItemUpdateDtoValidator(itemUpdateDto, documentStructure).Validate();
                if (!validationResult.result)
                {   
                    #warning Add logging for validation errors
                    return new BadSentRequest<string>(validationResult.errorMessage);
                }

                await documentStructureService.UpdateStructureAsync(documentStructure, itemUpdateDto, userID).
                    ConfigureAwait(false);

                return new Ok();
            }
            catch (ItemNotFoundException ex)
            {
                return new BadSentRequest<string>(ex.Message);
            }
            catch (DatabaseException ex)
            {
                return new InternalServerError(ex.Message);
            }
            catch (Exception ex)
            {
                exceptionLogger.Log(new ApplicationError(ex), LogLevel.Error, logConfiguration);
                return new InternalServerError();
            }
        }
    }
}