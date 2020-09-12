using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Amazon;
using Amazon.CloudWatchLogs;
using Amazon.Runtime;
using AWS.Logger;
using AWS.Logger.SeriLog;
using LoggerTest.Formatters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.AwsCloudWatch;

namespace LoggerTest
{
    public class Program
    {
        public static IConfiguration Configuration { get; } = new ConfigurationBuilder()
            .SetBasePath(Environment.CurrentDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
            .AddEnvironmentVariables()
            .AddUserSecrets<Startup>()
            .Build();

        public static void Main(string[] args)
        {
            // Add Logger services
            var loggerConfig = new LoggerConfiguration()
                .ReadFrom.Configuration(Configuration)
                .Enrich.FromLogContext();

            var config = new AmazonCloudWatchLogsConfig()
            {
                
            };

            loggerConfig = loggerConfig
                .WriteTo.AmazonCloudWatch(
                    new CloudWatchSinkOptions
                    {
                        LogGroupName = Configuration["Serilog:AmazonContext:LogGroup"],
                        TextFormatter = new AwsTextFormatter(),
                        CreateLogGroup = true,

                    },
                    new AmazonCloudWatchLogsClient(
                        new BasicAWSCredentials(
                            Configuration["Serilog:AmazonContext:Key"],
                            Configuration["Serilog:AmazonContext:Secret"]
                        ),
                        RegionEndpoint.GetBySystemName(Configuration["Serilog:AmazonContext:Region"])
                    )
                );

            Log.Logger = loggerConfig.CreateLogger();

            try
            {
                Log.Information("Starting web host");

                CreateHostBuilder(args)
                    .Build()
                    .Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>()
                        .UseConfiguration(Configuration)
                        .UseSerilog();
                });
    }
}
