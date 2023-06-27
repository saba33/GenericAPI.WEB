using GenericAPI.Domain.Models;
using GenericAPI.Services.Abstractions;
using GenericAPI.Services.Helper;
using GenericAPI.Services.Models.RequestModels;
using GenericAPI.Services.Models.ResponseModels;
using Microsoft.AspNetCore.Http;
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

        public async Task<WinResponse> AddToBalance(WinRequest request)
        {
            var playerId = (int)this.GetPrimaryKeyLong();

            if (playerId <= 0)
            {
                throw new ArgumentException("Invalid player ID.");
            }
            if (request.Win <= 0)
            {
                throw new ArgumentException("Invalid amount.");
            }
            var connectionString = _configuration.GetSection("ConnectionStrings").GetValue<string>("connectionString");
            var repository = new AdoNetRepository<Transaction>(connectionString);
            var tableName = "Transaction";

            var record = new Transaction
            {
                UserId = playerId,
                Win = request.Win,
                TransactionTime = DateTime.Now,
            };

            var currentBalance = await this.GetPlayerBalance();

            if (!repository.CheckIfTransactionExists(request.TransactionId))
            {
                await _transactionalPlayerState.PerformUpdate(async playerState =>
                {
                    playerState.Balance = currentBalance;
                    playerState.Version++;
                    var affectedRows = await repository.InsertRecord(tableName, record);
                    repository.UpdatePlayerBalance(playerId, currentBalance);
                    await Task.CompletedTask;
                });

                return new WinResponse
                {
                    Balance = playerState.Balance,
                    Message = "Balance is Deducted",
                    StatusCode = StatusCodes.Status200OK
                };
            }

            return new WinResponse
            {
                Balance = playerState.Balance,
                Message = "Transaction with same transaction Id is already exists",
                StatusCode = StatusCodes.Status400BadRequest
            };

        }
        public async Task<BetWinResponse> BetWin(BetWinRequest request)
        {
            var playerId = (int)this.GetPrimaryKeyLong();

            if (playerId <= 0)
            {
                throw new ArgumentException("Invalid player ID.");
            }

            if (request.BetAmount <= 0 || request.WinAmount <= 0)
            {
                throw new ArgumentException("Invalid bet or win amount.");
            }

            var currentBalance = await this.GetPlayerBalance();

            if (currentBalance < request.BetAmount)
            {
                throw new Exception("Insufficient balance to place the bet.");
            }


            var connectionString = _configuration.GetSection("ConnectionStrings").GetValue<string>("connectionString");
            var repository = new AdoNetRepository<Transaction>(connectionString);
            var tableName = "Transaction";
            var record = new Transaction
            {
                UserId = playerId,
                BetAmount = request.BetAmount,
                Win = request.WinAmount,
                TransactionTime = DateTime.Now,
            };

            var newBalance = currentBalance - request.BetAmount + request.WinAmount;

            if (!repository.CheckIfTransactionExists(request.TransactionId))
            {
                await _transactionalPlayerState.PerformUpdate(async state =>
                {
                    playerState.Balance = newBalance;
                    playerState.Version++;
                    var affectedRows = await repository.InsertRecord(tableName, record);
                    repository.UpdatePlayerBalance(playerId, currentBalance);
                    await Task.CompletedTask;
                });

                return new BetWinResponse
                {
                    Balance = playerState.Balance,
                    Message = "Transaction is Conplated.",
                    StatusCode = StatusCodes.Status200OK
                };
            }

            return new BetWinResponse
            {
                Balance = playerState.Balance,
                Message = "Transaction with same transaction Id is already exists",
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        public async Task<DeductBalanceResponce> DeductBalance(BetRequest request)
        {
            var playerId = (int)this.GetPrimaryKeyLong();

            if (playerId <= 0)
            {
                throw new ArgumentException("Invalid player ID.");
            }

            if (request.Amount <= 0)
            {
                throw new ArgumentException("Invalid amount.");
            }

            var currentBalance = await this.GetPlayerBalance();

            if (currentBalance < request.Amount)
            {
                throw new Exception("Insufficient balance to make the deduction.");
            }

            var connectionString = _configuration.GetSection("ConnectionStrings").GetValue<string>("connectionString");
            var repository = new AdoNetRepository<Transaction>(connectionString);
            var tableName = "Transaction";
            var record = new Transaction
            {
                UserId = playerId,
                BetAmount = request.Amount,
                TransactionTime = DateTime.Now,
            };

            var newBalance = await this.GetPlayerBalance();

            if (!repository.CheckIfTransactionExists(request.TransactionId))
            {
                await _transactionalPlayerState.PerformUpdate(async state =>
                {
                    playerState.Balance = newBalance;
                    playerState.Version++;
                    var affectedRows = await repository.InsertRecord(tableName, record);
                    repository.UpdatePlayerBalance(playerId, currentBalance);
                    await Task.CompletedTask;
                });

                return new DeductBalanceResponce
                {
                    Balance = playerState.Balance,
                    Message = "Balance is Deducted",
                    StatusCode = StatusCodes.Status200OK
                };
            }

            return new DeductBalanceResponce
            {
                Balance = playerState.Balance,
                Message = "Transaction with same transaction Id is already exists",
                StatusCode = StatusCodes.Status400BadRequest
            };
        }
        public Task<decimal> GetPlayerBalance()
        {
            var connectionString = _configuration.GetSection("ConnectionStrings").GetValue<string>("connectionString");
            var repository = new AdoNetRepository<Player>(connectionString);
            var playerId = (int)this.GetPrimaryKeyLong();
            var response = repository.GetPlayerBalance(playerId);
            return Task.FromResult(response);
        }

    }
}
