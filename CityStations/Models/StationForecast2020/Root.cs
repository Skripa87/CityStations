using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityStations.Models.StationForecast2020
{
    public class Root
    {
        public List<ForecastsItem> forecasts { get; set; }
        public string weather { get; set; }
        public string text { get; set; }
    }
}