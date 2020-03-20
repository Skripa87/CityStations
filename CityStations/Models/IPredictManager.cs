using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Web;
using Microsoft.Ajax.Utilities;

namespace CityStations.Models
{
    public interface IPredictManager
    {
        IEnumerable<StationForecast> GetStationForecast(string idStation);
        IEnumerable<Station> GetStations();
        string GetLatLngStationFor(string idStation);
    }

    public abstract class AbstractPredictManager : IPredictManager
    {
        
        public string GetLatLngStationFor(string idStation)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<StationForecast> GetStationForecast(string idStation)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Station> GetStations()
        {
            throw new NotImplementedException();
        }

        public static WeatherCity GetWeatherCity()
        {
            return CurrentWeather.GetWeather();
        }

        public DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public string KelvinToCelcia(double kelvinTemperature)
        {
            return ((int)(kelvinTemperature - 273.15)).ToString(CultureInfo.CurrentCulture);
        }

        public string CreateDirectionAndSpeedWind(Wind wind)
        {
            if (wind == null) return "слабый";
            string result = "";
            if (wind.deg > 0 && wind.deg <= 22.5)
            {
                result = "сев.восточный, ";
            }
            else if (wind.deg > 22.5 && wind.deg <= 45)
            {
                result = "сев.восточный, ";
            }
            else if (wind.deg > 45 && wind.deg <= 67.5)
            {
                result = "сев.восточный, ";
            }
            else if (wind.deg > 67.5 && wind.deg <= 90)
            {
                result = "сев.восточный, ";
            }
            else if (wind.deg > 90 && wind.deg <= 112.5)
            {
                result = "юго-восточный, ";
            }
            else if (wind.deg > 112.5 && wind.deg <= 135)
            {
                result = "юго-восточный, ";
            }
            else if (wind.deg > 157.5 && wind.deg <= 180)
            {
                result = "юго-восточный, ";
            }
            else if (wind.deg > 180 && wind.deg <= 202.5)
            {
                result = "юго-западный, ";
            }
            else if (wind.deg > 202.5 && wind.deg <= 225)
            {
                result = "юго-западный, ";
            }
            else if (wind.deg > 247.5 && wind.deg <= 270)
            {
                result = "юго-западный, ";
            }
            else if (wind.deg > 270 && wind.deg <= 292.5)
            {
                result = "сев.западный, ";
            }
            else if (wind.deg > 292.5 && wind.deg <= 315)
            {
                result = "сев.западный, ";
            }
            else if (wind.deg > 315 && wind.deg <= 337.5)
            {
                result = "сев.западный, ";
            }
            else if (wind.deg > 337.5 && wind.deg <= 360)
            {
                result = "северный, ";
            }
            return result + wind.speed + "м/с";
        }
    }
}