namespace GenericAPI.Services.Abstractions
{
    public interface IPlayerGrainFactory
    {
        public IPlayerGrain GetGrain(int playerId);
    }
}
