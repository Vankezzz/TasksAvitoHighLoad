using Newtonsoft.Json.Linq;
using System;

namespace OpenWeatherAPI
{
    public class Query
    {
        public string Name { get; private set; }
        public string Temp { get; private set; }
        public string Units { get; private set; }

        public void Current(string apiKey, string queryStr, string units = "metric")
        {
            JObject jsonData;
            using (var client = new System.Net.WebClient())
                jsonData = JObject.Parse(client.DownloadString($"http://api.openweathermap.org/data/2.5/weather?&units={units}&appid={apiKey}&q={queryStr}"));
            Name = jsonData.SelectToken("name").ToString();
            Temp = jsonData.SelectToken("main").SelectToken("temp").ToString();
            Units = units;

        }
        public void Forecast(string apiKey, string city,string cnt, string units = "metric")
        {
            JObject jsonData;
            var str = (Convert.ToInt32(cnt) - 1).ToString();
            using (var client = new System.Net.WebClient())
                jsonData = JObject.Parse(client.DownloadString($"http://api.openweathermap.org/data/2.5/forecast?&units={units}&q={city}&cnt={cnt}&appid={apiKey}"));
            Name = jsonData.SelectToken("city").SelectToken("name").ToString();
            Temp = jsonData.SelectToken($"list[{str}]").SelectToken("main").SelectToken("temp").ToString();
            Units = units;

        }
    }
}