namespace GenericAPI.Services.Models.ResponseModels
{
    [GenerateSerializer]
    public class BaseResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
    }
}
