namespace CityStations.Models
{
    public enum ContentType {FORECAST, TEXT, VIDEO, PICTURE, TICKER, TICKER_VERTICAL,  DATE_TIME, WEATHER_DATE_TIME}

    public interface IContent
    {
        string Id { get; }
        object PresentContent();
        int IndexInContent { get; }
        int TimeOut{ get;}
        ContentType ContentType { get;}
    }

    public abstract class AbstractContent:IContent
    {
        public string Id { get; set; }
        public int IndexInContent { get; protected set; }
        public int TimeOut { get; protected set; }
        public ContentType ContentType { get; protected set; }

        public object PresentContent()
        {
            throw new System.NotImplementedException();
        }
    }
}