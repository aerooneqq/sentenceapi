using Microsoft.AspNetCore.Http;
using SentenceAPI.Features.Response.Interfaces;

namespace SentenceAPI.Features.Response.Services
{
    public class ResponseService : IResponseService
    {
        #region Fields
        private int status;
        private string contentType;
        private int contentLength;
        private string content;
        #endregion

        #region Constructors
        public ResponseService()
        {
        }
        #endregion

        #region IResponseService implementation
        public void ConfigureResponse(HttpResponse response)
        {
            response.ContentType = contentType;
            response.StatusCode = status;
        }

        public IResponseService SetContent(string content)
        {
            this.content = content;
            return this;
        }

        public IResponseService SetContentLength(int contentLength)
        {
            this.contentLength = contentLength;
            return this;
        }

        public IResponseService SetContentType(string contentType)
        {
            this.contentType = contentType;
            return this;
        }

        public IResponseService SetStatus(int status)
        {
            this.status = status;
            return this;
        }
        #endregion
    }
}
