using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CityStations.Models
{
    
    public enum AnaliticFunctionality { NORMAL_WORK, BAD_WORK, NOT_WORK}
    public class MonitoringViewModel
    {
        public string StationId { get; set; }
        public string StationNameAndDescription { get; set; }
        public string ForecastSource { get; set; }
        public string LastStationActivity { get; set; }
        public string AnaliticFunctionalityString { get; set; }
        public AnaliticFunctionality AnaliticFunctionality { get; set; }
        public string LastError { get; set; }

        public MonitoringViewModel(StationModel station, List<Event> eventsByStation, List<Event> errorEvents)
        {
            StationId = station?.Id ?? "";
            StationNameAndDescription = $"{station?.Name ?? ""} {station?.Description ?? ""}";
            ForecastSource = string.Equals(station?.Id ?? "0", "0", new StringComparison())
                ? (string.IsNullOrEmpty(station?.IdForRnis)
                    ? "Нет поставщика данных"
                    : "Поставщиком является только РНИС")
                : (string.IsNullOrEmpty(station?.IdForRnis)
                    ? "Поставщик Умный транспорт"
                    : "Данные получены от Умного транспорта и РНИС");
            var lastEventDateTime = eventsByStation?.OrderBy(e => e.Date)
                                                   .LastOrDefault()
                                                   ?.Date ?? DateTime.MinValue;
            if (DateTime.Equals(lastEventDateTime, DateTime.MinValue)|| DateTime.Now - lastEventDateTime > TimeSpan.FromMinutes(5)) 
            {
                AnaliticFunctionalityString = "Табло не работает(мин. 1 день)";
                AnaliticFunctionality = AnaliticFunctionality.NOT_WORK;
                lastEventDateTime = errorEvents.OrderBy(e=>e.Date).LastOrDefault()
                                        ?.Date ?? DateTime.MinValue;
                LastStationActivity = lastEventDateTime.ToString($"yyyy/MM/dd HH:mm:ss", new CultureInfo("All"));
            }
            else if(DateTime.Now - lastEventDateTime > TimeSpan.FromSeconds(60.0) && DateTime.Now - lastEventDateTime < TimeSpan.FromMinutes(5))
            {
                AnaliticFunctionalityString = "Есть проблемы!";
                AnaliticFunctionality = AnaliticFunctionality.BAD_WORK;
                lastEventDateTime = errorEvents.OrderBy(e => e.Date).LastOrDefault()
                                        ?.Date ?? DateTime.MinValue;
                LastStationActivity = lastEventDateTime.ToString($"yyyy/MM/dd HH:mm:ss", new CultureInfo("All"));
            }
            else if (DateTime.Now - lastEventDateTime < TimeSpan.FromSeconds(60.0))
            {
                AnaliticFunctionalityString = "Табло работает нормально!";
                AnaliticFunctionality = AnaliticFunctionality.NORMAL_WORK;
                LastStationActivity = lastEventDateTime.ToString($"yyyy/MM/dd HH:mm:ss", new CultureInfo("All"));
            }
            LastError = errorEvents == null
                      ? "Табло не работает!"
                      : (errorEvents.Any()
                         ? (errorEvents.LastOrDefault()?.Description ?? "")
                         : "Без ошибок");
        }
    }
}