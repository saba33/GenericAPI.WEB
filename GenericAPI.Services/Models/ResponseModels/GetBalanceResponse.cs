namespace GenericAPI.Services.Models.ResponseModels
{
    [GenerateSerializer]
    public class GetBalanceResponse : BaseResponse
    {
        public decimal Balance { get; set; }
    }
}
