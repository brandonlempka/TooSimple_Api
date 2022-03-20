namespace TooSimple_Poco.Settings
{
    public static partial class PlaidSettings
    {
        public static readonly string BaseUrl = string.Empty;
        public static readonly string ClientId = string.Empty;
        public static readonly string Secret = string.Empty;
        public static readonly string Language = "en";
        public static readonly string[] Countries = new string[] 
        {
            "US"
        };

        public static readonly string[] Products = new string[] 
        {
            "auth", "transactions", "liabilities" 
        };
        
        public static readonly string ClientName = "Too Simple";
        public static readonly string[] DebitAcccountFilters = new string[] 
        {
            "checking", "savings", "hsa" 
        };
        
        public static readonly string[] CreditAccountFilters = new string[] 
        {
            "credit card" 
        };
    }
}
