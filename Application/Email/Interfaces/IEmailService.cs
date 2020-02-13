using System.Threading.Tasks;

using Domain.KernelInterfaces;


namespace Application.Email.Interfaces
{
    public interface IEmailService : IService
    {
        Task SendConfirmationEmailAsync(string code, string email);
    }
}
