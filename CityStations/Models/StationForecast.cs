using System;

namespace CityStations.Models
{
    public partial class StationForecast:IComparable
    {
        public int Id { get; set; }
        public double? Arrt { get; set; }
        public string Where { get; set; }
        public string Vehid { get; set; }
        public Nullable<int> Rid { get; set; }
        public string Rtype { get; set; }
        public string Rnum { get; set; }
        public string Lastst { get; set; }
        
        public int CompareTo(object obj)
        {
            return Arrt < ((obj as StationForecast)?.Arrt ?? 0)
                ? -1
                : (Arrt > (((StationForecast) obj)?.Arrt ?? 0)
                    ? 1
                    : 0);
        }
    }
}
