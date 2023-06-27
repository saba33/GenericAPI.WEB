namespace GenericAPI.Services.Models.RequestModels
{
    [GenerateSerializer]
    public record class BetRequest
    {
        public long TransactionId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
