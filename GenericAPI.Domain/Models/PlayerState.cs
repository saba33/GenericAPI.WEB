using Orleans;

namespace GenericAPI.Domain.Models
{
    [GenerateSerializer]
    public class PlayerState
    {
        public decimal Balance { get; set; }
        public int Version { get; set; }
    }
}
