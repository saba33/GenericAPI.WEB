using GenericAPI.Services.Abstractions;
using GenericAPI.Services.Models.RequestModels;
using GenericAPI.Services.Models.ResponseModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace GenericAPI.WEB.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class GenericAPIController : ControllerBase
    {
        private readonly IPlayerServices _playerServices;
        private readonly ILaunchGameService _launchService;
        public GenericAPIController(IPlayerServices playerServices, ILaunchGameService launchService)
        {
            _playerServices = playerServices;
            _launchService = launchService;
        }
        [HttpPost(nameof(LaunchGame))]
        public async Task<ActionResult<LaunchGameResponse>> LaunchGame(LaunchGameRequest request)
        {
            var result = await _launchService.LaunchProviderGame(request.GameId);
            return new LaunchGameResponse
            {
                URL = result.URL,
                Message = result.Message,
                StatusCode = result.StatusCode
            };
        }

        [HttpPost(nameof(Bet))]
        public async Task<ActionResult<DeductBalanceResponce>> Bet(BetRequest request)
        {
            var response = await _playerServices.DeductFromBalance(request);
            return Ok(response);
        }

        [HttpPost(nameof(Win))]
        public async Task<ActionResult<WinResponse>> Win(WinRequest request)
        {
            var response = await _playerServices.AddToBalance(request);
            return Ok();
        }


        [HttpPost(nameof(BetWin))]
        public async Task<ActionResult<BetWinResponse>> BetWin(BetWinRequest request)
        {
            var result = await _playerServices.BetWin(request);
            return Ok(result);
        }

        [HttpGet(nameof(GetBalance))]
        public async Task<ActionResult<GetBalanceResponse>> GetBalance(int playerId)
        {
            decimal balance = await _playerServices.GetPlayerBalance(playerId);
            return Ok(new GetBalanceResponse { Message = $"balance = {balance}", StatusCode = StatusCodes.Status200OK, Balance = balance });
        }

    }
}
