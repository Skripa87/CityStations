using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityStations.Models.JsonGoogleMapsGeocoder
{
    public class Geometry
    {
        public Location location { get; set; }
        public string location_type { get; set; }
        public Viewport viewport { get; set; }
        public Bounds bounds { get; set; }
    }
}