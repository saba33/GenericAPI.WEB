﻿namespace GenericAPI.Services.Models.ResponseModels
{
    [GenerateSerializer]
    public class BetWinResponse : BaseResponse
    {
        public decimal Balance { get; set; }
    }
}
