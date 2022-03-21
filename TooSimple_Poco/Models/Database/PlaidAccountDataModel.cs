using TooSimple_Poco.Enums;

namespace TooSimple_Poco.Models.Database
{
    public class PlaidAccountDataModel
    {
        public string PlaidAccountId { get; set; } = string.Empty;
        public PlaidAccountType PlaidAccountTypeId { get; set; } = 0;
        public string UserAccountId { get; set; } = string.Empty;
        public string Mask { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? NickName { get; set; }
        public decimal? CurrentBalance { get; set; }
        public decimal? AvailableBalance { get; set; }
        public string CurrencyCode { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
        public bool UseForBudgeting { get; set; }
        public DateTime? LastUpdated { get; set; }
        //public ICollection<Transaction> Transactions { get; set; }
        public bool ReLoginRequired { get; set; }
    }
}
