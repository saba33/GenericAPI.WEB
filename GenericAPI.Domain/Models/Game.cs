namespace GenericAPI.Domain.Models
{
    public class Game
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DateAdded { get; set; }
        public int ProviderId { get; set; }
        public Provider Provider { get; set; }
    }
}
