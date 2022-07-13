using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Shared.Contracts
{
    public interface IRestServiceClient
    {
        Task<ResponseDto> InvokePostAsync<T, R>(string requestUri, T model, string token = null) where R : new();
    }
}
