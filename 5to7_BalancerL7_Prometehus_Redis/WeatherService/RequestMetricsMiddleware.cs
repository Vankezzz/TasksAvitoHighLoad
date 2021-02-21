using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Prometheus;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public class RequestMetricsMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger _logger;

    public RequestMetricsMiddleware(
        RequestDelegate next
        , ILoggerFactory loggerFactory
        )
    {
        this._next = next;
        this._logger = loggerFactory.CreateLogger<RequestMetricsMiddleware>();
    }


    Counter counter1 = Metrics.CreateCounter("dotnet_request_per_seconds", "Requests Total per second", new CounterConfiguration
    {
        LabelNames = new[] { "method" }
    });
    Stopwatch sw = new Stopwatch();
    public async Task Invoke(HttpContext httpContext)
    {
        var path = httpContext.Request.Path.Value;
        var method = httpContext.Request.Method;

        sw.Start();

        counter1.Labels(method).Inc();
        if (sw.Elapsed.TotalSeconds>=10)
        {
            sw.Stop();
            counter1.IncTo(0);
            sw.Restart();

        }
        //var histogram =
        //Metrics.CreateHistogram(
        //        "dotnet_request_per_seconds",
        //        "Histogram for the duration in seconds.",
        //        new HistogramConfiguration
        //        {
        //            Buckets = Histogram.LinearBuckets(start: 1, width: 1, count: 5)
        //        });

        //histogram.Observe(sw.Elapsed.TotalSeconds);

        var counter = Metrics.CreateCounter("prometheus_demo_request_total", "HTTP Requests Total", new CounterConfiguration
        {
            LabelNames = new[] { "path", "method", "status" }
        });

        var statusCode = 200;

        try
        {
            await _next.Invoke(httpContext);
        }
        catch (Exception)
        {
            statusCode = 500;
            counter.Labels(path, method, statusCode.ToString()).Inc();

            throw;
        }

        if (path != "/metrics")
        {
            statusCode = httpContext.Response.StatusCode;
            counter.Labels(path, method, statusCode.ToString()).Inc();
        }


        
    }
}
