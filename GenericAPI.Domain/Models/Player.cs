namespace GenericAPI.Domain.Models
{
    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string lastName { get; set; }
        public string Mail { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public decimal Balance { get; set; }
    }
}
