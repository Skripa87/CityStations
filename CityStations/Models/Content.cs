using System.Collections.Generic;

namespace CityStations.Models
{
    public class Content
    {
        public string Id { get; set; }
        public ContentType ContentType { get; set; }
        public int TimeOut { get; set; }
        public string InnerContent { get; set; }        
        public virtual List<InformationTable> InformationTables { get; set; }

        public Content()
        {
            InformationTables = new List<InformationTable>();
        }
    }
}