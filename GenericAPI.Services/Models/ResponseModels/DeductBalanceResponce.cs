namespace GenericAPI.Services.Models.ResponseModels
{
    [GenerateSerializer]
    public class DeductBalanceResponce : BaseResponse
    {
        public decimal Balance { get; set; }
    }
}
