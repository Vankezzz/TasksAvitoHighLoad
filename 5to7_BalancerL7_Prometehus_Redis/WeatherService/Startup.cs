using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Prometheus;



namespace WeatherService
{
    public class Startup
    {
       
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }
        //private static readonly Counter TickTock =
      //Metrics.CreateCounter("sampleapp_ticks_total", "Just keeps on ticking");

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

           


            // Custom Metrics to count requests for each endpoint and the method
            var counter = Metrics.CreateCounter("peopleapi_path_counter", "Counts requests to the People API endpoints", new CounterConfiguration
            {
                LabelNames = new[] { "method", "endpoint" }
            });
            //var counter = Metrics.CreateGauge("peopleapi_path_counter", "Counts requests to the People API endpoints", new CounterConfiguration
            //{
            //    LabelNames = new[] { "method", "endpoint" }
            //});
            //var timer = new Stopwatch();
            //timer.Start();
            
            app.UseMetricServer();
            app.UseHttpMetrics();
            app.Use((context, next) =>
            {
                counter.WithLabels(context.Request.Method, context.Request.Path).Inc();
                //timer.Stop();
                //counter.Value = timer.Elapsed;
                return next();
            });

            app.UseRequestMetricsMiddleware();
            app.UseCityTimestamp();


        }
    }
}
