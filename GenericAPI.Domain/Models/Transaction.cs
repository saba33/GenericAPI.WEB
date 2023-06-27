namespace GenericAPI.Domain.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal BetAmount { get; set; }
        public DateTime TransactionTime { get; set; }
        public decimal? Win { get; set; }
    }
}
