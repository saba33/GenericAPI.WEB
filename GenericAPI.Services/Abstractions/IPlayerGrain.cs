namespace GenericAPI.Services.Abstractions
{
    public interface IPlayerGrain : IGrainWithIntegerKey
    {
        [Transaction(TransactionOption.CreateOrJoin)]
        Task<decimal> GetPlayerBalance();
        [Transaction(TransactionOption.CreateOrJoin)]
        Task<decimal> DeductBalance(decimal amount);
        [Transaction(TransactionOption.CreateOrJoin)]
        Task<decimal> AddToBalance(decimal amount);
    }
}
