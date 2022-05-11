using TooSimple_Poco.Models.Entities;

namespace TooSimple_Poco.Models.DataModels
{
    public class PlaidAccountDataModel
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

        /// <summary>
        /// Default constructor.
        /// </summary>
        public PlaidAccountDataModel() { }

        /// <summary>
        /// Converts from <see cref="PlaidAccount"/>
        /// to <see cref="PlaidAccountDataModel"/>
        /// </summary>
        /// <param name="plaidAccount"> <see cref="PlaidAccount"/>
        /// </param>
        public PlaidAccountDataModel(PlaidAccount plaidAccount)
        {
            PlaidAccountId = plaidAccount.PlaidAccountId;
            PlaidAccountTypeId = plaidAccount.PlaidAccountTypeId;
            UserAccountId = plaidAccount.UserAccountId;
            Mask = plaidAccount.Mask;
            Name = plaidAccount.Name;
            NickName = plaidAccount.NickName;
            CurrentBalance = plaidAccount.CurrentBalance;
            AvailableBalance = plaidAccount.AvailableBalance;
            CreditLimit = plaidAccount.CreditLimit;
            CurrencyCode = plaidAccount.CurrencyCode;
            AccessToken = plaidAccount.AccessToken;
            LastUpdated = plaidAccount.LastUpdated;
            IsActiveForBudgetingFeatures = plaidAccount.IsActiveForBudgetingFeatures;
            IsPlaidRelogRequired = plaidAccount.IsPlaidRelogRequired;
            ItemId = plaidAccount.ItemId;
        }
    }
}
