namespace GenericAPI.Services.Models.RequestModels
{
    [GenerateSerializer]
    public class WinRequest
    {
        public long TransactionId { get; set; }
        public int UserId { get; set; }
        public decimal Win { get; set; }
        public DateTime RequestTime { get; set; }
    }
}
