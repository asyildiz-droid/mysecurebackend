namespace MySecureBackend.WebApi.Models
{
    public class RegisterRequest
    {
        public string UserName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}
