using GenericAPI.Services.Abstractions;

namespace GenericAPI.Services.Implementations
{
    public class PlayerGrainFactory : IPlayerGrainFactory
    {
        private readonly IGrainFactory _grainFactory;

        public PlayerGrainFactory(IGrainFactory grainFactory)
        {
            _grainFactory = grainFactory;
        }
        public IPlayerGrain GetGrain(int playerId)
        {
            return _grainFactory.GetGrain<IPlayerGrain>(playerId);
        }
    }
}
