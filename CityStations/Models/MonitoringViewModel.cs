using System;
using System.Linq;
using System.Net.NetworkInformation;

namespace CityStations.Models
{

    public enum Status { NormalWork, BadWork, NotWork }
    public class MonitoringViewModel: IComparable
    {
        public string StationId { get; set; }
        public string Address { get; set; }
        public string StationNameAndDescription { get; set; }
        public string PingTest { get; set; }
        public string StatusDescription { get; set; }
        public Status Status { get; set; }
        public string LastError { get; set; }

        private bool TestPing(string ipAddress)
        {
            using (var ping = new Ping())
            {
                var timeout = 120;
                var reply = ping.Send(ipAddress, timeout);
                return reply?.Status == IPStatus.Success;
            }
        }

        public MonitoringViewModel(StationModel station)
        {
            var now = DateTime.Now;
            if (station == null) return;
            var rootIp = station.RouterIp;
            StationId = station.Id ?? "";
            Address =$"{station.DistrictOfTheCity ?? ""} ул. {station.Street ?? ""} д. {station.NumberNearHouse ?? ""}";
            StationNameAndDescription = $"{station.Name ?? ""} {station.Description ?? ""}";
            var ip = station.InformationTable
                            .IpDevice;
            if (string.IsNullOrEmpty(ip))
            {
                PingTest = "НЕ УСТАНОВЛЕН IP";
                StatusDescription = "УСТАНОВИТЕ IP АДРЕС УСТРОЙСТВА";
                Status = Status.BadWork;
            }
            else
            {
                PingTest = TestPing(ip)
                         ? "OK"
                         : "NOT_WORK";
                var manager = new ContextManager();
                if (string.Equals(PingTest, "OK", new StringComparison()))
                {
                    var events = manager.GetActulEvents(station.Id);
                    events.Reverse();
                    var lastEvent = events.FirstOrDefault();
                    if (lastEvent != null && (now - lastEvent.Date).TotalSeconds <= 60)
                    {
                        StatusDescription = "ВСЕ РАБОТАЕТ";
                        Status = Status.NormalWork;
                    }
                    else
                    {
                        StatusDescription = "ПРОБЛЕМЫ В РАБОТЕ (НУЖНА ПРОВЕРКА)";
                        Status = Status.BadWork;
                    }
                }
                else
                {
                    Status = Status.NotWork;
                    StatusDescription = "НЕ РАБОТАЕТ";
                    if (!(string.IsNullOrEmpty(rootIp) || (string.Equals(rootIp, "0.0.0.0", new StringComparison())) ||
                          string.Equals(rootIp, "0", new StringComparison())))
                    {
                        var testFirst = TestPing(rootIp)
                            ? "Проблемы с микроконтроллером табло"
                            : "Отсутствует электричество";
                        LastError = $"Нет связи. Возможные причины: {testFirst}" + '\n' + "Необходим выезд на место!";
                    }
                    else
                    {
                        LastError = $"Нет связи. Возможные причины: Ip корневого узла не назначен" + '\n' + "Устройство не отвечает, установите Ip корневого узла!";
                    }
                }
            }
        }
        public int CompareTo(object obj)
        {
            return obj == null
                ? -1
                : (string.CompareOrdinal(
                    StationNameAndDescription.ToUpperInvariant(),
                    (((MonitoringViewModel)obj).StationNameAndDescription).ToUpperInvariant()));
        }
    }
}