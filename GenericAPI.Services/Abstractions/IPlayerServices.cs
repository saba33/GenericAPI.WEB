using GenericAPI.Services.Models.RequestModels;

namespace GenericAPI.Services.Abstractions
{
    public interface IPlayerServices
    {
        Task<decimal> GetPlayerBalance(int playerId);
        [Transaction(TransactionOption.Join)]
        Task DeductFromBalance(BetRequest request);
        Task AddToBalance(WinRequest request);
        //Task<LaunchGameResponse> LaunchGame(LaunchGameRequest request);
    }
}
