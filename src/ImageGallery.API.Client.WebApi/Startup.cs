﻿using System;
using System.IO;
using ImageGallery.API.Client.Service.Configuration;
using ImageGallery.API.Client.Service.Helpers;
using ImageGallery.FlickrService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Swagger;
using zipkin4net;
using zipkin4net.Tracers.Zipkin;
using zipkin4net.Transport.Http;
using zipkin4net.Middleware;

namespace ImageGallery.API.Client.WebApi
{
    public class Startup
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="configuration"></param>
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        ///
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            services.AddOptions();
            services.Configure<ApplicationOptions>(Configuration);
            services.AddSingleton(Configuration);

            var config = ConfigurationHelper.Configuration.Get<ApplicationOptions>();

            // Swagger
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Title = "ImageGallery.API.Client.WebApi",
                    Description = "ImageGallery.API.Client.WebApi",
                    Version = "v1",
                    TermsOfService = "None",
                });
                options.IncludeXmlComments(GetXmlCommentsPath());
            });

            // AppMetrics
            services.AddMetrics();
            //https://github.com/powerumc/microservice-architecture-quick-start/tree/6a5515301d5f2d26ce21f927535dce2bb02ae49f
            //https://github.com/dotnet-architecture/eShopOnContainers/blob/dev/src/Services/Payment/Payment.API/Program.cs
            //https://www.app-metrics.io/web-monitoring/aspnet-core/quick-start/
            //https://github.com/fs7744/DataAccess/tree/master/src
            //https://github.com/fs7744/DataAccess/blob/master/src/VIC.DataAccess.Zipkin/DataAccessZipkinTrace.cs

            //Zipkin
            services.AddHttpClient("Tracer").AddHttpMessageHandler(provider =>
                TracingHandler.WithoutInnerHandler(provider.GetService<IConfiguration>()["applicationName"]));

            //Services 
            services.AddScoped<IFlickrSearchService>(_ => new FlickrSearchService(config.FlickrConfiguration.ApiKey, config.FlickrConfiguration.Secret));


            services.AddMvc();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        /// <param name="loggerFactory"></param>
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            var config = Configuration.Get<ApplicationOptions>();

            var applicationName = typeof(Program).Assembly.GetName().Name;
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Zipkin
            var lifetime = app.ApplicationServices.GetService<IApplicationLifetime>();
            lifetime.ApplicationStarted.Register(() =>
            {
                TraceManager.SamplingRate = 1.0f;
                var logger = new TracingLogger(loggerFactory, "zipkin4net");
                var httpSender = new HttpZipkinSender(config.ZipkinConfiguration.ZipkinServer, "application/json");
                var stats = new Statistics();
                var tracer = new ZipkinTracer(httpSender, new JSONSpanSerializer(), stats);
                TraceManager.RegisterTracer(tracer);
                TraceManager.Start(logger);
            });
            lifetime.ApplicationStopped.Register(() => TraceManager.Stop());
            app.UseTracing(applicationName);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ImageGallery.API.Client.WebApi V1");
            });
            app.UseMvc();
        }

        private static string GetXmlCommentsPath()
        {
            var basePath = AppContext.BaseDirectory;
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName().Name;
            var fileName = Path.GetFileName(assemblyName + ".xml");

            return Path.Combine(basePath, fileName);
        }
    }
}
