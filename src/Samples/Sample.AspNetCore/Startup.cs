﻿using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using PayEx.Client;

namespace Sample.AspNetCore
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IConfigureOptions<HttpClientFactoryOptions>, PayExClientConfigurator>();
            services.AddScoped<ISelectClient, QueryStringSelector>();
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddPayExHttpClient(Configuration, Constants.Someaccount);
            services.AddPayExHttpClient(Configuration, Constants.OtherAccount);
            services.AddMvc();
        }
        
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();
            app.UseMvc();
        }
    }

        public class QueryStringSelector : ISelectClient
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public QueryStringSelector(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        
        public string Select()
        {
            var val = _contextAccessor.HttpContext.Request.Query["selector"];
            return val;
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddPayExHttpClient(this IServiceCollection services, IConfiguration configuration, string payExClientName)
        {
            var payexConfigsection = configuration.GetSection($"PayEx:{payExClientName}");
            services.Configure<PayExOptions>(payExClientName, payexConfigsection);
            services.AddHttpClient<PayExHttpClient>(payExClientName);
            services.AddTransient<PayExClient>();
            return services;
        }
    }
}
