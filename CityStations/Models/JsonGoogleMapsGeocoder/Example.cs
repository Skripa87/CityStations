using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityStations.Models.JsonGoogleMapsGeocoder
{
    public class Example
    {
        public PlusCode plus_code { get; set; }
        public IList<Result> results { get; set; }
        public string status { get; set; }
    }
}