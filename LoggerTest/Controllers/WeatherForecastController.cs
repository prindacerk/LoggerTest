using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoggerTest.Core.Entities;
using LoggerTest.Core.Repositories;
using LoggerTest.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LoggerTest.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("weather/forecast")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly IBaseRepository _repository;

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IBaseRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        [ArgumentExceptionFilter]
        [NotImplementedExceptionFilter]
        public async Task<IEnumerable<WeatherForecast>> Get()
        {
            _logger.LogInformation("Get called");

            return await _repository.GetForecastsAsync(1, 5);
        }

        [HttpGet("list")]
        [ArgumentExceptionFilter]
        [NotImplementedExceptionFilter]
        public async Task<IEnumerable<WeatherForecast>> List()
        {
            throw new NotImplementedException("Not Implemented");
        }

        [HttpGet("error")]
        [ArgumentExceptionFilter]
        [NotImplementedExceptionFilter]
        public async Task<IEnumerable<WeatherForecast>> GetWithError()
        {
            _logger.LogInformation("Get With Error called");

            return await _repository.GetForecastsAsync(10, -2);
        }

        [HttpGet("restricted")]
        [ArgumentExceptionFilter]
        [NotImplementedExceptionFilter]
        public async Task Unknown()
        {
            throw new AccessViolationException("User should not be accessing this.");
        }
    }
}
