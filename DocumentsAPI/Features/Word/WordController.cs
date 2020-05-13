using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Application.Links.Interfaces;
using Application.Requests.Interfaces;
using Application.Templates;
using Application.Templates.Interfaces;
using Application.Tokens.Interfaces;
using Application.Word.Interfaces;
using Application.Word.RenderParams;
using DataAccessLayer.Exceptions;
using Domain.Links;
using Domain.Logs;
using Domain.Logs.Configuration;
using Domain.Templates;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using MongoDB.Bson;
using Newtonsoft.Json;
using SharedLibrary.ActionResults;
using SharedLibrary.FactoriesManager.Interfaces;
using SharedLibrary.Loggers.Interfaces;


namespace DocumentsAPI.Features.Word
{
    [ApiController, Route("documentsapi/[controller]")]
    public class WordController : ControllerBase
    {
        #region Services
        private readonly ILogger<ApplicationError> exceptionLogger;
        private readonly IWordService wordService;
        private readonly IRequestService requestService;
        private readonly ITokenService tokenService;
        private readonly ILinkService linkService;
        #endregion


        private readonly LogConfiguration logConfiguration;


        public WordController(IFactoriesManager factoriesManager)
        {
            factoriesManager.GetService<ILogger<ApplicationError>>().TryGetTarget(out exceptionLogger);
            factoriesManager.GetService<IWordService>().TryGetTarget(out wordService);
            factoriesManager.GetService<IRequestService>().TryGetTarget(out requestService);
            factoriesManager.GetService<ITokenService>().TryGetTarget(out tokenService);
            factoriesManager.GetService<ILinkService>().TryGetTarget(out linkService);

            logConfiguration = new LogConfiguration(GetType());
        }

        [HttpGet("link")]
        public async Task<IActionResult> GetLinkToDownload([FromQuery]string documentID)
        {
            try
            {
                ObjectId documentObjectID = ObjectId.Parse(documentID);
                ObjectId userID = ObjectId.Parse(tokenService.GetTokenClaim(requestService.GetToken(Request), "ID"));

                var wordLink = await linkService.CreateWordDownloadLink(documentObjectID, userID)
                    .ConfigureAwait(false);
                
                return new OkJson<WordDownloadLink>(wordLink);
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("Id was not in correct format");
            }
            catch (ArgumentException ex)
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
        
        
        [HttpGet("wordDocument")]
        public async Task<IActionResult> RenderDocument([FromQuery] string downloadLinkID)
        {
            try
            {
                ObjectId downloadLinkObjectID = ObjectId.Parse(downloadLinkID);
                RenderSettings renderSettings = new RenderSettings()
                {
                    DefaultColor = "#000000",
                    FontFamily = "Times New Roman",
                    DefaultTextSize = "24",
                };

                WordDownloadLink wordDownloadLink = await linkService.GetUnusedDownloadLink(downloadLinkObjectID);
                await linkService.MarkWordLinkAsUsed(wordDownloadLink.ID).ConfigureAwait(false);
                
                byte[] wordFile = await wordService.Render(wordDownloadLink.DocumentID, renderSettings)
                    .ConfigureAwait(false);

                return File(wordFile, "application/vnd.openxmlformats-officedocument.wordprocessingml.document;charset=UTF-8", "word.docx");
            }
            catch (FormatException)
            {
                return new BadSentRequest<string>("Id was not in correct format");
            }
            catch (ArgumentException ex)
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