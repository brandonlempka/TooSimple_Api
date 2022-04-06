namespace TooSimple_Poco.Models.Database
{
    public class UserAccountDataModel
	{
		public string UserAccountId { get; set; } = string.Empty;
		public string UserName { get; set; } = string.Empty;
		public string NormalizedUserName { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public string NormalizedEmail { get; set; } = string.Empty;
		public bool IsEmailConfirmed { get; set; }
		public string PasswordHash { get; set; } = string.Empty;
		public string PasswordSalt { get; set; } = string.Empty;
		public string? PhoneNumber { get; set; }
		public bool IsPhoneConfirmed { get; set; }
		public bool IsTwoFactorEnabled { get; set; }
		public int FailedLoginCount { get; set; }
	}
}
