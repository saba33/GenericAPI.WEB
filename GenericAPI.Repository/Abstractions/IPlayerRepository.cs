using GenericAPI.Domain.Models;

namespace GenericAPI.Repository.Abstractions
{
    public interface IPlayerRepository : IBaseRepository<Player>
    {
        Task<Player> GetPlayer(int id);
    }
}
