using System;

namespace CityStations.Models
{
    public enum EventType {ERROR, EVENT, WARNING}

    public class Event
    {
        public string Id { get; set; }
        public  EventType EventType { get; set; }
        public string Description { get; set; }
        public DateTime Date { get; set; }
        public string Initiator { get; set; } // user or station Id

        public Event()
        {
        }

        public Event(string message, string initiator)
        {
            Date = DateTime.Now;
            Id = $"{new TimeSpan(DateTime.MaxValue.Ticks - DateTime.Now.Ticks)}_{initiator}";
            EventType = message.ToUpperInvariant()
                               .Contains("ОШИБКА")
                      ? EventType.ERROR
                      : (message.ToUpperInvariant()
                                .Contains("ВНИМАНИЕ")  
                         ? EventType.WARNING
                         : EventType.EVENT);
            Initiator = initiator;
            Description = message;
        }

    }
}