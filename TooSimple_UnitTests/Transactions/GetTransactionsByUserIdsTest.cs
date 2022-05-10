using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TooSimple_Poco.Models.Database;

namespace TooSimple_UnitTests.Transactions
{
	[TestClass]
	public class GetTransactionsByUserIdsTest
	{
		[TestMethod]
		public async Task SuccessTest()
        {
			List<PlaidTransaction> transactions = new()
			{
				new()
				{
					Amount = 123M,
					TransactionDate = DateTime.UtcNow.AddDays(-10),

				}
			};
        }
	}
}
