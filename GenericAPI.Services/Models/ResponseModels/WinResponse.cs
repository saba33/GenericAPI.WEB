namespace GenericAPI.Services.Models.ResponseModels
{
    [GenerateSerializer]
    public class WinResponse : BaseResponse
    {
        public decimal Balance { get; set; }
    }
}
