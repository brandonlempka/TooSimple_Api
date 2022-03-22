namespace TooSimple_Poco.Models.Auth
{
    public class UserModel
    {
        public string UserName { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
    }
}
