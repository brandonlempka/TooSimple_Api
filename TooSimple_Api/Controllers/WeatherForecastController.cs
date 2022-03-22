using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TooSimple_DataAccessors.Database.Accounts;
using TooSimple_DataAccessors.Plaid.AccountUpdate;
using TooSimple_DataAccessors.Plaid.TokenExchange;
using TooSimple_Database;
using TooSimple_Database.Entities;
using TooSimple_Managers.Plaid.AccountUpdate;
using TooSimple_Poco.Models.Database;

namespace TooSimple_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private ITokenExchangeAccessor _testingAccessor;
        private IAccountUpdateManager _accountUpdateAccessor;
        public WeatherForecastController(ITokenExchangeAccessor tokenExchangeAccessor,
            IAccountUpdateManager accountUpdateAccessor)
        {
            _testingAccessor = tokenExchangeAccessor;
            _accountUpdateAccessor = accountUpdateAccessor;
        }

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };


        [HttpGet(Name = "GetWeatherForecast")]
        public async Task<bool> Get()
        {
            
            var test = await _accountUpdateAccessor.UpdateAccountBalancesAsync("test");
            return test;
        }

        [HttpPost]
        public void Testing([FromBody]JsonElement json)
        {
            var test = json;
        }
    }
}