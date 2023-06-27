using GenericAPI.Services.Models.ResponseModels;

namespace GenericAPI.Services.Models.AuthServiceModels.ResponseModel
{
    public class RegisterResponse : BaseResponse
    {
        public string Token { get; set; }
    }
}
