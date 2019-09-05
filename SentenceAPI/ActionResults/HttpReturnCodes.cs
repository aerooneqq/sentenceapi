using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SentenceAPI.ActionResults
{
    public enum HttpResponseCodes : int
    {
        #region OK codes
        OK = 200,
        Created = 201,
        NoContent = 204,
        #endregion

        #region Client error codes
        BadRequest = 400,
        Unauthorized = 401,
        NotFound = 404,
        BadSendedRequest = 452,
        #endregion

        #region Server error codes
        InternalServerError = 500,
        #endregion
    }
}
