using GenericAPI.Services.Abstractions;
using GenericAPI.Services.Models.RequestModels;
using GenericAPI.Services.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
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

        public async Task<BetWinResponse> BetWin(BetWinRequest request)
        {
            var player = _playerGrainFactory.GetGrain(request.UserId);

            try
            {
                if (_ongoingTransactions.ContainsKey(request.TransactionId))
                {
                    return new BetWinResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Another Transaction With this Transaction Id is Ongoing!"
                    };
                }
                var response = await player.BetWin(request);
                return new BetWinResponse
                {
                    Message = response.Message,
                    StatusCode = response.StatusCode,
                    Balance = response.Balance
                };
            }
            finally
            {
                _ongoingTransactions.TryRemove(request.UserId, out _);
            }
        }

        public async Task<WinResponse> AddToBalance(WinRequest request)
        {
            var player = _playerGrainFactory.GetGrain(request.UserId);
            try
            {
                if (_ongoingTransactions.ContainsKey(request.TransactionId))
                {
                    return new WinResponse
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Another Transaction With this Transaction Id is Ongoing!"
                    };
                }
                var response = await player.AddToBalance(request);
                return new WinResponse
                {
                    Message = response.Message,
                    StatusCode = response.StatusCode,
                    Balance = response.Balance
                };
            }
            finally
            {
                _ongoingTransactions.TryRemove(request.UserId, out _);
            }
        }

        public async Task<DeductBalanceResponce> DeductFromBalance(BetRequest request)
        {
            var player = _playerGrainFactory.GetGrain(request.UserId);

            try
            {
                if (_ongoingTransactions.ContainsKey(request.TransactionId))
                {
                    return new DeductBalanceResponce
                    {
                        StatusCode = StatusCodes.Status400BadRequest,
                        Message = "Another Transaction With this Transaction Id is Ongoing!"
                    };
                }
                var response = await player.DeductBalance(request);
                return new DeductBalanceResponce
                {
                    Message = response.Message,
                    StatusCode = response.StatusCode,
                    Balance = response.Balance
                };
            }
            finally
            {
                _ongoingTransactions.TryRemove(request.UserId, out _);
            }
        }

        public async Task<decimal> GetPlayerBalance(int playerId)
        {
            var playerGrain = _playerGrainFactory.GetGrain(playerId);
            return await playerGrain.GetPlayerBalance();
        }
    }
}
