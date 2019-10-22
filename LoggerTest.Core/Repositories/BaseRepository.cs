using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoggerTest.Core.Entities;
using Microsoft.Extensions.Logging;

namespace LoggerTest.Core.Repositories
{
    public interface IBaseRepository
    {
        Task<IEnumerable<WeatherForecast>> GetForecastsAsync(int start, int count);
    }

    public class BaseRepository : IBaseRepository
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<BaseRepository> _logger;

        public BaseRepository(ILogger<BaseRepository> logger)
        {
            _logger = logger;
        }

        public async Task<IEnumerable<WeatherForecast>> GetForecastsAsync(int start, int count)
        {
            _logger.LogInformation("Get Forecast called");

            var rng = new Random();
            return Enumerable.Range(start, count).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }
    }
}
