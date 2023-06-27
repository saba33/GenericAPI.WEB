using GenericAPI.Services.Models.ResponseModels;

namespace GenericAPI.Services.Abstractions
{
    public interface ILaunchGameService
    {
        public Task<int> GetProvider(int GameId);
        Task<LaunchGameResponse> LaunchProviderGame(int gameId);
    }
}
