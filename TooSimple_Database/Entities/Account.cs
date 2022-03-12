namespace TooSimple_Database.Entities
{
    public class Account
    {
        public string PlaidAccountId { get; set; } = string.Empty;
        public int PlaidAccountTypeId { get; set; }
        public string? Mask { get; set; }
        public string? Name { get; set; }
        public string? NickName { get; set; }
        public decimal CurrentBalance { get; set; }
        public decimal AvailableBalance { get; set; }
        public string? CurrencyCode { get; set; }
        public string? AccessToken { get; set; }
        public string UserAccountId { get; set; } = string.Empty;
        public DateTime? LastUpdated { get; set; }
        public bool UseForBudgeting { get; set; }
        public bool ReloginRequired { get; set; }
    }
}
