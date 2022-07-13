using OneMedify.DTO.Response;
using OneMedify.DTO.User;
using System.Threading.Tasks;

namespace OneMedify.Shared.Contracts
{
    public interface IOneAuthorityService
    {
        Task<ResponseDto> RegisterUser(UserRegisterModel authorityRegisterModel);
    }
}
