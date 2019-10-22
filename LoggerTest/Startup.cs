using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LoggerTest.Core.Repositories;
using LoggerTest.Filters;
using LoggerTest.Handlers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace LoggerTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // add application service instances
            services.AddSingleton<IConfiguration>(Configuration);
            //services.AddSingleton<IExceptionFilter, UnhandledExceptionLogger>();
            services.AddScoped<ArgumentExceptionFilterAttribute>();
            services.AddScoped<NotImplementedExceptionFilterAttribute>();
            
            services.AddScoped<IBaseRepository, BaseRepository>();

            // Add Routing configuration
            services.AddRouting(options => options.LowercaseUrls = true);

            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddSerilog();

            if (env.IsDevelopment() || env.IsStaging())
            {
                app.UseDeveloperExceptionPage();
            }
            
            app.UseMiddleware<ScopedLoggingHandler>();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
