using Microsoft.AspNetCore.Http;
using OpenWeatherAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Diagnostics;

namespace WeatherService
{
    public class CityTimestampMiddleware
    {
        private readonly RequestDelegate _next;//ссылка на тот делегат запроса, который стоит следующим в конвейере обработки запроса.
        private  string _openWeatherAPIkey;
        private string _localhostredis;
        private string _redisport;
        private ConnectionMultiplexer redis;
        IDatabase db;
        enum PeriodUpdate_db:int
        {
            hours = 0,
            minutes = 10,
            seconds = 0
        }
       

        public CityTimestampMiddleware(RequestDelegate next)
        {
            _next = next;
            _openWeatherAPIkey =System.Environment.GetEnvironmentVariable("OWM_API_KEY");
            _localhostredis = System.Environment.GetEnvironmentVariable("LOCALHOST_REDIS");
            _redisport = System.Environment.GetEnvironmentVariable("PORT_REDIS");
            Console.WriteLine($"OWM_API_KEY = {_openWeatherAPIkey}");
            Console.WriteLine($"LOCALHOST_REDIS = {_localhostredis}");
            Console.WriteLine($"PORT_REDIS= {_redisport}");
            Console.WriteLine($"Redis is located {_localhostredis + ":" + _redisport}");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            redis = ConnectionMultiplexer.Connect(_localhostredis+":"+_redisport);
            db = redis.GetDatabase();


            string path = context.Request.Path.Value.ToLower();
            if (path == "/v1/forecast/")
            {

                var city = context.Request.Query["city"];
                var timestamp = Convert.ToInt32(context.Request.Query["timestamp"]);
                if ((!string.IsNullOrWhiteSpace(city) & timestamp > 0 & timestamp < 97))
                {
                    var keyapi = new KeyAPI(_openWeatherAPIkey);
                    var dbkey = city.ToString();

                    if (db.StringGet(dbkey+timestamp.ToString()).ToString() != null)
                    {
                        await context.Response.WriteAsync(db.StringGet(dbkey + timestamp.ToString()).ToString());
                    }
                    else
                    {
                        var result = keyapi.DetectedForecastWeather(city, timestamp.ToString());
                        db.StringSet(dbkey + timestamp.ToString(), result, new TimeSpan((int)PeriodUpdate_db.hours, (int)PeriodUpdate_db.minutes, (int)PeriodUpdate_db.seconds));
                        await context.Response.WriteAsync(result + "Update");
                    }
                    return;
                }
            }
            else if (path == "/v1/current/")
            {
                var city = context.Request.Query["city"];
                if (!string.IsNullOrWhiteSpace(city))
                {
                    var keyapi = new KeyAPI(_openWeatherAPIkey);
                    var dbkey = city.ToString();
                    if (db.StringGet(dbkey).ToString()!=null)
                    {
                        await context.Response.WriteAsync(db.StringGet(dbkey).ToString());
                    }
                    else
                    {
                        var result = keyapi.DetectedCurrentWeather(city);
                        db.StringSet(dbkey, result, new TimeSpan((int)PeriodUpdate_db.hours, (int)PeriodUpdate_db.minutes, (int)PeriodUpdate_db.seconds));
                        await context.Response.WriteAsync(result + "Update");
                    }
                    return;
                }
            }
            else if (path == "/v1/debugweather/")
            {
                
                var city = "Moscow";

                if (!string.IsNullOrWhiteSpace(city))
                {
                    if (db.StringGet(city).ToString() != null)
                    {
                        await context.Response.WriteAsync(db.StringGet(city).ToString());
                    }
                    else
                    {
                        var keyapi = new KeyAPI("2d6d053b6a777cd0fe236dd2c0d9aa22");
                        var result = keyapi.DetectedCurrentWeather(city);
                        db.StringSet("Moscow", result, new TimeSpan((int)PeriodUpdate_db.hours, (int)PeriodUpdate_db.minutes, (int)PeriodUpdate_db.seconds));
                        await context.Response.WriteAsync(result+"Update");
                    }
                }

                return;
                //db.StringSet("Moscow", "Test string",new TimeSpan((int)PeriodUpdate_db.hours, (int)PeriodUpdate_db.minutes, (int)PeriodUpdate_db.seconds));
                //string str = db.StringGet("testKey").ToString();
            }
            else
            {
                context.Response.StatusCode = 404;
            }
            await _next.Invoke(context);
        }
    }
}
