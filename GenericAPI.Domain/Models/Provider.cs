namespace GenericAPI.Domain.Models
{
    public class Provider
    {
        public int Id { get; set; }
        public string ProviderName { get; set; }
        public string Secret { get; set; }
        public List<Game> Games { get; set; }
    }
}
