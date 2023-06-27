using GenericAPI.Services.Abstractions;
using GenericAPI.Services.Models.RequestModels;
using System.Collections.Concurrent;
namespace GenericAPI.Services.Implementations
{
    public class PlayerServices : IPlayerServices
    {
        private readonly IPlayerGrainFactory _playerGrainFactory;
        private readonly ConcurrentDictionary<long, Task> _ongoingTransactions;
        public PlayerServices(IPlayerGrainFactory playerGrainFactory)
        {
            _playerGrainFactory = playerGrainFactory;
            _ongoingTransactions = new ConcurrentDictionary<long, Task>();
        }

        //public async Task<LaunchGameResponse> LaunchGame(LaunchGameRequest request)
        //{

        //}
        public async Task AddToBalance(WinRequest request)
        {
            var player = _playerGrainFactory.GetGrain(request.UserId);
            try
            {
                if (_ongoingTransactions.ContainsKey(request.TransactionId))
                {
                    return;
                }
                await player.AddToBalance(request.Amount);
            }
            finally
            {
                _ongoingTransactions.TryRemove(request.UserId, out _);
            }
        }

        public async Task DeductFromBalance(BetRequest request)
        {
            var player = _playerGrainFactory.GetGrain(request.UserId);

            try
            {
                if (_ongoingTransactions.ContainsKey(request.TransactionId))
                {
                    return;
                }
                await player.DeductBalance(request.Amount);
            }
            finally
            {
                _ongoingTransactions.TryRemove(request.UserId, out _);
            }
            //deactivate ongoingtransaction delete

        }

        public async Task<decimal> GetPlayerBalance(int playerId)
        {
            var playerGrain = _playerGrainFactory.GetGrain(playerId);
            return await playerGrain.GetPlayerBalance();
        }
    }
}
