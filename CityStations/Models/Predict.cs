using System;

namespace CityStations.Models
{
    public class Predict
    {
        public string NumberBus { get; set; }
        public string EndStation { get; set; }
        public string TimePredict { get; set; }
        public string TypeBus { get; set; }
        public string LowFloor { get; set; }

        public Predict() { }

        public Predict(StationForecast stationForecast)
        {
            NumberBus = stationForecast.Rnum;
            EndStation = stationForecast.Where;
            TimePredict = stationForecast.Arrt != null
                        ? (((int)stationForecast.Arrt / 60) == 0
                          ? "1"
                          : ((int)stationForecast.Arrt / 60).ToString())
                        : "";
            TypeBus = string.Equals(stationForecast.Rtype.ToUpperInvariant(), "Т", new StringComparison())
                ? $"~/../../Content/Images/trolleybus.png"
                : (string.Equals(stationForecast.Rtype.ToUpperInvariant(), "М", new StringComparison())
                    ? $"~/../../Content/Images/marshrutka.png"
                    : $"~/../../Content/Images/bus.png");
            LowFloor = "";
        }

        public Predict(StationForecast2020.ForecastsItem forecastItem)
        {
            NumberBus = forecastItem.num;
            LowFloor = forecastItem.lowFloor
                     ? $"~/../../Content/Images/spec_bus.png"
                     : "";
            EndStation = forecastItem.whereGo;
            TimePredict = forecastItem.arrTime!= null
                        ? (((int)forecastItem.arrTime / 60) == 0
                            ? "1"
                            : ((int)forecastItem.arrTime / 60).ToString())
                        : "";
            TypeBus = string.Equals(forecastItem.type.ToUpperInvariant(),"Т",new StringComparison())
                ? $"~/../../Content/Images/trolleybus.png"
                : (string.Equals(forecastItem.type.ToUpperInvariant(), "М", new StringComparison())
                  ? $"~/../../Content/Images/marshrutka.png"
                  : $"~/../../Content/Images/bus.png");
        }
    }
}