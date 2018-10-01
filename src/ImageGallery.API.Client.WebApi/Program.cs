﻿using System;
using App.Metrics;
using App.Metrics.AspNetCore;
using App.Metrics.Formatters.Prometheus;
using ImageGallery.API.Client.Service.Helpers;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

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
            try
            {
                CreateWebHostBuilder(args).Build().Run();
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return 1;
            }
            finally
            {

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
                .UseMetrics(
                    options =>
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
