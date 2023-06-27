namespace GenericAPI.Services.Helper
{
    public class LaunchGameUrlGenerator
    {
        public static string GenerateLaunchGameUrl(int gameId, int providerId, string secretKey)
        {
            string url = "https://GamesWorld.com";


            var KeyHash = Hasher.CreateMd5Hasher(secretKey);
            // Generate the launch game URL based on the provided parameters
            string generatedUrl = $"{url}/games/{gameId}/launch?provider={providerId}&secret={KeyHash}";

            return generatedUrl;
        }
    }
}
