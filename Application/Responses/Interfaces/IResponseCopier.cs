using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Application.Responses.Interfaces
{
    /// <summary>
    /// Copies the HttpWebResponse content to HttpResponse object.
    /// Method COPY Must be called first
    /// </summary>
    public interface IResponseCopier
    {
        IResponseCopier Copy(HttpWebResponse response);
        Task To(HttpResponse response);
    }
}