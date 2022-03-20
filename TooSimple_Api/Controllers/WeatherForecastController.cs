using Microsoft.AspNetCore.Mvc;
using TooSimple_DataAccessors.Plaid.TokenExchange;
using TooSimple_Database;
using TooSimple_Database.Entities;

namespace TooSimple_Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private ITokenExchangeAccessor _testingAccessor;
        public WeatherForecastController(ITokenExchangeAccessor tokenExchangeAccessor)
        {
            _testingAccessor = tokenExchangeAccessor;
        }

        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };


        //[HttpGet(Name = "GetWeatherForecast")]
        //public IEnumerable<WeatherForecast> Get()
        //{
        //    return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        //    {
        //        Date = DateTime.Now.AddDays(index),
        //        TemperatureC = Random.Shared.Next(-20, 55),
        //        Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        //    })
        //    .ToArray();
        //}

        //[HttpGet(Name = "Test")]
        //public async Task<Account> TestEf()
        //{
        //    var newTest = await _testingAccessor.CreateLinkTokenAsync("test");
        //    var test = await _testingEf.GetData();
        //    return test;
        //}

    }
}