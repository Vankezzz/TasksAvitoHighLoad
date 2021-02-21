using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace OpenWeatherAPI
{
    public class KeyAPI
    {
		private string openWeatherAPIKey;

		public KeyAPI(string apiKey)
		{
			openWeatherAPIKey = apiKey;
		}

		public void UpdateAPIKey(string apiKey)
		{
			openWeatherAPIKey = apiKey;
		}

		//Returns null if invalid request
		public string DetectedCurrentWeather(string city)
		{
			Query newQuery = new Query();
			newQuery.Current(openWeatherAPIKey, city);
			var json = JsonConvert.SerializeObject(newQuery);
			return json;
		}
		public string DetectedForecastWeather(string city,string cnt)
		{
			Query newQuery = new Query();
			newQuery.Forecast(openWeatherAPIKey, city, cnt);
			var json = JsonConvert.SerializeObject(newQuery);
			return json;
		}
	}
}
