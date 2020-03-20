using System;

namespace CityStations.Models
{
    public class StationModel
    {
        public string Id { get; set; }
        public string IdForRnis { get; set; }
        public string Name { get; set; }
        public string NameForRnis { get; set; }
        public string NameOficial { get; set; }
        public string DistrictOfTheCity { get; set; }
        public string Street { get; set; }
        public string NumberNearHouse { get; set; }
        public string Description { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }
        public bool Type { get; set; }
        public bool Active { get; set; }
        public string AccessCode { get; set; }
        public virtual InformationTable InformationTable { get; set; }

        public StationModel() { }
        public StationModel(Station station )
        {
            Id = station.id;
            Name = station.name;
            Description = station.descr;
            try
            {
                Lat = Convert.ToDouble(station.lat.Substring(0, 2) + "." + station.lat.Substring(2, station.lat.Length - 2));
                Lng = Convert.ToDouble(station.lng.Substring(0, 2) + "." + station.lng.Substring(2, station.lng.Length - 2));
            }
            catch(Exception e)
            {
                Lat = Convert.ToDouble(station.lat.Substring(0, 2) + "," + station.lat.Substring(2, station.lat.Length - 2));
                Lng = Convert.ToDouble(station.lng.Substring(0, 2) + "," + station.lng.Substring(2, station.lng.Length - 2));
                Logger.WriteLog(
                    $"Ошибка {e.Message}, внутренне исключение {e.InnerException}, источник возникновения {e.Source}, подробности {e.StackTrace}",Id);
            }
            Type = string.Equals(station.type,"1");
            Active = false;
        }
    }      
}