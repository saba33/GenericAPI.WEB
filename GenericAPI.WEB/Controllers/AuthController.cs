using GenericAPI.Services.Abstractions.AuthServices;
using GenericAPI.Services.Models.AuthServiceModels.RequestModel;
using GenericAPI.Services.Models.AuthServiceModels.ResponseModel;
using Microsoft.AspNetCore.Mvc;

namespace GenericAPI.WEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService userService)
        {
            _authService = userService;
        }

        [HttpPost("Login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginModel request)
        {
            var result = await _authService.LoginUser(request);
            return Ok(result);
        }

        [HttpPost("Register")]
        public async Task<ActionResult<RegisterResponse>> Register(UserDto request)
        {
            var result = await _authService.RegisterUserAsync(request);
            return Ok(result);
        }
    }
}
