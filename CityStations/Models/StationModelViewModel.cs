using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityStations.Models
{
    public class StationModelViewModel
    {

        public string StationId { get; set; }
        public string VisualName { get; set; }
        public string DistrictOfTheCity { get; set; }
        public string StrFunction { get; set; }
        public string Geo { get; set; }
        public string Description { get; set; }
        //public string Contents { get; set; }
        public string Red { get; set; }
        public StationModelViewModel(StationModel station)
        {
            if (station == null) return;
            StationId = station.Id;
            VisualName = string.IsNullOrEmpty(station.NameOficial)
                       ? station.Name + " (не офиц.)"
                       : station.NameOficial;
            DistrictOfTheCity = station.DistrictOfTheCity;
            StrFunction = station.Name + " " + station.Description;
            Geo = "Geo";
            Red = "Red";
            //Contents = "";
            //if (station?.InformationTable?.Contents != null)
            //{
            //    foreach (var item in station.InformationTable.Contents)
            //    {
            //        Contents += (item?.ContentType.ToString() + " " ?? "");
            //    }
            //}
        }

    }
}