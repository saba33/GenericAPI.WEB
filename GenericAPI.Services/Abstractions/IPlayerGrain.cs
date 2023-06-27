using GenericAPI.Services.Models.RequestModels;
using GenericAPI.Services.Models.ResponseModels;

namespace GenericAPI.Services.Abstractions
{
    public interface IPlayerGrain : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.CreateOrJoin)]
        Task<decimal> GetPlayerBalance();

        [Transaction(TransactionOption.CreateOrJoin)]
        Task<DeductBalanceResponce> DeductBalance(BetRequest request);

        [Transaction(TransactionOption.CreateOrJoin)]
        Task<WinResponse> AddToBalance(WinRequest request);
        [Transaction(TransactionOption.CreateOrJoin)]
        Task<BetWinResponse> BetWin(BetWinRequest request);
    }
}
