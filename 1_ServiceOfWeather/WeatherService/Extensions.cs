using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WeatherService
{
    public static class Extensions
    {
        public static IApplicationBuilder UseCityTimestamp(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CityTimestampMiddleware>();
        }
    }
}
