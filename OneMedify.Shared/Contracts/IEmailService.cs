using OneMedify.DTO.Response;
using System.Threading.Tasks;

namespace OneMedify.Shared.Contracts
{
    public interface IEmailService
    {
        Task<ResponseDto> SendEmailAsync(string to, string subject, string body);
    }
}
