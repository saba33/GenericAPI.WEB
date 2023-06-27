using GenericAPI.Services.Models.ResponseModels;

namespace GenericAPI.Services.Models.AuthServiceModels.ResponseModel
{
    public class LoginResponse : BaseResponse
    {
        public string Token { get; set; }
    }
}
