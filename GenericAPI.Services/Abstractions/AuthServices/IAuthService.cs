using GenericAPI.Services.Models.AuthServiceModels.RequestModel;
using GenericAPI.Services.Models.AuthServiceModels.ResponseModel;

namespace GenericAPI.Services.Abstractions.AuthServices
{
    public interface IAuthService
    {
        Task<RegisterResponse> RegisterUserAsync(UserDto user);
        Task<LoginResponse> LoginUser(LoginModel request);
        Task<Dictionary<byte[], byte[]>> GetHashandSalt(string mail);
    }
}
