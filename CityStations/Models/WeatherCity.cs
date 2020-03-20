using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CityStations.Models
{
    public class WeatherCity
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public Rain rain { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }

        public static WeatherCity CreateWeatherCity(string lat, string lng)
        {
            var result = "";
            Uri uri = null;
            try
            {
                uri = new Uri(
                    $"http://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lng}&appid=8229e54f51796d0603d2444f781f0d00");
            }
            catch (UriFormatException uriEx)
            {
                Logger.WriteLog($"Не верный формат Uri: {uriEx.Message}, трассировка стека: {uriEx.StackTrace}", "WeatherCity");
                return null;
            }
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(uri);
                using (var response = (HttpWebResponse)request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream ?? throw new InvalidOperationException()))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Logger.WriteLog(e.Message + " " + e.StackTrace, "weather");
                return null;
            }
            try
            {
                var jResult = JToken.Parse(result).ToObject<WeatherCity>();
                return jResult;
            }
            catch(JsonException je)
            {
                Logger.WriteLog(je.Message + " " + je.StackTrace, "weather");
                return null;
            }
        }

        public void Set(WeatherCity weatherCity)
        {
            if (weatherCity == null) return;
            @base = weatherCity.@base;
            clouds = new Clouds()
            {
                all = weatherCity.clouds?.all ?? 0
            };
            cod = weatherCity.cod;
            coord = new Coord()
            {
                Id = weatherCity.coord?.Id,
                lat = weatherCity.coord?.lat,
                lon = weatherCity.coord?.lon
            };
            dt = weatherCity.dt;
            id = weatherCity.id;
            main = new Main()
            {
                humidity = weatherCity.main?.humidity ?? 0,
                pressure = weatherCity.main?.pressure ?? 0,
                temp = weatherCity.main?.temp ?? 0,
                temp_max = weatherCity.main?.temp_max ?? 0,
                temp_min = weatherCity.main?.temp_min ?? 0
            };
            name = weatherCity.name;
            rain = new Rain()
            {
                Precipitation_In_The_Last_3_Hours = weatherCity.rain?.Precipitation_In_The_Last_3_Hours ?? 0
            };
            sys = new Sys()
            {
                country = weatherCity.sys?.country,
                id = weatherCity.sys?.id ?? 0,
                message = weatherCity.sys?.message ?? 0,
                sunrise = weatherCity.sys?.sunrise ?? 0,
                sunset = weatherCity.sys?.sunset ?? 0,
                type = weatherCity.sys?.type ?? 0
            };
            weather = new List<Weather>()
                {
                    new Weather()
                    {
                        description = weatherCity.weather
                                                 .ToList()
                                                 .FirstOrDefault()
                                                 ?.description ?? "",
                        icon = weatherCity.weather
                                          .ToList()
                                          .FirstOrDefault()
                                          ?.icon,
                        id = weatherCity.weather
                                        .FirstOrDefault()
                                        ?.id ?? -1,
                        main = weatherCity.weather
                                          .FirstOrDefault()
                                          ?.main ?? ""                        
                    }
                };          
        }
    }
}