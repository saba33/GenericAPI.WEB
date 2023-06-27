using System.ComponentModel.DataAnnotations;

namespace GenericAPI.Services.Infrastructure.Attributes
{
    public class EmailValidatorAttribute : ValidationAttribute
    {
        private const string _emailRegexPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

        public override bool IsValid(object value)
        {
            var email = value as string;
            if (email == null)
            {
                return false;
            }

            var regex = new System.Text.RegularExpressions.Regex(_emailRegexPattern);
            return regex.IsMatch(email);
        }
    }
}
