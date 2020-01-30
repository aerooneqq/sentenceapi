using System.Threading.Tasks;

using Domain.KernelInterfaces;


namespace SentenceAPI.Features.Email.Interfaces
{
    public interface IEmailService : IService
    {
        Task SendConfirmationEmailAsync(string code, string email);
    }
}
