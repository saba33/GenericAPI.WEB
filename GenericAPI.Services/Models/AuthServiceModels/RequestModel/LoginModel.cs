using System.ComponentModel.DataAnnotations;

namespace GenericAPI.Services.Models.AuthServiceModels.RequestModel
{
    public class LoginModel
    {
        [Required]
        public string Mail { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
