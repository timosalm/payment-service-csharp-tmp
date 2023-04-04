using KubeServiceBinding;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Steeltoe.Connector.OAuth;
using Steeltoe.Extensions.Configuration.ConfigServer;
using Steeltoe.Management.Endpoint;
using Steeltoe.Management.Tracing;

namespace PaymentService
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
            var settings = new ConfigServerClientSettings { Uri = GetConfigServerUri() };
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder().AddConfigServer(settings);
            services.ConfigureConfigServerClientOptions(configurationBuilder.Build());
            services.AddOAuthServiceOptions(Configuration);
            services.AddAllActuators(Configuration);
            services.ActivateActuatorEndpoints();
            services.AddDistributedTracingAspNetCore();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v3", new OpenApiInfo { Title = "PaymentService", Version = "v3" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger(c => c.RouteTemplate = "{documentName}/api-docs");
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/v3/api-docs", "PaymentService"));
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static String GetConfigServerUri()
        {
            try
            {
                DotnetServiceBinding serviceBinding = new DotnetServiceBinding();
                Dictionary<string, string> configServerServiceBinding = serviceBinding.GetBindings("config");
                return configServerServiceBinding["uri"];
            }
            catch (Exception e)
            {
                Console.WriteLine("Service Binding not found");
                return ConfigServerClientSettings.DEFAULT_URI;
            }
        }
    }
}
