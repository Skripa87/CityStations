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
            TypeBus = stationForecast.Rtype != "T"
                ? $"~/../../Content/Images/bus.png"
                : $"~/../../Content/Images/trolleybus.png";
            LowFloor = ".";
        }

        public Predict(StationForecast2020.ForecastsItem forecastItem)
        {
            NumberBus = forecastItem.num;
            LowFloor = forecastItem.lowFloor
                     ? "yes"
                     : "no";
            EndStation = forecastItem.lastStation;
            TimePredict = forecastItem.arrTime!= null
                        ? (((int)forecastItem.arrTime / 60) == 0
                            ? "1"
                            : ((int)forecastItem.arrTime / 60).ToString())
                        : "";
            TypeBus = forecastItem.type != "T"
                ? $"~/../../Content/Images/bus.png"
                : $"~/../../Content/Images/trolleybus.png";
        }
    }
}