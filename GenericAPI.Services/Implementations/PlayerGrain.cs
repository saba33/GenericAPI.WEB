using GenericAPI.Domain.Models;
using GenericAPI.Services.Abstractions;
using GenericAPI.Services.Helper;
using Microsoft.Extensions.Configuration;
using Orleans.Concurrency;
using Orleans.Transactions.Abstractions;

namespace GenericAPI.Services.Implementations
{
    [Reentrant]
    public class PlayerGrain : Grain, IPlayerGrain
    {
        private PlayerState playerState;
        private readonly ITransactionalState<PlayerState> _transactionalPlayerState;
        private readonly IConfiguration _configuration;

        public PlayerGrain(IConfiguration configuration,
              [TransactionalState(nameof(balance))]
                ITransactionalState<PlayerState> balance)
        {
            _configuration = configuration;
            playerState = new PlayerState();
            _transactionalPlayerState = balance ?? throw new ArgumentNullException(nameof(balance));
        }

        public Task<decimal> AddToBalance(decimal amount)
        {
            var playerId = (int)this.GetPrimaryKeyLong();

            if (playerId <= 0)
            {
                throw new ArgumentException("Invalid player ID.");
            }
            if (amount <= 0)
            {
                throw new ArgumentException("Invalid amount.");
            }

            _transactionalPlayerState.PerformUpdate(playerState =>
            {
                playerState.Balance += amount;
                playerState.Version++;
                //player.Result.Balance += amount;
                //player.Result.Version++;
                //_playerRepository.Update(player.Result);
            });

            return Task.FromResult(playerState.Balance);

        }
        [Transaction(TransactionOption.Join)]
        public async Task<decimal> DeductBalance(decimal amount)
        {
            var playerId = (int)this.GetPrimaryKeyLong();

            if (playerId <= 0)
            {
                throw new ArgumentException("Invalid player ID.");
            }

            if (amount <= 0)
            {
                throw new ArgumentException("Invalid amount.");
            }

            var currentBalance = await this.GetPlayerBalance();

            if (currentBalance < amount)
            {
                throw new Exception("Insufficient balance to make the deduction.");
            }

            var connectionString = _configuration.GetSection("ConnectionStrings").GetValue<string>("connectionString");
            var repository = new AdoNetRepository<Transaction>(connectionString);
            var tableName = "Transaction";
            var record = new Transaction
            {
                UserId = playerId,
                BetAmount = amount,
                TransactionTime = DateTime.Now,
            };

            await _transactionalPlayerState.PerformUpdate(async state =>
            {
                playerState.Balance -= amount;
                playerState.Version++;
                var affectedRows = await repository.InsertRecord(tableName, record);
                await Task.CompletedTask;
            });

            return playerState.Balance;
        }

        public Task<decimal> GetPlayerBalance()
        {
            var playerId = (int)this.GetPrimaryKeyLong();
            return _transactionalPlayerState.PerformRead(balance => balance.Balance);
        }
    }
}
