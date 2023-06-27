namespace GenericAPI.Services.Abstractions.AuthServices
{
    public interface IJwtService
    {
        string GenerateToken(string userId);
    }
}
