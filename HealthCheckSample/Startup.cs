using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.HealthChecks;
using HealthCheckSample.HealthChecks;

namespace HealthCheckSample
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
            services.Configure<HealthCheckOptions>(Configuration);

            services.AddSingleton<RandomFailHealthCheck>();

            services.AddHealthChecks(checks =>
            {
                checks.AddValueTaskCheck("HTTP Endpoint", () => new ValueTask<IHealthCheckResult>(HealthCheckResult.Healthy("Ok")), TimeSpan.FromSeconds(5))
                      .AddUrlCheck(Configuration["ExternalApi"], TimeSpan.FromSeconds(5)); // url check external api

                checks.AddCheck<RandomFailHealthCheck>("RandomCheck", TimeSpan.FromSeconds(1));

                checks.AddHealthCheckGroup("Resources", groupChecks =>
                {
                    groupChecks.AddPrivateMemorySizeCheck(167772160)
                               .AddWorkingSetCheck(167772160)
                               .WithDefaultCacheDuration(TimeSpan.FromSeconds(30));
                });

                // check db availablity
            });

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHealthChecks();

            app.UseMvc();
        }
    }
}
