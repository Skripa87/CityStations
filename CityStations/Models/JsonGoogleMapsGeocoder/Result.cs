using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityStations.Models.JsonGoogleMapsGeocoder
{
    public class Result
    {
        public IList<AddressComponent> address_components { get; set; }
        public string formatted_address { get; set; }
        public Geometry geometry { get; set; }
        public string place_id { get; set; }
        public PlusCode plus_code { get; set; }
        public IList<string> types { get; set; }
}
}