using TooSimple_Poco.Enums;

namespace TooSimple_Poco.Models.Entities
{
	public class PlaidAccount
	{
        public string PlaidAccountId { get; set; } = string.Empty;
        public int PlaidAccountTypeId { get; set; } = 0;
        public string UserAccountId { get; set; } = string.Empty;
        public string Mask { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? NickName { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? AvailableBalance { get; set; }
        public decimal? CreditLimit { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public DateTime? LastUpdated { get; set; }
        public bool IsActiveForBudgetingFeatures { get; set; }
        public bool IsPlaidRelogRequired { get; set; }
        public string ItemId { get; set; } = string.Empty;
    }
}
