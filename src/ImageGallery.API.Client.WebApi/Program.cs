﻿using System;
using System.Diagnostics;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using ImageGallery.API.Client.Service.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Serilog;
using Serilog.Sinks.SystemConsole.Themes;

namespace ImageGallery.API.Client.WebApi
{
    /// <summary>
    /// 
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static int Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(ConfigurationHelper.Configuration)
                .WriteTo.Console(theme: SystemConsoleTheme.Literate)
                .WriteTo.MySQL("server=mysql;uid=zipkin;pwd=zipkin;database=zipkin;")
                .CreateLogger();

            Serilog.Debugging.SelfLog.Enable(msg =>
            {
                Log.Information("Init:ImageGallery.API.Client.WebApi");
                Debug.Print(msg);
                Debugger.Break();
            });

            try
            {
                CreateWebHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(ConfigurationHelper.Configuration)
                .UseUrls("http://*:8150")
                .UseStartup<Startup>()
                .UseSerilog()
                .UseMetrics(options =>
                 {
                     options.EndpointOptions = endpointsOptions =>
                     {
                         endpointsOptions.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                     };
                 })
                .ConfigureMetrics(options =>
                {
                    options.OutputMetrics.AsPrometheusPlainText();
                })
                .ConfigureAppMetricsHostingConfiguration(options =>
                {
                    options.AllEndpointsPort = 3333;
                    options.EnvironmentInfoEndpoint = "/env";
                    options.MetricsEndpoint = "/metrics";
                    options.MetricsTextEndpoint = "/metrics-text";
                });

    }
}
