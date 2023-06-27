using GenericAPI.Services.Models.RequestModels;
using GenericAPI.Services.Models.ResponseModels;

namespace GenericAPI.Services.Abstractions
{
    public interface IPlayerServices
    {
        Task<decimal> GetPlayerBalance(int playerId);
        [Transaction(TransactionOption.Join)]
        Task<DeductBalanceResponce> DeductFromBalance(BetRequest request);
        Task<WinResponse> AddToBalance(WinRequest request);
        Task<BetWinResponse> BetWin(BetWinRequest request);
    }
}
