using System.IO;
using System.Threading.Tasks;
using Application.Word.RenderParams;
using Domain.KernelInterfaces;
using MongoDB.Bson;

namespace Application.Word.Interfaces
{
    public interface  IWordService : IService
    {
        Task<byte[]> Render(ObjectId documentID, RenderSettings renderSettings);
    }
}