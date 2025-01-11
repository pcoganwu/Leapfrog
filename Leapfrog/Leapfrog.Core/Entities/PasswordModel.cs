namespace Leapfrog.Core.Entities
{
    public class PasswordModel
    {
        public string Password { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
        public bool IsErrorVisible { get; set; }
    }
}
