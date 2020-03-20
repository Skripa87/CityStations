using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CityStations.Models
{
    public class Station
    {
        public string id { get; set; }
        public string name { get; set; }
        public string descr { get; set; }
        public string lat { get; set; }
        public string lng {get; set;}
        public string type { get; set; }
    }
}