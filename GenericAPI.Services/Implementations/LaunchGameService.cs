using GenericAPI.Domain.Models;
using GenericAPI.Repository.Abstractions;
using GenericAPI.Services.Abstractions;
using GenericAPI.Services.Helper;
using GenericAPI.Services.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace GenericAPI.Services.Implementations
{
    public class LaunchGameService : ILaunchGameService
    {
        private readonly IConfiguration _config;
        private readonly IBaseRepository<Game> _gameRepository;
        public LaunchGameService(IBaseRepository<Game> gameRepository, IConfiguration config)
        {
            _gameRepository = gameRepository;
            _config = config;
        }

        public async Task<int> GetProvider(int gameId)
        {
            var game = (await _gameRepository.FindAsync(g => g.Id == gameId))
               .FirstOrDefault();
            return game.ProviderId;
        }

        public async Task<LaunchGameResponse> LaunchProviderGame(int gameId)
        {
            int providerId = await GetProvider(gameId);
            var secret = _config.GetSection($"{gameId.ToString()}:Secret").ToString();
            var url = LaunchGameUrlGenerator.GenerateLaunchGameUrl(gameId, providerId, secret);

            return new LaunchGameResponse
            {
                Message = "Game Url is Generated!",
                StatusCode = StatusCodes.Status200OK,
                URL = url
            };
        }

    }
}
