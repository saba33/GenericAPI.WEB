using GenericAPI.Domain.DBContext;
using GenericAPI.Domain.Models;
using GenericAPI.Repository.Abstractions;

namespace GenericAPI.Repository.Implementations
{
    public class PlayerRepository : BaseRepository<Player>, IPlayerRepository
    {
        public PlayerRepository(DatabaseContext playerContext) : base(playerContext)
        {

        }

        public async Task<Player> GetPlayer(int id)
        {
            return await base.GetByIdAsync(id);
        }

        //public async Task<>
    }
}
