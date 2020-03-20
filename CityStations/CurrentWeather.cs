using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CityStations.Models;

namespace CityStations
{
    public static class CurrentWeather
    {
        private static WeatherCity Weather { get; set; }
        private static DateTime? CurrentDateTime { get; set; }
        private static DateTime? PreviosDateTime { get; set; }

        private static void ActualizationWeather()
        {
            if (CurrentDateTime == null)
            {
                CurrentDateTime = DateTime.Now;
                PreviosDateTime = CurrentDateTime;
                Weather = new WeatherCity();
                Weather.Set(WeatherCity.CreateWeatherCity("54.735950", "55.982370"));
            }
            else if (DateTime.Now > ((DateTime)PreviosDateTime).AddMinutes(30))
            {
                CurrentDateTime=DateTime.Now;
                PreviosDateTime = DateTime.Now;
                Weather = new WeatherCity();
                Weather.Set(WeatherCity.CreateWeatherCity("54.735950", "55.982370"));
            }
            else
            {
                CurrentDateTime=DateTime.Now;
                if (Weather == null)
                {
                    Weather = new WeatherCity();
                    Weather.Set(WeatherCity.CreateWeatherCity("54.735950", "55.982370"));
                }
            }
        }

        public static WeatherCity GetWeather()
        {
            ActualizationWeather();
            return Weather;
        }
    }
}