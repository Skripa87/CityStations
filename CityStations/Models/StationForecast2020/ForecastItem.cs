using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityStations.Models.StationForecast2020
{
    public class ForecastsItem:IForecast, IComparable
    {
        public int routeId { get; set; }
        public string type { get; set; }
        public string num { get; set; }
        public int arrTime { get; set; }
        public string whereGo { get; set; }
        public string lastStation { get; set; }
        public string vehicleId { get; set; }
        public bool wifi { get; set; }
        public bool lowFloor { get; set; }
        public int CompareTo(object obj)
        {
            return arrTime < ((obj as ForecastsItem)?.arrTime ?? 0)
                ? -1
                : (arrTime > (((ForecastsItem)obj)?.arrTime ?? 0)
                    ? 1
                    : 0);
        }
    }
}