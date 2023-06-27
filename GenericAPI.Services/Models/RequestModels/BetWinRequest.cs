namespace GenericAPI.Services.Models.RequestModels
{
    [GenerateSerializer]
    public class BetWinRequest
    {
        public long TransactionId { get; set; }
        public int UserId { get; set; }
        public decimal BetAmount { get; set; }
        public decimal WinAmount { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
