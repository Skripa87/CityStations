using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityStations.Models.JsonGoogleMapsGeocoder
{
    public class AddressComponent
    {
        public string long_name { get; set; }
        public string short_name { get; set; }
        public IList<string> types { get; set; }
    }
}