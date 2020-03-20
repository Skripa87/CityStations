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
            Id = Guid.NewGuid()
                     .ToString();
            EventType = message.ToLowerInvariant()
                               .Contains("ошибка")
                      ? EventType.ERROR
                      : (message.ToLowerInvariant()
                                .Contains("внимание")  
                         ? EventType.WARNING
                         : EventType.EVENT);
            Date = DateTime.Now;
            Initiator = initiator;
            Description = message;
        }

    }
}