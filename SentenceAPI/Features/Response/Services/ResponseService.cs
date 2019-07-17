using Microsoft.AspNetCore.Http;
using SentenceAPI.Features.Response.Interfaces;

namespace SentenceAPI.Features.Response.Services
{
    public class ResponseService : IResponseService
    {
        #region Field
        private int status;
        private string contentType;
        #endregion

        #region Constructors
        public ResponseService()
        {
            status = 200;
            contentType = "application/json";
        }

        public ResponseService(int status, string contentType)
        {
            this.status = status;
            this.contentType = contentType;
        }
        #endregion


        public void ConfigureResponse(HttpResponse response)
        {
            response.ContentType = contentType;
            response.StatusCode = status;
        }
    }
}
